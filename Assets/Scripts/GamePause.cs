using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePause : MonoBehaviour
{
    [SerializeField] GameObject pauseUI; // 퍼즈 메뉴 UI

    [SerializeField] TextMeshProUGUI jumpCountText;
    [SerializeField] TextMeshProUGUI downCountText;
    [SerializeField] PlayerController player;

    [SerializeField] TextMeshProUGUI ResumeText;
    [SerializeField] TextMeshProUGUI OptionText;
    [SerializeField] TextMeshProUGUI QuitText;

    private int menuIndex = 0; // 메뉴 선택할 index
    private string[] menu = { "Resume", "Option", "Quit" };


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(); // ESC를 누르면 게임 일시정지
        }
        SelectMenu();
        currentMenu();
        UpdateCounts();
    }

    private void Pause()
    {
        pauseUI.SetActive(true); // pauseUI 띄움
        Time.timeScale = 0; // 게임 일시 정지
        player.isPaused = true; // 정지 상태일땐 플레이어 동작하지 않게 함
    }

    private void Resume()
    {
        pauseUI.SetActive(false); // pauseUI 끄고
        Time.timeScale = 1; // 게임 재개
        player.isPaused = false; // 정지 상태 해제
    }

    private void Quit()
    {
        SceneManager.LoadScene("TitleScene"); // 게임 종료(타이틀 씬으로 이동)
        Time.timeScale = 1;
    }

    private void Option()
    {
        // 키 설정 바꾸기
        // 볼륨 조절 바꾸기
    }

    private void SelectMenu()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) // 위,아래 화살표로 메뉴 선택
        {
            menuIndex--; // 위 누르면 인덱스 1씩 감소
            if (menuIndex < 0) // 0에서 맨위로 넘어가면
            {
                menuIndex = menu.Length - 1; // 가장 아래 메뉴로 이동
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            menuIndex++; // 아래 누르면 인덱스 1씩 증가
            if (menuIndex >= menu.Length) // 가장 아래로 넘어가면
            {
                menuIndex = 0; // 맨 위로 이동
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스바로 선택된 메뉴 실행
        {
            if (menuIndex == 0) // 인덱스가 0 (resume)이면, 게임 재개
            {
                Resume();
            }
            else if (menuIndex == 1) // 인덱스가 1 (option)이면, 옵션 메뉴 실행
            {
                Option();
            }
            else if (menuIndex == 2) // 인덱스가 2 (quit)이면, 게임 종료
            {
                Quit();
            }
        }
    }

    private void currentMenu()
    {
        ResumeText.text = (menuIndex == 0 ? ">" : "  ") + "Resume"; // menuIndex가 0이면 앞에 >, 아닐시 공백
        OptionText.text = (menuIndex == 1 ? ">" : "  ") + "Option";
        QuitText.text = (menuIndex == 2 ? ">" : "  ") + "Quit";
    }

    private void UpdateCounts()
    {
        jumpCountText.text = "Jump : " + player.jumpCount; // 플레이어의 Jump 카운트를 받아옴
        downCountText.text = "Fall : " + player.DownCount; // 플레이어의 Down 카운트를 받아옴
    }
}
