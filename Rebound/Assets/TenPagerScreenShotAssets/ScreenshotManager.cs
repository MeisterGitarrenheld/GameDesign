using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour {


    public Transform SceneContainer;

    private int currentScene;
    private int currentCameraPosition;

    private void Start()
    {
        currentScene = 0;
        currentCameraPosition = 0;
        foreach (Transform child in SceneContainer)
            child.gameObject.SetActive(false);

        SceneContainer.GetChild(currentScene).gameObject.SetActive(true);
        Camera.main.transform.position = SceneContainer.GetChild(currentScene).GetChild(0).GetChild(currentCameraPosition).position;
        Camera.main.transform.rotation = SceneContainer.GetChild(currentScene).GetChild(0).GetChild(currentCameraPosition).rotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SceneContainer.GetChild(currentScene).gameObject.SetActive(false);
            currentScene++;
            currentCameraPosition = 0;
            currentScene %= SceneContainer.childCount;
            Camera.main.transform.position = SceneContainer.GetChild(currentScene).GetChild(0).GetChild(currentCameraPosition).position;
            Camera.main.transform.rotation = SceneContainer.GetChild(currentScene).GetChild(0).GetChild(currentCameraPosition).rotation;
            SceneContainer.GetChild(currentScene).gameObject.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SceneContainer.GetChild(currentScene).gameObject.SetActive(false);
            currentScene--;
            currentCameraPosition = 0;
            if (currentScene < 0) currentScene = SceneContainer.childCount - 1;
            Camera.main.transform.position = SceneContainer.GetChild(currentScene).GetChild(0).GetChild(currentCameraPosition).position;
            Camera.main.transform.rotation = SceneContainer.GetChild(currentScene).GetChild(0).GetChild(currentCameraPosition).rotation;
            SceneContainer.GetChild(currentScene).gameObject.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentCameraPosition--;
            if (currentCameraPosition < 0) currentCameraPosition = SceneContainer.GetChild(currentScene).GetChild(0).childCount - 1;
            Camera.main.transform.position = SceneContainer.GetChild(currentScene).GetChild(0).GetChild(currentCameraPosition).position;
            Camera.main.transform.rotation = SceneContainer.GetChild(currentScene).GetChild(0).GetChild(currentCameraPosition).rotation;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentCameraPosition++;
            currentCameraPosition %= SceneContainer.GetChild(currentScene).GetChild(0).childCount;
            Camera.main.transform.position = SceneContainer.GetChild(currentScene).GetChild(0).GetChild(currentCameraPosition).position;
            Camera.main.transform.rotation = SceneContainer.GetChild(currentScene).GetChild(0).GetChild(currentCameraPosition).rotation;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
