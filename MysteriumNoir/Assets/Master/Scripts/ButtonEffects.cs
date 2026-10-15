using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEffects : MonoBehaviour
{
    [Header("Scaling")]
    [SerializeField] private float maxScale;
    [SerializeField] private float scaleSpeed;

    [Header("Sounds")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip selectSound;

    private float scale;
    private float targetScale;

    private void Start()
    {
        ResetScale();
    }

    private void OnDisable()
    {
        ResetScale();
    }

    private void ResetScale()
    {
        scale = 1f;
        targetScale = 1f;        
        transform.localScale = new Vector3(scale, scale, scale);
    }

    private void Update()
    {
        scale = Mathf.Lerp(scale, targetScale, Time.deltaTime * scaleSpeed);
        transform.localScale = new Vector3(scale, scale, scale);
    }

    public void MouseEnter()
    {
        targetScale = maxScale;
        PlaySoundSelect();
    }

    public void MouseExit()
    {
        targetScale = 1;
    }

    public void PlaySoundSelect()
    {
        audioSource.PlayOneShot(selectSound);
    }
}
