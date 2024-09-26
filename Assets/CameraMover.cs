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
        // �÷��̾��� ���̰� 0 ~ 10 �϶� 1�� ī�޶�
        // 10 ~ 20�� 2�� ī�޶�, 20~30�� 3�� ī�޶�,,, �� �ö�
        // Priority�� ���� ī�޶� ���� ī�޶�

        float playerHeight = player.position.y; // �÷��̾��� ����

        int cameraIndex = Mathf.FloorToInt(playerHeight / 10);
        // ���� ���̸� 10���� ���� FloorToInt �Լ��� ó��
        // cameraIndex�� 0~10�� 0��, 10~20�� 1��, ... �� ���ڸ� ������ ��

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
