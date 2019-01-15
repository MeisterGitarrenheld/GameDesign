using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBCharacterInfo : RBMonoBehaviourSingleton<RBCharacterInfo> {

    public RBCharacter[] CharacterPrefabs;

    public RBCharacter GetCharacterById(int id)
    {
        foreach(var @char in CharacterPrefabs)
        {
            if (@char.ID == id)
                return @char;
        }

        return null;
    }
}
