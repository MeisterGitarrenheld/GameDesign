using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using UnityEngine;

public delegate void ProcessMulticast(string fromAddress, int fromPort, string message);

public class RBTimedUdpMulticast
{
    public const string DEFAULT_MESSAGE = "HELLO";
    public const int DEFAULT_PORT = 2222;
    public const int DEFAULT_MULTICAST_INTERVAL = 1000;
    public const string DEFAULT_MULTICAST_ADDRESS = "239.0.0.222";
    public static readonly Encoding ENCODING = Encoding.Unicode;

    /// <summary>
    /// Called when a multicast has been received and <see cref="CheckForMulticasts"/> was called.
    /// </summary>
    public event ProcessMulticast OnMulticastReceived;

    /// <summary>
    /// The message that should be sent with each multicast.
    /// Can be set anytime.
    /// </summary>
    public string MulticastMessage
    {
        get
        {
            return _multicastMessage;
        }
        set
        {
            _multicastMessage = value ?? string.Empty;
            _multicastMessageBytes = ENCODING.GetBytes(MulticastMessage ?? string.Empty);
        }
    }
    private volatile byte[] _multicastMessageBytes = ENCODING.GetBytes(DEFAULT_MESSAGE);
    private volatile string _multicastMessage = DEFAULT_MESSAGE;

    /// <summary>
    /// The port that should be used for the multicast (both: receiving and sending).
    /// Changes will be active after a restart.
    /// </summary>
    public int Port { get; set; } = DEFAULT_PORT;

    /// <summary>
    /// The multicast intervall in ms.
    /// Can be set anytime.
    /// </summary>
    public int MulticastInterval
    {
        get { return _multicastInterval; }
        set
        {
            _multicastInterval = value;
            _multicastSendTimer.Interval = value;
        }
    }
    private volatile int _multicastInterval = DEFAULT_MULTICAST_INTERVAL;

    /// <summary>
    /// Defines the ip address that should be used for the sending and receiving multicasts.
    /// </summary>
    public string MulticastAddress
    {
        get { return _multicastAddress; }
        set
        {
            _multicastAddress = value ?? "";
            _multicastIpAddress = IPAddress.Parse(_multicastAddress);
        }
    }
    private string _multicastAddress = DEFAULT_MULTICAST_ADDRESS;
    private IPAddress _multicastIpAddress = IPAddress.Parse(DEFAULT_MULTICAST_ADDRESS);

    private RBEventQueue _eventQueue = new RBEventQueue();
    private UdpClient _udpSendClient = null;
    private IPEndPoint _sendIpEndPoint = null;

    private Timer _multicastSendTimer = new Timer()
    {
        Interval = DEFAULT_MULTICAST_INTERVAL,
        Enabled = false
    };

    private UdpClient _udpReceiveClient = null;
    private IPEndPoint _receiveIpEndPoint = null;

    /// <summary>
    /// Initializes the multicasts.
    /// </summary>
    public RBTimedUdpMulticast()
    {
        _multicastSendTimer.Elapsed += MulticastTimer_Elapsed;
    }

    /// <summary>
    /// Checks if there are new incoming multicasts and raises the
    /// <see cref="OnMulticastReceived"/>-Event for all of them.
    /// </summary>
    public void CheckForMulticasts()
    {
        _eventQueue.RaiseEvents();
    }

    /// <summary>
    /// Stops the listener if running and starts it afterwords.
    /// It listens asynchronous.
    /// </summary>
    public void StartListeningForMulticasts()
    {
        StopListeningForMulticasts();

        _udpReceiveClient = _udpReceiveClient ?? new UdpClient();

        _udpReceiveClient.ExclusiveAddressUse = false;
        _udpReceiveClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _udpReceiveClient.ExclusiveAddressUse = false;

        _receiveIpEndPoint = new IPEndPoint(IPAddress.Any, Port);
        _udpReceiveClient.Client.Bind(_receiveIpEndPoint);

        _udpReceiveClient.JoinMulticastGroup(_multicastIpAddress);
        _udpReceiveClient.BeginReceive(ReceiveMulticast, new object());
    }

    /// <summary>
    /// Called when a new multicast has been received.
    /// Reades the message, enqueues it and starts listening again.
    /// </summary>
    /// <param name="mcData"></param>
    private void ReceiveMulticast(IAsyncResult mcData)
    {
        if (_udpReceiveClient != null)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, Port);
            byte[] bytes = _udpReceiveClient.EndReceive(mcData, ref ip);
            string message = ENCODING.GetString(bytes);
            _eventQueue.Enqueue(new OnMulticastReceivedWrapper(this, ip.Address.ToString(), ip.Port, message));
            _udpReceiveClient.BeginReceive(ReceiveMulticast, mcData.AsyncState);
        }
    }

    /// <summary>
    /// Stops the listening process and clears the event queue.
    /// </summary>
    public void StopListeningForMulticasts()
    {
        _udpReceiveClient?.Close();
        _udpReceiveClient = null;
        _eventQueue.Clear();
    }

    /// <summary>
    /// Stops multicasting if already running and start again afterwords.
    /// </summary>
    public void StartMulticast()
    {
        StopMulticast();

        _udpSendClient = _udpSendClient ?? new UdpClient();

        _sendIpEndPoint = new IPEndPoint(_multicastIpAddress, Port);
        _multicastSendTimer.Enabled = true;
    }

    /// <summary>
    /// Stops active multicasting processes.
    /// </summary>
    public void StopMulticast()
    {
        _multicastSendTimer.Enabled = false;
        _udpSendClient?.Close();
        _udpSendClient = null;
    }

    /// <summary>
    /// Called when a timed multicast should be executed.
    /// Sends the message to the specified port.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    private void MulticastTimer_Elapsed(object sender, ElapsedEventArgs eventArgs)
    {
        _udpSendClient?.Send(_multicastMessageBytes, _multicastMessageBytes.Length, _sendIpEndPoint);
    }

    /// <summary>
    /// Wraps the <see cref="OnMulticastReceived"/>-Event for timed execution.
    /// </summary>
    private class OnMulticastReceivedWrapper : IEventWrapper
    {
        private RBTimedUdpMulticast _discovery = null;
        private string _fromAddress = null;
        private int _fromPort = DEFAULT_PORT;
        private string _message = DEFAULT_MESSAGE;

        public OnMulticastReceivedWrapper(RBTimedUdpMulticast discovery, string fromAddress, int fromPort, string message)
        {
            _discovery = discovery;
            _fromAddress = fromAddress;
            _fromPort = fromPort;
            _message = message;
        }

        public void RaiseEvent()
        {
            _discovery.OnMulticastReceived?.Invoke(_fromAddress, _fromPort, _message);
        }
    }
}
