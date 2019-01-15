using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBCharacterPreview : RBMonoBehaviourSingleton<RBCharacterPreview>
{
    public RectTransform CharacterPosition;

    private GameObject _displayedCharacter;

    public void SetPreview(int characterId)
    {
        var charPrefab = RBCharacterInfo.Instance.GetCharacterById(characterId).gameObject;

        if (_displayedCharacter != null)
        {
            Destroy(_displayedCharacter);
        }

        if (charPrefab == null) return;

        Vector3 position = CharacterPosition.transform.position;

        var @char = Instantiate(charPrefab, gameObject.transform);
        _displayedCharacter = @char;
        @char.transform.position = position;
        @char.gameObject.SetLayerRecursively(LayerMask.NameToLayer("CharacterPreview"));
    }

    public void ClearPreview()
    {
        SetPreview(-1);
    }

    public GameObject GetSelectedCharacter()
    {
        return _displayedCharacter;
    }
}
