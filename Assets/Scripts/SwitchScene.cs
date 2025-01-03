using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Code generated using ChatGPT and manually edited.
/// </summary>

public class SwitchScene : MonoBehaviour
{
    public GameObject fadeQuad; // Reference to the Quad GameObject for fading
    public float fadeDuration = 1.0f; // Duration for the fade effect

    private Renderer _renderer;
    private Material _material;
    private Color _color;

    private void Start()
    {
        _renderer = fadeQuad.GetComponent<Renderer>();
        _material = _renderer.material;
        _color = _material.color;
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    private IEnumerator FadeOut(string sceneName)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _color.a = Mathf.Clamp01(elapsed / fadeDuration);
            _renderer.material.color = _color;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeIn()
    {
        // Ensure material is fully opaque before starting
        _color.a = 1;
        _renderer.material.color = _color;

        float elapsed = fadeDuration;

        while (elapsed > 0f)
        {
            elapsed -= Time.deltaTime;
            _color.a = Mathf.Clamp01(elapsed / fadeDuration);
            _renderer.material.color = _color;
            yield return null;
        }

    }

}
