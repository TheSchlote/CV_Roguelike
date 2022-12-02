using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDelay = 1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    public bool playersTurn = true;

    private TMP_Text levelText;
    private GameObject levelImage;
    private int level = 1;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance);
        }
        DontDestroyOnLoad(gameObject);
        enemies= new List<Enemy>();
        boardScript= GetComponent<BoardManager>();
        InitGame();
    }
    private void OnSceneLoaded(int index)
    {
        
        level++;
        InitGame();
    }

    private void InitGame()
    {
        doingSetup= true;
        levelImage = GameObject.Find("lvlImage");
        levelText = GameObject.Find("LevelText").GetComponent<TMP_Text>();
        levelText.text = $"Day {level}";
        levelImage.SetActive(true);
        Invoke(nameof(HideLevelImage), levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        levelText.text = $"After {level} days, you starved.";
        levelImage.SetActive(true);
        enabled = false;
    }

    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
        {
            return;
        }

        StartCoroutine(MoveEnemies());
    }

    public void AddEmemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    private IEnumerator MoveEnemies()
    {
        enemiesMoving= true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving= false;
    }
}
