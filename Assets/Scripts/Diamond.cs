using UnityEngine;
using DG.Tweening;

public class Diamond : MonoBehaviour
{
    [SerializeField] private float floatHeight = 0.25f;
    [SerializeField] private float floatDuration = 1.2f;
    private Vector3 startPosition;

    private void Start()
    {
        ApplyAnimation();
    }

    private void ApplyAnimation()
    {
        startPosition = transform.position;

        // up-down
        transform.DOMoveY(startPosition.y + floatHeight, floatDuration).SetEase(Ease.InOutSine).
            SetLoops(-1, LoopType.Yoyo);
    }

    public void Collect()
    {
        // play collect animation
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}