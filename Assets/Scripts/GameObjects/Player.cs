using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Player : MonoBehaviour
{ 
    private bool canMove;
    private bool canShoot;
    private RescaleController rescaleController;
    private ScorePool scorePool;

    [SerializeField]
    private AudioClip _moveClip, _pointClip, _scoreClip, _loseClip;

    [SerializeField]
    private GameObject _explosionPrefab;

    [SerializeField] private float _startSpeed;
    [SerializeField] private float _boundsX;
    [SerializeField] private float _fastMoveSpeedMultiplier, _slowMoveSpeedMultiplier;

    private float speedMagnitude;
    private float speedMultiplier;
    private bool isSlow;

    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _fastRotateSpeedMultiplier, _slowRotateSpeedMultiplier;

    private float currentRotateValue;
    private float rotateMagnitude;
    private float rotateSpeedMultiplier;
    
    private void Awake()
    {
        rescaleController = GetComponent<RescaleController>();
        scorePool = GameObject.Find("SpawnController").GetComponent<ScorePool>();
        canShoot = false;
        canMove = false;
    }

    private void OnEnable()
    {
        GameManager.Instance.GameStarted += GameStarted;
        GameManager.Instance.GameEnded += OnGameEnded;
    }

    private void OnDisable()
    {
        GameManager.Instance.GameStarted -= GameStarted;
        GameManager.Instance.GameEnded -= OnGameEnded;
    }

    private void GameStarted()
    {
        canMove = true;
        canShoot = true;

        isSlow = true;
        speedMagnitude = 1f;
        speedMultiplier = _slowMoveSpeedMultiplier;

        currentRotateValue = 0f;
        rotateMagnitude = 1f;
        rotateSpeedMultiplier = _slowRotateSpeedMultiplier;
    }

    private void Update()
    {
        if(canShoot && Input.GetMouseButtonDown(0))
        {
            isSlow = !isSlow;
            speedMultiplier = isSlow ? _slowMoveSpeedMultiplier : _fastMoveSpeedMultiplier;
            rotateSpeedMultiplier = isSlow ? _slowRotateSpeedMultiplier : _fastMoveSpeedMultiplier;
            AudioManager.Instance.PlaySound(_moveClip);
        }
    }


    private void FixedUpdate()
    {
        if (!canMove) return;

        transform.position += (speedMagnitude * speedMultiplier * _startSpeed * Time.fixedDeltaTime *Vector3.right);

        currentRotateValue += (rotateMagnitude * rotateSpeedMultiplier * _rotateSpeed * Time.fixedDeltaTime);

        transform.rotation = Quaternion.Euler(0,0,currentRotateValue);

        if(transform.position.x < -_boundsX || transform.position.x  > _boundsX)
        {
            speedMagnitude *= -1f;

            AudioManager.Instance.PlaySound(_pointClip);

            if(currentRotateValue > 360f || currentRotateValue < 0f)
            {
                rotateMagnitude *= -1f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(Constants.Tags.SCORE))
        {
            GameManager.Instance.UpdateScore();
            AudioManager.Instance.PlaySound(_scoreClip);
            scorePool.ReturnToPool(collision.gameObject);
        }

        if(collision.CompareTag(Constants.Tags.OBSTACLE))
        {
            Destroy(Instantiate(_explosionPrefab,transform.position,Quaternion.identity), 3f);
            AudioManager.Instance.PlaySound(_loseClip);
            GameManager.Instance.EndGame();
            Destroy(gameObject);
        }
    }

    public void OnGameEnded()
    {
        StartCoroutine(rescaleController.Rescale());
    }
}