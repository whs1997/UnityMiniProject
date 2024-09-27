using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum State { Idle, Run, Charge, Jump, Fall, Land, Down, Slide, Size };
    [SerializeField] private State curState = State.Idle;
    private BaseState[] states = new BaseState[(int)State.Size];

    [Header("State")]
    [SerializeField] IdleState idleState;
    [SerializeField] RunState runState;
    [SerializeField] ChargeState chargeState;
    [SerializeField] JumpState jumpState;
    [SerializeField] FallState fallState;
    [SerializeField] LandState landState;
    [SerializeField] DownState downState;
    [SerializeField] SlideState slideState;

    [Header("Components")]
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] SpriteRenderer render;
    [SerializeField] Collider2D coll;
    [SerializeField] PhysicsMaterial2D wall;
    [SerializeField] Animator animator;

    [Header("Controls")]
    [SerializeField] float moveSpeed; // 이동 속도
    [SerializeField] float minJumpPower; // 점프 최소힘
    [SerializeField] float maxJumpPower; // 점프 최대힘
    [SerializeField] float jumpCharge; // 점프 충전력
    [SerializeField] float jumpPower; // 뛸 점프 힘
    [SerializeField] float maxFallSpeed; // 최대 낙하 속도
    [SerializeField] bool isGround; // 땅에 있는 여부
    [SerializeField] bool isCharging; // 충전 중
    [SerializeField] float currentFallSec = 0f; // 땅에 있지 않을 시 기록하는 체공 시간
    [SerializeField] Transform groundCheck; // isGround 체크를 위한 플레이어 밑 오브젝트
    [SerializeField] LayerMask groundLayer; // Ground 레이어 검사
    [SerializeField] bool frontWall;
    [SerializeField] LayerMask wallLayer; // Wall 레이어 검사

    private float x; // Horizontal 입력값 받음

    // 애니메이션
    private static int idleHash = Animator.StringToHash("Idle");
    private static int runHash = Animator.StringToHash("Run");
    private static int jumpHash = Animator.StringToHash("Jump");
    private static int fallHash = Animator.StringToHash("Fall");
    private static int chargeHash = Animator.StringToHash("Charge");
    private static int slideHash = Animator.StringToHash("Slide");
    private static int downHash = Animator.StringToHash("Down");
    private static int landHash = Animator.StringToHash("Land");

    // 사운드
    [Header("SFX")]
    [SerializeField] AudioClip jumpClip;
    [SerializeField] AudioClip landClip;
    [SerializeField] AudioClip downClip;
    [SerializeField] AudioClip hitClip;


    private void Start()
    {
        states[(int)curState].Enter();
    }

    private void Awake()
    {
        states[(int)State.Idle] = idleState;
        states[(int)State.Run] = runState;
        states[(int)State.Charge] = chargeState;
        states[(int)State.Jump] = jumpState;
        states[(int)State.Fall] = fallState;
        states[(int)State.Land] = landState;
        states[(int)State.Down] = downState;
        states[(int)State.Slide] = slideState;
    }

    private void Update()
    {
        states[(int)curState].Update();
        x = Input.GetAxisRaw("Horizontal");
        Flip();
        GroundChecker();
        FrontChecker();
    }

    private void FixedUpdate()
    {
        CheckFalling();
    }


    private void Flip()
    {
        if (curState == State.Jump ||
           curState == State.Fall ||
           curState == State.Down ||
           curState == State.Slide)
            return; // 해당 상태들에선 Flip 하지 않기

        if (x < 0)
            render.flipX = true; // 왼쪽으로 입력하면 좌우 반전
        else if (x > 0)
            render.flipX = false;
    }

    private void CheckFalling()
    {
        if (rigid.velocity.y < 0 && !isGround)
            currentFallSec += Time.deltaTime; // 낙하중일때 시간 증가

        if (Mathf.Abs(rigid.velocity.y) < 0.01f && isGround) // y 이동이 없으면 체공시간 0
            currentFallSec = 0;

        if (rigid.velocity.y < -maxFallSpeed) // 최대 낙하속도 제한
        {
            rigid.velocity = new Vector2(rigid.velocity.x, -maxFallSpeed);
        }
    }

    private void GroundChecker()
    {
        Vector2 boxSize = new Vector2(coll.bounds.size.x, 0.1f); // 플레이어의 좌우 크기 사이즈의 박스
        Vector2 boxOrigin = new Vector2(coll.bounds.center.x, coll.bounds.min.y); // 플레이어의 바닥 가운데 위치
        isGround = Physics2D.BoxCast(boxOrigin, boxSize, 0, Vector2.down, 0.1f, groundLayer); // 바닥면 검사
    }

    private void FrontChecker()
    {
        Vector2 rayDirection = Vector2.right * x; // ray를 쏠 방향
        frontWall = Physics2D.Raycast(transform.position, rayDirection, 0.6f, wallLayer); // 전방 0.1f 만큼에 벽 레이어가 있는지 검사
    }

    private void ChangeState(State nextState)
    {
        states[(int)curState].Exit();
        curState = nextState;
        states[(int)curState].Enter();
    }

    [System.Serializable]
    private class IdleState : BaseState
    {
        [SerializeField] PlayerController player;

        public override void Enter()
        {
            // Idle 애니메이션 재생
            player.animator.Play(idleHash);
        }

        public override void Update()
        {
            // 바닥에서 좌우 입력을 하면 Run 상태
            if (Mathf.Abs(player.x) > 0.01f && player.isGround && !player.isCharging)
            {
                player.ChangeState(State.Run);
            }
            // 스페이스바를 눌러 충전중이면 Charge 상태
            if (Input.GetKey(KeyCode.Space) && player.isGround)
            {
                player.ChangeState(State.Charge);
            }
            // velocity.y가 -0.01보다 작으면 Fall 상태
            if (player.rigid.velocity.y < -0.01f && !player.isGround)
            {
                player.ChangeState(State.Fall);
            }
        }
    }

    [System.Serializable]
    private class RunState : BaseState
    {
        [SerializeField] PlayerController player;

        public override void Enter()
        {
        }

        public override void Update()
        {
            // Run 애니메이션 재생, 달리고 있을때만
            if (player.rigid.velocity.sqrMagnitude > 0.01f)
            {
                player.animator.Play(runHash);
            }

            float moveDir = player.x * player.moveSpeed;
            // 달리기
            if (player.isGround && !player.isCharging) // 바닥에서 충전중이지 않을때 좌우입력하면 
            {
                if (player.frontWall)
                {
                    moveDir = 0;
                }
                player.rigid.velocity = new Vector2(moveDir, 0);
            }
            // 벽쪽으론 이동하지 않게
            // 전방에 Wall이 있으면, 앞으로 이동하지 않게 frontWall = true, 이동X            

            // 속도를 가지지 않은 가만히 있는 상태면 Idle 상태
            if (player.rigid.velocity.sqrMagnitude < 0.01f)
            {
                player.ChangeState(State.Idle);
            }
            // 스페이스바를 눌러 충전중이면 Charge 상태
            if (Input.GetKey(KeyCode.Space))
            {
                player.ChangeState(State.Charge);
            }
            // velocity.y가 -0.01보다 작으면 Fall 상태
            if (player.rigid.velocity.y < -0.01f && !player.isGround)
            {
                player.ChangeState(State.Fall);
            }
        }
    }

    [System.Serializable]
    private class ChargeState : BaseState
    {
        [SerializeField] PlayerController player;

        public override void Enter()
        {
            // Charge 애니메이션 재생
            player.animator.Play(chargeHash);
        }

        public override void Update()
        {
            // 바닥에있고 스페이스바를 누르고 있으면,
            if (Input.GetKey(KeyCode.Space) && player.isGround)
            {
                player.rigid.velocity = new Vector2(0, player.rigid.velocity.y); // 이동하지 않음
                player.isCharging = true; // isCharging 중엔 이동하지 않음
                player.jumpPower += player.jumpCharge * Time.deltaTime; // 점프힘 증가
                player.jumpPower = Mathf.Clamp(player.jumpPower, player.minJumpPower, player.maxJumpPower); // 최대 점프힘까지만 증가
            }

            // 최대 힘이 되면 따로 입력 안해도 점프되게

            // 충전 중 jumpPower가 maxJumpPower로 되면 Jump 상태
            if (player.jumpPower == player.maxJumpPower)
            {
                player.ChangeState(State.Jump);
            }
            // 충전 후 스페이스바를 떼면 Jump 상태
            else if (Input.GetKeyUp(KeyCode.Space) && player.isGround)
            {
                player.ChangeState(State.Jump);
            }    
            
        }
    }

    [System.Serializable]
    private class JumpState : BaseState
    {
        [SerializeField] PlayerController player;

        public override void Enter()
        {
            // Jump 애니메이션 재생
            player.animator.Play(jumpHash);
            // Jump 사운드 재생
            SoundManager.Instance.PlaySFX(player.jumpClip);

            // Jump
            player.rigid.velocity = new Vector2(player.x * player.moveSpeed, player.jumpPower);
            player.isGround = false; // 점프중이니 isGround = false
            player.isCharging = false; // 충전 해제
            player.jumpPower = 0f; // 점프힘 0 초기화
        }

        public override void Update()
        {
            // velocity.y가 -0.01보다 작아지면 Fall 상태
            if (player.rigid.velocity.y < -0.01f && !player.isGround)
            {
                player.ChangeState(State.Fall);
            }

            // 너무 낮게뛰어서 속도를 가지지 않은 가만히 있는 상태면 Idle 상태
            if (player.rigid.velocity.sqrMagnitude < 0.01f)
            {
                player.ChangeState(State.Idle);
            }
        }
    }

    [System.Serializable]
    private class FallState : BaseState
    {
        [SerializeField] PlayerController player;

        public override void Enter()
        {
            // Fall 애니메이션 재생
            player.animator.Play(fallHash);
        }

        public override void Update()
        {
            // 낙하 시간이 0.6초 이하로 착지하면 Land 상태
            if (player.currentFallSec <= 0.6 && player.isGround)
            {
                player.ChangeState(State.Land);
            }
            // 낙하시간이 0.6초보다 크고 착지하면 Down 상태
            else if (player.currentFallSec > 0.6 && player.isGround)
            {
                player.ChangeState(State.Down);
            }
            // 경사면에 떨어지면 미끄러져 내려가는 Slide 상태
            // player.ChangeState(State.Slide);
        }
    }

    [System.Serializable]
    private class LandState : BaseState
    {
        [SerializeField] PlayerController player;

        public override void Enter()
        {
            // Land 애니메이션 재생
            player.animator.Play(landHash);
            // Land 효과음 재생
            SoundManager.Instance.PlaySFX(player.landClip);
        }

        public override void Update()
        {
            // 착지 후 속도를 가지지 않은 가만히 있는 상태면 Idle 상태
            if (player.rigid.velocity.sqrMagnitude < 0.01f)
            {
                player.ChangeState(State.Idle);
            }
        }
    }

    [System.Serializable]
    private class DownState : BaseState
    {
        [SerializeField] PlayerController player;
        private bool isDowned;
        public override void Enter()
        {
            // Down 애니메이션 재생
            player.animator.Play(downHash);
            // Down 효과음 재생
            SoundManager.Instance.PlaySFX(player.downClip);
            // 1초동안 못움직이는 효과?
        }
        public override void Update()
        {
            if (!isDowned)
            {
                isDowned = true;
                player.StartCoroutine(DownCoroutine());
            }
            /*
            // 속도를 가지지 않은 가만히 있는 상태면 Idle 상태
            if (player.rigid.velocity.sqrMagnitude < 0.01f)
            {
                player.ChangeState(State.Idle);
            }*/
        }
        private IEnumerator DownCoroutine()
        {
            yield return new WaitForSeconds(1f); // 1초 동안 대기
            isDowned = false; // 1초 후 isDowned상태 해제
            player.ChangeState(State.Idle); // 1초 후 Idle 상태로 전환
        }
    }

    [System.Serializable]
    private class SlideState : BaseState
    {
        [SerializeField] PlayerController player;

        public override void Enter()
        {
            // Slide 애니메이션 재생
            player.animator.Play(slideHash);
        }

        public override void Update()
        {
            // 경사면을 따라 쭉 내려오게됨
            // 바닥에 떨어지면 Down 상태
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (curState == State.Jump ||
                curState == State.Fall ||
                curState == State.Down ||
                curState == State.Slide)
            {
                // 공중에서 벽(옆면)에 부딪히면 Hit 효과음 재생
                SoundManager.Instance.PlaySFX(hitClip);
                /*
                Vector2 normal = collision.GetContact(0).normal; // 처음 충돌한 지점의 벡터 normal
                rigid.AddForce(normal * 3f, ForceMode2D.Impulse); // 처음 충돌한 지점에서 튕겨나감
                */
                // 벽 콜라이더에 Physics Metarial 2D에 bounce를 주는게 나은거같음

                Debug.Log("벽에 튕겨나감");
            }
        }
    }
}
