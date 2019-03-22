using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RBHideLockCursor : MonoBehaviour
{
    public static bool CursorLocked = true;

    void Update()
    {

        // pressing esc toggles between hide/show
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CursorLocked = !CursorLocked;
        }

        if (CursorLocked)
            LockCursor();
        else UnlockCursor();
    }

    public static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
