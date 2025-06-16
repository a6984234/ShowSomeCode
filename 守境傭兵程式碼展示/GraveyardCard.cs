using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class GraveyardCard : CharacterCardBase, IPointerClickHandler
{

    [SerializeField] private TextMeshProUGUI soulText;
    int price = 100;

    public override void SetupCard(CharacterData data)
    {
        base.SetupCard(data);
        price = characterData.SoulPrice();
        soulText.text = price.ToString();
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
        GraveyardUI.Instance.OpenSacrificeSelectionUI(characterData);
        // characterData.ResetHp();
        // PlayerDataManager.Instance.AddMercenary(characterData);
        // GraveyardManager.Instance.Revive(characterData);
        // Destroy(gameObject);
    }
    #endregion
}
