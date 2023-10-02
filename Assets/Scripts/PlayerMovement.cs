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
        
    }
}
