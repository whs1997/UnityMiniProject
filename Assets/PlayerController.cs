using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] SpriteRenderer render;
    private float x;

    [SerializeField] bool isGround;

    [SerializeField] float jumpPower;
    [SerializeField] float moveSpeed;


    

    private void Start()
    {
    }

    private void Update()
    {
        x = Input.GetAxisRaw("Horizontal");

        // Move();
        // Jump();
    }

    private void StartMove()
    {
    }

    private void Jump()
    {
    }

    private void Flip()
    {
        if (x < 0)
        {
            render.flipX = true; // 왼쪽으로 입력하면 좌우 반전
        }
        else if (x > 0)
        {
            render.flipX = false;
        }
    }

}
