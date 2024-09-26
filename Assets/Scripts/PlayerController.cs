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

    // 애니메이션
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
        switch (state) // 상태 패턴 구현
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
        if (isGround && !isCharging && rigid.velocity.y == 0) // 바닥에 있어야하고 점프중엔 이동 X
        {
            rigid.velocity = new Vector2(x * moveSpeed, rigid.velocity.y);
        }

        // 벽쪽으론 이동하지 않게 해야함 코드 추가
        //


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

    private void Falling()
    {
        Vector2 curPos = transform.position;
        float fallHeight = previousPos.y - curPos.y;

        Debug.Log($"fallHeight = {fallHeight}");
        if(fallHeight > fallTheashold)
        {
            Debug.Log("아야");
        }
        previousPos = curPos;
    }

    private void GroundChecker()
    {
        Vector2 boxSize = new Vector2(coll.bounds.size.x, 0.1f); // 플레이어의 좌우 크기 사이즈의 박스
        Vector2 boxOrigin = new Vector2(coll.bounds.center.x, coll.bounds.min.y); // 플레이어의 바닥 가운데 위치
        isGround = Physics2D.BoxCast(boxOrigin, boxSize, 0, Vector2.down, 0.1f, groundLayer); // 바닥면 검사
    }

    public void AnimatorPlay()
    {
        int checkAniHash = 0; // 플레이 할 애니메이션 해시 값

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


            Debug.Log("벽에 튕겨나감");
        }
    }
}
