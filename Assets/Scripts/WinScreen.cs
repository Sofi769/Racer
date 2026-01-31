using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WinScreen : MonoBehaviour
{
    [Header("Star Settings")]
    public Image[] stars; // Assign 3 UI Images in Inspector
    public Sprite filledStarSprite; // Assign your filled star sprite
    public float fillSpeed = 0.5f;

    public void ShowStars(int starsEarned)
    {
        StartCoroutine(FillStars(starsEarned));
    }

    private IEnumerator FillStars(int starsEarned)
    {
        foreach (Image star in stars)
        {
            if (star != null)
            {
                star.type = Image.Type.Filled;
                star.fillAmount = 0;
                star.sprite = filledStarSprite;
                star.preserveAspect = true;
            }
        }
        
        for (int i = 0; i < starsEarned && i < stars.Length; i++)
        {
            if (stars[i] == null) continue;

            float fillAmount = 0;
            while (fillAmount < 1)
            {
                fillAmount += Time.deltaTime * fillSpeed;
                stars[i].fillAmount = fillAmount;
                yield return null;
            }
            stars[i].fillAmount = 1f;
        }
    }
}