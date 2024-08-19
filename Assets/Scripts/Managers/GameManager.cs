using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour,IToggleSound,IUpdateScore,ISceneController
{
    #region MONOBEHAVIOUR METHODS

    public static GameManager Instance { get; private set; }
    private CameraMoveController cameraController;
    private SpawnController spawnController;
    private ColorChangeController colorChangeController;

    [SerializeField]
    private TMP_Text _scoreText, _endScoreText,_highScoreText;

    private int score;

    [SerializeField]
    private Animator _scoreAnimator;

    [SerializeField]
    private AnimationClip _scoreClip;
 
    [SerializeField]
    private GameObject _endPanel;

    [SerializeField]
    private Image _soundImage;

    [SerializeField]
    private Sprite _activeSoundSprite, _inactiveSoundSprite;

    private void Awake()
    {
        Instance = this;
        colorChangeController = GetComponent<ColorChangeController>();
        cameraController = GetComponent<CameraMoveController>();
        spawnController = GameObject.Find("SpawnController").GetComponent<SpawnController>();
    }

    private void OnEnable()
    {
        colorChangeController.ColorChanged += colorChangeController.OnColorChanged;
    }

    private void OnDisable()
    {
        colorChangeController.ColorChanged -= colorChangeController.OnColorChanged;
    }

    private void Start()
    {
        AudioManager.Instance.AddButtonSound();

        StartCoroutine(IStartGame());
    }

    #endregion

    #region UI

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(Constants.DATA.MAIN_MENU_SCENE);
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToggleSound()
    {
        bool sound = (PlayerPrefs.HasKey(Constants.DATA.SETTINGS_SOUND) ? PlayerPrefs.GetInt(Constants.DATA.SETTINGS_SOUND)
            : 1) == 1;
        sound = !sound;
        PlayerPrefs.SetInt(Constants.DATA.SETTINGS_SOUND, sound ? 1 : 0);
        _soundImage.sprite = sound ? _activeSoundSprite : _inactiveSoundSprite;
        AudioManager.Instance.ToggleSound();
    }

    public void UpdateScore()
    {
        score++;
        _scoreText.text = score.ToString();
        _scoreAnimator.Play(_scoreClip.name, -1, 0f);

        if(score % 2 == 0)
        {
            colorChangeController.CurrentColorId =  (colorChangeController.CurrentColorId + 1) % colorChangeController._colors.Count;
        }
    }

    #endregion

    #region GAME_START_END

    public void EndGame()
    {
        StartCoroutine(GameOver());
    }

    [SerializeField] private Animator _highScoreAnimator;
    [SerializeField] private AnimationClip _highScoreClip;

    private IEnumerator GameOver()
    {
        hasGameEnded = true;
        _scoreText.gameObject.SetActive(false);
        GameEnded?.Invoke();

        yield return new WaitForSeconds(1f);
        yield return cameraController.MoveCamera(new Vector3(_cameraStartPos.x,-_cameraStartPos.y,_cameraStartPos.z));

        _endPanel.SetActive(true);
        _endScoreText.text = score.ToString();
        _endPanel.GetComponent<Image>().color = colorChangeController.CurrentColor;

        bool sound = (PlayerPrefs.HasKey(Constants.DATA.SETTINGS_SOUND) ?
          PlayerPrefs.GetInt(Constants.DATA.SETTINGS_SOUND) : 1) == 1;
        _soundImage.sprite = sound ? _activeSoundSprite : _inactiveSoundSprite;

        int highScore = PlayerPrefs.HasKey(Constants.DATA.HIGH_SCORE) ? PlayerPrefs.GetInt(Constants.DATA.HIGH_SCORE) : 0;
        if (score > highScore)
        {
            _highScoreText.text = "NEW BEST";

            //Play HighScore Animation
            _highScoreAnimator.Play(_highScoreClip.name,-1,0f);

            highScore = score;
            PlayerPrefs.SetInt(Constants.DATA.HIGH_SCORE, highScore);
        }
        else
        {
            _highScoreText.text = "BEST " + highScore.ToString();
        }
    }


    [SerializeField]
    private Vector3 _cameraStartPos, _cameraEndPos;
    
    [HideInInspector] public bool hasGameEnded;
    public UnityAction GameStarted, GameEnded;

    private IEnumerator IStartGame()
    {
        hasGameEnded = false;
        Camera.main.transform.position = _cameraStartPos;
        _scoreText.gameObject.SetActive(false);
        yield return cameraController.MoveCamera(_cameraEndPos);

        //Score
        _scoreText.gameObject.SetActive(true);
        score = 0;
        _scoreText.text = score.ToString();
        _scoreAnimator.Play(_scoreClip.name, -1, 0f);

        //Set the Color
        colorChangeController.CurrentColorId = 0;

        StartCoroutine(spawnController.SpawnObjects());
        GameStarted?.Invoke();
    }
    
    #endregion
}
