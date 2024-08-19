using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed,
        _maxOffset
        ;
    private RescaleController rescaleController;
    private ObstaclePool obstaclePool;
    private OpositeDirectioneMoveController directionController;

    [SerializeField] private List<Vector3> _spawnPos;

    private Vector3 currentSpawnPos;
    private int spawnIndex;

    private bool hasGameFinished;
    private bool isLeft;
    private bool canSwitch;

    private void Awake()
    {
        obstaclePool = GameObject.Find("SpawnController").GetComponent<ObstaclePool>();
        rescaleController = GetComponent<RescaleController>();
        directionController = GetComponent<OpositeDirectioneMoveController>();
    }
    private void Start()
    {
        hasGameFinished = false;
        
        spawnIndex = Random.Range(0, _spawnPos.Count);
        currentSpawnPos = _spawnPos[spawnIndex];
        isLeft = Random.Range(0,2) == 0;
        currentSpawnPos.x *= isLeft ? -1f : 1f;
        transform.position = currentSpawnPos;
        canSwitch = Random.Range(0, 4) == 0 && spawnIndex != 0;
        
        
        if(canSwitch)
        {
            if(directionController != null) { StartCoroutine(directionController.MoveToOppositeDirection()); }
            else { Debug.Log("DIRECTION ERROR"); }
        }
        
    }

    private void OnEnable()
    {
        GameManager.Instance.GameEnded += OnGameEnded;
    }

    private void OnDisable()
    {
        GameManager.Instance.GameEnded -= OnGameEnded;
    }


    private void FixedUpdate()
    {
        if (hasGameFinished) return;

        transform.position += _moveSpeed * Time.fixedDeltaTime * Vector3.up;
        
        if(transform.position.y > _maxOffset)
        {
            obstaclePool.ReturnToPool(gameObject);
        }
    }

    public void OnGameEnded()
    {
        GetComponent<Collider2D>().enabled = false;
        hasGameFinished = true;
        if(rescaleController != null) { StartCoroutine(rescaleController.Rescale()); }
        else { Debug.Log("RESCALE ERROR"); }
    }
}
