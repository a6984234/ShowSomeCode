using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class GraveyardUI : Singleton<GraveyardUI>
{
    [SerializeField] GraveyardCard graveyardCardPrefab;
    [SerializeField] SacrificeSelectionUI sacrificeSelectionUI;
    [SerializeField] Transform graveyardCardParent;
    [SerializeField] Button closeButton;
    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }
    public void Setup()
    {
        gameObject.SetActive(true);
        UpdateUI();
    }

    private void UpdateUI()
    {
        ClearLayout();
        foreach (var merc in GraveyardManager.Instance.GetDeadCharacters())
        {
            var card = Instantiate(graveyardCardPrefab, graveyardCardParent);
            card.SetupCard(merc);
        }
    }

    private void ClearLayout()
    {
        foreach (Transform child in graveyardCardParent)
        {
            Destroy(child.gameObject);
        }
    }
    public void OpenSacrificeSelectionUI(CharacterData characterData)
    {
        graveyardCardParent.gameObject.SetActive(false);
        sacrificeSelectionUI.Setup(characterData);
    }
    public void BackToGraveyardUI()
    {
        graveyardCardParent.gameObject.SetActive(true);
        sacrificeSelectionUI.gameObject.SetActive(false);
        UpdateUI();
    }
}
