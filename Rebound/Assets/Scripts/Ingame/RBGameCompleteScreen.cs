using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RBGameEndType { Victory, Loss, Draw }

[RequireComponent(typeof(CanvasGroup))]
public class RBGameCompleteScreen : MonoBehaviour
{
    [SerializeField]
    private RBGameEndType _gameEndType;

    [SerializeField]
    private Text _team1PointsField;

    [SerializeField]
    private Text _team2PointsField;

    private CanvasGroup _canvasGroup;

    private int _localPlayerTeam;

    private bool _prevGameState = false;

    // Use this for initialization
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _localPlayerTeam = RBMatch.Instance.GetLocalUser().Team;
    }

    // Update is called once per frame
    void Update()
    {
        if (ARBArenaSetup.Instance.GameDone && !_prevGameState)
        {
            _team1PointsField.text = ARBArenaSetup.Instance.Team1Points.ToString();
            _team2PointsField.text = ARBArenaSetup.Instance.Team2Points.ToString();

            switch (_gameEndType)
            {
                case RBGameEndType.Victory:
                    if (IsLocalPlayerWinner())
                        ShowScreen();
                    else HideScreen();
                    break;
                case RBGameEndType.Draw:
                    if (IsDraw())
                        ShowScreen();
                    else HideScreen();
                    break;
                case RBGameEndType.Loss:
                    if (!IsDraw() && !IsLocalPlayerWinner())
                        ShowScreen();
                    else HideScreen();
                    break;
            }
        }

        _prevGameState = ARBArenaSetup.Instance.GameDone;
    }

    private bool IsDraw()
    {
        return ARBArenaSetup.Instance.WinnerTeam == 0;
    }

    private bool IsLocalPlayerWinner()
    {
        return ARBArenaSetup.Instance.WinnerTeam == _localPlayerTeam;
    }

    private void ShowScreen()
    {
        _canvasGroup.alpha = 1.0f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    private void HideScreen()
    {
        _canvasGroup.alpha = 0.0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
}
