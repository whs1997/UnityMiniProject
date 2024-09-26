using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // public enum State { Idle, Walk, Charging, Jumping, Flatten };
    // private State state = State.Idle;

    [SerializeField] Rigidbody2D rigid;
    [SerializeField] SpriteRenderer render;
    [SerializeField] Collider2D coll;
    [SerializeField] PhysicsMaterial2D wall;

    [SerializeField] float moveSpeed;

    [SerializeField] float minJumpPower;
    [SerializeField] float maxJumpPower;
    [SerializeField] float jumpCharge;
    [SerializeField] float jumpPower;
    [SerializeField] float maxFallSpeed;

    [SerializeField] bool isGround;
    [SerializeField] bool isCharging;
    [SerializeField] bool isJumping;

    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;

    private float x;

    private void Start()
    {
    }

    private void Update()
    {
        x = Input.GetAxisRaw("Horizontal");
        Move();
        Jump();
        GroundChecker();
        /*
        switch (state) // 상태 패턴 구현
        {
            case State.Idle:
                Idle();
                break;
            case State.Walk:
                Walk();
                break;
            case State.Charging:
                Charging();
                break;
            case State.Jumping:
                Jumping();
                break;
            case State.Flatten:
                Flatten();
                break;
            
        }
        */
    }

    private void Move()
    {
        if (isGround && !isCharging && rigid.velocity.y == 0) // 바닥에 있어야하고 점프중엔 이동 X
        {
            rigid.velocity = new Vector2(x * moveSpeed, rigid.velocity.y);
        }

        // 벽쪽으론 이동하지 않게 해야

        if (rigid.velocity.y < -maxFallSpeed) // 최대 낙하속도 제한
        {
            rigid.velocity = new Vector2(rigid.velocity.x, -maxFallSpeed);
        }

        if (x < 0)
        {
            render.flipX = true; // 왼쪽으로 입력하면 좌우 반전
        }
        else if (x > 0)
        {
            render.flipX = false;
        }
    }

    private void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            // charging
            rigid.velocity = new Vector2(0, 0); // 이동하지 않음
            isCharging = true; // 점프 충전중엔 이동할 수 없음
            jumpPower += jumpCharge * Time.deltaTime; // 점프힘 증가
            jumpPower = Mathf.Clamp(jumpPower, minJumpPower, maxJumpPower); // 최대 점프힘까지만
        }

        // 최대 힘이 되면 따로 입력 안해도 점프되게

        if (Input.GetKeyUp(KeyCode.Space) && isGround)
        {
            rigid.velocity = new Vector2(x * moveSpeed, jumpPower); // x방향 moveSpeed, y방향 jumpPower만큼 이동. 포물선?
            isGround = false;
            isCharging = false; // 충전 해제
            jumpPower = 0f; // 점프힘 0 초기화
        }
    }

    private void GroundChecker()
    {
        Vector2 boxSize = new Vector2(coll.bounds.size.x, 0.1f); // 플레이어의 좌우 크기 사이즈의 박스
        Vector2 boxOrigin = new Vector2(coll.bounds.center.x, coll.bounds.min.y); // 플레이어의 바닥 가운데 위치
        isGround = Physics2D.BoxCast(boxOrigin, boxSize, 0, Vector2.down, 0.1f, groundLayer); // 바닥면 검사
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
                Vector2 normal = collision.GetContact(0).normal;
                rigid.AddForce(normal * 3f, ForceMode2D.Impulse);

                Debug.Log("벽에 튕겨나감");
        }
        
    }
}
