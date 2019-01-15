using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RBCharacterSelectionBtn : MonoBehaviour
{
    [SerializeField]
    public RBLobbyPlayerUISlot _localPlayerSlot = null;

    [SerializeField]
    public RBCharacter _character = null;

    void Awake()
    {
        // Set the button label
        var btnLabel = gameObject.GetComponentInChildren<Text>();
        btnLabel.text = _character.Name;

        // Add the action to update the preview window on click
        var btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(() => RBCharacterPreview.Instance.SetPreview(_character.ID));

        // Change the selected character in the local player slot on click
        btn.onClick.AddListener(() => _localPlayerSlot.SetCharacter(_character.ID));

        // Activate/deactivate the button when the player is ready or not
        foreach (Transform child in _localPlayerSlot.gameObject.transform)
        {
            var readyToggle = child.gameObject.GetComponent<Toggle>();
            if (readyToggle != null)
            {
                readyToggle.onValueChanged.AddListener((bool ready) => btn.interactable = !ready);
                break;
            }
        }
    }
}
