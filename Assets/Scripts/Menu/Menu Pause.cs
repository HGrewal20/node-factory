using UnityEngine;
using UnityEngine.UI;

public class MenuPause : MonoBehaviour
{
    // Member Variables
    [SerializeField] private Button buttonResume;
    [SerializeField] private Button buttonSave;
    [SerializeField] private Button buttonRestart;
    [SerializeField] private Button buttonQuit;

    private void Awake()
    {
        MenuStyle.Apply(buttonResume );
        MenuStyle.Apply(buttonSave   );
        MenuStyle.Apply(buttonRestart);
        MenuStyle.Apply(buttonQuit   );
    }
}
