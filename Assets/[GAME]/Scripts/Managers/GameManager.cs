using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance = null;

    private void Awake()
    {
        if (instance == null) instance = this;
    }
    #endregion


    [HideInInspector] public int fakeLevelNum = 1; 
    [HideInInspector] public int levelNum = 1; 

    private void Start() => UpdatePlayerPerfs();
    void UpdatePlayerPerfs() // initial uı adjust
    {
        fakeLevelNum = PlayerPrefs.GetInt("FakeLevel",1);
        levelNum = PlayerPrefs.GetInt("Level", 1);
        UIManager.instance.UpdateLevelText();
    }


    public void EndGame(int endType) // 2 success // 1 fail // call this after level ended 
    {
        switch (endType)
        {
            case 2:
                UIManager.instance.OpenClosePanels(2);
               
                break;
            case 1:
                UIManager.instance.OpenClosePanels(1);
                break;
        }
    }

    public void NextLevel() 
    {
        fakeLevelNum++;
        levelNum++;
        RecordLevel();
        RecordFakeLevel();
        if (levelNum == SceneManager.sceneCountInBuildSettings)
        {
            levelNum = 1;
            RecordLevel();
        }
        SceneManager.LoadScene(levelNum);
    }

    public void RetryLevel() =>SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    void RecordLevel() => PlayerPrefs.SetInt("Level", levelNum);
    void RecordFakeLevel() => PlayerPrefs.SetInt("FakeLevel", fakeLevelNum);

 
}
