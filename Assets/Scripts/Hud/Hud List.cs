using System;
using System.Collections.Generic;
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
    [SerializeField] private TMP_Text               title;
    [SerializeField] private Transform              content;
    [SerializeField] private HudMachineEntry        prefabMachine;
    [SerializeField] private HudResourceEntry       prefabResource;
    [SerializeField] private HudRequirementEntry    prefabRequirement;
    [SerializeField] private HudShopEntry           prefabShop;
    [SerializeField] private GameInteraction        interaction;
    [SerializeField] private Button                 buttonMachines;
    [SerializeField] private Button                 buttonDelivered;
    [SerializeField] private Button                 buttonShop;
    [SerializeField] private Button                 buttonRequirements;

    private readonly List<HudMachineEntry    > entriesMachine       = new();
    private readonly List<HudResourceEntry   > entriesResource      = new();
    private readonly List<HudShopEntry       > entriesShop          = new();
    private readonly List<HudRequirementEntry> entriesRequirement   = new();
    private          Tab                       currentTab;
    private          int                       resourceCount;

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

    private void ShowMachines()
    {
        Show(Tab.Machines, "Machines");

        int count = MachineCollection.Count();
        for (byte i = 0; i < count; i++)
        {
            MachineType machine = MachineCollection.Get(i);

            HudMachineEntry entry = Instantiate(prefabMachine, content);
            entry.Setup(machine, interaction);

            entriesMachine.Add(entry);
        }
    }

    private void ShowDelivered()
    {
        Show(Tab.Delivered, "Delivered");

        ItemAmounts delivered = GameData.INSTANCE.delivered;
        resourceCount = delivered.Count();
        for (int i = 0; i < resourceCount; i++)
        {
            Item item = delivered.Get(i).Item;

            HudResourceEntry entry = Instantiate(prefabResource, content);
            entry.Setup(item);

            entriesResource.Add(entry);
        }
    }

    public void ShowShop()
    {
        Show(Tab.Shop, "Shop");

        int count = ShopCollection.Count();
        for (byte i = 0; i < count; i++)
        {
            ShopEntry shop = ShopCollection.Get(i);

            HudShopEntry entry = Instantiate(prefabShop, content);
            entry.Setup(shop);

            entriesShop.Add(entry);
        }
    }

    public void ShowRequirements()
    {
        Show(Tab.Requirements, "Level");

        for (int i = 0; i < GameData.INSTANCE.Level.deliveryGoal.Count(); i++)
        {
            HudRequirementEntry entry = Instantiate(prefabRequirement, content);
            entriesRequirement.Add(entry);
        }

        UpdateRequirements();
    }

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
        if ( !data.IsPlaying() || data.IsPaused )
            return;

        switch (currentTab)
        {
            case Tab.Machines:      UpdateMachines    ();  break;
            case Tab.Delivered:     UpdateDelivered   ();  break;
            case Tab.Shop:          UpdateShop        ();  break;
            case Tab.Requirements:  UpdateRequirements();  break;
        }
    }
    private void UpdateMachines()
    {
        foreach (HudMachineEntry entry in entriesMachine)
            entry.Refresh();
    }
    private void UpdateDelivered()
    {
        if (GameData.INSTANCE.delivered.Count() != resourceCount)
        {
            ShowDelivered();
        }
        else
        {
            foreach (HudResourceEntry entry in entriesResource)
                entry.Refresh();
        }
    }

    private void UpdateShop()
    {
        foreach (HudShopEntry entry in entriesShop)
            entry.Refresh();
    }

    private void UpdateRequirements()
    {
        GameData data = GameData.INSTANCE;

        ItemRequirements requirements = ItemRequirements.INSTANCE;
        requirements.Set(data.delivered, data.Level.deliveryGoal);

        for (int i = 0; i < entriesRequirement.Count; i++)
            entriesRequirement[i].Set(requirements.Get(i));
    }

    // Clear
    private void ClearEntries()
    {
        foreach (Transform child in content)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }

        entriesMachine    .Clear();
        entriesResource   .Clear();
        entriesShop       .Clear();
        entriesRequirement.Clear();
    }
}
