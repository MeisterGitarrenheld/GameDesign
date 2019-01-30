using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RBCharacterSelectionBtn : MonoBehaviour
{
    [SerializeField]
    public RBLobbyCanvas _lobbyCanvas = null;

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
        btn.onClick.AddListener(() => _lobbyCanvas.GetLocalPlayerUISlot().Player?.SetCharacterId(_character.ID));

        // Activate/deactivate the button when the player is ready or not
        foreach (var uiPlayerSlot in _lobbyCanvas.GetAllPlayerUISlots())
        {
            foreach (Transform child in uiPlayerSlot.gameObject.transform)
            {
                var readyToggle = child.gameObject.GetComponent<Toggle>();
                if (readyToggle != null)
                {
                    readyToggle.onValueChanged.AddListener(
                        (bool ready) =>
                        {
                            if (uiPlayerSlot.IsLocalPlayer)
                                btn.interactable = !ready;
                        }
                    );
                    break;
                }
            }
        }
    }
}
