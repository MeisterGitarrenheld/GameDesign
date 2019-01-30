using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RBCharacterInfo : RBMonoBehaviourSingleton<RBCharacterInfo>
{

    public RBCharacter[] CharacterPrefabs;

    public RBCharacter GetCharacterById(int id)
    {
        foreach (var @char in CharacterPrefabs)
        {
            if (@char.ID == id)
                return @char;
        }

        return null;
    }

    public RBCharacter GetDefaultCharacter()
    {
        return CharacterPrefabs[0];
    }
}
