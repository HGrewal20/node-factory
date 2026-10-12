using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuGameOver : MonoBehaviour
{
    // Member Variables
    [SerializeField] private TMP_Text       textLevel;
    [SerializeField] private TMP_Text       textTime;
    [SerializeField] private TMP_Text       textProgress;
    [SerializeField] private Button         buttonRestart;
    [SerializeField] private Button         buttonQuit;
    [SerializeField] private MenuController menuController;

    private void Awake()
    {
        buttonRestart.onClick.AddListener(Restart);
        buttonQuit   .onClick.AddListener(Quit   );
    }

    public void Refresh()
    {
        GameData data = GameData.INSTANCE;

        textLevel   .text = $"LEVEL - {data.Level.Index + 1:00}";
        textTime    .text = $"TIME - {HudTools.FormatTime(data.TimeElapsed)}";
        textProgress.text = $"PROGRESS - {data.Completion * 100f:F0}%";
    }

    private void Restart()
    {
        menuController.RestartLevel();
    }

    private void Quit()
    {
        menuController.ExitLevel();
    }
}