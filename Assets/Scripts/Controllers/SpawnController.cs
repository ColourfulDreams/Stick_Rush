using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField]
    private float _obstacleSpawnTime;
    private ObstaclePool obstaclePool;
    private ScorePool scorePool;
    private void Awake()
    {
        obstaclePool = GetComponent<ObstaclePool>();
        scorePool = GetComponent<ScorePool>();
    }
    public IEnumerator SpawnObjects()
    {
        while (!GameManager.Instance.hasGameEnded)
        {
            // Случайным образом решаем, какой объект будем спавнить
            bool isScore = UnityEngine.Random.Range(0, 4) != 0;
            GameObject spawnObject = isScore ? scorePool.GetObjectFromPool() : obstaclePool.GetObjectFromPool();

            if (spawnObject != null)
            {
                spawnObject.SetActive(true);
            }
            yield return new WaitForSeconds(_obstacleSpawnTime);
        }
    }   
}
