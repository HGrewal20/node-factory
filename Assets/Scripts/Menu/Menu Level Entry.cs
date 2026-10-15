using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuLevelEntry : MonoBehaviour
{
    // Member Variables
    [SerializeField] private TMP_Text       textName;
    [SerializeField] private TMP_Text       textInfo;
    [SerializeField] private TMP_Text       textPlay;
    [SerializeField] private Button         buttonPlay;
    [SerializeField] private Button         buttonReset;
                     private Level          level;
                     private Action<Level>  onPlay;
                     private Action<Level>  onReset;

    public void Setup(Level newLevel, MenuController menuController)
    {
        level   = newLevel;
        onPlay  = menuController.PlayLevel;
        onReset = menuController.ResetLevel;

        string mapSize = $"{level.mapSize.Width} x {level.mapSize.Height}";
        string time    = $"{level.TimeSec} + Sec";

        textName.text = $"{level.Index + 1:00} - {level.Name}";
        textInfo.text = mapSize + " - " + time;

        buttonPlay .onClick.RemoveAllListeners();
        buttonReset.onClick.RemoveAllListeners();

        buttonPlay .onClick.AddListener(Play      );
        buttonReset.onClick.AddListener(ResetLevel);

        Refresh();
    }

    public void Refresh()
    {
        bool hasSave = GameSavePaths.Exists(level.Index);

        textPlay.text = hasSave ? "Load" : "Start";
        buttonReset.gameObject.SetActive(hasSave);
    }

    private void Play()
    {
        onPlay?.Invoke(level);
    }

    private void ResetLevel()
    {
        onReset?.Invoke(level);
        Refresh();
    }
}