using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LevelCountdown : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public float stepTime = 1f;

    private PlayerController player;
    private Rigidbody2D playerRb;

    // Enemy AI scriptlerini burada saklayacağız
    private readonly List<MonoBehaviour> enemyAIScripts = new List<MonoBehaviour>();
    private readonly List<Rigidbody2D> enemyRBs = new List<Rigidbody2D>();

    private void Start()
    {
        // Player bul
        player = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);
        if (player != null) playerRb = player.GetComponent<Rigidbody2D>();

        // Enemy tagli tüm objeleri bul ve SADECE AI scriptini yakala
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        enemyAIScripts.Clear();
        enemyRBs.Clear();

        foreach (var e in enemies)
        {
            if (e == null) continue;

            // Enemy üzerindeki AI scripti: genelde custom MonoBehaviour olur.
            // Burada en güvenlisi: "PlayerController olmayan" ve "enabled true" olan ilk custom scripti almak.
            // Eğer enemy üzerinde tek script varsa (Enemy Checkpoint AI), zaten o alınır.
            var scripts = e.GetComponents<MonoBehaviour>();
            foreach (var s in scripts)
            {
                if (s == null) continue;

                // Unity built-in UI/Renderer değil; enemy üzerinde zaten genelde 1 tane custom script olur.
                // Biz sadece custom AI'ı hedefliyoruz:
                if (s.GetType().Name.Contains("TrackFollower") ||
                    s.GetType().Name.Contains("Checkpoint") ||
                    s.GetType().Name.Contains("AI"))
                {
                    enemyAIScripts.Add(s);
                    break; // 1 enemy için 1 AI script yeter
                }
            }

            var rb = e.GetComponent<Rigidbody2D>();
            if (rb != null) enemyRBs.Add(rb);
        }

        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        Freeze(true);

        countdownText.text = "3";
        yield return Wait(stepTime);

        countdownText.text = "2";
        yield return Wait(stepTime);

        countdownText.text = "1";
        yield return Wait(stepTime);

        countdownText.text = "START";
        yield return Wait(stepTime * 0.8f);

        Freeze(false);

        gameObject.SetActive(false);
    }

    private void Freeze(bool freeze)
    {
        // Player kontrol
        if (player != null) player.enabled = !freeze;

        // Player kaymasını kes
        if (freeze && playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
        }

        // Enemy AI scriptlerini kapat/aç
        for (int i = 0; i < enemyAIScripts.Count; i++)
        {
            if (enemyAIScripts[i] != null)
                enemyAIScripts[i].enabled = !freeze;
        }

        // Enemy kaymasını kes
        if (freeze)
        {
            for (int i = 0; i < enemyRBs.Count; i++)
            {
                if (enemyRBs[i] == null) continue;
                enemyRBs[i].linearVelocity = Vector2.zero;
                enemyRBs[i].angularVelocity = 0f;
            }
        }
        else
        {
            // Bazı AI'lar enable olunca hemen hareket etmeyebiliyor; uyandıralım
            for (int i = 0; i < enemyRBs.Count; i++)
            {
                if (enemyRBs[i] == null) continue;
                enemyRBs[i].WakeUp();
            }
        }
    }

    private IEnumerator Wait(float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
