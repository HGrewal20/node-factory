using TMPro;
using UnityEngine;

public class HudTopBar : MonoBehaviour
{
    [SerializeField] private TMP_Text textLevel;
    [SerializeField] private TMP_Text textTime;
    [SerializeField] private TMP_Text textCompletion;

    private void Update()
    {
        GameData data = GameData.INSTANCE;
        if ( !data.IsPlaying() || data.IsPaused )
            return;

        textLevel     .text = "Level " + (data.Level.Index + 1);
        textTime      .text = HudTools.FormatTime(data.TimeElapsed);
        textCompletion.text = data.IsCompleted ? "Completed" : $"{data.Completion * 100f:F0}%";
    }
}