using System;
using UnityEngine;

public class GameData
{
    // Instance
    public static readonly GameData INSTANCE = new GameData();

    // State
    public readonly byte STATE_NONE      = 0;
    public readonly byte STATE_PLAYING   = 1;
    public readonly byte STATE_COMPLETED = 2;
    public readonly byte STATE_FAILED    = 3;

    // Member Variables
    public readonly Map             map               = new Map           ();
    public readonly Machines        machinesUsed      = new Machines      ();
    public readonly MachineAmounts  machinesAvailable = new MachineAmounts();
    public readonly ItemAmounts     delivered         = new ItemAmounts   ();
    public          Level           Level             { get; private set; }     // Can be null
    public          byte            State             { get; private set; }
    public          int             Score             { get; private set; }
    public          float           TimeElapsed       { get; private set; }
    public          float           TimeRemaining     { get; private set; }
    public          float           Completion        { get; private set; }
    public          bool            IsPaused          { get; private set; }

    // Constructor
    private GameData(){}

    // Clear
    private void Clear()
    {
        map              .Clear();
        machinesUsed     .Clear();
        machinesAvailable.Clear();
        delivered        .Clear();
        Level            = null;
        State            = STATE_NONE;
        Score            = 0;
        TimeElapsed      = 0f;
        TimeRemaining    = 0f;
        Completion       = 0f;
        IsPaused         = false;
    }

    // Game Tick
    public void GameTick()
    {
        if (Level == null || IsPaused || !CanPlay())
            return;

        map           .GameTick();
        machinesUsed  .GameTick();
        TimeElapsed   += GameConstants.FRAME_TIME;
        UpdateTimeRemaining();
        UpdateCompletion   ();
    }
    private void UpdateTimeRemaining()
    {
        if ( !IsStatePlaying() )
            return;

        TimeRemaining -= GameConstants.FRAME_TIME;
        if (TimeRemaining < 0f)
        {
            TimeRemaining = 0f;
            State         = STATE_FAILED;

            MenuController menu = UnityEngine.Object.FindAnyObjectByType<MenuController>(FindObjectsInactive.Include);
            menu.OpenGameOver();
        }
    }
    private void UpdateCompletion   ()
    {
        if ( !IsStatePlaying() )
            return;

        ItemRequirements requirements = ItemRequirements.INSTANCE;
        requirements.Set(delivered, Level.deliveryGoal);
        Completion = requirements.GetCompletion();

        if (Completion == 1f)
        {
            State = STATE_COMPLETED;
            Score = 20 * (int) TimeRemaining;

            HudLevelCompleted hud = UnityEngine.Object.FindAnyObjectByType<HudLevelCompleted>(FindObjectsInactive.Include);
            hud.Show(); 
        }
    }

    // Level
    public void LevelStart(Level newLevel)
    {
        Clear();

        map              .Start(newLevel.mapSize         );
        machinesUsed     .Clear();
        machinesAvailable.Set  (newLevel.machinesStarting);
        delivered        .Set  (newLevel.deliveryStarting);
        Level            = newLevel;
        State            = STATE_PLAYING;
        Score            = 0;
        TimeElapsed      = 0f;
        TimeRemaining    = newLevel.TimeSec;
    }
    public void LevelStop()
    {
        Clear();
    }
    public bool IsPlaying()
    {
        return (Level != null);
    }

    // Pause & Resume
    public void Pause()
    {
        if ( IsPlaying() )
            IsPaused = true;
    }

    public void Resume()
    {
        IsPaused = false;
    }

    // Get State
    public bool IsStateNone     () { return State == STATE_NONE;        }
    public bool IsStatePlaying  () { return State == STATE_PLAYING;     }
    public bool IsStateCompleted() { return State == STATE_COMPLETED;   }
    public bool IsStateFailed   () { return State == STATE_FAILED;      }

    public bool CanPlay         () { return IsStatePlaying() || IsStateCompleted(); }

    // Link
    private void Link()
    {
        GameLinks links = GameLinks.INSTANCE;
        links.Fill(this);

        map         .Link(links);
        machinesUsed.Link(links);

        links.Clear();
    }

    // IO - Interface
    public void Save()                      // Save Current Level
    {
        if (Level != null)
            Save( GameSavePaths.GetLevelPath(Level.Index) );
    }

    public void Load(int levelIndex)        // Load the Level if it Exists, Otherwise Start a New One
    {
        string path = GameSavePaths.GetLevelPath(levelIndex);

        if (GameSavePaths.Exists(levelIndex))   Load(path);
        else                                    LevelStart(LevelCollection.Get((byte)levelIndex));
    }

    // IO - Paths
    private void Save(string path) { using (IOWriterFile writer = new IOWriterFile(path)) Save(writer); }
    private void Load(string path) { using (IOReaderFile reader = new IOReaderFile(path)) Load(reader); }

    // IO - Workers
    private void Save(IOWriter writer)
    {
        map              .Save(writer);
        machinesUsed     .Save(writer);
        machinesAvailable.Save(writer);
        delivered        .Save(writer);
        writer           .WriteLevel(Level        );
        writer           .WriteByte (State        );
        writer           .WriteInt  (Score        );
        writer           .WriteFloat(TimeElapsed  );
        writer           .WriteFloat(TimeRemaining);
        writer           .WriteFloat(Completion   );
    }

    private void Load(IOReader reader)
    {
        Clear();

        map              .Load(reader);
        machinesUsed     .Load(reader);
        machinesAvailable.Load(reader);
        delivered        .Load(reader);
        Level            = reader.ReadLevel();
        State            = reader.ReadByte ();
        Score            = reader.ReadInt  ();
        TimeElapsed      = reader.ReadFloat();
        TimeRemaining    = reader.ReadFloat();
        Completion       = reader.ReadFloat();
        IsPaused         = false;

        Link(); // Final post processing
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("GameData", indentation);
        indentation++;

        map              .Debug(indentation);
        machinesUsed     .Debug(indentation);
        machinesAvailable.Debug(indentation);
        delivered        .Debug(indentation);

        String levelIndex = (Level == null) ? "null" : "" + Level.Index;
        DebugFile.Log("Level:         " + levelIndex   , indentation);
        DebugFile.Log("State:         " + State        , indentation);
        DebugFile.Log("Score:         " + Score        , indentation);
        DebugFile.Log("TimeElapsed:   " + TimeElapsed  , indentation);
        DebugFile.Log("TimeRemaining: " + TimeRemaining, indentation);
        DebugFile.Log("Completion:    " + Completion   , indentation);
        DebugFile.Log("IsPaused:      " + IsPaused     , indentation);
    }
}
