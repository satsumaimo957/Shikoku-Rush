using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void OnStart()
    {
        SceneManager.LoadScene("MainScene"); // ゲームシーン名
    }

    public void OnQuit()
    {
        Application.Quit();

        // エディタ用（重要）
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}