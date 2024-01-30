using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int PlayerKeyPoints = 0;
    public int playerHealth = 1;
    [HideInInspector] public bool playersTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private GameObject Restard;
    private Text Restart;
    private bool doingSetup;
    private int level = 1;
    private List<Enemy> enemies;
    private bool enemiesMoving;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]

    static public void CallbackInitialization()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.level++;
        instance.InitGame();
    }

    public void ResetLevel()
    {
        level = 1;  // Reset the level to 1
        playerHealth = 1; // Reset player health or any other relevant variables
        PlayerKeyPoints = 0; // Reset player points
        InitGame();  // Reinitialize the game
    }


    void InitGame()
    {
        turnDelay = 0.1f;
        PlayerKeyPoints = 0;
        doingSetup = true;
        Restard = GameObject.Find("Restard");
        levelImage = GameObject.Find("LevelImage");
        Restart = GameObject.Find("Restart").GetComponent<Text>();
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        levelText.text = "Level: " + level;

        levelImage.SetActive(true);
        Restard.SetActive(false);
        Invoke("HideLevelImage", levelStartDelay);
        enemies.Clear();
        boardScript.Setup(level);
    }

 

    void HideLevelImage()
    {
        //Disable the levelImage gameObject.
        levelImage.SetActive(false);

        //Set doingSetup to false allowing player to move again.
        doingSetup = false;
    }

    public void GameOver()
    {
        // Set levelText to display number of levels passed and game over message
        levelText.text = "You made it to level " + level + "!";

        // Enable black background image gameObject.
        levelImage.SetActive(true);
        Restard.SetActive(true);

        // Disable this GameManager after a delay.
        Invoke("DisableGameManager", 2f); // Change to the delay you want in seconds.
    }

    void DisableGameManager()
    {
        enabled = false;
    }
    // Update is called once per frame
    void Update()
    { 
        if (playersTurn || enemiesMoving || doingSetup)
            return;
        StartCoroutine(MoveEnemies());
    }
    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
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
        enemiesMoving = false;
    }
}