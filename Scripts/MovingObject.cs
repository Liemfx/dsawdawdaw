using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour
{
    // Time it will take object to move, in seconds
    public float moveTime = 0.1f;

    // Layer on which collision will be checked
    public LayerMask blockingLayer;

    // Private variables
    private BoxCollider2D boxCollider;  // The BoxCollider2D component attached to this object
    private Rigidbody2D rb2D;           // The Rigidbody2D component attached to this object
    private float inverseMoveTime;      // Used to make movement more efficient

    // Called before the first frame update
    protected virtual void Start()
    {
        // Get the BoxCollider2D component attached to this object
        boxCollider = GetComponent<BoxCollider2D>();

        // Get the Rigidbody2D component attached to this object
        rb2D = GetComponent<Rigidbody2D>();

        // Calculate the inverse of moveTime for more efficient movement calculations
        inverseMoveTime = 1f / moveTime;
    }

    // Attempt to move the object
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        // Disable the box collider to avoid self-collision detection
        boxCollider.enabled = false;

        // Cast a line from start to end position to check for collisions
        hit = Physics2D.Linecast(start, end, blockingLayer);

        // Re-enable the box collider after collision check
        boxCollider.enabled = true;

        // If no collision, move smoothly towards the target position
        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        // Return false if movement is blocked by an obstacle
        return false;
    }

    // Coroutine for smooth movement
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon)
        {
            // Calculate the next position to move to
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            // Move the object to the new position
            rb2D.MovePosition(newPosition);

            // Recalculate the remaining distance
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            // Wait for the next frame
            yield return null;
        }
    }

    // Attempt to move the object and handle collisions
    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        // If there was no collision, return
        if (hit.transform == null)
            return;

        // Get the component on the collided object
        T hitComponent = hit.transform.GetComponent<T>();

        // If movement is blocked and the collided object has the required component, handle collision
        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);
    }

    // Abstract method to be implemented by inheriting classes to handle collisions
    protected abstract void OnCantMove<T>(T component)
        where T : Component;
}
