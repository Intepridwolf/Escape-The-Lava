using UnityEngine;
using DG.Tweening;

public class Diamond : MonoBehaviour
{
    [SerializeField] private float floatHeight = 0.25f;
    [SerializeField] private float floatDuration = 1.2f;
    private Vector3 startPosition;
    private GameManager gameManager;
    private bool collected;

    private void Start()
    {
        gameManager = GameManager.instance;
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
        if(collected)
            return;
             
        collected = true;
        // play collect animation
        gameManager.ShowFloatingTextAt(transform.position);

        transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameManager.CollectDiamond();
            Destroy(gameObject);
        });
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}