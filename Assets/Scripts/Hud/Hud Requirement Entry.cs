using TMPro;
using UnityEngine;

public class HudRequirementEntry : MonoBehaviour
{
    // Constants
    private static readonly Color colorMet      = Color.green;
    private static readonly Color colorRequired = Color.black;

    // Member Variables
    [SerializeField] private TMP_Text textName;
    [SerializeField] private TMP_Text textAmount;

    // Set
    public void Set(ItemRequirement requirement)
    {
        textName  .text  = " " + requirement.Item.Name;
        textAmount.text  = " " + requirement.Amount + " / " + requirement.Required;
        textAmount.color = requirement.IsMet() ? colorMet : colorRequired;
    }
}
