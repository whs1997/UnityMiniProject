using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    [SerializeField] GameObject pressStart;
    [SerializeField] float blinkSpeed; // �����̴� ȿ��

    private void Awake()
    {
        StartCoroutine(Blink());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    private IEnumerator Blink()
    {
        while (true)
        {
            pressStart.SetActive(true); // pressStart ������ ����� �������� �ϴ� ȿ��
            yield return new WaitForSeconds(blinkSpeed);
            pressStart.SetActive(false);
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
}
