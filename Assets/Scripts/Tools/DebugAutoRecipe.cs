using UnityEngine;

// TEMPORARY TEST HARNESS - Michael, Oct 1.
//
// The recipe-assignment UI does not exist yet, so MachineProcessor.SetRecipe()
// is never called and processors sit at Recipe == null doing nothing. This makes
// the level untestable. Until Harjap's HUD can assign recipes, this script gives
// every placed processor its FIRST recipe automatically so the sim actually runs.
//
// It self-installs at runtime (no scene/inspector setup) and nothing else
// references it. TO REMOVE: delete this single file. That's it.
public class DebugAutoRecipe : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        GameObject go = new GameObject("DebugAutoRecipe");
        go.AddComponent<DebugAutoRecipe>();
        DontDestroyOnLoad(go);
    }

    void Update()
    {
        GameData data = GameData.INSTANCE;
        if ( !data.IsPlaying() )
            return;

        Machines used = data.machinesUsed;
        int count = used.Count();
        for (int i = 0; i < count; i++)
        {
            if ( used.Get(i) is not MachineProcessor processor )
                continue;

            if ( processor.Recipe != null )
                continue;   // already has one - leave it alone

            Recipes recipes = processor.GetRecipes();
            if ( recipes != null && recipes.Count() > 0 )
                processor.SetRecipe(0);
        }
    }
}
