using UnityEngine;
using DG.Tweening;

public class Screen : MonoBehaviour
{
    [SerializeField] private float duration = 0.45f;

    private void OnEnable()
    {
        // reset scale
        transform.localScale = Vector3.zero;
        // pop in
        transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack);
    }

    private void OnDisable()
    {
        // stop animation
        transform.DOKill();
    }
}