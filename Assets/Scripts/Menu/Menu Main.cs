using TMPro;
using UnityEngine;

public class MenuMain : MonoBehaviour
{
    // Member Variables
    [SerializeField] private TMP_Text textVersion;

    private void Awake()
    {
        textVersion.text = "Version - " + GameConstants.VERSION_TEXT;
    }
}