using System.Collections;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f;
    public LayerMask blockigLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inversMoveTime;

    protected virtual void Start()
    {
        boxCollider= GetComponent<BoxCollider2D>();
        rb2D= GetComponent<Rigidbody2D>();
        inversMoveTime = 1f / moveTime;
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled= false;
        hit = Physics2D.Linecast(start, end, blockigLayer);
        boxCollider.enabled= true;

        if(hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        return false;
    }

    protected IEnumerator SmoothMovement (Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inversMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    protected virtual void AttemptMove <T> (int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
        {
            return;
        }

        T hitComponenet = hit.transform.GetComponent<T>();

        if(!canMove && hitComponenet != null)
        {
            OnCantMove(hitComponenet);
        }
    }

    protected abstract void OnCantMove<T>(T component)
        where T : Component;
}
