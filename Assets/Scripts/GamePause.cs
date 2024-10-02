using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePause : MonoBehaviour
{
    [SerializeField] PlayerController player;

    [SerializeField] GameObject pauseUI; // 퍼즈 메뉴 UI

    [SerializeField] TextMeshProUGUI jumpCountText; // 점프 카운트 
    [SerializeField] TextMeshProUGUI downCountText; // 낙하 카운트

    [SerializeField] TextMeshProUGUI ResumeText; // 게임 재개 텍스트
    [SerializeField] TextMeshProUGUI SettingText; // 게임 세팅 텍스트
    [SerializeField] TextMeshProUGUI QuitText; // 게임 종료 텍스트(타이틀 씬으로)

    [SerializeField] GameObject settingUI; // 세팅 메뉴 UI
    [SerializeField] Slider bgmSlider; // BGM 볼륨 슬라이더
    [SerializeField] Slider sfxSlider; // SFX 볼륨 슬라이더

    [SerializeField] TextMeshProUGUI curJumpKeyText; // 현재 점프 키를 보여줄 텍스트
    public KeyCode newJumpKey = KeyCode.Space; // 변경할 점프 키, 기본은 스페이스
    private bool waitForKey = false; // 새로운 키 입력 대기할 상태

    private int menuIndex = 0; // 메뉴 선택할 index
    private string[] menu = { "Resume", "Option", "Quit" };

    private bool isSetting = false; // 세팅 변경중일땐 위아래 입력 못하게

    private void Start()
    {
        bgmSlider.value = SoundManager.Instance.BGMVolume; // 현재 볼륨으로 슬라이더 초기화
        sfxSlider.value = SoundManager.Instance.SFXVolume;
        curJumpKeyText.text = newJumpKey.ToString();
    }

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
        player.isPaused = true; // 게임 정지 상태일땐 플레이어 동작하지 않게 함
    }

    private void Resume()
    {
        pauseUI.SetActive(false); // pauseUI 끄고
        Time.timeScale = 1; // 게임 재개
        player.isPaused = false; // 게임 정지 상태 해제
    }

    private void Quit()
    {
        SceneManager.LoadScene("TitleScene"); // 게임 종료(타이틀 씬으로 이동)
        Time.timeScale = 1;
    }

    private void Setting()
    {
        settingUI.SetActive(true); // 세팅 UI 활성화

        // 키 설정 바꾸기 버튼을 누르고 키를 입력하면 점프 space 키를 해당 키로 변경

        // 볼륨 조절 바꾸기 bgm, sfx 볼륨 슬라이더
        bgmSlider.onValueChanged.AddListener(SetBGMVolume); // 볼륨 조절 함수를 실행할 슬라이더의 이벤트 리스너
        sfxSlider.onValueChanged.AddListener(setSFXVolume);
    }

    private void SetBGMVolume(float volume)
    {
        SoundManager.Instance.BGMVolume = volume; // BGM의 볼륨 조절 함수
    }

    private void setSFXVolume(float volume)
    {
        SoundManager.Instance.SFXVolume = volume; // SFX의 볼륨 조절 함수
    }

    public void ChangeJumpKey() // 세팅 UI에서 점프 키를 변경할 버튼
    {
        StartCoroutine(AssignJumpKey()); // 점프 키 할당 코루틴;
    }

    private IEnumerator AssignJumpKey()
    {
        waitForKey = true;

        while (waitForKey)
        {
            if (Input.anyKeyDown) // 아무 키를 입력하면
            {
                foreach(KeyCode keyCode in Enum.GetValues(typeof(KeyCode))) // 입력한 newJumpKey 감지
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        newJumpKey = keyCode; // 입력한 새 점프키로 변경
                        curJumpKeyText.text = newJumpKey.ToString(); // 점프 키 버튼의 텍스트 변경
                        waitForKey = false; // 키 입력 완료

                        AssignNewJumpKey(newJumpKey); // 새 점프키 할당
                    }
                }
            }
            yield return null; // 코루틴 종료
        }
    }

    public void AssignNewJumpKey(KeyCode jumpKey)
    {
        newJumpKey = jumpKey; // 점프 키를 새 점프 키로 변경
        player.AssignJumpKey(newJumpKey); // 플레이어의 점프키에 할당
    }

    public void CloseSetting()
    {
        settingUI.SetActive(false); // 세팅 UI 종료
        isSetting = false; // 세팅상태 종료
    }

    private void SelectMenu()
    {
        if (isSetting == false)
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
                    isSetting = false; // 세팅상태 종료
                }
                else if (menuIndex == 1) // 인덱스가 1 (Setting)이면, 옵션 메뉴 실행
                {
                    Setting();
                    isSetting = true; // 세팅 메뉴로 진입하고 위아래 조작 불가능
                }
                else if (menuIndex == 2) // 인덱스가 2 (quit)이면, 게임 종료
                {
                    Quit();
                    isSetting = false; // 세팅상태 종료
                }
            }
        }
    }

    private void currentMenu()
    {
        ResumeText.text = (menuIndex == 0 ? ">" : "  ") + "Resume"; // menuIndex가 0이면 앞에 >, 아닐시 공백
        SettingText.text = (menuIndex == 1 ? ">" : "  ") + "Setting";
        QuitText.text = (menuIndex == 2 ? ">" : "  ") + "Quit";
    }

    private void UpdateCounts()
    {
        jumpCountText.text = "Jump : " + player.jumpCount; // 플레이어의 Jump 카운트를 받아옴
        downCountText.text = "Fall : " + player.DownCount; // 플레이어의 Down 카운트를 받아옴
    }
}
