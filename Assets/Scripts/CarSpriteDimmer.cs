using UnityEngine;

public class CarSpriteDimmer : MonoBehaviour
{
    [Range(0f, 1f)]
    public float winBrightness = 0.4f;

    private SpriteRenderer sr;
    private Color originalColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    public void DimForWin()
    {
        sr.color = originalColor * winBrightness;
    }

    public void Restore()
    {
        sr.color = originalColor;
    }
}
