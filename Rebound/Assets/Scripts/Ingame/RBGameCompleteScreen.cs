using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class RBGameCompleteScreen : MonoBehaviour
{
    public static RBGameCompleteScreen Instance;

    [SerializeField]
    private Text _team1PointsField;

    [SerializeField]
    private Text _team2PointsField;

    [SerializeField]
    private Text _winStateText;

    [SerializeField]
    private float _totalScreenDuration = 7.0f;

    private RBCanvasNavigation _canvasNavigation;
    private Animator _anim;


    private int _localPlayerTeam;
    private bool _prevGameState = false;
    private bool _completed = false;
    private string _winState;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        _canvasNavigation = GetComponent<RBCanvasNavigation>();
        _anim = GetComponentInChildren<Animator>();
        _localPlayerTeam = RBMatch.Instance.GetLocalUser().Team;

        _canvasNavigation.OnFadeInEnded += _canvasNavigation_OnFadeInEnded;

        foreach (var anim in _anim.GetCurrentAnimatorClipInfo(0))
            print("Anim: " + anim.clip.name + ", length: " + anim.clip.length);
    }

    public bool WinScreenCompleted()
    {
        return _completed;
    }

    private void _canvasNavigation_OnFadeInEnded(RBCanvasNavigation obj)
    {
        _anim.SetTrigger("Show");

        StartCoroutine(TypeWinStateAfterAnim());
    }

    IEnumerator TypeWinStateAfterAnim()
    {
        yield return new WaitForSeconds(1.5f);

        var time = _totalScreenDuration;

        foreach(var @char in _winState)
        {
            _winStateText.text += @char;
            yield return new WaitForSeconds(.2f);
            _totalScreenDuration -= .2f;
        }

        yield return new WaitForSeconds(time);
        HideScreen();

    }

    // Update is called once per frame
    void Update()
    {
        if(ARBArenaSetup.Instance.GameDone && !_prevGameState)
        {
            if (IsLocalPlayerWinner()) _winState = "VICTORY!";
            else if (IsDraw()) _winState = "DRAW!";
            else _winState = "DEFEAT!";

            ShowScreen();
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
        _team1PointsField.text = ARBArenaSetup.Instance.Team1Points.ToString();
        _team2PointsField.text = ARBArenaSetup.Instance.Team2Points.ToString();

        RBCanvasNavigation.CurrentPage = null;
        _canvasNavigation.Show();
    }

    private void HideScreen()
    {
        _anim.SetTrigger("Hide");
        StartCoroutine(WaitForAnimToComplete());
    }

    IEnumerator WaitForAnimToComplete()
    {
        yield return new WaitForSeconds(1.5f);

        _completed = true;
    }
}
