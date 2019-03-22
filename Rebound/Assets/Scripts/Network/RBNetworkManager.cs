using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public enum RBCustomMsgTypes { RBInitPlayerMessage = 1000, RBLobbyMatchUpdateMessage, RBPlayerMovementMessage, RBPlayerPhysicsMessage, RBGameEventMessage, RBShowLoadscreenMessage }

public class RBNetworkManager : NetworkManager
{
    private short _clientCount = 0;

    public static RBNetworkManager Instance { get { return singleton as RBNetworkManager; } }

    public event Action OnClientStarted;
    public event Action OnPlayerAdded;
    public event Action OnPlayerRemoved;
    public int LoadingScreenDelay = 3;

    private Dictionary<string, NetworkConnection> _connMapping = new Dictionary<string, NetworkConnection>(); // <username, connection>

    /// <summary>
    /// True, if the local player is the host.
    /// </summary>
    public bool IsHost { get; private set; } = false;

    /// <summary>
    /// Starts the host and one client for the local user.
    /// </summary>
    /// <returns></returns>
    public override NetworkClient StartHost()
    {
        IsHost = true;
        var client = base.StartHost();
        NetworkServer.RegisterHandler(RBInitPlayerMessage.MSG_TYPE, OnInitPlayerMessageReceived);
        NetworkServer.RegisterHandler(RBLobbyMatchUpdateMessage.MSG_TYPE, OnClientCharacterUpdateReceived);
        return client;
    }


    /// <summary>
    /// Starts only a client for the local user to connect to a host.
    /// </summary>
    /// <param name="connInfo"></param>
    /// <returns></returns>
    public NetworkClient StartClient(RBLanConnectionInfo connInfo)
    {
        Reset();

        networkAddress = connInfo.IpAddress;
        networkPort = connInfo.Port;

        return base.StartClient();
    }


    /// <summary>
    /// This hook is called on the host machine when a host is stopped.
    /// </summary>
    public override void OnStopHost()
    {
        Reset();

        base.OnStopHost();
    }


    /// <summary>
    /// This hook is called when the local client is started.
    /// </summary>
    /// <param name="client"></param>
    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);

        if (IsHost)
        {
            // reset the match and add the host
            RBMatch.Instance.Reset();

            RBPlayer host = new RBPlayer
            {
                Username = RBLocalUser.Instance.Username,
                IsHost = true
            };

            host.OnReadyStateChanged += Player_OnReadyStateChanged;
            host.OnTeamChanged += Player_OnTeamChanged;
            host.OnCharacterIdChanged += Player_OnCharacterIdChanged;

            RBMatch.Instance.AddPlayer(host);
        }
        client.RegisterHandler(RBLobbyMatchUpdateMessage.MSG_TYPE, OnLobbyMatchUpdateMessageReceived);
        client.RegisterHandler(RBShowLoadscreenMessage.MSG_TYPE, OnShowLoadscreenMessageReceived);

        OnClientStarted?.Invoke();
    }

    private AsyncOperation _loadingSceneAsync;

    public override void ServerChangeScene(string newSceneName)
    {
        //base.ServerChangeScene(newSceneName);
        NetworkServer.SetAllClientsNotReady();
        NetworkServer.SendToAll(RBShowLoadscreenMessage.MSG_TYPE, new RBShowLoadscreenMessage() { SceneName = newSceneName, Show = true });
    }

    private void OnShowLoadscreenMessageReceived(NetworkMessage netMsg)
    {
        var gameMsg = netMsg.ReadMessage<RBShowLoadscreenMessage>();

        if (gameMsg.Show)
        {
            networkSceneName = gameMsg.SceneName;

            if (SceneManager.GetActiveScene().name != gameMsg.SceneName && gameMsg.SceneName != "")
            {
                RBLoadingScreen.Instance.ShowLoadingScreen(gameMsg.SceneName);
                LoadSceneAsync(gameMsg.SceneName);
            }
        }
        else
        {
            RBLoadingScreen.Instance.HideLoadingScreen();
        }
    }

    private void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncCR(sceneName));
    }

    IEnumerator LoadSceneAsyncCR(string sceneName)
    {
        _loadingSceneAsync = SceneManager.LoadSceneAsync(sceneName);

        if (_loadingSceneAsync == null)
            yield return null;

        _loadingSceneAsync.allowSceneActivation = true;
        while (!_loadingSceneAsync.isDone)
            yield return null;


        _loadingSceneAsync = null;

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        FinishSceneLoad();
    }

    private void FinishSceneLoad()
    {
        if (IsClientConnected() && client != null)
            OnClientSceneChanged(client.connection);

        if (NetworkServer.active)
        {
            NetworkServer.SpawnObjects();
            OnServerSceneChanged(networkSceneName);
        }
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        StartCoroutine(WaitLoadingScreenDelay());
    }

    IEnumerator WaitLoadingScreenDelay()
    {
        yield return new WaitForSeconds(LoadingScreenDelay);

        NetworkServer.SendToAll(RBShowLoadscreenMessage.MSG_TYPE, new RBShowLoadscreenMessage { Show = false, SceneName = "" });
    }

    /// <summary>
    /// Called on the client, when an update message for the match has been received
    /// from the host.
    /// </summary>
    /// <param name="netMsg"></param>
    private void OnLobbyMatchUpdateMessageReceived(NetworkMessage netMsg)
    {
        var updateMsg = netMsg.ReadMessage<RBLobbyMatchUpdateMessage>();

        if (!IsHost)
        {
            RBMatch.Instance.RemoveAllPlayers(true);

            foreach (var playerData in updateMsg.Players)
            {
                Debug.Log(playerData);
                var player = RBSerializer.Deserialize<RBPlayer>(playerData);
                player.OnReadyStateChanged += Player_OnReadyStateChanged;
                player.OnTeamChanged += Player_OnTeamChanged;
                player.OnCharacterIdChanged += Player_OnCharacterIdChanged;
                RBMatch.Instance.AddPlayer(player);
            }
        }
    }

    /// <summary>
    /// Called on the server when the client changes his character settings.
    /// </summary>
    /// <param name="netMsg"></param>
    private void OnClientCharacterUpdateReceived(NetworkMessage netMsg)
    {
        var updateMsg = netMsg.ReadMessage<RBLobbyMatchUpdateMessage>();

        var changedPlayer = RBSerializer.Deserialize<RBPlayer>(updateMsg.Players[0]);
        Debug.Log("Received player update: " + updateMsg.Players[0]);
        foreach (var existingPlayer in RBMatch.Instance.Players)
        {
            if (changedPlayer.Username == existingPlayer.Username)
            {
                existingPlayer.SetIsReady(changedPlayer.IsReady);
                existingPlayer.SetCharacterId(changedPlayer.CharacterId);
            }
        }
    }

    private void Player_OnReadyStateChanged(RBPlayer player, bool isReady)
    {
        Debug.Log("The ready state of " + player.Username + " changed to " + (isReady ? "ready" : "not ready") + ".");
        SendLobbyMatchUpdateToClients();
    }


    private void Player_OnTeamChanged(RBPlayer player, int team)
    {
        Debug.Log("The team of " + player.Username + " changed to " + team + ".");

        if (IsHost)
            SendLobbyMatchUpdateToClients();
    }

    private void Player_OnCharacterIdChanged(RBPlayer player, int characterId)
    {
        Debug.Log("The character id of " + player.Username + " changed to " + characterId + ".");
        SendLobbyMatchUpdateToClients();
    }

    /// <summary>
    /// Sends the current match configuration to the clients in the lobby screen.
    /// </summary>
    private void SendLobbyMatchUpdateToClients()
    {
        Debug.Log("Send match update... (from host? " + IsHost + ")");
        var updateMsg = new RBLobbyMatchUpdateMessage();
        var players = RBMatch.Instance.Players;
        updateMsg.FromHost = IsHost;

        if (IsHost)
        {
            updateMsg.Players = new string[players.Count];
            for (int i = 0; i < players.Count; i++)
            {
                Debug.Log("Send " + RBSerializer.Serialize(players[i]));
                updateMsg.Players[i] = RBSerializer.Serialize(players[i]);
            }

            NetworkServer.SendToAll(RBLobbyMatchUpdateMessage.MSG_TYPE, updateMsg);
        }
        else
        {
            // Notify server about client update
            updateMsg.Players = new string[1];

            foreach (var player in players)
            {
                if (player.IsLocalUser)
                {
                    updateMsg.Players[0] = RBSerializer.Serialize(player);
                    break;
                }
            }

            client.Send(RBLobbyMatchUpdateMessage.MSG_TYPE, updateMsg);
        }
    }


    /// <summary>
    /// Called on the client when connected to a server.
    /// The default implementation of this function sets the client as ready and adds a player.
    /// </summary>
    /// <param name="conn"></param>
    public override void OnClientConnect(NetworkConnection conn)
    {
        var localPlayerData = new RBInitPlayerMessage { Username = RBLocalUser.Instance.Username };

        conn.Send(RBInitPlayerMessage.MSG_TYPE, localPlayerData);

        ClientScene.AddPlayer(conn, conn.GetPlayerId());
    }


    /// <summary>
    /// Called on the server when a client has sent his initial player information.
    /// </summary>
    /// <param name="netMsg"></param>
    private void OnInitPlayerMessageReceived(NetworkMessage netMsg)
    {
        var initMsg = netMsg.ReadMessage<RBInitPlayerMessage>();

        var isNewPlayerHost = RBLocalUser.Instance.Username == initMsg.Username;

        _connMapping[initMsg.Username] = netMsg.conn;

        if (!isNewPlayerHost)
        {
            var player = new RBPlayer
            {
                IsHost = isNewPlayerHost,
                Username = initMsg.Username,
                ConnectionId = netMsg.conn.connectionId
            };

            player.OnReadyStateChanged += Player_OnReadyStateChanged;
            player.OnTeamChanged += Player_OnTeamChanged;
            player.OnCharacterIdChanged += Player_OnCharacterIdChanged;

            RBMatch.Instance.AddPlayer(player);

            SendLobbyMatchUpdateToClients();
        }
    }


    /// <summary>
    /// Called on the server when a client adds a player with ClientScene.AddPlayer
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="playerControllerId"></param>
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        //base.OnServerAddPlayer(conn, playerControllerId);
        _clientCount++;
        OnPlayerAdded?.Invoke();

        /*
        if (_clientCount == RBMatch.Instance.MaxPlayerCount)
        {
            ServerChangeScene("VerticalPrototypeArena");
        }
        */
    }


    /// <summary>
    /// Called on the server when a client removes a player.
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="player"></param>
    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        base.OnServerRemovePlayer(conn, player);
        _clientCount--;
        OnPlayerRemoved?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("New client connected: " + conn.address + " " + conn.connectionId);
    }

    /// <summary>
    /// Called on the server when a client disconnects.
    /// </summary>
    /// <param name="conn"></param>
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("Client disconnected: " + conn.address + " " + conn.connectionId);
        _clientCount--;
        OnPlayerRemoved?.Invoke();

        var playerToRemove = RBMatch.Instance.FindPlayerById(conn.connectionId);

        RBMatch.Instance.RemovePlayer(playerToRemove);
        SendLobbyMatchUpdateToClients();

        _connMapping.Remove(playerToRemove.Username);
    }

    /// <summary>
    /// Should be called on the host to send the character creation to the clients.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="playerObject"></param>
    /// <returns></returns>
    public bool SpawnPlayer(RBPlayer player, GameObject playerObject)
    {
        var playerConn = _connMapping[player.Username];
        return NetworkServer.AddPlayerForConnection(playerConn, playerObject, playerConn.GetPlayerId());
    }


    /// <summary>
    /// Resets the network manager to the default state,
    /// but does not stop the host or the client.
    /// </summary>
    private void Reset()
    {
        IsHost = false;
        _clientCount = 0;
    }
}
