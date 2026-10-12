using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuLevelEntry : MonoBehaviour
{
    // Member Variables
    [SerializeField] private TMP_Text       textName;
    [SerializeField] private TMP_Text       textSize;
    [SerializeField] private TMP_Text       textStatus;
    [SerializeField] private Button         buttonPlay;
    [SerializeField] private Button         buttonReset;
                     private Level          level;
                     private Action<Level>  onPlay;
                     private Action<Level>  onReset;

    public void Setup(Level level, MenuController menuController)
    {
        this.level = level;
        onPlay  = menuController.PlayLevel;
        onReset = menuController.ResetLevel;

        textName.text = $"{level.Index + 1:00} - {level.Name}";
        textSize.text = $"{level.mapSize.Width} x {level.mapSize.Height}";

        buttonPlay .onClick.RemoveAllListeners();
        buttonReset.onClick.RemoveAllListeners();

        buttonPlay .onClick.AddListener(Play      );
        buttonReset.onClick.AddListener(ResetLevel);

        Refresh();
    }

    public void Refresh()
    {
        bool hasSave = GameSavePaths.Exists(level.Index);

        textStatus.text = hasSave ? "Saved" : "New";
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