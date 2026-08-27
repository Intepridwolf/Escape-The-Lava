using DG.Tweening;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private bool isLava;
    private ObjectPooler pooler;
    private GameManager gameManager;

    private void Start()
    {
        pooler = ObjectPooler.instance;
        gameManager = GameManager.instance;
    }

    private void OnMouseDown()
    {
        if(gameManager.CurrentState != GameManager.GameState.Playing)
            return;

        if (isLava)
        {
            gameManager.TakeDamage();
            GameObject blastFX = pooler.Get("BlastFX", transform.position + new Vector3(0f, 1.1f, 0f),
                 Quaternion.identity);
            DOVirtual.DelayedCall(1f, () => pooler.Return(blastFX));
            return;
        }

        Diamond diamond = GetComponentInChildren<Diamond>();

        if (diamond != null)
        {
            diamond.Collect();
        }
    }
}