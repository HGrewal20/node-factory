using UnityEngine;
using UnityEngine.InputSystem;

// Handles mouse clicks on the map:
//   Left click empty cell  -> place the currently selected machine
//   Left click a node      -> try to connect it to whatever is next to it
//   Right click a machine  -> remove it
// Number keys 1-7 pick which machine gets placed. This is a stand-in for
// the real shop UI, which comes later.
public class GameInteraction : MonoBehaviour
{
    // Note: Junction B is left out here. MachineCollection currently gives
    // JUNCTION_A and JUNCTION_B the same Index (4), a known bug in
    // MachineCollection.cs (Matthew's file, not mine to fix). Because of
    // that mix-up, every junction you place behaves like Junction A no
    // matter which one you ask for, so there is no point offering both.
    private static readonly MachineType[] HOTBAR = new MachineType[]
    {
        MachineCollection.PROCESSOR_MINER,
        MachineCollection.PROCESSOR_SMELTER,
        MachineCollection.DELIVERY,
        MachineCollection.SPLITTER_2,
        MachineCollection.MERGER_2,
        MachineCollection.FLIPPER_2,
        MachineCollection.JUNCTION_A,
    };

    private byte selectedType = MachineCollection.PROCESSOR_MINER.Index;

    // Matthew built Scroll()/ZoomChange() into MapView already, nothing
    // was ever calling them though. These just control how fast WASD/
    // arrow keys pan and how much one scroll wheel notch zooms.
    private const float PAN_SPEED  = 20f;
    private const float ZOOM_SPEED = 0.1f;

    void Update()
    {
        GameData data = GameData.INSTANCE;
        if ( !data.IsPlaying() || GameMenu.IsPaused )
            return;

        // This project uses the new Input System package (not the old
        // Input class), so keys are read through Keyboard.current instead.
        Keyboard kb = Keyboard.current;
        if (kb == null)
            return; // no keyboard detected, nothing to do

        Vector2 pan = Vector2.zero;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    pan.y += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  pan.y -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) pan.x += 1f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  pan.x -= 1f;

        if (pan != Vector2.zero)
            data.map.mapView.Scroll(pan * PAN_SPEED * Time.deltaTime);
    }

    void OnGUI()
    {
        GameData data = GameData.INSTANCE;

        // Menu (main menu or pause menu) is showing - don't place stuff,
        // don't react to the hotbar, don't draw the hint over it.
        if ( !data.IsPlaying() || GameMenu.IsPaused )
            return;

        Event e = Event.current;

        // Mouse scroll wheel zooms in/out.
        if (e.type == EventType.ScrollWheel)
        {
            data.map.mapView.ZoomChange(-e.delta.y * ZOOM_SPEED);
            e.Use();
        }

        // Number keys change what left-click places next.
        if (e.type == EventType.KeyDown)
        {
            int number = KeyToNumber(e.keyCode);
            if (number >= 1 && number <= HOTBAR.Length)
                selectedType = HOTBAR[number - 1].Index;
        }

        // Mouse clicks place, connect, or remove.
        if (e.type == EventType.MouseDown)
        {
            Vector2Int gridPos = data.map.mapView.ScreenToGridInt(e.mousePosition);

            if ( InBounds(data, gridPos) )
            {
                if (e.button == 0)   LeftClick (data, gridPos);
                if (e.button == 1)   RightClick(data, gridPos);
            }

            e.Use();
        }

        DrawHint();
    }

    private bool InBounds(GameData data, Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < data.map.mapSize.Width
            && gridPos.y >= 0 && gridPos.y < data.map.mapSize.Height;
    }

    private void LeftClick(GameData data, Vector2Int gridPos)
    {
        Machine machine = data.map.mapMachines.Get(gridPos);

        // Empty cell -> place the selected machine here.
        if (machine == null)
        {
            MachineOperations.TryPlaceMachine(selectedType, gridPos);
            return;
        }

        // Existing machine -> try to connect whichever node is under the click.
        MachineNode node = machine.GetNode(gridPos);
        if (node != null)
            MachineOperations.TryConnect(machine, node);
    }

    private void RightClick(GameData data, Vector2Int gridPos)
    {
        Machine machine = data.map.mapMachines.Get(gridPos);
        if (machine != null)
            MachineOperations.RemoveMachine(machine);
    }

    private void DrawHint()
    {
        string name = MachineCollection.Get(selectedType).Name;

        GUI.Label(new Rect(10, 10, 420, 130),
            "Placing: " + name + "\n" +
            "Left click empty cell: place it\n" +
            "Left click a machine's node: connect it\n" +
            "Right click a machine: remove it\n\n" +
            "1 Miner  2 Smelter  3 Delivery  4 Splitter\n" +
            "5 Merger  6 Flipper  7 Junction\n\n" +
            "WASD/arrows: pan   Scroll wheel: zoom"
        );
    }

    private int KeyToNumber(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Alpha1: return 1;
            case KeyCode.Alpha2: return 2;
            case KeyCode.Alpha3: return 3;
            case KeyCode.Alpha4: return 4;
            case KeyCode.Alpha5: return 5;
            case KeyCode.Alpha6: return 6;
            case KeyCode.Alpha7: return 7;
            default:             return 0;
        }
    }
}
