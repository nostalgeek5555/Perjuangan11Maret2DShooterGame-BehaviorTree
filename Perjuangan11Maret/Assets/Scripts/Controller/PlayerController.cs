using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using System.Linq;

public class PlayerController : MonoBehaviour, IDamageAble<float>, IKillAble
{
    public PlayerStateType stateType;
    public Rigidbody2D playerRb;
    public Animator playerAnimator;
    private Vector3 localScale;
    public GameObject bullet;
    public Transform firePoint;
    [SerializeField] private float playerHP;
    [SerializeField] private float movementSpeed = 0.5f;
    [SerializeField] private float currentSpeed = 0;
    [SerializeField] private float dirX;
    [SerializeField] private bool facingRight = true;
    public float jumpStrength = 600;
    [SerializeField] private float fallMultiplier = 0.4f;
    [SerializeField] private float lowFallMultiplier = 0.6f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumping = false;

    
    public float PlayerHP
    {
        get => playerHP;
        set => playerHP = value;
    }

    private void Start()
    {
        isGrounded = true;
        stateType = PlayerStateType.Alive;
    }

    private void Awake()
    {
        localScale = transform.localScale;
    }

    private void Update()
    {
        if (stateType != PlayerStateType.Shoot && isGrounded)
        {
            dirX = Input.GetAxisRaw("Horizontal") * movementSpeed;

            if (Mathf.Abs(dirX) > 0 && playerRb.velocity.y == 0)
            {
                SwitchPlayerState(PlayerStateType.Run);
                playerAnimator.SetBool(PlayerStateType.Run.ToString(), true);
            }

            if (Mathf.Abs(dirX) <= 0 && playerRb.velocity.y == 0)
            {
                SwitchPlayerState(PlayerStateType.Run);
                playerAnimator.SetBool(PlayerStateType.Run.ToString(), false);
            }
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();

            if (playerRb.velocity.y > 0)
            {
                playerAnimator.SetBool(PlayerStateType.Jump.ToString(), true);
            }

            if (playerRb.velocity.y == 0)
            {
                playerAnimator.SetBool(PlayerStateType.Jump.ToString(), false);
                //playerAnimator.SetBool(PlayerStateType.Fall.ToString(), true);
            }
        }

        if (Input.GetButtonDown("Shoot") && playerRb.velocity.y <= 0)
        {
            Shoot();
        }

        if (stateType == PlayerStateType.Shoot && Input.GetButtonUp("Shoot"))
        {
            if (isGrounded)
            {
                SwitchPlayerState(PlayerStateType.Idle);
            }
        }
    }

    private void FixedUpdate()
    {
        playerRb.velocity = new Vector2(dirX, playerRb.velocity.y);
        //transform.Translate(Vector3.forward * dirX);
    }

    private void LateUpdate()
    {
        if (dirX > 0)
        {
            facingRight = true;
        }

        else if (dirX < 0)
        {
            facingRight = false;
        }

        if ((facingRight && (transform.localScale.x < 0)) || (!facingRight && (transform.localScale.x > 0)))
        {
            localScale.x *= -1;
            transform.localScale = localScale;
        }
        
    }

    private void Jump()
    {
        if (isGrounded)
        {
            SwitchPlayerState(PlayerStateType.Jump);
            isGrounded = false;
            if (playerRb.velocity.y <= 0)
            {
                playerRb.velocity += Vector2.up * /*Physics2D.gravity.y * (fallMultiplier - playerRb.gravityScale) **/ Time.deltaTime * jumpStrength;
            }
            else if (playerRb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                playerRb.velocity += Vector2.up * /*Physics2D.gravity.y * (lowFallMultiplier - 1) * Time.deltaTime **/ jumpStrength;
            }
        }
    }

    private void Shoot()
    {
        SwitchPlayerState(PlayerStateType.Shoot);
        playerAnimator.SetTrigger(PlayerStateType.Shoot.ToString());
        LeanPool.Spawn(bullet, firePoint.position, firePoint.rotation);
        Bullet bulletSc = bullet.GetComponent<Bullet>();
        bulletSc.agentTransform = transform;
        bulletSc.SetDirection(localScale.x);
        bulletSc.Traverse(localScale.x, bulletSc.traverseSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("gameobject layer " + LayerMask.LayerToName(collision.gameObject.layer));
        //Debug.Log($"Ground layer {Convert.ToString(collision.gameObject.layer).PadLeft(32, '0')}");

        string collisionLayer = LayerMask.LayerToName(collision.gameObject.layer);

        switch (collisionLayer)
        {
            case "Ground":
                isGrounded = true;
                break;

            case "Enemy":
                break;

            default:
                break;
        }
    }

    public void SwitchPlayerState(PlayerStateType playerStateType)
    {
        stateType = playerStateType;
        switch (playerStateType)
        {
            case PlayerStateType.Run:
                break;
            case PlayerStateType.Jump:
                break;
            case PlayerStateType.Fall:
                break;
            default:
                break;
        }
    }

    public void TakeDamage(float damageTaken)
    {
        if (playerHP > 0)
        {
            playerHP -= damageTaken;
        }

        else
        {
            Kill();
        }
    }

    public void Kill()
    {
        Debug.Log("kill player");
    }

    public enum PlayerStateType
    {
        Alive, Idle, Run, Jump, Fall, Shoot, Hit, Dead
    }

    public enum PlayerFacing
    {
        Left, Right
    }

    public enum CollisionLayer
    {
        Bullet, Ground, Enemy
    }

}
