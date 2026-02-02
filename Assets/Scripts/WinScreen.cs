using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WinScreen : MonoBehaviour
{
    [Header("Star Settings")]
    public Image[] stars;
    public Sprite filledStarSprite;
    public float fillSpeed = 0.5f;

    private Coroutine fillCoroutine;

    public void ShowStars(int starsEarned)
    {
        // Panel kapalıysa coroutine başlayamaz
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        if (fillCoroutine != null)
            StopCoroutine(fillCoroutine);

        fillCoroutine = StartCoroutine(FillStars(starsEarned));
    }

    private IEnumerator FillStars(int starsEarned)
    {
        foreach (Image star in stars)
        {
            if (star == null) continue;

            star.type = Image.Type.Filled;
            star.fillAmount = 0f;
            star.sprite = filledStarSprite;
            star.preserveAspect = true;
        }

        for (int i = 0; i < starsEarned && i < stars.Length; i++)
        {
            if (stars[i] == null) continue;

            float fillAmount = 0f;
            while (fillAmount < 1f)
            {
                // Time.timeScale = 0 olsa bile akar
                fillAmount += Time.unscaledDeltaTime * fillSpeed;
                stars[i].fillAmount = fillAmount;
                yield return null;
            }

            stars[i].fillAmount = 1f;
            yield return new WaitForSecondsRealtime(0.15f);
        }

        fillCoroutine = null;
    }
}
