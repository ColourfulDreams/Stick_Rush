using System.Collections.Generic;
using UnityEngine;

public class ScorePool : MonoBehaviour
{
    public GameObject scorePrefab;
    [HideInInspector] public int initialSize = 6;
    private List<GameObject> pool;
    [SerializeField] private List<float> _spawnPosX;
    private float spawnPosY = -25f;
    private float spawnPosZ = 0f;
    private void Awake()
    {        
        pool = new List<GameObject>();
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(scorePrefab, GetRandomPosition(), Quaternion.identity);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }
    public GameObject GetObjectFromPool()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        GameObject newObject = Instantiate(scorePrefab, GetRandomPosition(), Quaternion.identity);
        pool.Add(newObject);
        return newObject;
    }
    public void ReturnToPool(GameObject obj)
    {
        obj.transform.position = GetRandomPosition();
        obj.SetActive(false);
    }
    private Vector3 GetRandomPosition()
    {
        float randomX = _spawnPosX[Random.Range(0, _spawnPosX.Count)];
        return new Vector3(randomX, spawnPosY, spawnPosZ);
    }
}
