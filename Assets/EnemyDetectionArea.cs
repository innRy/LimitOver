using UnityEngine;
using System.Collections;

public class EnemyDetectionArea : MonoBehaviour
{
    public AudioSource bgmSource;
    public float fadeDuration = 2f; // フェードイン時間（秒）
    private int enemyCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyCount++;
            if (!bgmSource.isPlaying)
            {
                bgmSource.volume = 0f;
                bgmSource.Play();
                StartCoroutine(FadeIn());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyCount--;
            if (enemyCount <= 0)
            {
                enemyCount = 0;
                bgmSource.Stop();
                bgmSource.volume = 0f;
            }
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        bgmSource.volume = 1f;
    }
}