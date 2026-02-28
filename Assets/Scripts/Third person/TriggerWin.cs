using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class TriggerWin : MonoBehaviour
{
    [SerializeField] private int sceneToLoad;
    [SerializeField] private float delay = 2f;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private Image fadePanel;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(ChangeSceneAfterDelay());
        }
    }

    private IEnumerator ChangeSceneAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        yield return StartCoroutine(FadeOut());

        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color color = fadePanel.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);

            color.a = alpha;
            fadePanel.color = color;

            yield return null;
        }

        color.a = 1f;
        fadePanel.color = color;
    }
}
