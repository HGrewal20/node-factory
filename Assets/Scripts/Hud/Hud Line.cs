using TMPro;
using UnityEngine;

public class HudLine : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void Set(string value, Color color)
    {
        text.text  = value;
        text.color = color;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}