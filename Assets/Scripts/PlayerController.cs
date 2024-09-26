using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum State { Idle, Run, Charge, Jump, Down, Slide, Size };
    private State state = State.Idle;

    [SerializeField] Rigidbody2D rigid;
    [SerializeField] SpriteRenderer render;
    [SerializeField] Collider2D coll;
    [SerializeField] PhysicsMaterial2D wall;
    [SerializeField] Animator animator;

    [SerializeField] float moveSpeed;

    [SerializeField] float minJumpPower;
    [SerializeField] float maxJumpPower;
    [SerializeField] float jumpCharge;
    [SerializeField] float jumpPower;
    [SerializeField] float maxFallSpeed;

    [SerializeField] bool isGround;
    [SerializeField] bool isCharging;
    [SerializeField] bool isJumping;

    private float startFallHeight;
    public float fallTheashold = 5f;
    Vector2 previousPos;

    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;

    private float x;

    // �ִϸ��̼�
    private static int idleHash = Animator.StringToHash("Idle");
    private static int runHash = Animator.StringToHash("Run");
    private static int jumpHash = Animator.StringToHash("Jump");
    private static int fallHash = Animator.StringToHash("Fall");
    private static int chargeHash = Animator.StringToHash("Charge");
    private static int slideHash = Animator.StringToHash("Slide");
    private static int downHash = Animator.StringToHash("Down");
    public int curAniHash;

    private void Start()
    {
        previousPos = transform.position;
    }

    private void Update()
    {
        x = Input.GetAxisRaw("Horizontal");
        Run();
        Jump();
        GroundChecker();
        AnimatorPlay();

        Falling();

        /*
        switch (state) // ���� ���� ����
        {
            case State.Idle:
                Idle();
                break;
            case State.Run:
                Run();
                break;
            case State.Charge:
                Charge();
                break;
            case State.Jump:
                Jumping();
                break;
            case State.Falled:
                Falled();
                break;            
        }
        */
    }

    private void Idle()
    {

    }

    private void Run()
    {
        if (isGround && !isCharging && rigid.velocity.y == 0) // �ٴڿ� �־���ϰ� �����߿� �̵� X
        {
            rigid.velocity = new Vector2(x * moveSpeed, rigid.velocity.y);
        }

        // �������� �̵����� �ʰ� �ؾ��� �ڵ� �߰�
        //


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

    private void Falling()
    {
        Vector2 curPos = transform.position;
        float fallHeight = previousPos.y - curPos.y;

        Debug.Log($"fallHeight = {fallHeight}");
        if(fallHeight > fallTheashold)
        {
            Debug.Log("�ƾ�");
        }
        previousPos = curPos;
    }

    private void GroundChecker()
    {
        Vector2 boxSize = new Vector2(coll.bounds.size.x, 0.1f); // �÷��̾��� �¿� ũ�� �������� �ڽ�
        Vector2 boxOrigin = new Vector2(coll.bounds.center.x, coll.bounds.min.y); // �÷��̾��� �ٴ� ��� ��ġ
        isGround = Physics2D.BoxCast(boxOrigin, boxSize, 0, Vector2.down, 0.1f, groundLayer); // �ٴڸ� �˻�
    }

    public void AnimatorPlay()
    {
        int checkAniHash = 0; // �÷��� �� �ִϸ��̼� �ؽ� ��

        if (rigid.velocity.y > 0.01f && !isGround)
            checkAniHash = jumpHash;

        if (rigid.velocity.y < -0.01f && !isGround)
            checkAniHash = fallHash;

        if (rigid.velocity.sqrMagnitude < 0.01f && isGround)
            checkAniHash = idleHash;

        if (rigid.velocity.sqrMagnitude > 0.01f && isGround)
            checkAniHash = runHash;

        if (Input.GetKey(KeyCode.Space) && isGround && rigid.velocity.sqrMagnitude < 0.1f)
            checkAniHash = chargeHash;
        if (Input.GetKeyUp(KeyCode.Space))
            checkAniHash = idleHash;
        
        if(curAniHash != checkAniHash)
        {
            curAniHash = checkAniHash;
            animator.Play(curAniHash);
        }

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
