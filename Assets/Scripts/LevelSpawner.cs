using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private int rows = 16;
    [SerializeField] private int columns = 8;
    [SerializeField] private float tileSpacing = 2.1f;

    [Header("Tile Prefabs")]
    [SerializeField] private GameObject greenTilePrefab;
    [SerializeField] private GameObject lavaTilePrefab;
    [SerializeField] private GameObject diamondPrefab;

    [Header("Difficulty")]
    [Range(0f, 1f)]
    [SerializeField] private float lavaChance = 0.45f;
    [SerializeField] private int diamondCount = 20;
    [SerializeField] private int minimumDiamondDistance = 2;

    private List<GreenTileData> greenTiles = new();

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        greenTiles.Clear();

        float xOffset = (columns - 1) * tileSpacing * 0.5f;
        float zOffset = (rows - 1) * tileSpacing * 0.5f;

        // grid spawn
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector2Int gridPosition = new Vector2Int(column, row);

                Vector3 worldPosition = new Vector3(
                    column * tileSpacing - xOffset,
                    0f,
                    row * tileSpacing - zOffset
                );

                bool isLava = Random.value < lavaChance;

                if (isLava)
                {
                    // spawn lava
                    Instantiate(
                        lavaTilePrefab,
                        worldPosition,
                        lavaTilePrefab.transform.rotation,
                        transform
                    );
                }
                else
                {
                    // spawn green tile
                    GameObject greenTile = Instantiate(
                        greenTilePrefab,
                        worldPosition,
                        greenTilePrefab.transform.rotation,
                        transform
                    );

                    // store green tile
                    greenTiles.Add(
                        new GreenTileData(
                            gridPosition,
                            greenTile
                        )
                    );
                }
            }
        }

        SpawnDiamonds();
    }

    private void SpawnDiamonds()
    {
        // set diamond count
        int targetCount = Mathf.Min(diamondCount, greenTiles.Count);

        // shuffle green tiles
        Shuffle(greenTiles);

        List<GreenTileData> selectedTiles = new();

        // spread diamonds
        foreach (GreenTileData tile in greenTiles)
        {
            if (selectedTiles.Count >= targetCount)
                break;

            bool farEnough = true;

            foreach (GreenTileData selected in selectedTiles)
            {
                int distance =
                    Mathf.Abs(tile.gridPosition.x - selected.gridPosition.x) +
                    Mathf.Abs(tile.gridPosition.y - selected.gridPosition.y);

                if (distance < minimumDiamondDistance)
                {
                    farEnough = false;
                    break;
                }
            }

            if (farEnough)
            {
                selectedTiles.Add(tile);
            }
        }

        // fill remaining diamonds
        foreach (GreenTileData tile in greenTiles)
        {
            if (selectedTiles.Count >= targetCount)
                break;

            if (!selectedTiles.Contains(tile))
            {
                selectedTiles.Add(tile);
            }
        }

        // spawn diamonds
        foreach (GreenTileData tile in selectedTiles)
        {
            GameObject diamond = Instantiate(
                diamondPrefab,
                tile.tileObject.transform
            );

            diamond.transform.localPosition = new Vector3(
                0f,
                0f,
                2.2f
            );

            diamond.transform.localEulerAngles = new Vector3(
                90f,
                0f,
                0f
            );
        }

        Debug.Log($"Spawned {selectedTiles.Count} diamonds");
    }

    // shuffle list
    private void Shuffle(List<GreenTileData> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            GreenTileData temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // store green tile data
    private class GreenTileData
    {
        public Vector2Int gridPosition;
        public GameObject tileObject;

        public GreenTileData(
            Vector2Int gridPosition,
            GameObject tileObject)
        {
            this.gridPosition = gridPosition;
            this.tileObject = tileObject;
        }
    }
}