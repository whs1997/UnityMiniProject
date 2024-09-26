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
        switch (state) // ���� ���� ����
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
        if (isGround && !isCharging && rigid.velocity.y == 0) // �ٴڿ� �־���ϰ� �����߿� �̵� X
        {
            rigid.velocity = new Vector2(x * moveSpeed, rigid.velocity.y);
        }

        // �������� �̵����� �ʰ� �ؾ�

        if (rigid.velocity.y < -maxFallSpeed) // �ִ� ���ϼӵ� ����
        {
            rigid.velocity = new Vector2(rigid.velocity.x, -maxFallSpeed);
        }

        if (x < 0)
        {
            render.flipX = true; // �������� �Է��ϸ� �¿� ����
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
            rigid.velocity = new Vector2(0, 0); // �̵����� ����
            isCharging = true; // ���� �����߿� �̵��� �� ����
            jumpPower += jumpCharge * Time.deltaTime; // ������ ����
            jumpPower = Mathf.Clamp(jumpPower, minJumpPower, maxJumpPower); // �ִ� ������������
        }

        // �ִ� ���� �Ǹ� ���� �Է� ���ص� �����ǰ�

        if (Input.GetKeyUp(KeyCode.Space) && isGround)
        {
            rigid.velocity = new Vector2(x * moveSpeed, jumpPower); // x���� moveSpeed, y���� jumpPower��ŭ �̵�. ������?
            isGround = false;
            isCharging = false; // ���� ����
            jumpPower = 0f; // ������ 0 �ʱ�ȭ
        }
    }

    private void GroundChecker()
    {
        Vector2 boxSize = new Vector2(coll.bounds.size.x, 0.1f); // �÷��̾��� �¿� ũ�� �������� �ڽ�
        Vector2 boxOrigin = new Vector2(coll.bounds.center.x, coll.bounds.min.y); // �÷��̾��� �ٴ� ��� ��ġ
        isGround = Physics2D.BoxCast(boxOrigin, boxSize, 0, Vector2.down, 0.1f, groundLayer); // �ٴڸ� �˻�
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
                Vector2 normal = collision.GetContact(0).normal;
                rigid.AddForce(normal * 3f, ForceMode2D.Impulse);

                Debug.Log("���� ƨ�ܳ���");
        }
        
    }
}
