using UnityEngine;

public class GameEngine : MonoBehaviour
{
    void Update()
    {
        GameData.INSTANCE.GameTick();
    }
}
