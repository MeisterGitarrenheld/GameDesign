using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RBPersistentGameObject : MonoBehaviour
{

    public static HashSet<string> ActivePersistentObjects = new HashSet<string>();

    void Awake()
    {
        string id = SceneManager.GetActiveScene().name + "_" + gameObject.name;

        if (ActivePersistentObjects.Contains(id)) Destroy(gameObject);
        else
        {
            ActivePersistentObjects.Add(id);
            DontDestroyOnLoad(gameObject);
        }
    }
}
