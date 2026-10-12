using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudLevelCompleted : MonoBehaviour
{
    // Member Variables
    [SerializeField] private GameObject     panel;
    [SerializeField] private TMP_Text       textLevel;
    [SerializeField] private TMP_Text       textTime;
    [SerializeField] private Button         buttonContinue;
    [SerializeField] private Button         buttonNext;
    [SerializeField] private Button         buttonQuit;
    [SerializeField] private MenuController menuController;

    public static bool IsShowing { get; private set; }

    private void Awake()
    {
        buttonContinue.onClick.AddListener(Continue);
        buttonNext    .onClick.AddListener(Next    );
        buttonQuit    .onClick.AddListener(Quit    );

        panel.SetActive(false);
        IsShowing = false;
    }

    public void Show()
    {
        GameData data = GameData.INSTANCE;

        textLevel.text = "Level " + (data.Level.Index + 1);
        textTime .text = HudTools.FormatTime(data.TimeElapsed);

        int nextIndex = data.Level.Index + 1;
        buttonNext.gameObject.SetActive(nextIndex < LevelCollection.Count());

        IsShowing = true;
        panel.SetActive(true);
    }

    private void Continue()
    {
        Hide();
    }

    private void Next()
    {
        GameData data = GameData.INSTANCE;

        int nextIndex = data.Level.Index + 1;
        if (nextIndex >= LevelCollection.Count())
            return;

        Hide();
        menuController.StartLevel( LevelCollection.Get((byte)nextIndex) );
    }

    private void Quit()
    {
        Hide();
        menuController.ExitLevel();
    }

    private void Hide()
    {
        IsShowing = false;
        panel.SetActive(false);
    }

    private void OnDisable()
    {
        IsShowing = false;
    }
}