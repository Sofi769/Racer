using UnityEngine;

public class CheckpointPath : MonoBehaviour
{
    public Transform[] points;

    private void Reset()
    {
        // Child'ları otomatik doldur
        int count = transform.childCount;
        points = new Transform[count];
        for (int i = 0; i < count; i++)
            points[i] = transform.GetChild(i);
    }

    public Transform GetPoint(int index)
    {
        if (points == null || points.Length == 0) return null;
        index = Mathf.Clamp(index, 0, points.Length - 1);
        return points[index];
    }

    public int Count => points != null ? points.Length : 0;
}
