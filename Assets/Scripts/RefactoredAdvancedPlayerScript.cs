using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class RefactoredAdvancedPlayerScript : MonoBehaviour
{
    [Header("Player Settings")]
    public float speed = 10f;
    public float jumpHeight = 7f;
    public float dashSpeed = 20f;
    public float crouchHeight = 0.5f;
    public LayerMask whatIsGround;
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackRange = 1f;
    public LayerMask enemyLayers;
    private Rigidbody2D body;
    private Animator anim;
    private AudioSource audioPlayer;
    private bool grounded;
    private bool canDoubleJump = false;
    private bool isDashing = false;
    private bool isCrouching = false;

    private bool facingRight = true;

    // Start is called before the first frame update
    void Awake()
    {
        InitializeComponents();
    }

    private void InitializeComponents() {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() 
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        grounded = CheckIfGrounded();
        HandleMovement();
    }

    private void Flip() {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        facingRight = !facingRight;
    }

    private void Jump() {
        body.velocity = new Vector2(body.velocity.x, jumpHeight);
        anim.SetTrigger("jump");
        grounded = false;
        AudioManager.instance.PlayJumpSound();
    }

    //Coroutine
    IEnumerator Dash() {
        AudioManager.instance.PlayDashSound();
        float originalSpeed = speed;
        speed = dashSpeed;
        isDashing = true;
        yield return new WaitForSeconds(0.2f);
        speed = originalSpeed;
        isDashing = false;
    }

    private void PlaySound(AudioClip clip) {
        audioPlayer.clip = clip;
        audioPlayer.Play();
    }

    void Attack() {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayers);
        foreach(Collider2D enemy in hitEnemies) {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null) {
                enemyController.TakeDamage(attackDamage);
                Debug.Log("Enemy Damaged");
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void HandleInput() {
        HandleCrouch();
        HandleAttack();
        HandleDash();
        HandleJump();
    }

    private void HandleJump() {
        if (Input.GetKeyDown(KeyCode.Space) && grounded) {
            canDoubleJump = true;
            Jump();
        }

        else if (Input.GetKeyDown(KeyCode.Space) && canDoubleJump) {
            Jump();
            canDoubleJump = false;
        }
    }

    private void HandleMovement() {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        anim.SetBool("walk", horizontalInput != 0);

        if (horizontalInput != 0 && grounded) {
            AudioManager.instance.PlayFootstepSound();
        }
        
        if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight)) {
            Flip();
        }
    }

    private void HandleDash() {
        if (Input.GetKey(KeyCode.LeftShift) && !isDashing) {
            StartCoroutine(Dash());
        }
    }

    private void HandleCrouch() {
        if (Input.GetKeyDown(KeyCode.DownArrow) && grounded) {
            if (!isCrouching) {
                transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
                isCrouching = true;
            }
            
            else if (isCrouching) {
            transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
            isCrouching = false;
            }
        }
    }

    private void HandleAttack() {
        if (Input.GetKeyDown(KeyCode.E)) {
            Attack();
        }
    }

    private bool CheckIfGrounded() {
        return Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, whatIsGround);
    }
}
