using UnityEngine;
using DG.Tweening;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private float strength = 0.15f;
    [SerializeField] private int vibrato = 15;

    private Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

    public void Shake()
    {
        // stop previous shake
        transform.DOKill();

        // reset position
        transform.localPosition = originalPosition;

        // shake camera
        transform.DOShakePosition(duration, strength, vibrato, 90f, false, true).OnComplete(() =>
        {
            transform.localPosition = originalPosition;
        });
    }
}