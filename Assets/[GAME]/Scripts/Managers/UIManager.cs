using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager instance = null;

    private void Awake()
    {
        if (instance == null) instance = this;     
    }
    #endregion

    [Header("Power UP Button")]
    [SerializeField] Image activeButtonImage;
    [SerializeField] Button activeButton;
    [SerializeField] Image timer;
    [SerializeField] GameObject timerObj;
    int time = 1;

    [Header("Panels")]
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject gameInPanel;
    [SerializeField] GameObject retryPanel;
    [SerializeField] GameObject nextLevelPanel;

    [Header("Texts")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI moneyNumText;
    [SerializeField] TextMeshProUGUI moneyRetryNumText;
    [SerializeField] TextMeshProUGUI moneyNextNumText;
    [SerializeField] TextMeshProUGUI gameInPanelLevelText;
    [SerializeField] TextMeshProUGUI retryPanelLevelText;
    [SerializeField] TextMeshProUGUI successPanelLevelText;

    [Header("Money Variables")]
    [SerializeField] Image moneyImage;
    [SerializeField] GameObject money;
    [HideInInspector] public int moneyNum;
    Vector2 anchoredDiamondPos;

    [Header("Level Progress")]
    [SerializeField] Image levelProgressBar;
    [SerializeField] Transform endOfLevel;
    Transform player;
    float distToEnd;
    float currentDistToEnd;

    [Header("Scipt References")]
    PlayerController playerController;

    [Header("References")]
    [SerializeField] Camera cam;

    private void Start()
    {       
        playerController = FindObjectOfType<PlayerController>();
        player = playerController.transform;

        MoneyFirstSet();
        StartCalculate();
    }

    public void UpdateLevelText()
    {
        levelText.text = "LEVEL " + PlayerPrefs.GetInt("FakeLevel", 1).ToString();
        gameInPanelLevelText.text = "LEVEL " + PlayerPrefs.GetInt("FakeLevel", 1).ToString();
        retryPanelLevelText.text = "LEVEL " + PlayerPrefs.GetInt("FakeLevel", 1).ToString();
        successPanelLevelText.text = "LEVEL " + PlayerPrefs.GetInt("FakeLevel", 1).ToString();
    }

    #region Panels

    public void OpenClosePanels(int panelType) //  0 start // 1 fail // 2 nexxtLevel
    {
        switch (panelType)
        {
            case 0:
                PanelController(false, true, false, false);
                StartCoroutine(SetButtonActive());
                playerController.UserActiveController(true);
                break;

            case 1:
                PanelController(false, false, true, false);
                playerController.UserActiveController(false);
                break;

            case 2:
                PanelController(false, false, false, true);
                playerController.UserActiveController(false);                
                break;
        }
    }

    void PanelController(bool startPanelVal, bool gameInPanelVal, bool retryPanelVal, bool nextPanelVal)
    {
        startPanel.SetActive(startPanelVal);
        gameInPanel.SetActive(gameInPanelVal);
        retryPanel.SetActive(retryPanelVal);
        nextLevelPanel.SetActive(nextPanelVal);
    }
    #endregion

    #region Money Update
    private void MoneyFirstSet() 
    {
        moneyNum = PlayerPrefs.GetInt("Money", 0);
        moneyNumText.text = moneyNum.ToString();
        moneyNextNumText.text = moneyNum.ToString();
        moneyRetryNumText.text = moneyNum.ToString();
        anchoredDiamondPos = moneyImage.GetComponent<RectTransform>().anchoredPosition;
    }

    public void MoneyCollectAnim(Vector3 diamondPos) // just call this method to increase diamond count "1" 
    {
        Vector2 screenPos = cam.WorldToScreenPoint(diamondPos);
        GameObject clone = PoolManager.Instance.pool.PullObjFromPool();

        clone.transform.localScale = Vector3.one * .5f; 

        RectTransform rectClone = clone.GetComponent<RectTransform>();
        rectClone.anchoredPosition = screenPos;

        clone.transform.parent = gameInPanel.transform;

        rectClone.DOAnchorPos(anchoredDiamondPos, .5f)
            .OnComplete(() => 
            { 
                clone.SetActive(false);
                UpdateMoneyText();
            });
    }

    void UpdateMoneyText()
    {
        moneyNum+=2;
        PlayerPrefs.SetInt("Money", moneyNum);
        moneyNumText.text = PlayerPrefs.GetInt("Money").ToString();
        moneyNextNumText.text = PlayerPrefs.GetInt("Money").ToString();
        moneyRetryNumText.text = PlayerPrefs.GetInt("Money").ToString();
    }
    #endregion

    #region Level Progress Bar

    void StartCalculate()
    {
        distToEnd = (player.position - endOfLevel.position).sqrMagnitude;
    }

    public void UpdateProgressBar()
    {
        currentDistToEnd = (player.position - endOfLevel.position).sqrMagnitude;
        levelProgressBar.fillAmount = currentDistToEnd / distToEnd;
    }

    #endregion

    #region Power Up
    public void PowerUp()
    {
        time = 1;
        activeButton.interactable = false;
        activeButtonImage.fillAmount = 0;
        timerObj.SetActive(true);
        StartCoroutine(UpSpeed());
    }
    IEnumerator UpSpeed()
    {
       
        playerController.windEffect.SetActive(true);
        playerController.bonusScale = .1f;
        playerController.forwardSpeed *= 1.2f;
        while (time <= 50)
        {
            time += 1;
            timer.fillAmount -= .02f;
            yield return new WaitForSeconds(.1f);
        }
        timerObj.SetActive(false);
        timer.fillAmount = 1;
        playerController.bonusScale = 0;
        playerController.forwardSpeed /= 1.2f;
        playerController.windEffect.SetActive(false);
        StartCoroutine(SetButtonActive());

    }
    IEnumerator SetButtonActive()
    {
        time = 1;
        while (time <= 100)
        {
            time += 1;
            activeButtonImage.fillAmount += .01f;
            yield return new WaitForSeconds(.1f);
        }
        activeButton.interactable = true;
    }
    #endregion
}
