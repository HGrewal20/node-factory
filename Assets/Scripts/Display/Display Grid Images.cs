using System;
using System.Collections.Generic;
using UnityEngine;

// Reusable Images For Drawing Display Grid Cells
public sealed class DisplayGridImages
{
    // Static Cache
    private static readonly Dictionary<int, DisplayGridImages> imagesBySize = new Dictionary<int, DisplayGridImages>();
    public static DisplayGridImages Get(int size)
    {
        if (!imagesBySize.TryGetValue(size, out DisplayGridImages images))
        {
            images = new DisplayGridImages(size);
            imagesBySize.Add(size, images);
        }
        return images;
    }

    // Colors
    private static readonly Color32 ColorEmpty          = new Color32( 19,  30,  39, 255);
    private static readonly Color32 ColorGridLine       = new Color32( 42,  65,  78, 255);
    private static readonly Color32 ColorMachine        = new Color32( 85, 110, 135, 255);
    private static readonly Color32 ColorMachineInvalid = new Color32(220,  55,  65, 255);
    private static readonly Color32 ColorWire           = new Color32( 35, 218, 245, 255);
    private static readonly Color32 ColorNodeEmpty      = new Color32(255, 172,  54, 255);
    private static readonly Color32 ColorNodeFull       = new Color32( 63, 245, 132, 255);

    // Member Variables
    public readonly DisplayGridImage Empty;
    public readonly DisplayGridImage MachineEmpty;
    public readonly DisplayGridImage MachineInvalid;

    // Wires
    public readonly DisplayGridImage WireUp;
    public readonly DisplayGridImage WireRight;
    public readonly DisplayGridImage WireDown;

    // Empty Nodes
    public readonly DisplayGridImage NodeEmptyUp;
    public readonly DisplayGridImage NodeEmptyRight;
    public readonly DisplayGridImage NodeEmptyDown;
    public readonly DisplayGridImage NodeEmptyLeft;

    // Full Nodes
    public readonly DisplayGridImage NodeFullUp;
    public readonly DisplayGridImage NodeFullRight;
    public readonly DisplayGridImage NodeFullDown;
    public readonly DisplayGridImage NodeFullLeft;

    // Constructor
    private DisplayGridImages(int size)
    {
        // Empty Grid
        Empty = new DisplayGridImage(size);
        Empty.Clear(ColorEmpty);
        Empty.Rect(ColorGridLine, 0, 0, size - 1, size - 1);

        // Machine Empty
        MachineEmpty = new DisplayGridImage(size);
        MachineEmpty.Clear(ColorMachine);

        // Machine Invalid
        MachineInvalid = new DisplayGridImage(size);
        MachineInvalid.Clear(ColorMachineInvalid);

        // Wire Arrows
        WireUp = new DisplayGridImage(size);
        WireUp.Clear(ColorEmpty);
        DisplayGridArrow.Draw(WireUp, ColorWire);

        WireRight = WireUp   .Rotate90CW();
        WireDown  = WireRight.Rotate90CW();

        // Node Arrows - Empty
        NodeEmptyUp = new DisplayGridImage(size);
        NodeEmptyUp.Clear(ColorMachine);
        DisplayGridArrow.Draw(NodeEmptyUp, ColorNodeEmpty);

        NodeEmptyRight = NodeEmptyUp   .Rotate90CW();
        NodeEmptyDown  = NodeEmptyRight.Rotate90CW();
        NodeEmptyLeft  = NodeEmptyDown .Rotate90CW();

        // Node Arrows - Full
        NodeFullUp = new DisplayGridImage(size);
        NodeFullUp.Clear(ColorMachine);
        DisplayGridArrow.Draw(NodeFullUp, ColorNodeFull);

        NodeFullRight = NodeFullUp   .Rotate90CW();
        NodeFullDown  = NodeFullRight.Rotate90CW();
        NodeFullLeft  = NodeFullDown .Rotate90CW();
    }

    // Get Wire Image
    public DisplayGridImage GetWire(byte direction)
    {
        switch (direction)
        {
            case MachineConstants.DIRECTION_UP:    return WireUp;
            case MachineConstants.DIRECTION_RIGHT: return WireRight;
            case MachineConstants.DIRECTION_DOWN:  return WireDown;
            default:                                throw new ArgumentOutOfRangeException(nameof(direction), direction, "Invalid wire direction.");
        }
    }

    // Get Node Image
    public DisplayGridImage GetNode(byte direction, bool isEmpty)
    {
        if (isEmpty)
        {
            switch (direction)
            {
                case MachineConstants.DIRECTION_UP:    return NodeEmptyUp;
                case MachineConstants.DIRECTION_RIGHT: return NodeEmptyRight;
                case MachineConstants.DIRECTION_DOWN:  return NodeEmptyDown;
                case MachineConstants.DIRECTION_LEFT:  return NodeEmptyLeft;
            }
        }
        else
        {
            switch (direction)
            {
                case MachineConstants.DIRECTION_UP:    return NodeFullUp;
                case MachineConstants.DIRECTION_RIGHT: return NodeFullRight;
                case MachineConstants.DIRECTION_DOWN:  return NodeFullDown;
                case MachineConstants.DIRECTION_LEFT:  return NodeFullLeft;
            }
        }

        throw new ArgumentOutOfRangeException(nameof(direction), direction, "Invalid node direction.");
    }
}