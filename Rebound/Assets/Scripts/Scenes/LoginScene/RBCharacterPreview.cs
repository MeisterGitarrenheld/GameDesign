using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Character preview position for teammembers (contains the character object)
/// </summary>
public class PreviewPosition
{
    public readonly RectTransform Position;
    public readonly int Id;
    public Text PreviewName;
    public bool Used { get { return CharacterObj != null; } }

    public GameObject CharacterObj = null;

    public PreviewPosition(RectTransform pos, int id)
    {
        Position = pos;
        Id = id;
        PreviewName = pos.GetComponentInChildren<Text>();
    }
}

public class RBCharacterPreview : RBMonoBehaviourSingleton<RBCharacterPreview>
{
    public RectTransform CharacterPosition;

    public RectTransform TeamMember1Position;
    public RectTransform TeamMember2Position;
    public RectTransform TeamMember3Position;

    private GameObject _displayedCharacter;
    private List<PreviewPosition> _positions = new List<PreviewPosition>();

    protected override void AwakeSingleton()
    {
        _positions.Add(new PreviewPosition(TeamMember1Position, 1));
        _positions.Add(new PreviewPosition(TeamMember2Position, 2));
        _positions.Add(new PreviewPosition(TeamMember3Position, 3));
    }

    /// <summary>
    /// Sets the character object for the local player
    /// </summary>
    /// <param name="characterId"></param>
    public void SetPreview(int characterId)
    {
        var charPrefab = RBCharacterInfo.Instance.GetCharacterById(characterId);

        SetPreview(charPrefab);
    }

    /// <summary>
    /// <see cref="SetPreview(int)"/>
    /// </summary>
    /// <param name="character"></param>
    public void SetPreview(RBCharacter character)
    {
        ClearPreview();

        if (character == null) return;

        Vector3 position = CharacterPosition.transform.position;

        var @char = Instantiate(character.gameObject, gameObject.transform);
        _displayedCharacter = @char;
        @char.transform.position = position;
        @char.transform.localScale *= 1.5f;
        @char.gameObject.SetLayerRecursively(LayerMask.NameToLayer("CharacterPreview"));
    }

    /// <summary>
    /// Destroys the displayed character object of the local player character preview
    /// </summary>
    public void ClearPreview()
    {
        if (_displayedCharacter != null)
        {
            Destroy(_displayedCharacter);
        }
    }

    /// <summary>
    /// Set the character object for the teammember preview
    /// </summary>
    /// <param name="player">the owning player object</param>
    /// <param name="prevPos">position to spawn the object</param>
    /// <returns>the position with the object</returns>
    public PreviewPosition SetTeamPreview(RBPlayer player, PreviewPosition prevPos)
    {
        var charPrefab = RBCharacterInfo.Instance.GetCharacterById(player.CharacterId);

        return SetTeamPreview(charPrefab, prevPos, player);
    }

    /// <summary>
    /// <see cref="SetTeamPreview(RBPlayer, PreviewPosition)"/>
    /// </summary>
    /// <param name="character"></param>
    /// <param name="prevPos"></param>
    /// <returns></returns>
    public PreviewPosition SetTeamPreview(RBCharacter character, PreviewPosition prevPos, RBPlayer player)
    {
        ClearTeamPreview(prevPos);

        if ((prevPos = _positions.FirstOrDefault(x => x.Used == false)) == null)
            return null; // there is no open slot

        prevPos.PreviewName.text = player.Username;

        var @char = Instantiate(character.gameObject, gameObject.transform);
        @char.transform.position = prevPos.Position.transform.position;
        @char.gameObject.SetLayerRecursively(LayerMask.NameToLayer("CharacterPreview"));
        prevPos.CharacterObj = @char;

        return prevPos;
    }

    /// <summary>
    /// Destroys the character object of given position
    /// </summary>
    /// <param name="prevPos"></param>
    public void ClearTeamPreview(PreviewPosition prevPos)
    {
        if (prevPos == null) return;

        Destroy(prevPos.CharacterObj);
        prevPos.PreviewName.text = string.Empty;
        prevPos.CharacterObj = null;
    }

    public GameObject GetSelectedCharacter()
    {
        return _displayedCharacter;
    }
}
