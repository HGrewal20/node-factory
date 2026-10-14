public static class MachineConstants
{
    // Side
    public const byte SIDE_NONE  = 0;
    public const byte SIDE_TOP   = 1;
    public const byte SIDE_BOT   = 2;
    public const byte SIDE_LEFT  = 3;
    public const byte SIDE_RIGHT = 4;

    // Direction
    public const byte DIRECTION_NONE  = 0;
    public const byte DIRECTION_UP    = 1;
    public const byte DIRECTION_DOWN  = 2;
    public const byte DIRECTION_LEFT  = 3;
    public const byte DIRECTION_RIGHT = 4;

    public static byte GetDirection(byte side, bool isInput)
    {
        if (isInput)
        {
            switch (side)
            {
                case SIDE_TOP:     return DIRECTION_DOWN;
                case SIDE_BOT:     return DIRECTION_UP;
                case SIDE_LEFT:    return DIRECTION_RIGHT;
                case SIDE_RIGHT:   return DIRECTION_LEFT;
            }
        }
        else
        {
            switch (side)
            {
                case SIDE_TOP:     return DIRECTION_UP;
                case SIDE_BOT:     return DIRECTION_DOWN;
                case SIDE_LEFT:    return DIRECTION_LEFT;
                case SIDE_RIGHT:   return DIRECTION_RIGHT;
            }
        }

        return DIRECTION_NONE;
    }
}
