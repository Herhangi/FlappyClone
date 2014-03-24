﻿using System;
using System.IO;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class FlappyController : MonoBehaviour
{
    #region Properties
    private int _score;
    public int Score { get { return _score; } set { _score = value; ScoreText.text = string.Format("{0}", _score); } }
    private int _hiscore;
    public int HiScore { get { return _hiscore; } set { _hiscore = value; File.WriteAllText(_highscoreFile, string.Format("{0}", _hiscore)); } }
    private Button _touchedButton;
    private Button TouchedButton { set { if (value == null && _touchedButton != null) _touchedButton.OnClick(); _touchedButton = value; } }
    private bool IsDead { get { return BirdieAnimation.GetBool("IsDead"); } set { BirdieAnimation.SetBool("IsDead", value); } }
    #endregion
    #region Public Variables
    public BirdieSwing Swing;
    public GameObject EndMenu;
    public GameObject BeginMenu;
    public GameObject GetReadyMenu;
    public TextMesh TapText;
    public TextMesh ScoreText;
    public TextMesh FinalScoreText;
    public TextMesh FinalHiScoreText;
    public Transform AspectRatioScaler;
    public float Gravity;
    public float JumpSpeed;
    public float TubeSpeed;
    public float MaximumFallSpeed;
    public Transform TubeHolder;
    public Animator BirdieAnimation;
    public Animator GroundAnimation;
    public Transform BirdieTransform;
    public SpriteRenderer GetReadySprite;
    public SpriteRenderer BackgroundSprite;

    public Transform[] Tubes;
    public Sprite[] Backgrounds;
    #endregion
    #region Private Variables
    private bool _canPlay;
    private bool _isPlaying;
    private int _recycledTube;
    private float _verticalSpeed;
    private string _highscoreFile;
    private Vector3 _currentPosition;
    private Vector3 _tubeCurrentPosition;
    private Vector3 _birdieStartingPosition;
    private Vector3 _tubeHolderStartingPosition;
    #endregion

    #region Unity Methods
    void Start()
    {
        Instance = this;

        _tubeHolderStartingPosition = TubeHolder.position;
        _birdieStartingPosition = BirdieTransform.position;
        _highscoreFile = Path.Combine(Application.persistentDataPath, "HIGHSCORE");

        BirdieTransform.position = Vector3.zero;

        if (File.Exists(_highscoreFile))
            HiScore = int.Parse(File.ReadAllText(_highscoreFile));
        else
            HiScore = 0;

        AspectRatioScaler.localScale = Vector2.right * ((((float)Screen.width) / Screen.height) / (9.0f / 16.0f)) + Vector2.up;
        
#if UNITY_ANDROID && !UNITY_EDITOR
        AdMobUnityPlugin.StartAds();
        AdMobUnityPlugin.HideAds();
#endif
    }

	void Update ()
    {
	    if (_isPlaying)
        {
            _verticalSpeed = Mathf.Max(-MaximumFallSpeed, _verticalSpeed - (Gravity * Time.deltaTime));
	        _currentPosition.y += _verticalSpeed*Time.deltaTime;
	        BirdieTransform.position = _currentPosition;

	        _tubeCurrentPosition.x -= TubeSpeed*Time.deltaTime;
	        TubeHolder.position = _tubeCurrentPosition;
            
            #region Birdie Rotation
            if (_verticalSpeed > 0.25f)
            {
                if (BirdieAnimation.transform.localRotation.eulerAngles.z < 29 || BirdieAnimation.transform.localRotation.eulerAngles.z > 31)
                    BirdieAnimation.transform.localRotation = Quaternion.Euler(BirdieAnimation.transform.localRotation.eulerAngles + new Vector3(0,0, 30));
	        }
            else if (Math.Abs(_verticalSpeed + MaximumFallSpeed) < 0.01f)
                BirdieAnimation.transform.localRotation = Quaternion.Euler(0, 0, -60);
            else if (_verticalSpeed < -MaximumFallSpeed / 2f)
                BirdieAnimation.transform.localRotation = Quaternion.Euler(0, 0, -30);
            else
                BirdieAnimation.transform.localRotation = Quaternion.Euler(0, 0, 0);
            #endregion
        }
        else if (IsDead && _currentPosition.y > -1.60f)
        {
            _verticalSpeed = -MaximumFallSpeed;
            _currentPosition.y += _verticalSpeed * Time.deltaTime;
            BirdieTransform.position = _currentPosition;
	    }

	    UpdateTouch();
	}
    #endregion
    #region Touch Methods
    void UpdateTouch()
    {
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
                OnTouch();      
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                TouchedButton = null;
            }
        }
#else
        if (Input.GetMouseButtonDown(0))
        {
            OnTouch();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            TouchedButton = null;
        }
#endif
    }

    void OnTouch()
    {
        RaycastHit2D[] hits = Physics2D.LinecastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.ScreenToWorldPoint(Input.mousePosition));

        foreach (RaycastHit2D hit in hits)
        {
            Button button = hit.transform.GetComponent<Button>();
            if (button != null)
            {
                TouchedButton = button;
                button.Touched();
                return;
            }
        }
        TouchedButton = null;

        if (!_isPlaying)
        {
            if (_canPlay)
            {
                _canPlay = false;
                _isPlaying = true;
                Swing.enabled = false;
                _currentPosition = BirdieTransform.position;
                _tubeCurrentPosition = TubeHolder.position;
                StartCoroutine(ChangeTextOpacity(TapText, 0, 100));
                StartCoroutine(ChangeOpacity(GetReadySprite, 0, 100));
                _verticalSpeed = JumpSpeed;
                audio.Play();
            }
        }
        else
        {
            if(BirdieTransform.position.y < 2.25f)
                _verticalSpeed = JumpSpeed;
            audio.Play();
        }
    }
    #endregion

    public void Reset()
    {
        Score = 0;
        IsDead = false;
        _canPlay = true;
        _recycledTube = 0;
        Swing.enabled = true;
        GroundAnimation.speed = 1;
#if UNITY_ANDROID && !UNITY_EDITOR
        AdMobUnityPlugin.HideAds();
#endif
        ScoreText.gameObject.SetActive(true);
        TubeHolder.position = _tubeHolderStartingPosition;
        BirdieTransform.position = _birdieStartingPosition;
        BirdieAnimation.SetInteger("Color", Random.Range(0, 3));
        BirdieAnimation.transform.localRotation = Quaternion.Euler(0, 0, 0);
        BackgroundSprite.sprite = Backgrounds[Random.Range(0, Backgrounds.Length)];

        foreach (Transform tube in Tubes)
        {
            RecycleTube(tube);
        }
        EndMenu.SetActive(false);
        BeginMenu.SetActive(false);
        GetReadyMenu.SetActive(true);
        StartCoroutine(ChangeTextOpacity(TapText, 1, 50));
        StartCoroutine(ChangeOpacity(GetReadySprite, 1, 50));
        TapText.gameObject.SetActive(true);
    }
    public bool BirdieCrashed()
    {
        if (!_isPlaying) return false;

        IsDead = true;
        _isPlaying = false;
        if (Score > HiScore) HiScore = Score;
        ScoreText.gameObject.SetActive(false);
        FinalScoreText.text = string.Format("{0}", Score);
        FinalHiScoreText.text = string.Format("{0}", HiScore);
        BirdieAnimation.transform.localRotation = Quaternion.Euler(0, 0, -90);
        GroundAnimation.speed = 0;
        EndMenu.SetActive(true);
        
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Random.Range(0, 100) < 50) AdMobUnityPlugin.SetPosition("middle", "top");
        else AdMobUnityPlugin.SetPosition("middle", "bottom");
        AdMobUnityPlugin.ShowAds();
#endif
        return true;
    }
    public void RecycleTube(Transform tube)
    {
        for (int i = 0; i < tube.childCount; i++)
            tube.GetChild(i).collider2D.enabled = true;

        tube.localPosition = new Vector3(_recycledTube * 2, Random.Range(-0.5f, 1.5f), 0);
        _recycledTube++;
    }

    IEnumerator ChangeTextOpacity(TextMesh sprite, float targetOpacity, int updateCount)
    {
        int current = 0;
        Color color = sprite.color;

        while (current < updateCount)
        {
            color.a = Mathf.Lerp(color.a, targetOpacity, ((float)current) / updateCount);
            sprite.color = color;
            foreach (SpriteRenderer child in sprite.GetComponentsInChildren<SpriteRenderer>())
            {
                child.color = color;
            }
            yield return new WaitForEndOfFrame();
            current++;
        }

        color.a = targetOpacity;
        sprite.color = color;
        foreach (SpriteRenderer child in sprite.GetComponentsInChildren<SpriteRenderer>())
        {
            child.color = color;
        }
    }
    IEnumerator ChangeOpacity(SpriteRenderer sprite, float targetOpacity, int updateCount)
    {
        int current = 0;
        Color color = sprite.color;

        while (current < updateCount)
        {
            color.a = Mathf.Lerp(color.a, targetOpacity, ((float)current)/updateCount);
            sprite.color = color;
            foreach (SpriteRenderer child in sprite.GetComponentsInChildren<SpriteRenderer>())
            {
                child.color = color;
            }
            yield return new WaitForEndOfFrame();
            current++;
        }

        color.a = targetOpacity;
        sprite.color = color;
        foreach (SpriteRenderer child in sprite.GetComponentsInChildren<SpriteRenderer>())
        {
            child.color = color;
        }
    }

    #region Singleton Implementation
    public static FlappyController Instance { get; private set; }
    #endregion
}