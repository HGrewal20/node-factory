using UnityEngine;
using UnityEngine.UI;

public class MenuInfo : MonoBehaviour
{
    // Member Variables
    [SerializeField] private Button buttonBack;

    private void Awake()
    {
        MenuStyle.Apply(buttonBack);
    }
}
