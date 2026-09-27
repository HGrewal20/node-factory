public class MachineProcessor : Machine
{
    // Constants
    public const byte STATE_IDLE    = 0;
    public const byte STATE_RUNNING = 1;
    public const byte STATE_BLOCKED = 2;

    // Member Variables
    public byte   State          { get; private set; }
    public float  TimeRemaining  { get; private set; }
    public Recipe Recipe         { get; private set; }

    // Constructor
    public MachineProcessor(byte machineTypeIndex, long id)
        : base(machineTypeIndex, id)
    {
        SetStandard( Height() );
        DisableAllNodes();
    }

    // Game Tick
    public override void GameTick()
    {
        base.GameTick();

        switch (State)
        {
            case STATE_IDLE:        UpdateIdle   ();    break;
            case STATE_RUNNING:     UpdateRunning();    break;
            case STATE_BLOCKED:     UpdateBlocked();    break;
        }
    }
    private void UpdateIdle()
    {
        // If No Recipe -> Do Nothing
        if (Recipe == null)
            return;

        // If Not Enough Inputs -> Do Nothing
        if ( !inputs.IsAllFull() )
            return;

        // Consume Inputs
        inputs.InputEmpty();

        // Start Timer
        TimeRemaining = Recipe.TimeSecs;

        // Switch to Running
        State = STATE_RUNNING;
    }
    private void UpdateRunning()
    {
        TimeRemaining -= GameConstants.FRAME_TIME;
        if (TimeRemaining <= 0)
        {
            TimeRemaining = 0;
            State         = STATE_BLOCKED;
            UpdateBlocked();        // Try Clear Immediately -> For Constant Running
        }
    }
    private void UpdateBlocked()
    {
        // If Output Not Empty -> Do Nothing
        if ( !outputs.IsAllEmpty() )
            return;

        // Push into Outputs
        outputs.Output(Recipe.outputs);

        // Switch to Idle
        State = STATE_IDLE;

        // Try Start Immediately -> For Constant Running
        UpdateIdle();
    }

    // Recipe - Warning! This is the index relative to this classes recipes not the global recipes collection
    public void SetRecipe(int index)
    {
        ClearRecipe();
        Recipe = GetRecipes().Get(index);
        inputs .SetAccepts(Recipe.inputs );
        outputs.SetAccepts(Recipe.outputs);
    }

    public void ClearRecipe()
    {
        Recipe = null;
        State  = STATE_IDLE;
        DisableAllNodes();
    }

    public Recipes GetRecipes()
    {
        if ( IsMiner  () )  return RecipeCollection.RECIPES_MINER;
        if ( IsSmelter() )  return RecipeCollection.RECIPES_SMELTER;
        else                return null;
    }

    // Getters
    public bool IsMiner  () { return (MachineTypeIndex == MachineCollection.PROCESSOR_MINER  .Index); }
    public bool IsSmelter() { return (MachineTypeIndex == MachineCollection.PROCESSOR_SMELTER.Index); }

    private int Height()
    {
        if ( IsMiner  () )  return 3;
        if ( IsSmelter() )  return 3;
        else                return 3;
    }

    public bool IsStateIdle   () { return (State == STATE_IDLE   ); }
    public bool IsStateRunning() { return (State == STATE_RUNNING); }
    public bool IsStateBlocked() { return (State == STATE_BLOCKED); }

    // IO
    public override void Save(IOWriter writer)
    {
        base  .Save       (writer       );
        writer.WriteByte  (State        );
        writer.WriteFloat (TimeRemaining);
        writer.WriteRecipe(Recipe       );
    }
    public override void Load(IOReader reader)
    {
        base          .Load(reader);
        State         = reader.ReadByte  ();
        TimeRemaining = reader.ReadFloat ();
        Recipe        = reader.ReadRecipe();
    }

    // Debug
    public override void Debug(int indentation)
    {
        byte recipeIndex = (Recipe == null) ? (byte) 255 : Recipe.Index;

        DebugFile.Log("MachineProcessor", indentation);
        indentation++;

        base.Debug(indentation);
        DebugFile.Log("State:         " + State        , indentation);
        DebugFile.Log("TimeRemaining: " + TimeRemaining, indentation);
        DebugFile.Log("recipeIndex:   " + recipeIndex  , indentation);
    }
}
