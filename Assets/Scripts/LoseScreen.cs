using UnityEngine;
using TMPro;
using System.Collections;

public class LoseScreen : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup group;              // GameOverPanel'e CanvasGroup ekle
    public TextMeshProUGUI messageText;    // opsiyonel
    public float fadeSpeed = 3f;

    private Coroutine fadeCo;

    public void ShowLose(string message = null)
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        if (messageText != null)
            messageText.text = string.IsNullOrEmpty(message) ? "Game Over!" : message;

        if (group != null)
        {
            group.alpha = 0f;
            group.interactable = true;
            group.blocksRaycasts = true;

            if (fadeCo != null) StopCoroutine(fadeCo);
            fadeCo = StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn()
    {
        float a = 0f;
        while (a < 1f)
        {
            a += Time.unscaledDeltaTime * fadeSpeed;
            group.alpha = a;
            yield return null;
        }
        group.alpha = 1f;
        fadeCo = null;
    }
}
