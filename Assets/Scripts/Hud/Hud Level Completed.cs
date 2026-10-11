using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudLevelCompleted : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text   textLevel;
    [SerializeField] private TMP_Text   textTime;
    [SerializeField] private Button     buttonContinue;
    [SerializeField] private Button     buttonNext;
    [SerializeField] private Button     buttonQuit;
    [SerializeField] private HudList    hudList;

    private void Awake()
    {
        buttonContinue.onClick.AddListener(Continue);
        buttonNext    .onClick.AddListener(Next    );
        buttonQuit    .onClick.AddListener(Quit    );

        panel.SetActive(false);
    }

    public void Show()
    {
        GameData data = GameData.INSTANCE;

        textLevel.text = "Level " + (data.Level.Index + 1);
        textTime .text = HudTools.FormatTime(data.TimeElapsed);

        IsShowing = true;
        panel.SetActive(true);

        int nextIndex = data.Level.Index + 1;
        buttonNext.gameObject.SetActive( nextIndex < LevelCollection.Count() );
    }

    private void Continue()
    {
        IsShowing = false;
        panel.SetActive(false);
    }

    private void Next()
    {
        GameData data = GameData.INSTANCE;

        int nextIndex = data.Level.Index + 1;

        if (nextIndex >= LevelCollection.Count())
            return;

        IsShowing = false;
        panel.SetActive(false);

        data.LevelStart(LevelCollection.Get((byte)nextIndex));

        // Reset the new HUD for the next level.
        hudList.ResetHud();
    }

    private void Quit()
    {
        IsShowing = false;
        panel.SetActive(false);
        GameData.INSTANCE.LevelStop();
    }

    public static bool IsShowing { get; private set; }

    private void OnDisable()
    {
        IsShowing = false;
    }
}