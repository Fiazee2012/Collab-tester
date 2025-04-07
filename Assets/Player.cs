using System;
using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [Serializable] public struct MovementSettings { public float speed, jumpForce; }
    [Serializable] public struct DashSettings { public float force, duration, cooldown; }

    public MovementSettings movement;
    public DashSettings dash;

    private Rigidbody2D _rb;
    private Animator _anim;
    private bool _isGrounded, _canDoubleJump, _isDashing, _canDash = true;
    private bool _facingRight = true;
    private Vector3 _originalScale;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _originalScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void Update()
    {
        HandleJump();

        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash)
            StartCoroutine(Dash());
    }

    private void HandleMovement()
    {
        if (_isDashing) return;

        float move = 0f;
        if (Input.GetKey(KeyCode.A)) move = -1f;
        if (Input.GetKey(KeyCode.D)) move = 1f;

        _rb.linearVelocity = new Vector2(move * movement.speed, _rb.linearVelocity.y);

        if (move != 0)
        {
            _facingRight = move > 0;
            transform.localScale = new Vector3((_facingRight ? 1 : -1) * Mathf.Abs(_originalScale.x),
                                               _originalScale.y,
                                               _originalScale.z);
        }

        _anim.SetBool("isRunning", move != 0);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (_isGrounded)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, movement.jumpForce);
                _canDoubleJump = true;
                _isGrounded = false; 
                _anim.SetBool("isJumping", true);
            }
            else if (_canDoubleJump)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, movement.jumpForce);
                _canDoubleJump = false;
                _anim.SetBool("isJumping", true);
            }
        }
    }

    private IEnumerator Dash()
    {
        _isDashing = true;
        _canDash = false;

        _anim.SetBool("isDashing", true);
        _anim.SetBool("isJumping", false); 

        _rb.gravityScale = 0; 
        _rb.linearVelocity = new Vector2((_facingRight ? 1 : -1) * dash.force, 0);

        yield return new WaitForSeconds(dash.duration);

        _rb.gravityScale = 1; 
        _isDashing = false;

        _anim.SetBool("isDashing", false);

        if (!_isGrounded)
            _anim.SetBool("isJumping", true);

        yield return new WaitForSeconds(dash.cooldown);
        _canDash = true;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _isGrounded = true;
            _canDoubleJump = false;
            _anim.SetBool("isJumping", false);
            int a = 1;
        }
    }
}
