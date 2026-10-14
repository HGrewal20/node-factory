using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DisplayInteraction : MonoBehaviour
{
    private static readonly MachineType[] HOTBAR =
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

    private const float PAN_SCREEN_FRACTION_PER_SECOND = 0.75f;
    private const float ZOOM_SCALE_PER_WHEEL_STEP      = 1.10f;
    private const float SCROLL_DELTA_PER_WHEEL_STEP    = 120f;

    [SerializeField] private HudDetails     hudDetails;
    [SerializeField] private MenuController menuController;
                     private byte           selectedType        = MachineCollection.WIRE.Index;
                     public  byte           SelectedMachineType => selectedType;

                     public bool            PreviewVisible      { get; private set; }
                     public bool            PreviewValid        { get; private set; }

                     public Vector2Int      PreviewPosition     { get; private set; }
                     public Vector2Int      PreviewSize         { get; private set; }
                     public Machine         PreviewMachine      { get; private set; }

    public void SelectMachine(byte machineTypeIndex)
    {
        if (GameData.INSTANCE.machinesAvailable.Amount(machineTypeIndex) > 0)
            selectedType = machineTypeIndex;
    }

    public bool TryUnselectMachine()
    {
        if (selectedType == MachineCollection.WIRE.Index)
            return false;

        selectedType = MachineCollection.WIRE.Index;
        return true;
    }

    private void Update()
    {
        PreviewVisible = false;
        PreviewMachine = null;
        hudDetails.Set(null, null);

        // Do Nothing if Not Visible
        GameData data = GameData.INSTANCE;
        if (!data.IsPlaying() || data.IsPaused || HudLevelCompleted.IsShowing || (menuController != null && menuController.IsGameOverShowing))
            return;

        // Update View
        MapView view = data.map.mapView;
        view.GameTick();

        // Update Keyboard
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            HandleHotbar(keyboard);
            HandlePan   (keyboard, view);
        }

        // Block if Mouse Over Hud
        Mouse mouse = Mouse.current;
        if (mouse == null || IsOverHud())
            return;

        // Get Information
        Vector2     mouseScreen = mouse.position.ReadValue();
        Vector2     guiPosition = new Vector2(mouseScreen.x, Screen.height - mouseScreen.y);
        Vector2Int  grid        = view.ScreenToGridInt(guiPosition);

        // Details
        Machine     hoveredMachine = InBounds(data, grid) ? data.map.mapMachines.Get(grid) : null;
        MachineNode hoveredNode    = hoveredMachine?.GetNode(grid);
        hudDetails.Set(hoveredMachine, hoveredNode);

        // Machine Preview
        if (selectedType != MachineCollection.WIRE.Index &&
            GameData.INSTANCE.machinesAvailable.Amount(selectedType) > 0 &&
            mouseScreen.x >= 0 && mouseScreen.x < Screen.width &&
            mouseScreen.y >= 0 && mouseScreen.y < Screen.height)
        {
            PreviewVisible  = true;
            PreviewPosition = grid;
            PreviewSize     = MachineConstructor.GetSize(selectedType);

            PreviewMachine = MachineConstructor.Create(selectedType, -1);
            PreviewMachine.SetPos(grid);

            PreviewValid = grid.x >= 0
                        && grid.y >= 0
                        && grid.x + PreviewSize.x <= data.map.mapSize.Width
                        && grid.y + PreviewSize.y <= data.map.mapSize.Height;

            if (PreviewValid)
            {
                for (int y = grid.y; y < grid.y + PreviewSize.y && PreviewValid; y++)
                    for (int x = grid.x; x < grid.x + PreviewSize.x; x++)
                        if (data.map.mapMachines.Get(x, y) != null)
                        {
                            PreviewValid = false;
                            break;
                        }
            }
        }

        // Mouse Wheel - Zoom
        float wheel = mouse.scroll.ReadValue().y;
        if (wheel != 0f)
        {
            float steps = (wheel / SCROLL_DELTA_PER_WHEEL_STEP);
            view.ChangePixelSize(Mathf.Pow(ZOOM_SCALE_PER_WHEEL_STEP, steps));
        }

        // Allow Unselecting Machines Even if The Mouse Is Not Over The Map
        if (mouse.rightButton.wasPressedThisFrame && TryUnselectMachine())
            return;

        // Mouse Buttons
        if (!InBounds(data, grid))
            return;

             if (mouse. leftButton.wasPressedThisFrame)      LeftClick(data, grid);
        else if (mouse.rightButton.wasPressedThisFrame)     RightClick(data, grid);
    }

    private static void HandlePan(Keyboard keyboard, MapView view)
    {
        Vector2 direction = Vector2.zero;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey   .isPressed)  direction.y += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey .isPressed)  direction.y -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)  direction.x += 1f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey .isPressed)  direction.x -= 1f;

        if (direction.sqrMagnitude > 0f)
            view.Scroll(direction.normalized * PAN_SCREEN_FRACTION_PER_SECOND * Time.deltaTime);
    }

    private void HandleHotbar(Keyboard keyboard)
    {
        if (keyboard.digit1Key.wasPressedThisFrame) SelectHotbar(0);
        if (keyboard.digit2Key.wasPressedThisFrame) SelectHotbar(1);
        if (keyboard.digit3Key.wasPressedThisFrame) SelectHotbar(2);
        if (keyboard.digit4Key.wasPressedThisFrame) SelectHotbar(3);
        if (keyboard.digit5Key.wasPressedThisFrame) SelectHotbar(4);
        if (keyboard.digit6Key.wasPressedThisFrame) SelectHotbar(5);
        if (keyboard.digit7Key.wasPressedThisFrame) SelectHotbar(6);
        if (keyboard.digit8Key.wasPressedThisFrame) SelectHotbar(7);
    }

    private void SelectHotbar(int index)
    {
        SelectMachine( HOTBAR[index].Index );
    }

    private static bool IsOverHud()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    private static bool InBounds(GameData data, Vector2Int grid)
    {
        return grid.x >= 0 && grid.x < data.map.mapSize.Width
            && grid.y >= 0 && grid.y < data.map.mapSize.Height;
    }

    private void LeftClick(GameData data, Vector2Int grid)
    {
        Machine machine = data.map.mapMachines.Get(grid);
        if (machine == null)
        {
            if (selectedType != MachineCollection.WIRE.Index)
            {
                MachineOperations.TryPlaceMachine(selectedType, grid);

                // Unselect if no more machines left
                if (data.machinesAvailable.Amount(selectedType) < 1)
                    selectedType = MachineCollection.WIRE.Index;
            }

            return;
        }

        MachineNode node = machine.GetNode(grid);
        if (node != null && node.IsEnabled)
            MachineOperations.TryConnect(machine, node);
    }

    private static void RightClick(GameData data, Vector2Int grid)
    {
        Machine machine = data.map.mapMachines.Get(grid);
        if (machine != null)
            MachineOperations.RemoveMachine(machine);
    }
}
