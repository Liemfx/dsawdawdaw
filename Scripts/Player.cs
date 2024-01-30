using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    // Public variables
    public int pointsPerKey = 1;
    public float restartLevelDelay = 1f;
    public Text foodText;

    // Private variables
    private Animator animator;
    private int Health;
    private int Key;
    private bool isGameOver = false;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    protected override void Start()
    {
        // Get the Animator component
        animator = GetComponent<Animator>();

        // Get player's health and key points from GameManager
        Health = GameManager.instance.playerHealth;
        Key = GameManager.instance.PlayerKeyPoints;

        // Update key text
        KeyText.text = "Keys: " + Key;

        // Call the Start method of the base class
        base.Start();

        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Called when the GameObject is disabled
    private void OnDisable()
    {
        // Update GameManager with player's health and key points
        GameManager.instance.playerHealth = Health;
        GameManager.instance.PlayerKeyPoints = Key;
    }

    // Set the game over status
    public void SetGameOver(bool gameOver)
    {
        isGameOver = gameOver;
    }

    // Update is called once per frame
    void Update()
    {
        // If game over or not player's turn or health is zero, return
        if (isGameOver || !GameManager.instance.playersTurn || Health <= 0)
        {
            return;
        }

        // Read input for movement
        int horizontal = 0;
        int vertical = 0;

        if (Input.GetKeyDown(KeyCode.A))
        {
            horizontal = -1;
            vertical = 0;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            horizontal = 1;
            vertical = 0;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            horizontal = 0;
            vertical = 1;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            horizontal = 0;
            vertical = -1;
        }

        // If there's a movement input, attempt to move
        if (horizontal != 0 || vertical != 0)
        {
            // Flip the sprite horizontally
            spriteRenderer.flipX = !spriteRenderer.flipX;
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    // Attempt to move the player
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        base.AttemptMove<T>(xDir, yDir);

        // Check if the game is over after movement
        CheckIfGameOver();

        // It's not the player's turn after the move
        GameManager.instance.playersTurn = false;
    }

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the player collides with the exit and has a key, restart the level
        if (other.tag == "Exit" && Key == 1)
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        // If the player collides with key, collect the key
        else if (other.tag == "Key")
        {
            Key += pointsPerKey;
            KeyText.text = " Keys: " + Key;
            other.gameObject.SetActive(false);
        }
    }

    // Handle what happens when the player can't move
    protected override void OnCantMove<T>(T component)
    {
        // Currently, do nothing
    }

    // Restart the level
    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    // Decrease player's health
    public void LoseHealth(int loss)
    {
        // Trigger the player death animation
        animator.SetTrigger("playerDeath");

        // Decrease player's health
        Health -= loss;

        // Check if the game is over after health loss
        CheckIfGameOver();
    }

    // Check if the game is over
    private void CheckIfGameOver()
    {
        // If the player's health reaches zero, trigger game over after a delay
        if (Health <= 0)
        {
            Invoke("GameOver", 3f);
        }
    }

    // Handle the game over state
    private void GameOver()
    {
        // Trigger the game over state in GameManager
        GameManager.instance.GameOver();
    }
}
