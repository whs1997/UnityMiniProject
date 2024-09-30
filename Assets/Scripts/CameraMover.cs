using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera[] cameras;
    [SerializeField] Transform player;

    private void Update()
    {
        // ���� ù ��ġ���� 1 ~ n�� ����� ī�޶�
        // �÷��̾��� ���̰� 0 ~ 14 �϶� 1�� ī�޶�
        // 14 ~ 28�� 2�� ī�޶�, 28~42�� 3�� ī�޶�,,, �� �ö�
        // Priority�� ���� ī�޶� ���� ī�޶�

        float playerHeight = player.position.y; // �÷��̾��� ����

        int cameraIndex = Mathf.FloorToInt(playerHeight / 14);
        // ���� ���̸� 14���� ���� FloorToInt �Լ��� ó��
        // cameraIndex�� 0~14�� 0��, 14~28�� 1��, ... �� ���ڸ� ������ ��

        for(int i = 0; i < cameras.Length; i++)
        {
            cameras[i].Priority = (i == cameraIndex) ? 10 : 0; // ���� ���� i�� ī�޶��� Priority 10, �ٸ� ī�޶�� 0���� �ݺ�
        }
        
        /*
        if (playerHeight >= 0 && playerHeight < 10)
            cameras[0].Priority = 10;
        else if (playerHeight >= 10 && playerHeight < 20)
        {
            cameras[0].Priority = 0;
            cameras[1].Priority = 10;
        }
        else if(playerHeight >= 20 && playerHeight < 30)
        {
            cameras[1].Priority = 0;
            cameras[2].Priority = 10;
        }
        */
        
        
    }
}
