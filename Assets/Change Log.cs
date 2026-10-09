
public class ChangeLog
{
    /*
    We can share changes here

    October 9, 2026 - Michael
    Add circuit theme items and recipes
    - Renamed Iron Ore to Silicon and Iron Plate to Wafer
    - Added items: Copper, Copper Trace, Chip, Circuit Board
    - Added recipes: Mine Copper, Draw Trace, Etch Chip, Print Board
    - Removed the duplicate Mine Iron Ore line from the Miner recipes
    - Level Collection and Shop Collection only updated for the new item names

    October 1, 2026 - Michael (temp test harnesses)
    Two disposable self-installing scripts in Assets/Scripts/Tools, needed to actually see
    the sim run while the real recipe UI and HUD are still missing. Both self-install via
    [RuntimeInitializeOnLoadMethod] and nothing else references them. TO REMOVE: delete the
    file (and its .meta). Harjap: your real recipe UI / HUD / level-complete popup replace these.
    - DebugAutoRecipe.cs: each tick, gives any placed processor with Recipe == null its FIRST
      recipe (SetRecipe(0)). Without it no processor ever runs, because SetRecipe() is never
      called until the recipe UI exists.
    - DebugHudOverlay.cs: top-right debug panel (GUI.depth = -1000) showing each processor's
      recipe/state, delivery goal progress, completion %, time, and ">>> LEVEL COMPLETE! <<<".

    Sept 30, 2026 - Michael (code TEST-FIXES only)
    Temporary, reversible fixes in core files so a miner -> smelter -> delivery
    chain actually runs while the real recipe UI / HUD are still missing. Matthew: please
    review, both of these. Each spot is marked "TEST-FIX (Michael, Sept 30)"
    with its own revert note.
    - Machine Nodes.cs, IsAllFull()/IsAllEmpty(). Revert = delete the two "continue" lines.
    - Machine Operations.cs, TryConnect(): the two connection calls were passed the wrong
      machine/index pair; swapped them. Revert = swap the two argument pairs back.



    Sept 29, 2026 - Matthew
    - Added starting wires to test level
    - Basically made connecting wires much easier
    - MachineNode now know what side of the machine it is on
    - Some helper method for working with MachineNode. Added to MachineNode, MachineNodes, and Machine.
    - Modified Machine Operations for Connecting Machines. Fixed a bug. Interface is now good. Super easy to use.
    - I think some small fixes like spacing and possible bug fixes
    - Added "Notes for Adding Content (Michael)" to Claude Readme

    Sept 29, 2026 - Harjap
    - Building Display/Interaction scripts (my own folder, Assets/Scripts/Display)
    - Found a bug: connecting a Miner (no recipe assigned yet) to Delivery via a wire spams
    "ArgumentOutOfRangeException: Index was out of range" in the Console every frame.
    Doesn't crash, just floods the log. Repro: place a Miner and Delivery next to each
    other, connect them with a wire.

    */
}
