using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuMain : MonoBehaviour
{
    // Member Variables
    [SerializeField] private Button   buttonPlay;
    [SerializeField] private Button   buttonInfo;
    [SerializeField] private Button   buttonQuit;
    [SerializeField] private TMP_Text textVersion;

    private void Awake()
    {
        MenuStyle.Apply(buttonPlay);
        MenuStyle.Apply(buttonInfo);
        MenuStyle.Apply(buttonQuit);

        textVersion.text = GameConstants.VERSION_TEXT;

        #if UNITY_WEBGL && !UNITY_EDITOR
            buttonQuit.gameObject.SetActive(false);
        #endif
    }
}