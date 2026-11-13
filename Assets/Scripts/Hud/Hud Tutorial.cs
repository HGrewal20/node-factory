using TMPro;
using UnityEngine;

public class HudTutorial : MonoBehaviour
{
    // Member Variables
    [SerializeField] private TMP_Text   text;
    [SerializeField] private GameObject background;

    // Awake
    private void Awake()
    {
        Set(null);
    }

    // Update
    void Update()
    {
        Set( GameTutorial.Text );
    }
    private void Set(string value)
    {
        bool visible = !string.IsNullOrEmpty(value);

        text.text = value ?? "";
        text.gameObject.SetActive(visible);
        this.background.SetActive(visible);
    }
}