using UnityEngine;
using UnityEngine.InputSystem;

public class GameInteraction : MonoBehaviour
{
    private static readonly MachineType[] HOTBAR = new MachineType[]
    {
        MachineCollection.PROCESSOR_MINER,
        MachineCollection.PROCESSOR_SMELTER,
        MachineCollection.DELIVERY,
        MachineCollection.SPLITTER_2,
        MachineCollection.MERGER_2,
        MachineCollection.FLIPPER_2,
        MachineCollection.JUNCTION_A,
        MachineCollection.JUNCTION_B,
    };

    private byte selectedType = MachineCollection.PROCESSOR_MINER.Index;

    public void SelectMachine(byte machineTypeIndex)
    {
        selectedType = machineTypeIndex;
    }

    public byte SelectedMachineType => selectedType;

    private const float PAN_SPEED  = 20f;
    private const float ZOOM_SPEED = 0.1f;

    void Update()
    {
        GameData data = GameData.INSTANCE;
        if (!data.IsPlaying() || data.IsPaused || HudLevelCompleted.IsShowing)
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
        if (!data.IsPlaying() || data.IsPaused || HudLevelCompleted.IsShowing)
            return;

        Event e         = Event.current;
        bool  isOverHud = (UnityEngine.EventSystems.EventSystem.current != null) && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

        // Mouse scroll wheel zooms in/out.
        if (e.type == EventType.ScrollWheel && !isOverHud)
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
        if (e.type == EventType.MouseDown && !isOverHud)
        {
            Vector2Int gridPos = data.map.mapView.ScreenToGridInt(e.mousePosition);

            if ( InBounds(data, gridPos) )
            {
                if (e.button == 0)   LeftClick (data, gridPos);
                if (e.button == 1)   RightClick(data, gridPos);
            }

            e.Use();
        }
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
