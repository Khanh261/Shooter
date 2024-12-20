using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public TextMeshProUGUI newWaveTitle;
    public TextMeshProUGUI newWaveEnemyCount;
    public TextMeshProUGUI scoreUI;
    public TextMeshProUGUI gameOverScoreUI;
    public RectTransform healthBar;
    public RectTransform healthBarPlayerTwo;
    public TextMeshProUGUI winnerUI;

    Spawner spawner;
    Player player;
    PlayerTwo playerTwo;

    void Start()
    {
        player = FindObjectOfType<Player>();
        playerTwo = FindObjectOfType<PlayerTwo>();
        FindObjectOfType<Player>().OnDeath += OnGameOver;
        FindObjectOfType<PlayerTwo>().OnDeath += OnGameOver;
    }

    void Update()
    {
        scoreUI.text = ScoreKeeper.score.ToString("D6");
        CalculateAndSetHealthUI(player, healthBar);
        CalculateAndSetHealthUI(playerTwo, healthBarPlayerTwo);
    }

    void CalculateAndSetHealthUI(LivingEntity player, RectTransform healthBar)
    {
        float healthPercent = 0;
        if (player != null)
        {
            healthPercent = player.health / player.startingHealth;
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
    }

    void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        string[] numbers = { "One", "Two", "Three", "Four", "Five", "Six", "Seven" };
        newWaveTitle.text = "- Wave " + numbers[waveNumber - 1] + " -";
        string enemyCountString = (
            (spawner.waves[waveNumber - 1].infinite)
                ? "Infinite"
                : spawner.waves[waveNumber - 1].enemyCount + ""
        );
        newWaveEnemyCount.text = "Enemies: " + enemyCountString;

        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    void OnGameOver()
    {
        if (player.health > 0)
        {
            winnerUI.text = "Player 1 win";
        }
        else
        {
            winnerUI.text = "Player 2 win";
        }
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, .95f), 1));
        gameOverScoreUI.text = scoreUI.text;
        scoreUI.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float delayTime = 1.5f;
        float speed = 3f;
        float animatePercent = 0;
        int dir = 1;

        float endDelayTime = Time.time + 1 / speed + delayTime;

        while (animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed * dir;

            if (animatePercent >= 1)
            {
                animatePercent = 1;
                if (Time.time > endDelayTime)
                {
                    dir = -1;
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-170, 45, animatePercent);
            yield return null;
        }
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
