using System;
using UnityEngine;

public class GameData
{
    // Instance
    public static readonly GameData INSTANCE = new GameData();

    // Member Variables
    public readonly Map             map               = new Map           ();
    public readonly Machines        machinesUsed      = new Machines      ();
    public readonly MachineAmounts  machinesAvailable = new MachineAmounts();
    public readonly ItemAmounts     delivered         = new ItemAmounts   ();
    public          Level           Level             { get; private set; }     // Can be null
    public          float           TimeElapsed       { get; private set; }
    public          float           Completion        { get; private set; }
    public          bool            IsCompleted       { get; private set; }
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
        TimeElapsed      = 0f;
        Completion       = 0f;
        IsCompleted      = false;
        IsPaused         = false;
    }

    // Game Tick
    public void GameTick()
    {
        if (Level == null || IsPaused)
            return;

        map         .GameTick();
        machinesUsed.GameTick();
        TimeElapsed += GameConstants.FRAME_TIME;
        UpdateCompletion();
    }
    private void UpdateCompletion()
    {
        if (IsCompleted)
            return;

        ItemRequirements requirements = ItemRequirements.INSTANCE;
        requirements.Set(delivered, Level.deliveryGoal);
        Completion = requirements.GetCompletion();

        if (Completion == 1f)
        {
            IsCompleted = true;

            HudLevelCompleted hud = UnityEngine.Object.FindAnyObjectByType<HudLevelCompleted>(FindObjectsInactive.Include);
            if (hud != null)
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
        TimeElapsed      = 0f;
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
        writer           .WriteLevel(Level      );
        writer           .WriteFloat(TimeElapsed);
        writer           .WriteFloat(Completion );
        writer           .WriteBool (IsCompleted);
    }

    private void Load(IOReader reader)
    {
        Clear();

        map              .Load(reader);
        machinesUsed     .Load(reader);
        machinesAvailable.Load(reader);
        delivered        .Load(reader);
        Level            = reader.ReadLevel();
        TimeElapsed      = reader.ReadFloat();
        Completion       = reader.ReadFloat();
        IsCompleted      = reader.ReadBool ();
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
        DebugFile.Log("Level:       " + levelIndex , indentation);
        DebugFile.Log("TimeElapsed: " + TimeElapsed, indentation);
        DebugFile.Log("Completion:  " + Completion , indentation);
        DebugFile.Log("IsCompleted: " + IsCompleted, indentation);
    }
}
