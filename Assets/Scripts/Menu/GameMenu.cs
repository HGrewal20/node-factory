using UnityEngine;

// The main menu and pause menu.
// Nothing starts the level on its own anymore - you have to press
// "Start Game" here first (GameDisplay used to do that automatically,
// that line got removed). Escape opens/closes the pause menu once a
// level is running.
public class GameMenu : MonoBehaviour
{
    // GameDisplay and GameInteraction check this so they know to stop
    // drawing/responding to clicks while the pause menu is open.
    public static bool IsPaused { get; private set; }

    void OnGUI()
    {
        GameData data = GameData.INSTANCE;

        if ( !data.IsPlaying() )
        {
            IsPaused = false; // no such thing as "paused" when not even playing
            DrawMainMenu(data);
            return;
        }

        HandleEscape();

        if (IsPaused)
            DrawPauseMenu(data);
    }

    private void HandleEscape()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            IsPaused = !IsPaused;
            e.Use();
        }
    }

    private void DrawMainMenu(GameData data)
    {
        Rect box = CenteredBox(240, 150);

        GUILayout.BeginArea(box, GUI.skin.box);
        GUILayout.Label("Node Factory");

        if ( GUILayout.Button("Start Game") )
            data.LevelStart( LevelCollection.Get(0) );

        if ( GUILayout.Button("Quit") )
            Application.Quit();

        GUILayout.EndArea();
    }

    private void DrawPauseMenu(GameData data)
    {
        Rect box = CenteredBox(240, 170);

        GUILayout.BeginArea(box, GUI.skin.box);
        GUILayout.Label("Paused");

        if ( GUILayout.Button("Resume") )
            IsPaused = false;

        if ( GUILayout.Button("Exit to Main Menu") )
        {
            data.LevelStop();
            IsPaused = false;
        }

        if ( GUILayout.Button("Quit Game") )
            Application.Quit();

        GUILayout.EndArea();
    }

    private Rect CenteredBox(float w, float h)
    {
        return new Rect((Screen.width - w) / 2, (Screen.height - h) / 2, w, h);
    }
}
