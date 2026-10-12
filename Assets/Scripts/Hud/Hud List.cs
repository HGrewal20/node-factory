using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudList : MonoBehaviour
{
    // Constants
    private enum Tab
    {
        Machines,
        Delivered,
        Shop,
        Requirements,
    }

    // Member Variables
    [SerializeField] private TMP_Text         title;
    [SerializeField] private Transform        content;
    [SerializeField] private HudMachines      machines;
    [SerializeField] private HudResources     resources;
    [SerializeField] private HudRequirements  requirements;
    [SerializeField] private HudShop          shop;
    [SerializeField] private Button           buttonMachines;
    [SerializeField] private Button           buttonDelivered;
    [SerializeField] private Button           buttonShop;
    [SerializeField] private Button           buttonRequirements;

    private Tab currentTab;

    // Initialize
    private void Awake()
    {
        buttonMachines    .onClick.AddListener(ShowMachines    );
        buttonDelivered   .onClick.AddListener(ShowDelivered   );
        buttonShop        .onClick.AddListener(ShowShop        );
        buttonRequirements.onClick.AddListener(ShowRequirements);
    }

    private void Start()
    {
        ResetHud();
    }

    // Reset
    public void ResetHud()
    {
        ShowMachines();
    }

    // Tabs
    private void Show(Tab tab, String name)
    {
        currentTab = tab;
        title.text = name;

        ClearEntries     ();
        RefreshTabButtons();
    }

    private void ShowMachines    () { Show(Tab.Machines    , "Machines" ); machines    .Build(content); }
    private void ShowDelivered   () { Show(Tab.Delivered   , "Delivered"); resources   .Build(content); }
    public  void ShowShop        () { Show(Tab.Shop        , "Shop"     ); shop        .Build(content); }
    public  void ShowRequirements() { Show(Tab.Requirements, "Level"    ); requirements.Build(content); }

    private void RefreshTabButtons()
    {
        SetTabColor(buttonMachines    , currentTab == Tab.Machines    );
        SetTabColor(buttonDelivered   , currentTab == Tab.Delivered   );
        SetTabColor(buttonShop        , currentTab == Tab.Shop        );
        SetTabColor(buttonRequirements, currentTab == Tab.Requirements);
    }

    private void SetTabColor(Button button, bool selected)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = selected ? Color.green : Color.white;
        button.colors = colors;
    }

    // Update
    private void Update()
    {
        GameData data = GameData.INSTANCE;
        if (!data.IsPlaying() || data.IsPaused)
            return;

        switch (currentTab)
        {
            case Tab.Machines:     machines    .Refresh();  break;
            case Tab.Delivered:    UpdateDelivered();       break;
            case Tab.Shop:         shop        .Refresh();  break;
            case Tab.Requirements: requirements.Refresh();  break;
        }
    }

    private void UpdateDelivered()
    {
        if (resources.NeedsRebuild())   ShowDelivered();
        else                            resources.Refresh();
    }

    // Clear
    private void ClearEntries()
    {
        foreach (Transform child in content)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }

        machines    .Clear();
        resources   .Clear();
        requirements.Clear();
        shop        .Clear();
    }
}