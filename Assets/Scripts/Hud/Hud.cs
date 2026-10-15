using UnityEngine;

public class Hud : MonoBehaviour
{
    // References
    [SerializeField] private HudList    hudList;
    [SerializeField] private HudRecipes hudRecipes;

    public void ResetHud()
    {
        hudList   .ResetHud();
        hudRecipes.TryClose();
    }
}