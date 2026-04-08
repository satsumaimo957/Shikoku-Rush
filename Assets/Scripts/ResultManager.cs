using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI resultText;
    public Image resultImage;

    public Sprite greatSprite;
    public Sprite goodSprite;
    public Sprite gameOverSprite;

    void Start()
    {
        int score = GameResultData.score;

        scoreText.text = "Score: " + score;

        if (score >= 100)
        {
            resultText.text = "GREAT!";
            resultImage.sprite = greatSprite;
        }
        else if (score >= 50)
        {
            resultText.text = "GOOD!";
            resultImage.sprite = goodSprite;
        }
        else
        {
            resultText.text = "GAME OVER";
            resultImage.sprite = gameOverSprite;
        }
    }

    public void OnRetry()
    {
        SceneManager.LoadScene("MainScene"); // ゲームシーン名
    }

    public void OnTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}