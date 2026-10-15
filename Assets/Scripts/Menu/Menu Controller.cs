using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    // Member Variables
    [SerializeField] private DisplayInteraction displayInteraction;
    [SerializeField] private GameObject         menuMain;
    [SerializeField] private GameObject         menuPause;
    [SerializeField] private GameObject         menuLevels;
    [SerializeField] private GameObject         menuInfo;
    [SerializeField] private GameObject         menuGameOver;
    [SerializeField] private Hud                hud;
    [SerializeField] private HudRecipes         hudRecipes;
    [SerializeField] private MenuGameOver       gameOver;
                     private GameObject         currentMenu;

    public bool IsGameOverShowing => currentMenu == menuGameOver;

    private void Start()
    {
        Refresh();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleEscape();
        }

        GameData data = GameData.INSTANCE;

        if (IsGameOverShowing || HudLevelCompleted.IsShowing)
            return;

        if (data.IsPlaying())
        {
            if (data.IsPaused)
            {
                if (currentMenu != menuPause)
                    Show(menuPause);
            }
            else if (currentMenu != null)
            {
                Show(null);
            }
        }
        else if (currentMenu == null || currentMenu == menuPause)
        {
            Show(menuMain);
        }
    }

    private void HandleEscape()
    {
        
        if (IsGameOverShowing || HudLevelCompleted.IsShowing)
            return;

        GameData data = GameData.INSTANCE;
        if (data.IsPlaying())
        {
            if (data.IsPaused)
            {
                Resume();
                return;
            }

            if ( hudRecipes        .TryClose          () )  return;
            if ( displayInteraction.TryUnselectMachine() )  return;

            Pause();
        }
        else if (currentMenu != menuMain)
        {
            OpenMain();
        }
    }

    private void Show(GameObject menu)
    {
        menuMain    .SetActive(menu == menuMain    );
        menuPause   .SetActive(menu == menuPause   );
        menuLevels  .SetActive(menu == menuLevels  );
        menuInfo    .SetActive(menu == menuInfo    );
        menuGameOver.SetActive(menu == menuGameOver);

        currentMenu = menu;

        RefreshHud();
    }

    private void RefreshHud()
    {
        GameData data = GameData.INSTANCE;

        bool visible = data.IsPlaying() && !data.IsPaused && !IsGameOverShowing;
        hud.gameObject.SetActive(visible);
    }

    public void Refresh()
    {
        GameData data = GameData.INSTANCE;

        if (!data.IsPlaying())  Show(menuMain );
        else if (data.IsPaused) Show(menuPause);
        else                    Show(null     );
    }

    public void OpenMain()
    {
        Show(menuMain);
    }

    public void OpenLevels()
    {
        Show(menuLevels);
    }

    public void OpenInfo()
    {
        Show(menuInfo);
    }

    public void PlayLevel(Level level)
    {
        GameData.INSTANCE.Load(level.Index);
        hud.ResetHud();
        Refresh();
    }

    public void StartLevel(Level level)
    {
        GameData.INSTANCE.LevelStart(level);
        hud.ResetHud();
        Refresh();
    }

    public void ResetLevel(Level level)
    {
        GameSavePaths.Delete(level.Index);
    }

    public void RestartLevel()
    {
        Level level = GameData.INSTANCE.Level;
        if (level == null)
            return;

        ResetLevel(level);
        StartLevel(level);
    }

    public void Pause()
    {
        if (HudLevelCompleted.IsShowing || IsGameOverShowing)
            return;

        GameData.INSTANCE.Pause();
        Refresh();
    }

    public void Resume()
    {
        GameData.INSTANCE.Resume();
        Refresh();
    }

    public void Save()
    {
        GameData.INSTANCE.Save();
    }

    public void ExitLevel()
    {
        GameData.INSTANCE.LevelStop();
        Refresh();
    }

    public void OpenGameOver()
    {
        if ( !GameData.INSTANCE.IsPlaying() )
            return;

        gameOver.Refresh();
        Show(menuGameOver);
    }

    public void Quit()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
    }
}