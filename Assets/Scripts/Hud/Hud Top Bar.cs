using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudTopBar : MonoBehaviour
{
    [SerializeField] private Button   buttonMenu;
    [SerializeField] private TMP_Text textLevel;
    [SerializeField] private TMP_Text textTime;
    [SerializeField] private TMP_Text textCompletion;

    private void Awake()
    {
        buttonMenu.onClick.AddListener(OpenMenu);
    }

    private void Update()
    {
        GameData data = GameData.INSTANCE;
        if ( !data.IsPlaying() || data.IsPaused )
            return;

        textLevel     .text = "Level " + (data.Level.Index + 1);
        textTime      .text = HudTools.FormatTime(data.TimeElapsed);
        textCompletion.text = data.IsCompleted ? "Completed" : $"{data.Completion * 100f:F0}%";
    }

    private void OpenMenu()
    {
        GameData.INSTANCE.Pause();
    }
}