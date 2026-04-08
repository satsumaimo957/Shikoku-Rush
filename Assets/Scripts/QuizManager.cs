using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public static class GameResultData
{
    public static int score;
}

public class QuizManager : MonoBehaviour
{
    // public Image leftImage;
    // public Image rightImage;
    public Image[] images;

    public Sprite shikokuSprite;
    public Sprite australiaSprite;

    public TextMeshProUGUI scoreText;
    private int score = 0;

    public TextMeshProUGUI addScoreText;

    private int combo = 0;

    public TextMeshProUGUI timeText;

    public float startTime = 30f; // Inspectorで変更可
    private float currentTime;

    public TextMeshProUGUI addTimeText;

    public TextMeshProUGUI resultText;

    private bool isGameOver = false;

    private int questionCount = 0;

    private bool isRotating = false;

    private int correctIndex;

    private float[] rotationSpeeds;

    public TextMeshProUGUI startText;
    public TextMeshProUGUI tutorialText;

    private Vector2[] positions = new Vector2[]
    {
        new Vector2(-400, 300),
        new Vector2(400, 300),
        new Vector2(-400, -300),
        new Vector2(400, -300),
        new Vector2(0, 200),
        new Vector2(0, -200)
    };

    void Start()
    {
        rotationSpeeds = new float[images.Length];
        
        score = 0;
        combo = 0;

        currentTime = startTime;

        UpdateScoreText();

        tutorialText.gameObject.SetActive(true);

        // ゲーム停止
        Time.timeScale = 0f;

        StartCoroutine(StartCountdown());
    }

    void Update()
    {
        if (isGameOver) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            EndGame();
        }

        timeText.text = "Time: " + currentTime.ToString("F1");

        if (isRotating)
        {
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i].gameObject.activeSelf)
                {
                    images[i].rectTransform.Rotate(0, 0, rotationSpeeds[i] * Time.deltaTime);
                }
            }
        }
    }

    IEnumerator StartCountdown()
    {
        yield return ShowText("3", 1.0f);
        yield return ShowText("2", 1.0f);
        yield return ShowText("1", 1.0f);
        yield return ShowText("START!", 1.3f);

        startText.gameObject.SetActive(false);
        tutorialText.gameObject.SetActive(false);

        // ゲーム開始
        Time.timeScale = 1f;

        SetQuestion();
    }

    IEnumerator ShowText(string text, float duration)
    {
        startText.text = text;
        startText.gameObject.SetActive(true);

        float t = 0;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;

            // フェードアウト
            float alpha = 1 - (t / duration);
            startText.alpha = alpha;

            // 拡大
            float scale = 1 + t;
            startText.transform.localScale = Vector3.one * scale;

            yield return null;
        }
    }

    void ShufflePositions(Vector2[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int rand = Random.Range(i, array.Length);
            Vector2 temp = array[i];
            array[i] = array[rand];
            array[rand] = temp;
        }
    }

    void SetQuestion()
    {
        questionCount++;

        int imageCount = 2;

        if (questionCount > 40)
        {
            imageCount = 6;
        }
        else if (questionCount > 30)
        {
            imageCount = 5;
        }
        else if (questionCount > 20)
        {
            imageCount = 4;
        }
        else if (questionCount > 10)
        {
            imageCount = 3;
        }

        // 正解位置
        correctIndex = Random.Range(0, imageCount);

        // ★位置シャッフル
        Vector2[] shuffled = (Vector2[])positions.Clone();
        ShufflePositions(shuffled);

        for (int i = 0; i < images.Length; i++)
        {
            if (i < imageCount)
            {
                images[i].gameObject.SetActive(true);

                if (i == correctIndex)
                    images[i].sprite = shikokuSprite;
                else
                    images[i].sprite = australiaSprite;

                ResetTransform(images[i]);

                // ★ここが重要
                images[i].rectTransform.anchoredPosition = shuffled[i];
            }
            else
            {
                // 非表示（←これが重要）
                images[i].gameObject.SetActive(false);
            }
        }

        // 変化（回転・スケール）は2問目以降
        if (questionCount >= 2)
        {
            for (int i = 0; i < imageCount; i++)
            {
                ApplyTransform(images[i], i, imageCount);
            }
        }
        // ★回転（配列対応版にする必要あり）
        SetupRotation(imageCount);
    }

    void ApplyBasePosition(Image img, int index, int imageCount)
    {
        RectTransform rt = img.rectTransform;

        float spacing = 400f;
        float startX = -spacing * (imageCount - 1) / 2f;

        float posX = startX + index * spacing;

        rt.anchoredPosition = new Vector2(posX, 0);
    }

    void SetupRotation(int imageCount)
    {
        isRotating = Random.value > 0.5f;

        if (!isRotating) return;

        for (int i = 0; i < imageCount; i++)
        {
            float speed = Random.Range(150f, 300f);
            float dir = Random.value > 0.5f ? 1 : -1;

            rotationSpeeds[i] = speed * dir;
        }
    }

    void ApplyTransform(Image img, int index, int imageCount)
    {
        RectTransform rt = img.rectTransform;

        // 回転
        float angle = Random.Range(0f, 360f);
        rt.rotation = Quaternion.Euler(0, 0, angle);

        // 拡大縮小
        float scale = Random.Range(0.7f, 1.3f);
        rt.localScale = new Vector3(scale, scale, 1);

        // ★位置は「現在位置」を基準にする
        float offsetX = Random.Range(-30f, 30f);
        float offsetY = Random.Range(-30f, 30f);

        rt.anchoredPosition += new Vector2(offsetX, offsetY);
    }

    void ResetTransform(Image img)
    {
        RectTransform rt = img.rectTransform;

        rt.rotation = Quaternion.identity;
        rt.localScale = Vector3.one;
        rt.anchoredPosition = Vector2.zero;
    }

    // public void OnClickLeft()
    // {
    //     if (isGameOver) return;
    //     CheckAnswer(0);
    // }

    // public void OnClickRight()
    // {
    //     if (isGameOver) return;
    //     CheckAnswer(1);
    // }

    public void OnClickImage(int index)
    {
        if (isGameOver) return;
        CheckAnswer(index);
    }

    void CheckAnswer(int clickedPosition)
    {
        isRotating = false;
        
        bool isCorrect = (clickedPosition == correctIndex);

        if (isCorrect)
        {
            combo++;

            int addScore = 1;
            float addTime = 0f;

            if (combo >= 10)
            {
                addScore = 3;
            }
            else if (combo >= 5)
            {
                addScore = 2;
            }

            if(combo >= 5 && combo % 3 == 0)
            {
                addTime = 2f;
            }

            score += addScore;
            currentTime += addTime;

            if (addTime > 0)
            {
                ShowAddTime(addTime);
            }

            // 表示
            ShowAddScore(addScore, combo);
            Debug.Log("正解！");
            // score++;
        }
        else
        {
            combo = 0;
            currentTime -= 2f;
            ShowAddTime(-2f);
            Debug.Log("不正解！");
        }

        ShowResult(isCorrect);
        UpdateScoreText();
        // 次の問題
        SetQuestion();
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    void ShowAddScore(int addScore, int combo)
    {
        if (combo >= 2)
        {
            addScoreText.text = combo + "Combo! +" + addScore;
        }
        else
        {
            addScoreText.text = "+" + addScore;
        }

        StopCoroutine("ClearAddScoreText");
        StartCoroutine(ClearAddScoreText());
    }

    IEnumerator ClearAddScoreText()
    {
        yield return new WaitForSeconds(0.5f);
        addScoreText.text = "";
    }

    void EndGame()
    {
        isGameOver = true;

        Debug.Log("ゲーム終了！ スコア: " + score);

        // スコア保存
        GameResultData.score = score;

        Time.timeScale = 1f;

        SceneManager.LoadScene("ResultScene");
    }

    void ShowAddTime(float time)
    {
        if (time > 0)
        {
            addTimeText.text = "+" + time.ToString("F1") + "s";
        }
        else
        {
            addTimeText.text = time.ToString("F1") + "s"; // マイナスはそのまま
        }

        StopCoroutine("ClearAddTimeText");
        StartCoroutine(ClearAddTimeText());
    }

    IEnumerator ClearAddTimeText()
    {
        yield return new WaitForSeconds(0.5f);
        addTimeText.text = "";
    }

    void ShowResult(bool isCorrect)
    {
        if (isCorrect)
        {
            resultText.text = "O";
            resultText.color = Color.green;
        }
        else
        {
            resultText.text = "X";
            resultText.color = Color.red;
        }

        resultText.gameObject.SetActive(true);

        StopCoroutine("ClearResult");
        StartCoroutine(ClearResult());
    }

    IEnumerator ClearResult()
    {
        float duration = 0.5f;
        float t = 0;

        Color startColor = resultText.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = 1 - (t / duration);

            resultText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        resultText.gameObject.SetActive(false);
    }
}