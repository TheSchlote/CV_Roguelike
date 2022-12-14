using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public TMP_Text foodText;

    private Animator animator;
    private int food;

    bool verticalInputInUse = false, horizontalInputInUse = false;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        foodText.text = $"Food: {food}";
        base.Start();
    }

    private void Update()
    {
        if (GameManager.instance.playersTurn)
        {
            
            int horizontalInput = (int)Input.GetAxisRaw("Horizontal");
            int verticalInput = (int)Input.GetAxisRaw("Vertical");
            if (verticalInput == 0)
            {
                verticalInputInUse = false;
            }
            if (horizontalInput == 0)
            {
                horizontalInputInUse = false;
            }

            if (verticalInput != 0 && !verticalInputInUse)
            {
                verticalInputInUse = true;
                AttemptMove<Wall>(horizontalInput, verticalInput);
            }
            if (horizontalInput != 0 && !horizontalInputInUse)
            {
                horizontalInputInUse = true;
                AttemptMove<Wall>(horizontalInput, verticalInput);
            }
        }
    }
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;
        foodText.text = $"Food: {food}";
        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;
        CheckIfGameOver();
        GameManager.instance.playersTurn = false;
    }
    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    private void Restart()
    {
        SceneManager.LoadScene("Main");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Exit":
                Invoke(nameof(Restart), restartLevelDelay);
                enabled = false;
                break;
            case "Food":
                food += pointsPerFood;
                foodText.text = $"+ {pointsPerFood} Food: {food}";
                other.gameObject.SetActive(false);
                break;
            case "Soda":
                food += pointsPerSoda;
                foodText.text = $"+ {pointsPerSoda} Food: {food}";
                other.gameObject.SetActive(false);
                break;
        }
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = $"-{loss}Food: {food}";
        CheckIfGameOver();
    }

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            GameManager.instance.GameOver();
        }
    }
}
