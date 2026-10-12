using UnityEngine;

public class MenuLevelList : MonoBehaviour
{
    // Member Variables
    [SerializeField] private MenuController menuController;
    [SerializeField] private MenuLevelEntry entryPrefab;
    [SerializeField] private Transform      content;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);

        for (int i = 0; i < LevelCollection.Count(); i++)
        {
            Level level = LevelCollection.Get((byte)i);
            
            MenuLevelEntry entry = Instantiate(entryPrefab, content);
            entry.Setup(level, menuController);
        }
    }
}