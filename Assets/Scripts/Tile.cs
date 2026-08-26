using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private bool isLava;

    private void OnMouseDown()
    {
        if (isLava)
        {
            Debug.Log("lava hit");
            return;
        }

        Diamond diamond = GetComponentInChildren<Diamond>();

        if (diamond != null)
        {
            diamond.Collect();
        }
    }
}