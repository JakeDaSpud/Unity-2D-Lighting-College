using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f;
    public float jumpHeight = 7f;
    private Rigidbody2D body;
    private Animator anim;
    private bool grounded;
    private bool facingRight = true;

//Called before start(), use for declaring GameObjects like Animator, RigidBody2D etc
    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // FixedUpdate is called once per specified amount of frames, disregarding game's framerate
    void FixedUpdate()
    {
        //GetAxisRaw has no smoothing
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

        anim.SetBool("walk", horizontalInput != 0);
        //anim.SetFloat("run", anim.GetFloat("run")+=0.1);

        if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight)) {
            Flip();
        }

        if (Input.GetKey(KeyCode.Space) && grounded) {
            Jump();
        }
    }

    private void Jump() {
        body.velocity = new Vector2(body.velocity.x, jumpHeight);
        anim.SetTrigger("jump");
        grounded = false;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Ground")) {
            anim.ResetTrigger("jump");
            grounded = true;
        }
    }

    private void Flip() {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        facingRight = !facingRight;
    }
}
