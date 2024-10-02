using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    [SerializeField] GameObject pressStart;
    [SerializeField] float blinkSpeed; // 깜빡이는 효과

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
            pressStart.SetActive(true); // pressStart 문구가 생겼다 없어졌다 하는 효과
            yield return new WaitForSeconds(blinkSpeed);
            pressStart.SetActive(false);
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
}
