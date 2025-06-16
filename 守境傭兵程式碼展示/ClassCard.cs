using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClassCard : CharacterCardBase , IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI priceText;
    int price = 100;


    public override void SetupCard(CharacterData data)
    {
        base.SetupCard(data);
        price = CalculatePrice();
        priceText.text = price.ToString();
    }
    private int CalculatePrice()
    {
        int calculatedPrice = Mathf.RoundToInt(
            characterData.Atk * 5f +
            characterData.Def * 3f +
            characterData.Hp * 0.5f
        );
        return calculatedPrice;
    }
    #region Test Methods
    [ContextMenu("Test SetupClassCard")]
    public void TestSetupClassCard()
    {
        CharacterData characterData = new CharacterData(
            CharacterData.FactionType.Ally,
            CharacterData.JobType.Archer,
            CharacterData.CharacterType.Human,
            10, 5, 100, 100, 1, 0.1f, 1.5f, 0.05f
        );
        SetupCard(characterData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!PlayerDataManager.Instance.SpendGold(price))
        {
            Debug.Log("Not enough gold");
            return;
        }
        PlayerDataManager.Instance.AddMercenary(characterData);
        MercenaryManager.Instance.Hire(characterData);
        Destroy(gameObject);
        UniTask.Void(async () =>
        {
            await UniTask.Yield();
            HireMercenary.Instance.RefreshIfEmpty();
        });
        
        
    }
    #endregion
}
