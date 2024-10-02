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

    [SerializeField] GameObject pauseUI; // ���� �޴� UI

    [SerializeField] TextMeshProUGUI jumpCountText; // ���� ī��Ʈ 
    [SerializeField] TextMeshProUGUI downCountText; // ���� ī��Ʈ

    [SerializeField] TextMeshProUGUI ResumeText; // ���� �簳 �ؽ�Ʈ
    [SerializeField] TextMeshProUGUI SettingText; // ���� ���� �ؽ�Ʈ
    [SerializeField] TextMeshProUGUI QuitText; // ���� ���� �ؽ�Ʈ(Ÿ��Ʋ ������)

    [SerializeField] GameObject settingUI; // ���� �޴� UI
    [SerializeField] Slider bgmSlider; // BGM ���� �����̴�
    [SerializeField] Slider sfxSlider; // SFX ���� �����̴�

    [SerializeField] TextMeshProUGUI curJumpKeyText; // ���� ���� Ű�� ������ �ؽ�Ʈ
    public KeyCode newJumpKey = KeyCode.Space; // ������ ���� Ű, �⺻�� �����̽�
    private bool waitForKey = false; // ���ο� Ű �Է� ����� ����

    private int menuIndex = 0; // �޴� ������ index
    private string[] menu = { "Resume", "Option", "Quit" };

    private bool isSetting = false; // ���� �������϶� ���Ʒ� �Է� ���ϰ�

    private void Start()
    {
        bgmSlider.value = SoundManager.Instance.BGMVolume; // ���� �������� �����̴� �ʱ�ȭ
        sfxSlider.value = SoundManager.Instance.SFXVolume;
        curJumpKeyText.text = newJumpKey.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(); // ESC�� ������ ���� �Ͻ�����
        }
        SelectMenu();
        currentMenu();
        UpdateCounts();
    }

    private void Pause()
    {
        pauseUI.SetActive(true); // pauseUI ���
        Time.timeScale = 0; // ���� �Ͻ� ����
        player.isPaused = true; // ���� ���� �����϶� �÷��̾� �������� �ʰ� ��
    }

    private void Resume()
    {
        pauseUI.SetActive(false); // pauseUI ����
        Time.timeScale = 1; // ���� �簳
        player.isPaused = false; // ���� ���� ���� ����
    }

    private void Quit()
    {
        SceneManager.LoadScene("TitleScene"); // ���� ����(Ÿ��Ʋ ������ �̵�)
        Time.timeScale = 1;
    }

    private void Setting()
    {
        settingUI.SetActive(true); // ���� UI Ȱ��ȭ

        // Ű ���� �ٲٱ� ��ư�� ������ Ű�� �Է��ϸ� ���� space Ű�� �ش� Ű�� ����

        // ���� ���� �ٲٱ� bgm, sfx ���� �����̴�
        bgmSlider.onValueChanged.AddListener(SetBGMVolume); // ���� ���� �Լ��� ������ �����̴��� �̺�Ʈ ������
        sfxSlider.onValueChanged.AddListener(setSFXVolume);
    }

    private void SetBGMVolume(float volume)
    {
        SoundManager.Instance.BGMVolume = volume; // BGM�� ���� ���� �Լ�
    }

    private void setSFXVolume(float volume)
    {
        SoundManager.Instance.SFXVolume = volume; // SFX�� ���� ���� �Լ�
    }

    public void ChangeJumpKey() // ���� UI���� ���� Ű�� ������ ��ư
    {
        StartCoroutine(AssignJumpKey()); // ���� Ű �Ҵ� �ڷ�ƾ;
    }

    private IEnumerator AssignJumpKey()
    {
        waitForKey = true;

        while (waitForKey)
        {
            if (Input.anyKeyDown) // �ƹ� Ű�� �Է��ϸ�
            {
                foreach(KeyCode keyCode in Enum.GetValues(typeof(KeyCode))) // �Է��� newJumpKey ����
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        newJumpKey = keyCode; // �Է��� �� ����Ű�� ����
                        curJumpKeyText.text = newJumpKey.ToString(); // ���� Ű ��ư�� �ؽ�Ʈ ����
                        waitForKey = false; // Ű �Է� �Ϸ�

                        AssignNewJumpKey(newJumpKey); // �� ����Ű �Ҵ�
                    }
                }
            }
            yield return null; // �ڷ�ƾ ����
        }
    }

    public void AssignNewJumpKey(KeyCode jumpKey)
    {
        newJumpKey = jumpKey; // ���� Ű�� �� ���� Ű�� ����
        player.AssignJumpKey(newJumpKey); // �÷��̾��� ����Ű�� �Ҵ�
    }

    public void CloseSetting()
    {
        settingUI.SetActive(false); // ���� UI ����
        isSetting = false; // ���û��� ����
    }

    private void SelectMenu()
    {
        if (isSetting == false)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) // ��,�Ʒ� ȭ��ǥ�� �޴� ����
            {
                menuIndex--; // �� ������ �ε��� 1�� ����
                if (menuIndex < 0) // 0���� ������ �Ѿ��
                {
                    menuIndex = menu.Length - 1; // ���� �Ʒ� �޴��� �̵�
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                menuIndex++; // �Ʒ� ������ �ε��� 1�� ����
                if (menuIndex >= menu.Length) // ���� �Ʒ��� �Ѿ��
                {
                    menuIndex = 0; // �� ���� �̵�
                }
            }

            if (Input.GetKeyDown(KeyCode.Space)) // �����̽��ٷ� ���õ� �޴� ����
            {
                if (menuIndex == 0) // �ε����� 0 (resume)�̸�, ���� �簳
                {
                    Resume();
                    isSetting = false; // ���û��� ����
                }
                else if (menuIndex == 1) // �ε����� 1 (Setting)�̸�, �ɼ� �޴� ����
                {
                    Setting();
                    isSetting = true; // ���� �޴��� �����ϰ� ���Ʒ� ���� �Ұ���
                }
                else if (menuIndex == 2) // �ε����� 2 (quit)�̸�, ���� ����
                {
                    Quit();
                    isSetting = false; // ���û��� ����
                }
            }
        }
    }

    private void currentMenu()
    {
        ResumeText.text = (menuIndex == 0 ? ">" : "  ") + "Resume"; // menuIndex�� 0�̸� �տ� >, �ƴҽ� ����
        SettingText.text = (menuIndex == 1 ? ">" : "  ") + "Setting";
        QuitText.text = (menuIndex == 2 ? ">" : "  ") + "Quit";
    }

    private void UpdateCounts()
    {
        jumpCountText.text = "Jump : " + player.jumpCount; // �÷��̾��� Jump ī��Ʈ�� �޾ƿ�
        downCountText.text = "Fall : " + player.DownCount; // �÷��̾��� Down ī��Ʈ�� �޾ƿ�
    }
}
