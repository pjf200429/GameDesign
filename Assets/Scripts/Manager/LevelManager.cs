using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Info")]
    public int currentLevel = 1;
    public int maxLevel = 5;

    [Header("Scene Settings")]
    public string baseSceneName = "Level_"; // e.g., Level_1, Level_2, ...
    public float loadDelay = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadCurrentLevel();
    }

    /// <summary>
    /// 加载当前关卡
    /// </summary>
    public void LoadCurrentLevel()
    {
        string levelSceneName = baseSceneName + currentLevel;
        Debug.Log($"Loading Scene: {levelSceneName}");
        SceneManager.LoadScene(levelSceneName);
    }

    /// <summary>
    /// 推进到下一层
    /// </summary>
    public void GoToNextLevel()
    {
        if (currentLevel < maxLevel)
        {
            currentLevel++;
            Invoke(nameof(LoadCurrentLevel), loadDelay);
        }
        else
        {
            Debug.Log("All levels complete!");
            // TODO: Trigger victory screen or loop
        }
    }

    /// <summary>
    /// 重新开始当前层（失败或重试）
    /// </summary>
    public void RestartCurrentLevel()
    {
        Invoke(nameof(LoadCurrentLevel), loadDelay);
    }

    /// <summary>
    /// 初始化游戏（主菜单或新开局）
    /// </summary>
    public void StartNewRun()
    {
        currentLevel = 1;
        LoadCurrentLevel();
    }
}


