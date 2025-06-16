using TMPro;
using UnityEngine;

public class PlayerGold : Singleton<PlayerGold>
{
    [SerializeField] private TextMeshProUGUI goldText;

    private void Awake()
    {
        PlayerDataManager.Instance.OnGoldChanged += UpdateGoldText;
    }

    private void Start()
    {
        UpdateGoldText(PlayerDataManager.Instance.Gold);
    }

    public void UpdateGoldText(int currentGold)
    {
        goldText.text = currentGold.ToString();
    }
}
