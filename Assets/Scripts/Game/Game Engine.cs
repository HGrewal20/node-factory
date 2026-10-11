using UnityEngine;

public class GameEngine : MonoBehaviour
{
    private float accumulator;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = GameConstants.FPS;
        Application.runInBackground = true;
    }

    void Update()
    {
        float tickDuration = GameConstants.FRAME_TIME;

        accumulator += Time.unscaledDeltaTime;

        // Prevent excessive catch-up after a large frame delay
        accumulator = Mathf.Min(accumulator, tickDuration * 3f);

        while (accumulator >= tickDuration)
        {
            GameData.INSTANCE.GameTick();
            accumulator -= tickDuration;
        }
    }
}
