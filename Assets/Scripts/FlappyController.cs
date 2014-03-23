using System;
using System.IO;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class FlappyController : MonoBehaviour
{
    private bool _canPlay;
    private bool _isPlaying;
    private int _recycledTube;
    private float _verticalSpeed;
    private Vector3 _currentPosition;
    private Vector3 _tubeCurrentPosition;
    private Vector3 _birdieStartingPosition;
    private Vector3 _tubeHolderStartingPosition;

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

    public BirdieSwing Swing;
    public GameObject EndMenu;
    public GameObject BeginMenu;
    public GameObject GetReadyMenu;

    public Transform[] Tubes;
    public Sprite[] Backgrounds;

    private int _score;
    public int Score { get { return _score; } set { _score = value; ScoreText.text = string.Format("{0}", _score); } }
    private int _hiscore;
    public int HiScore { get { return _hiscore; } set { _hiscore = value; File.WriteAllText(_highscoreFile, string.Format("{0}", _hiscore)); } }
    private Button _touchedButton;
    private Button TouchedButton { set { if(value == null && _touchedButton != null) _touchedButton.OnClick(); _touchedButton = value; } }
    private bool IsDead { get { return BirdieAnimation.GetBool("IsDead"); } set { BirdieAnimation.SetBool("IsDead", value); } }

    private string _highscoreFile;

    void Start()
    {
        Instance = this;

        _tubeHolderStartingPosition = TubeHolder.position;
        _birdieStartingPosition = BirdieTransform.position;
        _highscoreFile = Path.Combine(Application.persistentDataPath, "HIGHSCORE");

        BirdieTransform.position = Vector3.zero;
        AssignLayerOrders();

        if (File.Exists(_highscoreFile))
            HiScore = int.Parse(File.ReadAllText(_highscoreFile));
        else
            HiScore = 0;

        AspectRatioScaler.localScale = Vector2.right * ((((float)Screen.width)/Screen.height)/(9.0f/16.0f)) + Vector2.up;
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

    void AssignLayerOrders()
    {
        ScoreText.renderer.sortingOrder = 2;
    }

    void UpdateTouch()
    {
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
                OnTouch(touch.position);      
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                TouchedButton = null;
            }
        }
#else
        if (Input.GetMouseButtonDown(0))
        {
            OnTouch(Input.mousePosition);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            TouchedButton = null;
        }
#endif
    }

    void OnTouch(Vector3 position)
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
            }
        }
        else
        {
            if(BirdieTransform.position.y < 2.25f)
                _verticalSpeed = JumpSpeed;
        }
    }

    public void Reset()
    {
        Score = 0;
        _canPlay = true;
        _recycledTube = 0;
        Swing.enabled = true;
        GroundAnimation.speed = 1;
        ScoreText.gameObject.SetActive(true);
        BirdieAnimation.SetBool("IsDead", false);
        BirdieAnimation.SetInteger("Color", Random.Range(0, 3));
        TubeHolder.position = _tubeHolderStartingPosition;
        BirdieTransform.position = _birdieStartingPosition;
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

    public void BirdieCrashed()
    {
        _isPlaying = false;
        if (Score > HiScore) HiScore = Score;
        ScoreText.gameObject.SetActive(false);
        FinalScoreText.text = string.Format("{0}", Score);
        FinalHiScoreText.text = string.Format("{0}", HiScore);
        BirdieAnimation.transform.localRotation = Quaternion.Euler(0, 0, -90);
        BirdieAnimation.SetBool("IsDead", true);
        GroundAnimation.speed = 0;
        EndMenu.SetActive(true);
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

    public void RecycleTube(Transform tube)
    {
        for (int i = 0; i < tube.childCount; i++)
            tube.GetChild(i).collider2D.enabled = true;
         
        tube.localPosition = new Vector3(_recycledTube * 2, Random.Range(-0.5f, 1.5f), 0);
        _recycledTube++;
    }

    #region Singleton Implementation
    public static FlappyController Instance { get; private set; }
    #endregion

     RaycastHit2D? GetFrontmostRaycastHit()
     {
        RaycastHit2D[] hits = Physics2D.LinecastAll (Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.ScreenToWorldPoint(Input.mousePosition));
 
        if (hits.Length != 0)
        {
            int topSortingLayer = 0;
            int[] sortingLayerIDArray = new int[hits.Length];
            int[] sortingOrderArray= new int[hits.Length];
            int topSortingOrder= 0;
            int indexOfTopSortingOrder = 0;
 
            for (var i = 0; i < hits.Length; i++)
            {
                SpriteRenderer spriteRenderer = (SpriteRenderer)hits[i].collider.gameObject.GetComponent(typeof(SpriteRenderer));
                if (spriteRenderer == null) continue;
                sortingLayerIDArray[i] = spriteRenderer.sortingLayerID;
                sortingOrderArray[i] = spriteRenderer.sortingOrder;
            }
               
            for (var j = 0; j < sortingLayerIDArray.Length; j++)
            {
                if (sortingLayerIDArray[j] >= topSortingLayer)
                {
                    topSortingLayer = sortingLayerIDArray[j];
                }
            }
 
            for (var k = 0; k < sortingOrderArray.Length; k++)
            {
                if (sortingOrderArray[k] >= topSortingOrder && sortingLayerIDArray[k] == topSortingLayer)
                {
                    topSortingOrder = sortingOrderArray[k];
                    indexOfTopSortingOrder = k;
                }
            }
 
            return hits[indexOfTopSortingOrder];
        }
        return null;
    }
}
