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
    [SerializeField] float moveSpeed; // �̵� �ӵ�
    [SerializeField] float minJumpPower; // ���� �ּ���
    [SerializeField] float maxJumpPower; // ���� �ִ���
    [SerializeField] float jumpCharge; // ���� ������
    [SerializeField] float jumpPower; // �� ���� ��
    [SerializeField] float maxFallSpeed; // �ִ� ���� �ӵ�
    [SerializeField] bool isGround; // ���� �ִ� ����
    [SerializeField] bool isCharging; // ���� ��
    [SerializeField] float currentFallSec = 0f; // ���� ���� ���� �� ����ϴ� ü�� �ð�
    [SerializeField] Transform groundCheck; // isGround üũ�� ���� �÷��̾� �� ������Ʈ
    [SerializeField] LayerMask groundLayer; // Ground ���̾� �˻�
    [SerializeField] bool frontWall;
    [SerializeField] LayerMask wallLayer; // Wall ���̾� �˻�

    private float x; // Horizontal �Է°� ����

    // �ִϸ��̼�
    private static int idleHash = Animator.StringToHash("Idle");
    private static int runHash = Animator.StringToHash("Run");
    private static int jumpHash = Animator.StringToHash("Jump");
    private static int fallHash = Animator.StringToHash("Fall");
    private static int chargeHash = Animator.StringToHash("Charge");
    private static int slideHash = Animator.StringToHash("Slide");
    private static int downHash = Animator.StringToHash("Down");
    private static int landHash = Animator.StringToHash("Land");

    // ����
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
            return; // �ش� ���µ鿡�� Flip ���� �ʱ�

        if (x < 0)
            render.flipX = true; // �������� �Է��ϸ� �¿� ����
        else if (x > 0)
            render.flipX = false;
    }

    private void CheckFalling()
    {
        if (rigid.velocity.y < 0 && !isGround)
            currentFallSec += Time.deltaTime; // �������϶� �ð� ����

        if (Mathf.Abs(rigid.velocity.y) < 0.01f && isGround) // y �̵��� ������ ü���ð� 0
            currentFallSec = 0;

        if (rigid.velocity.y < -maxFallSpeed) // �ִ� ���ϼӵ� ����
        {
            rigid.velocity = new Vector2(rigid.velocity.x, -maxFallSpeed);
        }
    }

    private void GroundChecker()
    {
        Vector2 boxSize = new Vector2(coll.bounds.size.x, 0.1f); // �÷��̾��� �¿� ũ�� �������� �ڽ�
        Vector2 boxOrigin = new Vector2(coll.bounds.center.x, coll.bounds.min.y); // �÷��̾��� �ٴ� ��� ��ġ
        isGround = Physics2D.BoxCast(boxOrigin, boxSize, 0, Vector2.down, 0.1f, groundLayer); // �ٴڸ� �˻�
    }

    private void FrontChecker()
    {
        Vector2 rayDirection = Vector2.right * x; // ray�� �� ����
        frontWall = Physics2D.Raycast(transform.position, rayDirection, 0.6f, wallLayer); // ���� 0.1f ��ŭ�� �� ���̾ �ִ��� �˻�
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
            // Idle �ִϸ��̼� ���
            player.animator.Play(idleHash);
        }

        public override void Update()
        {
            // �ٴڿ��� �¿� �Է��� �ϸ� Run ����
            if (Mathf.Abs(player.x) > 0.01f && player.isGround && !player.isCharging)
            {
                player.ChangeState(State.Run);
            }
            // �����̽��ٸ� ���� �������̸� Charge ����
            if (Input.GetKey(KeyCode.Space) && player.isGround)
            {
                player.ChangeState(State.Charge);
            }
            // velocity.y�� -0.01���� ������ Fall ����
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
            // Run �ִϸ��̼� ���, �޸��� ��������
            if (player.rigid.velocity.sqrMagnitude > 0.01f)
            {
                player.animator.Play(runHash);
            }

            float moveDir = player.x * player.moveSpeed;
            // �޸���
            if (player.isGround && !player.isCharging) // �ٴڿ��� ���������� ������ �¿��Է��ϸ� 
            {
                if (player.frontWall)
                {
                    moveDir = 0;
                }
                player.rigid.velocity = new Vector2(moveDir, 0);
            }
            // �������� �̵����� �ʰ�
            // ���濡 Wall�� ������, ������ �̵����� �ʰ� frontWall = true, �̵�X            

            // �ӵ��� ������ ���� ������ �ִ� ���¸� Idle ����
            if (player.rigid.velocity.sqrMagnitude < 0.01f)
            {
                player.ChangeState(State.Idle);
            }
            // �����̽��ٸ� ���� �������̸� Charge ����
            if (Input.GetKey(KeyCode.Space))
            {
                player.ChangeState(State.Charge);
            }
            // velocity.y�� -0.01���� ������ Fall ����
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
            // Charge �ִϸ��̼� ���
            player.animator.Play(chargeHash);
        }

        public override void Update()
        {
            // �ٴڿ��ְ� �����̽��ٸ� ������ ������,
            if (Input.GetKey(KeyCode.Space) && player.isGround)
            {
                player.rigid.velocity = new Vector2(0, player.rigid.velocity.y); // �̵����� ����
                player.isCharging = true; // isCharging �߿� �̵����� ����
                player.jumpPower += player.jumpCharge * Time.deltaTime; // ������ ����
                player.jumpPower = Mathf.Clamp(player.jumpPower, player.minJumpPower, player.maxJumpPower); // �ִ� ������������ ����
            }

            // �ִ� ���� �Ǹ� ���� �Է� ���ص� �����ǰ�

            // ���� �� jumpPower�� maxJumpPower�� �Ǹ� Jump ����
            if (player.jumpPower == player.maxJumpPower)
            {
                player.ChangeState(State.Jump);
            }
            // ���� �� �����̽��ٸ� ���� Jump ����
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
            // Jump �ִϸ��̼� ���
            player.animator.Play(jumpHash);
            // Jump ���� ���
            SoundManager.Instance.PlaySFX(player.jumpClip);

            // Jump
            player.rigid.velocity = new Vector2(player.x * player.moveSpeed, player.jumpPower);
            player.isGround = false; // �������̴� isGround = false
            player.isCharging = false; // ���� ����
            player.jumpPower = 0f; // ������ 0 �ʱ�ȭ
        }

        public override void Update()
        {
            // velocity.y�� -0.01���� �۾����� Fall ����
            if (player.rigid.velocity.y < -0.01f && !player.isGround)
            {
                player.ChangeState(State.Fall);
            }

            // �ʹ� ���Զپ �ӵ��� ������ ���� ������ �ִ� ���¸� Idle ����
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
            // Fall �ִϸ��̼� ���
            player.animator.Play(fallHash);
        }

        public override void Update()
        {
            // ���� �ð��� 0.6�� ���Ϸ� �����ϸ� Land ����
            if (player.currentFallSec <= 0.6 && player.isGround)
            {
                player.ChangeState(State.Land);
            }
            // ���Ͻð��� 0.6�ʺ��� ũ�� �����ϸ� Down ����
            else if (player.currentFallSec > 0.6 && player.isGround)
            {
                player.ChangeState(State.Down);
            }
            // ���鿡 �������� �̲����� �������� Slide ����
            // player.ChangeState(State.Slide);
        }
    }

    [System.Serializable]
    private class LandState : BaseState
    {
        [SerializeField] PlayerController player;

        public override void Enter()
        {
            // Land �ִϸ��̼� ���
            player.animator.Play(landHash);
            // Land ȿ���� ���
            SoundManager.Instance.PlaySFX(player.landClip);
        }

        public override void Update()
        {
            // ���� �� �ӵ��� ������ ���� ������ �ִ� ���¸� Idle ����
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
            // Down �ִϸ��̼� ���
            player.animator.Play(downHash);
            // Down ȿ���� ���
            SoundManager.Instance.PlaySFX(player.downClip);
            // 1�ʵ��� �������̴� ȿ��?
        }
        public override void Update()
        {
            if (!isDowned)
            {
                isDowned = true;
                player.StartCoroutine(DownCoroutine());
            }
            /*
            // �ӵ��� ������ ���� ������ �ִ� ���¸� Idle ����
            if (player.rigid.velocity.sqrMagnitude < 0.01f)
            {
                player.ChangeState(State.Idle);
            }*/
        }
        private IEnumerator DownCoroutine()
        {
            yield return new WaitForSeconds(1f); // 1�� ���� ���
            isDowned = false; // 1�� �� isDowned���� ����
            player.ChangeState(State.Idle); // 1�� �� Idle ���·� ��ȯ
        }
    }

    [System.Serializable]
    private class SlideState : BaseState
    {
        [SerializeField] PlayerController player;

        public override void Enter()
        {
            // Slide �ִϸ��̼� ���
            player.animator.Play(slideHash);
        }

        public override void Update()
        {
            // ������ ���� �� �������Ե�
            // �ٴڿ� �������� Down ����
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
                // ���߿��� ��(����)�� �ε����� Hit ȿ���� ���
                SoundManager.Instance.PlaySFX(hitClip);
                /*
                Vector2 normal = collision.GetContact(0).normal; // ó�� �浹�� ������ ���� normal
                rigid.AddForce(normal * 3f, ForceMode2D.Impulse); // ó�� �浹�� �������� ƨ�ܳ���
                */
                // �� �ݶ��̴��� Physics Metarial 2D�� bounce�� �ִ°� �����Ű���

                Debug.Log("���� ƨ�ܳ���");
            }
        }
    }
}
