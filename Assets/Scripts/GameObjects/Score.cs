using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed,
        _maxOffset,
        _rotateSpeed;
    private RescaleController rescaleController;
    private ScorePool scorePool;

    private bool hasGameFinished;

    [SerializeField] private List<Vector3> _spawnPos;

    private void Awake()
    {
        scorePool = GameObject.Find("SpawnController").GetComponent<ScorePool>();
        rescaleController = GetComponent<RescaleController>();
    }
    private void Start()
    {
        hasGameFinished = false;
        
        transform.position = _spawnPos[UnityEngine.Random.Range(0, _spawnPos.Count)];
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

        transform.Rotate(_rotateSpeed * Time.fixedDeltaTime * Vector3.forward);

        if (transform.position.y > _maxOffset)
        {
            scorePool.ReturnToPool(gameObject);
        }
    }

    public void OnGameEnded()
    {
        GetComponent<Collider2D>().enabled = false;
        hasGameFinished = true;
        StartCoroutine(rescaleController.Rescale());
    }    
}
