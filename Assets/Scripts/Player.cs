using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float jumpYVelocity = 8f;
    [SerializeField] float runXVelocity = 4f;
    [SerializeField] float raycastDistance = 0.7f;
    [SerializeField] LayerMask collisionMask;

    Animator animator;
    Rigidbody2D physics;
    SpriteRenderer sprite;

    enum State { Idle, Walk, Jump, Fall, Attack, Crouch, Climb }

    State state = State.Idle;
    bool isGrounded = false;
    bool jumpInput = false;
    bool attackInput = false;
    bool isAttacking = false;
    bool crouchInput = false;
    bool climbInput = false;
    bool isWalled = false;


    float horizontalInput = 0f;

    private void Update()
    {
        // get player input
        isGrounded = Physics2D.Raycast(this.transform.position, Vector2.down, raycastDistance, collisionMask).collider != null;
        jumpInput = Input.GetKey(KeyCode.Space);
        attackInput = Input.GetKey(KeyCode.K);
        crouchInput = Input.GetKey(KeyCode.LeftControl);
        climbInput = Input.GetKey(KeyCode.L);
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {

        Debug.Log(isWalled);

        // flip sprite based on horizontal input
        if (horizontalInput < 0f)
        {
            sprite.flipX = false;
        }
        else if (horizontalInput > 0f)
        {
            sprite.flipX = true;
        }

        // run current state
        switch (state)
        {
            case State.Idle: IdleState(); break;
            case State.Walk: WalkState(); break;
            case State.Jump: JumpState(); break;
            case State.Fall: FallState(); break;
            case State.Attack: AttackState(); break;
            case State.Crouch: CrouchState(); break;
            case State.Climb: ClimbState(); break;
        }
    }

    void IdleState()
    {
        // actions
        animator.Play("Idle");

        // transitions
        if (isGrounded)
        {
            if (jumpInput)
            {
                state = State.Jump;
            }
            else if (horizontalInput != 0f)
            {
                state = State.Walk;
            }
            else if (attackInput)
            {
                isAttacking = true;
                state = State.Attack;
            }
            else if (crouchInput)
            {
                state = State.Crouch;
            }
            else if (climbInput && isWalled)
            {
                state = State.Climb;
            }
        }
        else
        {
            state = State.Fall;
        }
    }

    void WalkState()
    {
        // actions
        animator.Play("Run");
        physics.velocity = runXVelocity * horizontalInput * Vector2.right;

        // transitions
        if (isGrounded)
        {
            if (jumpInput)
            {
                state = State.Jump;
            }
            else if (horizontalInput == 0f)
            {
                state = State.Idle;
            }
            //else if (attackInput) Só rola com animação de Attack enquanto anda
            //{
            //    state = State.Attack;
            //}
            else if (crouchInput)
            {
                state = State.Crouch;
            }
        }
        else if (climbInput && isWalled)
        {
            state = State.Climb;
        }
        else
        {
            state = State.Fall;
        }

    }

    void JumpState()
    {
        // actions
        animator.Play("Jump");

        physics.velocity = (runXVelocity * horizontalInput * Vector2.right) + (jumpYVelocity * Vector2.up);

        if (climbInput && isWalled)
        {
            state = State.Climb;
        }
        else
        {
            // transitions
            state = State.Fall;
        }
    }

    void FallState()
    {
        // actions
        if (physics.velocity.y > 0f)
        {
            animator.Play("Jump");
        }
        else
        {
            animator.Play("Fall");
        }

        physics.velocity = physics.velocity.y * Vector2.up + runXVelocity * horizontalInput * Vector2.right;

        // transitions
        if (isGrounded)
        {
            if (horizontalInput != 0f)
            {
                state = State.Walk;
            }
            else
            {
                state = State.Idle;
            }
        }
    }

    void AttackState()
    {
        // actions
        if(isAttacking && Input.GetKey(KeyCode.S)) 
        {
            animator.Play("Attack1");
        }
        else if(isAttacking && Input.GetKey(KeyCode.W))
        {
            animator.Play("Attack3");
        }
        else
        {
            animator.Play("Attack2");
        }

        // transitions
        if (!isAttacking)
        {
            if (isGrounded)
            {
                if (jumpInput)
                {
                    state = State.Jump;
                }
                else if (horizontalInput != 0f)
                {
                    state = State.Walk;
                }
                else if (horizontalInput == 0f)
                {
                    state = State.Idle;
                }
            }
            else
            {
                state = State.Fall;
            }
        }
    }

    public void EndOfAttack()
    {
        isAttacking = false;
    }

    void CrouchState()
    {
        // actions
        physics.velocity = new Vector2(0, physics.velocity.y);
        animator.Play("Crouch");

        // transitions
        if (!crouchInput)
        {
            if (isGrounded)
            {
                if (jumpInput)
                {
                    state = State.Jump;
                }
                else if (horizontalInput != 0f)
                {
                    state = State.Walk;  
                }
                else if (horizontalInput == 0f)
                {
                    state = State.Idle;
                }
            }
        }
        else if (!isGrounded)
        {
            state = State.Fall;
        }
    }

    void ClimbState()
    {
        // actions
        animator.Play("Climb");

        // transitions
        if (isGrounded)
        {
            if (jumpInput)
            {
                state = State.Jump;
            }
            else if (horizontalInput != 0f)
            {
                state = State.Walk;
            }
            else if (horizontalInput == 0f)
            {
                state = State.Idle;
            }
        }
        else
        {
            state = State.Fall;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isWalled = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isWalled = false;
        }
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        physics = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

}
