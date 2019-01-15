using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBCharacterPreview : RBMonoBehaviourSingleton<RBCharacterPreview>
{
    public RectTransform CharacterPosition;

    private GameObject _displayedCharacter;

    public void SetPreview(int characterId)
    {
        var charPrefab = RBCharacterInfo.Instance.GetCharacterById(characterId);

        SetPreview(charPrefab);
    }

    public void SetPreview(RBCharacter character)
    {
        ClearPreview();

        if (character == null) return;

        Vector3 position = CharacterPosition.transform.position;

        var @char = Instantiate(character.gameObject, gameObject.transform);
        _displayedCharacter = @char;
        @char.transform.position = position;
        @char.gameObject.SetLayerRecursively(LayerMask.NameToLayer("CharacterPreview"));
    }

    public void ClearPreview()
    {
        if (_displayedCharacter != null)
        {
            Destroy(_displayedCharacter);
        }
    }

    public GameObject GetSelectedCharacter()
    {
        return _displayedCharacter;
    }
}
