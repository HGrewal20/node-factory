using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HudRecipe : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // References
    [SerializeField] private Button   button;
    [SerializeField] private TMP_Text textName;

    // Colors
    private static readonly Color COLOR_NORMAL   = Color.white;
    private static readonly Color COLOR_SELECTED = Color.green;

    // State
    public Recipe Recipe { get; private set; }

    private Action<Recipe> onClick;
    private Action<Recipe> onEnter;
    private Action<Recipe> onExit;

    // Set
    public void Set(
        Recipe recipe,
        Action<Recipe> click,
        Action<Recipe> enter,
        Action<Recipe> exit)
    {
        Recipe  = recipe;

        onClick = click;
        onEnter = enter;
        onExit  = exit;

        textName.text = recipe.Name;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);

        SetSelected(false);
    }

    private bool isSelected;

    public void SetSelected(bool selected)
    {
        if (isSelected == selected)
            return;
        isSelected = selected;

        Color      color  = selected ? COLOR_SELECTED : COLOR_NORMAL;
        ColorBlock colors = button.colors;

        colors.normalColor      = color;
        colors.selectedColor    = color;

        button.colors = colors;
    }

    // Events
    private void OnClick()
    {
        onClick?.Invoke(Recipe);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke(Recipe);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onExit?.Invoke(Recipe);
    }
}