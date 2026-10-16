using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeTransition : MonoBehaviour
{
    [SerializeField] private float fadeSpeed;

    private float alpha;
    private float targetAlpha;
    private Image image;
    public static FadeTransition instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        image = GetComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 1f);

        alpha = 1f;
        FadeIn();
    }

    private void Update()
    {
        alpha = Mathf.Lerp(alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        image.color = new Color(0f, 0f, 0f, alpha);
    }

    private void FadeIn()
    {
        targetAlpha = 0f;
    }

    private void FadeOut()
    {
        targetAlpha = 1f;
    }
}
