using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    // Public variable for the player's health decrease when hit by enemy
    public int playerDeath;

    // Private variables
    private Animator animator;
    private Transform target;
    private bool skipMove;

    // Override Start method
    protected override void Start()
    {
        // Add this enemy to the GameManager's list of enemies
        GameManager.instance.AddEnemyToList(this);

        // Get the Animator component attached to this object
        animator = GetComponent<Animator>();

        // Find the player GameObject by its tag and get its transform
        target = GameObject.FindGameObjectWithTag("Player").transform;

        // Call the Start method of the base class
        base.Start();
    }

    // Override AttemptMove method
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        // If it's time to skip move, return
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        // Call AttemptMove method of the base class
        base.AttemptMove<T>(xDir, yDir);

        // Set skipMove to true for the next move
        skipMove = true;
    }

    // Method to move the enemy towards the player
    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        // Determine direction to move based on player's position relative to enemy
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            yDir = target.position.y > transform.position.y ? 1 : -1;
        else
            xDir = target.position.x > transform.position.x ? 1 : -1;

        // Attempt to move the enemy in the determined direction
        AttemptMove<Player>(xDir, yDir);
    }

    // Override OnCantMove method
    protected override void OnCantMove<T>(T component)
    {
        // Cast the component to Player
        Player hitPlayer = component as Player;

        // Trigger the enemyAttack animation
        animator.SetTrigger("enemyAttack");

        // Reduce player's health
        hitPlayer.LoseHealth(playerDeath);
    }
}
