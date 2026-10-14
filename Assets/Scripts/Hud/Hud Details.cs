using System;
using UnityEngine;

public class HudDetails : MonoBehaviour
{
    // References
    [SerializeField] private HudDetailsLine[] lines;

    // Colors
    private static readonly Color COLOR_NAME   = Color.white;
    private static readonly Color COLOR_HEADER = new Color(0.3f, 0.7f, 1.0f);
    private static readonly Color COLOR_NORMAL = Color.white;
    private static readonly Color COLOR_EMPTY  = Color.gray;
    private static readonly Color COLOR_FULL   = new Color(0.3f, 1.0f, 0.3f);

    // State
    private int nextLine;

    // Set
    public void Set(Machine machine, MachineNode node)
    {
        // No machine
        if (machine == null)
        {
            gameObject.SetActive(false);
            return;
        }

        // If Disabled Node Only Show The Machine
        if (node != null && !node.IsEnabled)
            node = null;

        // Begin Display
        gameObject.SetActive(true);
        nextLine = 0;

        // Handle Type
        if (node == null)   Set(machine);
        else                Set(node   );

        // Lines - Empty
        for (int i = nextLine; i < lines.Length; i++)
            lines[i].Hide();
    }

    public void Set(Machine machine)
    {
        MachineType type = MachineCollection.Get(machine.MachineTypeIndex);

                                                    AddLine     (type.Name, COLOR_NAME      );
        if (machine is MachineProcessor processor)  AddProcessor(processor);
                                                    AddNodes    ("Inputs"  , machine.inputs );
                                                    AddNodes    ("Outputs" , machine.outputs);
    }

    private void AddProcessor(MachineProcessor processor)
    {
        // State
        string state = processor.State switch
        {
            MachineProcessor.STATE_RUNNING => "Running",
            MachineProcessor.STATE_BLOCKED => "Blocked",
            _ => "Idle"
        };

        Color color = processor.State switch
        {
            MachineProcessor.STATE_RUNNING => COLOR_FULL,
            MachineProcessor.STATE_BLOCKED => COLOR_EMPTY,
            _ => COLOR_NORMAL
        };

        AddLine(state, color);

        // Recipe
        Recipe recipe = processor.Recipe;

        if (recipe == null)
        {
            AddLine("Recipe: None", COLOR_EMPTY);
            return;
        }

        AddLine($"Recipe: {recipe.Name}", COLOR_NORMAL);

        // Time
        AddLine($"Time: {processor.TimeRemaining:0.0} / {recipe.TimeSecs:0.0} sec", COLOR_NORMAL);
    }

    private void AddNodes(string title, MachineNodes nodes)
    {
        bool isNode = false;

        for (int i = 0; i < nodes.Count(); i++)
        {
            MachineNode node = nodes.Get(i);
            if ( !node.IsEnabled )
                continue;

            // Header
            if ( !isNode )
            {
                AddLine(title, COLOR_HEADER);
                isNode = true;
            }

            // Node
            string connection = node.IsConnected() ? "+ " : "- ";
            AddNode(node, connection);
        }
    }

    public void Set(MachineNode node)
    {
        // Title
        AddLine(node.IsInput ? "Input Node" : "Output Node", COLOR_NAME);

        // Connection
        bool connected = node.IsConnected();
        if (connected)  AddLine("Connected"   , COLOR_FULL );
        else            AddLine("Disconnected", COLOR_EMPTY);

        // Item
        AddNode(node, "");
    }

    // Add
    private void AddLine(string value, Color color)
    {
        if (nextLine < lines.Length)
        {
            lines[nextLine].Set(value, color);
            nextLine++;
        }
    }

    private void AddNode(MachineNode node, String connection)
    {
        Item   item       = node.ItemCurrent ?? node.ItemAllowed;
        string itemName   = (item != null) ? item.Name : "Any";

        string value      = $"{connection}{node.Amount} / {node.Max} {itemName}";

        Color color = COLOR_NORMAL;

             if ( node.IsEmpty() ) color = COLOR_EMPTY;
        else if ( node.IsFull () ) color = COLOR_FULL;

        AddLine(value, color);
    }
}