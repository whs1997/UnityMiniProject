using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePause : MonoBehaviour
{
    [SerializeField] GameObject pauseUI; // ���� �޴� UI

    [SerializeField] TextMeshProUGUI jumpCountText;
    [SerializeField] TextMeshProUGUI downCountText;
    [SerializeField] PlayerController player;

    [SerializeField] TextMeshProUGUI ResumeText;
    [SerializeField] TextMeshProUGUI OptionText;
    [SerializeField] TextMeshProUGUI QuitText;

    private int menuIndex = 0; // �޴� ������ index
    private string[] menu = { "Resume", "Option", "Quit" };


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
        player.isPaused = true; // ���� �����϶� �÷��̾� �������� �ʰ� ��
    }

    private void Resume()
    {
        pauseUI.SetActive(false); // pauseUI ����
        Time.timeScale = 1; // ���� �簳
        player.isPaused = false; // ���� ���� ����
    }

    private void Quit()
    {
        SceneManager.LoadScene("TitleScene"); // ���� ����(Ÿ��Ʋ ������ �̵�)
        Time.timeScale = 1;
    }

    private void Option()
    {
        // Ű ���� �ٲٱ�
        // ���� ���� �ٲٱ�
    }

    private void SelectMenu()
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
            }
            else if (menuIndex == 1) // �ε����� 1 (option)�̸�, �ɼ� �޴� ����
            {
                Option();
            }
            else if (menuIndex == 2) // �ε����� 2 (quit)�̸�, ���� ����
            {
                Quit();
            }
        }
    }

    private void currentMenu()
    {
        ResumeText.text = (menuIndex == 0 ? ">" : "  ") + "Resume"; // menuIndex�� 0�̸� �տ� >, �ƴҽ� ����
        OptionText.text = (menuIndex == 1 ? ">" : "  ") + "Option";
        QuitText.text = (menuIndex == 2 ? ">" : "  ") + "Quit";
    }

    private void UpdateCounts()
    {
        jumpCountText.text = "Jump : " + player.jumpCount; // �÷��̾��� Jump ī��Ʈ�� �޾ƿ�
        downCountText.text = "Fall : " + player.DownCount; // �÷��̾��� Down ī��Ʈ�� �޾ƿ�
    }
}
