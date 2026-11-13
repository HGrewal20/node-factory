using UnityEngine;
using UnityEngine.UI;

public class MenuLevelList : MonoBehaviour
{
    // Member Variables
    [SerializeField] private MenuController menuController;
    [SerializeField] private MenuLevelEntry entryPrefab;
    [SerializeField] private Transform      content;
    [SerializeField] private ScrollRect     scrollRect;

    private MenuLevelEntry[] entries;

    private void Awake()
    {
        entries = new MenuLevelEntry[ LevelCollection.Count() ];

        for (int i = 0; i < entries.Length; i++)
            entries[i] = Instantiate(entryPrefab, content);
    }

    private void OnEnable()
    {
        // Update Entries
        for (int i = 0; i < entries.Length; i++)
        {
            Level level = LevelCollection.Get((byte)i);
            entries[i].Setup(level, menuController);
        }

        // Reset Scroll To Top
        // Canvas.ForceUpdateCanvases();    // Use if needed
        scrollRect.StopMovement();
        scrollRect.verticalNormalizedPosition = 1f;
    }
}