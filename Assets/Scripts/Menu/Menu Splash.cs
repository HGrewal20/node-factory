using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuSplash : MonoBehaviour
{
    // Member Variables
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image       progressBar;
    [SerializeField] private float       duration = 5.0f;

    private void Awake()
    {
        canvasGroup.alpha          = 1f;
        canvasGroup.interactable   = true;
        canvasGroup.blocksRaycasts = true;

        progressBar.fillAmount = 0f;
        progressBar.type       = Image.Type.Filled;
        progressBar.fillMethod = Image.FillMethod.Horizontal;
        progressBar.fillOrigin = (int)Image.OriginHorizontal.Left;
    }

    private void Start()
    {
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        float elapsed       = 0f;
        float totalDuration = Mathf.Max(0.01f, duration);

        while (elapsed < totalDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            progressBar.fillAmount = Mathf.Clamp01(elapsed / totalDuration);

            yield return null;
        }

        progressBar.fillAmount = 1f;

        canvasGroup.interactable   = false;
        canvasGroup.blocksRaycasts = false;

        gameObject.SetActive(false);
    }
}