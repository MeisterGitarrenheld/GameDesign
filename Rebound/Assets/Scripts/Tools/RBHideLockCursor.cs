using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBHideLockCursor : MonoBehaviour
{
    public static bool LockCursor = true;

    void Update()
    {

        // pressing esc toggles between hide/show
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LockCursor = !LockCursor;
        }

        Cursor.lockState = LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !LockCursor;
    }
}
