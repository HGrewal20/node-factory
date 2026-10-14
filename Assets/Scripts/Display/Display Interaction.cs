using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// New Input System only. Mouse.position is converted to the top-left
/// convention used by MapView before placement and connection checks.
/// </summary>
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

    // Fraction of the visible screen traversed per second
    private const float PAN_SCREEN_FRACTION_PER_SECOND = 0.75f;
    private const float ZOOM_SCALE_PER_WHEEL_STEP      = 1.10f;
    private const float SCROLL_DELTA_PER_WHEEL_STEP    = 120f;

    [SerializeField] private MenuController menuController;
                     private byte           selectedType        = MachineCollection.PROCESSOR_MINER.Index;
                     public  byte           SelectedMachineType => selectedType;

    public void SelectMachine(byte machineTypeIndex)
    {
        selectedType = machineTypeIndex;
    }

    private void Update()
    {
        GameData data = GameData.INSTANCE;
        if (!data.IsPlaying() || data.IsPaused || HudLevelCompleted.IsShowing || (menuController != null && menuController.IsGameOverShowing))
            return;

        MapView view = data.map.mapView;
        view.GameTick();

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            HandleHotbar(keyboard);
            HandlePan   (keyboard, view);
        }

        Mouse mouse = Mouse.current;
        if (mouse == null || IsOverHud())
            return;

        float wheel = mouse.scroll.ReadValue().y;
        if (wheel != 0f)
        {
            float steps = (wheel / SCROLL_DELTA_PER_WHEEL_STEP);
            view.ChangePixelSize(Mathf.Pow(ZOOM_SCALE_PER_WHEEL_STEP, steps));
        }

        if (!mouse.leftButton.wasPressedThisFrame && !mouse.rightButton.wasPressedThisFrame)
            return;

        Vector2    mouseScreen  = mouse.position.ReadValue();
        Vector2    guiPosition  = new Vector2(mouseScreen.x, Screen.height - mouseScreen.y);
        Vector2Int grid         = view.ScreenToGridInt(guiPosition);

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
        selectedType = HOTBAR[index].Index;
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
            MachineOperations.TryPlaceMachine(selectedType, grid);
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
