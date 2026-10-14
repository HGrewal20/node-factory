using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudMachineEntry : MonoBehaviour
{
    // Constants
    private static readonly Color colorNormal   = Color.white;
    private static readonly Color colorSelected = Color.green;

    // Member Variables
    [SerializeField] private TMP_Text           text;
    [SerializeField] private Button             button;
                     private MachineType        machine;
                     private DisplayInteraction interaction;

    // Setup
    public void Setup(MachineType machine, DisplayInteraction interaction)
    {
        this.machine     = machine;
        this.interaction = interaction;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener       (SelectMachine);

        Refresh();
    }

    // Refresh
    public void Refresh()
    {
        int count = GameData.INSTANCE.machinesAvailable.Amount(machine.Index);
        text.text = " " + count + " - " + machine.Name;

        button.interactable = (count > 0);

        Color color = (interaction != null) && (interaction.SelectedMachineType == machine.Index) ? colorSelected : colorNormal;

        ColorBlock colors = button.colors;
        colors.normalColor = color;
        button.colors = colors;
    }

    private void SelectMachine()
    {
        interaction?.SelectMachine(machine.Index);
        Refresh();
    }
}
