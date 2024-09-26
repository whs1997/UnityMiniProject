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
        // 맵의 첫 위치부터 1 ~ n개 버츄얼 카메라
        // 플레이어의 높이가 0 ~ 10 일땐 1번 카메라
        // 10 ~ 20은 2번 카메라, 20~30은 3번 카메라,,, 쭉 올라감
        // Priority가 높은 카메라가 현재 카메라

        float playerHeight = player.position.y; // 플레이어의 높이

        int cameraIndex = Mathf.FloorToInt(playerHeight / 10);
        // 현재 높이를 10으로 나눠 FloorToInt 함수로 처리
        // cameraIndex는 0~10은 0번, 10~20은 1번, ... 의 숫자를 가지게 함

        for(int i = 0; i < cameras.Length; i++)
        {
            cameras[i].Priority = (i == cameraIndex) ? 10 : 0; // 현재 높이 i번 카메라의 Priority 10, 다른 카메라는 0으로 반복
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
