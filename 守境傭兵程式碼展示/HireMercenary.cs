using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HireMercenary : Singleton<HireMercenary>
{
    [SerializeField] ClassCard classCardPrefab;
    [SerializeField] Transform layoutGroup;
    [SerializeField] float refreshIntervalSeconds = 60f;
    [SerializeField] Button closeButton;
    [SerializeField] TextMeshProUGUI countDownText;
    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }
    float refreshTimer = 0f;
    bool isFirstTime = true;
    public void Setup()
    {
        gameObject.SetActive(true);
        if (isFirstTime)
        {
            isFirstTime = false;
            CountingRefreshTimer().Forget();
            RefreshMercenaries();
        }
    }
    private void SetupMercenaryCard(CharacterData characterData)
    {
        ClassCard classCard = Instantiate(classCardPrefab, layoutGroup);
        classCard.SetupCard(characterData);
    }
    private CharacterData GenerateRandomCharacter()
    {
        int jobCount = System.Enum.GetValues(typeof(CharacterData.JobType)).Length;
        var job = (CharacterData.JobType)UnityEngine.Random.Range(0, jobCount);
        return CharacterData.Create(CharacterData.FactionType.Ally, job, CharacterData.CharacterType.Human, true);
    }
    private async UniTask CountingRefreshTimer()
    {
        while (true)
        {
            refreshTimer += 1;
            countDownText.text = TimeSpan.FromSeconds(refreshIntervalSeconds - refreshTimer).ToString("mm\\:ss");
            if (refreshTimer >= refreshIntervalSeconds)
            {
                refreshTimer = 0;
                RefreshMercenaries();
            }
            await UniTask.Delay(1000);
        }
    }
    private void RefreshMercenaries()
    {
        for (int i = 0; i < layoutGroup.childCount; i++)
        {
            Destroy(layoutGroup.GetChild(i).gameObject);
        }
        for (int i = 0; i < 3; i++)
        {
            CharacterData characterData = GenerateRandomCharacter();
            SetupMercenaryCard(characterData);
        }
    }
    public void RefreshIfEmpty()
    {
        if (layoutGroup.transform.childCount <= 0)
        {
            RefreshMercenaries();
        }
    }
    #region Test Methods
    [ContextMenu("Test Setup")]
    public void TestSetup()
    {
        Setup();
    }
    [ContextMenu("Test Refresh")]
    public void TestRefresh()
    {
        RefreshMercenaries();
    }
    #endregion
}
