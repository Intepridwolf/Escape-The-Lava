using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler instance;

    [System.Serializable]
    public class Pool
    {
        public string id;
        public GameObject prefab;
        public int size = 10;
    }

    [SerializeField] private List<Pool> pools;

    private Dictionary<string, Queue<GameObject>> poolDictionary = new();

    private void Awake()
    {
        instance = this;
        // create pools
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectQueue = new();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(
                    pool.prefab,
                    transform
                );

                obj.SetActive(false);
                objectQueue.Enqueue(obj);
            }

            poolDictionary.Add(pool.id, objectQueue);
        }
    }

    public GameObject Get(string id, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(id))
        {
            Debug.LogWarning($"Pool not found: {id}");
            return null;
        }

        GameObject obj = poolDictionary[id].Dequeue();

        obj.transform.SetPositionAndRotation(
            position,
            rotation
        );

        obj.SetActive(true);

        poolDictionary[id].Enqueue(obj);

        return obj;
    }

    public void Return(GameObject obj)
    {
        if(obj.transform.parent != transform)
            obj.transform.SetParent(transform);
            
        obj.SetActive(false);
    }
}