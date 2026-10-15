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

        float time = data.IsStateCompleted() ? data.TimeElapsed : data.TimeRemaining;

        textLevel     .text = "Level " + (data.Level.Index + 1);
        textTime      .text = HudTools.FormatTime(time);
        textCompletion.text = data.IsStateCompleted() ? "Completed" : $"{data.Completion * 100f:F0}%";
    }
}