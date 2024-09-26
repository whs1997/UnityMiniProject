using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum State { Idle, Jump };

    [SerializeField] Rigidbody2D rigid;
    [SerializeField] SpriteRenderer render;
    [SerializeField] Collider2D coll;
    private State state;

    [SerializeField] float moveSpeed;

    [SerializeField] float minJumpPower;
    [SerializeField] float maxJumpPower;
    [SerializeField] float jumpCharge;
    [SerializeField] float jumpPower;
    // [SerializeField] bool isGroundLeft;
    // [SerializeField] bool isGroundRight;
    [SerializeField] bool isGround;
    [SerializeField] bool isCharging;

    // [SerializeField] Transform groundCheckLeft;
    // [SerializeField] Transform groundCheckRight;
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
    }

    private void Move()
    {
        if((isGround) && !isCharging && rigid.velocity.y == 0)
        {
            float moveDir = Input.GetAxisRaw("Horizontal");
            rigid.velocity = new Vector2(moveDir * moveSpeed, rigid.velocity.y);
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
        // isGround = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.2f, groundLayer);

        if (Input.GetKey(KeyCode.Space) && (isGround))
        {
            // charging
            rigid.velocity = new Vector2(0, 0); // �̵����� ����
            isCharging = true; // ���� �����߿� �̵��� �� ����
            jumpPower += jumpCharge * Time.deltaTime; // ������ ����
            jumpPower = Mathf.Clamp(jumpPower, minJumpPower, maxJumpPower); // �ִ� ������������
        }

        if (Input.GetKeyUp(KeyCode.Space) && (isGround))
        {
            float jumpDirection = x;
            rigid.velocity = new Vector2(jumpDirection * moveSpeed, jumpPower);
            // isGroundLeft = false;
            // isGroundRight = false;
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

        // isGroundLeft = Physics2D.Raycast(groundCheckLeft.position, Vector2.down, 0.1f, groundLayer);
        // isGroundRight = Physics2D.Raycast(groundCheckRight.position, Vector2.down, 0.1f, groundLayer);
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // isGroundLeft = false;
            // isGroundRight = false;
            isGround = false;
        }
    }
}
