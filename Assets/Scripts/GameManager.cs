using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState
    {
        Playing,
        Won,
        Lost
    }

    private ObjectPooler pooler;
    private AudioManager audioManager;
    private AudioSource music;

    [SerializeField] private LevelSpawner level;
    [SerializeField] private ScreenShake cameraScreen;

    [Header("Game Settings")]
    [SerializeField] private int startingLives = 5;
    [SerializeField] private float gameTime = 30f;

    [Header("UI")]
    [SerializeField] private Transform canvas;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text diamondText;
    [SerializeField] private GameObject[] heartFills;

    [Header("End Screens")]
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    private int currentLives;
    private int collectedDiamonds;

    private float currentTime;
    private bool countdownStarted;
    private GameState currentState;

    public GameState CurrentState => currentState;
    public int CollectedDiamonds => collectedDiamonds;
    public int CurrentLives => currentLives;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        pooler = ObjectPooler.instance;
        audioManager = AudioManager.instance;
        music = GetComponent<AudioSource>();

        level.GenerateGrid();

        // reset game
        currentLives = startingLives;
        collectedDiamonds = 0;
        currentTime = gameTime;
        currentState = GameState.Playing;

        // reset hearts
        for (int i = 0; i < heartFills.Length; i++)
        {
            heartFills[i].SetActive(true);
        }

        UpdateUI();
    }

    private void Update()
    {
        if (currentState != GameState.Playing)
            return;

        // countdown
        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            LoseGame();
        }

        UpdateTimerUI();
    }

    public void CollectDiamond()
    {
        if (currentState != GameState.Playing)
            return;

        collectedDiamonds++;

        // update score
        UpdateDiamondUI();

        // check win
        if (collectedDiamonds >= level.TotalDiamonds)
        {
            WinGame();
        }
        audioManager.Play("Collect");
    }

    public void TakeDamage()
    {
        if (currentState != GameState.Playing)
            return;

        cameraScreen.Shake();
        
        currentLives--;

        // disable last heart
        if (currentLives >= 0 && currentLives < heartFills.Length)
            heartFills[currentLives].transform.GetChild(0).gameObject.SetActive(false);

        // update lives
        UpdateLivesUI();

        // check lose
        if (currentLives <= 0)
        {
            currentLives = 0;
            LoseGame();
        }
        
        audioManager.Play("Wrong");
    }

    private void WinGame()
    {
        audioManager.Stop();
        music.Stop();

        if (currentState != GameState.Playing)
            return;

        currentState = GameState.Won;
        Invoke(nameof(ShowWinScreen), 2f);
    }

    private void ShowWinScreen()
    {
        // show win screen
        if (winScreen != null)
            winScreen.SetActive(true);

        audioManager.Play("Win");
    }

    private void LoseGame()
    {
        audioManager.Stop();
        music.Stop();

        if (currentState != GameState.Playing)
            return;

        currentState = GameState.Lost;
        Invoke(nameof(ShowLoseScreen), 2f);
    }

    private void ShowLoseScreen()
    {
        // show game over screen
        if (loseScreen != null)
            loseScreen.SetActive(true);

        audioManager.Play("Lose");
    }

    public void RestartLevel()
    {
        audioManager.Play("Click");
        // reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateUI()
    {
        UpdateTimerUI();
        UpdateLivesUI();
        UpdateDiamondUI();
    }

    private void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int seconds = Mathf.CeilToInt(currentTime);
        if(!countdownStarted && seconds < 11)
        {
            countdownStarted = true;
            audioManager.Play("Countdown");
        }

        timerText.text = seconds.ToString();
    }

    private void UpdateLivesUI()
    {
        if (livesText == null)
            return;

        livesText.text = currentLives.ToString();
    }

    private void UpdateDiamondUI()
    {
        if (diamondText == null)
            return;

        diamondText.text = $"<color=#FFE700>{collectedDiamonds}</color>/<color=#18B8F2>{level.TotalDiamonds}</color>";
    }

    public void ShowFloatingTextAt(Vector3 worldPosition)
    {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        Transform floatingTxt = pooler.Get("FloatingText", screenPosition, Quaternion.identity).transform;
        floatingTxt.SetParent(canvas);

        // pop & float
        floatingTxt.localScale = Vector3.zero;
        floatingTxt.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        floatingTxt.DOMoveY(floatingTxt.position.y + 80f, 0.6f).SetEase(Ease.OutQuad);

        // fade
        Image floatingImg = floatingTxt.GetComponent<Image>();
        floatingImg.DOFade(0f, 0.3f).SetDelay(0.25f).OnComplete(() =>
        {
            floatingImg.DOFade(1f, 0f);
            pooler.Return(floatingTxt.gameObject);
        });
    }
}