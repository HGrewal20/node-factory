using UnityEngine;

// Controls The Hud Visibility
public class HudController : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
                     private bool       wasPlaying = true;

    private void Start()
    {
        UpdateVisibility();
    }

    private void Update()
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        GameData data = GameData.INSTANCE;

        bool playing = data.IsPlaying() && !data.IsPaused;
        if (playing != wasPlaying)
        {
            wasPlaying = playing;
            canvas.SetActive(playing);
        }
    }
}