public static class GameTutorial
{
    public static string Text { get; private set; } = null;

    public static void Update()
    {
        Text = null;

        // If Wrong Level -> Do Nothing
        Level level = GameData.INSTANCE.Level;
        if (level       == null)    return;
        if (level.Index != 0   )    return;

        //
        switch ( GameData.INSTANCE.TutorialIndex )
        {
            case 0: Update00(); return;
            case 1: Update01(); return;
            case 2: Update02(); return;
            case 3: Update03(); return;
            case 4: Update04(); return;
            case 5: Update05(); return;
            case 6: Update06(); return;
        }
    }

    private static void Update00()
    {
        // Select and place Farm

        // If Placed Farm -> Advance
        if ( IsPlaced(MachineCollection.PROCESSOR_FARM) )
        {
            GameData.INSTANCE.SetTutorialIndex(1);
            Update();
            return;
        }

        // If Farm Selected -> Place It
        if ( IsSelected(MachineCollection.PROCESSOR_FARM) )
        {
            Text = "TUTORIAL: Place the farm on the left side of the map with the left mouse button";
            return;
        }

        // If Not List Machines Selected -> Select It
        if ( !IsListMachines() )
        {
            Text = "TUTORIAL: Select the machines tab from the hud near the top right";
            return;
        }

        // If Not Farm Selected -> Select It
        if ( !IsSelected(MachineCollection.PROCESSOR_FARM) )
        {
            Text = "TUTORIAL: Select the farm machine from hud list on the right side of the screen";
            return;
        }
    }
    private static void Update01()
    {
        // Select and place Mixer

        // If Placed Mixer -> Advance
        if ( IsPlaced(MachineCollection.PROCESSOR_MIXER) )
        {
            GameData.INSTANCE.SetTutorialIndex(2);
            Update();
            return;
        }

        // If Mixer Selected -> Place It
        if ( IsSelected(MachineCollection.PROCESSOR_MIXER) )
        {
            Text = "TUTORIAL: Place the mixer a few squares directly right of the farm";
            return;
        }

        // If Not List Machines Selected -> Select It
        if ( !IsListMachines() )
        {
            Text = "TUTORIAL: Select the machines tab from the hud near the top right";
            return;
        }

        // If Not Mixer Selected -> Select It
        if ( !IsSelected(MachineCollection.PROCESSOR_MIXER) )
        {
            Text = "TUTORIAL: Select the mixer machine from hud list on the right side of the screen";
            return;
        }
    }
    private static void Update02()
    {
        // Connect Farm to Mixer

        // If Farm Connected to Mixer -> Advance
        if ( IsConnected(MachineCollection.PROCESSOR_FARM, MachineCollection.PROCESSOR_MIXER) )
        {
            GameData.INSTANCE.SetTutorialIndex(3);
            Update();
            return;
        }

        // Connect It
        Text = "TUTORIAL: Connect the farm to the mixer by clicking on either of the nodes facing each other";
    }
    private static void Update03()
    {
        // Select and place Delivery

        // If Placed Delivery -> Advance
        if ( IsPlaced(MachineCollection.DELIVERY) )
        {
            GameData.INSTANCE.SetTutorialIndex(4);
            Update();
            return;
        }

        // If Delivery Selected -> Place It
        if ( IsSelected(MachineCollection.DELIVERY) )
        {
            Text = "TUTORIAL: Place the delivery machine a few squares directly right of the mixer";
            return;
        }

        // If Not List Machines Selected -> Select It
        if ( !IsListMachines() )
        {
            Text = "TUTORIAL: Select the machines tab from the hud near the top right";
            return;
        }

        // If Not Delivery Selected -> Select It
        if ( !IsSelected(MachineCollection.DELIVERY) )
        {
            Text = "TUTORIAL: Select the delivery machine from hud list on the right side of the screen";
            return;
        }
    }
    private static void Update04()
    {
        // Connect Mixer to Delivery

        // If Mixer Connected to Delivery -> Advance
        if ( IsConnected(MachineCollection.PROCESSOR_MIXER, MachineCollection.DELIVERY) )
        {
            GameData.INSTANCE.SetTutorialIndex(5);
            Update();
            return;
        }

        // Connect It
        Text = "TUTORIAL: Connect the mixer to the delivery";
    }
    private static void Update05()
    {
        // Wait for 6 Flour to be delivered

        // If 6 or More Flour Delivered -> Advance
        int amountDelivered = AmountDelivered(ItemCollection.FLOUR);
        if (amountDelivered >= 6)
        {
            GameData.INSTANCE.SetTutorialIndex(6);
            Update();
            return;
        }

        // Delivery It
        Text = "TUTORIAL: " + amountDelivered + " of 6 flour delivered";
    }
    private static void Update06()
    {
        // Tutorial Complete
        Text = "TUTORIAL: Complete";
    }

    // Helpers
    private static bool IsPlaced(MachineType machineType)
    {
        return GameData.INSTANCE.machinesUsed.IsContained(machineType.Index);
    }
    private static bool IsSelected(MachineType machineType)
    {
        return (DisplayInteraction.INSTANCE.SelectedMachineType == machineType.Index);
    }
    private static bool IsConnected(MachineType machineTypeSrc, MachineType machineTypeDst)
    {
        return GameData.INSTANCE.machinesUsed.IsConnected(machineTypeSrc.Index, machineTypeDst.Index);
    }
    private static bool IsListMachines()
    {
        return HudList.INSTANCE.IsSelectedMachines();
    }
    private static int AmountDelivered(Item item)
    {
        return GameData.INSTANCE.delivered.Count(item);
    }
}
