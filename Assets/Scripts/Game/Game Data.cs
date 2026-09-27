using System;

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

    // Constructor
    private GameData(){}

    // Clear
    public void Clear()
    {
        map              .Clear();
        machinesUsed     .Clear();
        machinesAvailable.Clear();
        delivered        .Clear();
        Level            = null;
        TimeElapsed      = 0f;
        Completion       = 0f;
    }

    // Game Tick
    public void GameTick()
    {
        if (Level == null)
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
            int todoNotifyLevelFinished;
        }
    }

    // Level
    public void LevelStart(Level newLevel)
    {
        LevelStop();

        map              .Start(newLevel.mapSize         );
        machinesUsed     .Clear();
        machinesAvailable.Set  (newLevel.machinesStarting);
        delivered        .Set  (newLevel.deliveryStarting);
        Level            = newLevel;
        TimeElapsed      = 0f;
    }
    public void LevelStop()
    {
        map              .Clear();
        machinesUsed     .Clear();
        machinesAvailable.Clear();
        delivered        .Clear();
        Level            = null;
        TimeElapsed      = 0f;
        Completion       = 0f;
    }
    public bool IsPlaying()
    {
        return (Level != null);
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

    // IO
    public void Save(IOWriter writer)
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
    public void Load(IOReader reader)
    {
        map              .Load(reader);
        machinesUsed     .Load(reader);
        machinesAvailable.Load(reader);
        delivered        .Load(reader);
        Level            = reader.ReadLevel();
        TimeElapsed      = reader.ReadFloat();
        Completion       = reader.ReadFloat();
        IsCompleted      = reader.ReadBool ();

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
