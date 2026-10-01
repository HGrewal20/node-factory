using System.Text;
using UnityEngine;

// TEMPORARY TEST OVERLAY - Michael, Oct 1.
//
// There is no HUD yet, so nothing on screen shows whether processors are producing
// or the level has been won (GameData tracks it, but the level-finished
// notification is still a stub). This draws a small top-right panel so we can verify
// recipes are running and the level completes.
//
// Self-installs at runtime and nothing else references it. TO REMOVE: delete this
// single file. Harjap: your real HUD / level-complete popup replaces this.
public class DebugHudOverlay : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        GameObject go = new GameObject("DebugHudOverlay");
        go.AddComponent<DebugHudOverlay>();
        DontDestroyOnLoad(go);
    }

    void OnGUI()
    {
        GameData data = GameData.INSTANCE;
        if ( !data.IsPlaying() || data.Level == null )
            return;

        // Draw on top of everything (the map's OnGUI would otherwise paint over this).
        // Lower GUI.depth = rendered in front. Harjap's display leaves it at the default 0.
        GUI.depth = -1000;

        StringBuilder sb = new StringBuilder();

        // Processors: assigned recipe + state, so we can see items actually being made.
        sb.AppendLine("PROCESSORS");
        Machines used = data.machinesUsed;
        bool anyProcessor = false;
        for (int i = 0; i < used.Count(); i++)
        {
            if ( used.Get(i) is not MachineProcessor processor )
                continue;

            anyProcessor  = true;
            string typeName = MachineCollection.Get(processor.MachineTypeIndex).Name;
            string recipe   = (processor.Recipe == null) ? "<no recipe>" : processor.Recipe.Name;
            string state    = processor.IsStateIdle   () ? "Idle"
                            : processor.IsStateRunning() ? $"Running {processor.TimeRemaining:F1}s"
                            :                              "Blocked (output full)";
            sb.AppendLine($"  {typeName}: {recipe} [{state}]");
        }
        if ( !anyProcessor )
            sb.AppendLine("  (none placed)");

        // Delivery goal progress.
        sb.AppendLine("GOAL");
        ItemAmounts goal = data.Level.deliveryGoal;
        for (int i = 0; i < goal.Count(); i++)
        {
            ItemAmount g    = goal.Get(i);
            ItemAmount have = data.delivered.Get(g.Item);
            int amount      = (have == null) ? 0 : have.Amount;
            sb.AppendLine($"  {g.Item.Name}: {amount} / {g.Amount}");
        }

        sb.AppendLine($"Completion: {data.Completion * 100f:F0}%   Time: {data.TimeElapsed:F1}s");
        if ( data.IsCompleted )
            sb.AppendLine(">>> LEVEL COMPLETE! <<<");

        // Top-right panel (Harjap's control hint is top-left).
        float w = 330f;
        float h = 190f;
        Rect rect = new Rect(Screen.width - w - 10f, 10f, w, h);

        GUI.color = new Color(0f, 0f, 0f, 0.6f);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);

        GUI.color        = Color.white;
        GUI.contentColor = data.IsCompleted ? new Color(0.45f, 1f, 0.55f) : Color.white;
        GUI.Label(new Rect(rect.x + 8f, rect.y + 6f, rect.width - 16f, rect.height - 12f), sb.ToString());
        GUI.contentColor = Color.white;
    }
}
