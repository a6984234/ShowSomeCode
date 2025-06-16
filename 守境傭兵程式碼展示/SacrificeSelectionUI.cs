using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SacrificeSelectionUI : Singleton<SacrificeSelectionUI>
{
    [SerializeField] SacrificeCandidateCard sacrificeCandidateCardPrefab;
    [SerializeField] Transform sacrificeCandidateCardParent;
    [SerializeField] Button closeButton;
    [SerializeField] TextMeshProUGUI needSoulText;
    [SerializeField] Button reviveButton;
    private CharacterData reviveTarget;
    int soulPrice;
    private int currentSoul;
    private List<SacrificeCandidateCard> activeCards = new List<SacrificeCandidateCard>();
    private List<SacrificeCandidateCard> selectedCards = new List<SacrificeCandidateCard>();
    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            GraveyardUI.Instance.BackToGraveyardUI();
        });
        Character.OnCharacterDied += OnCharacterDied;
        reviveButton.onClick.AddListener(() =>
        {
            if (CheckCanRevive())
            {
                GraveyardManager.Instance.Revive(reviveTarget);
                GraveyardUI.Instance.BackToGraveyardUI();
            }
        });
    }

    private void OnCharacterDied(Character arg0)
    {
        var card = activeCards.Find(x => x.CharacterData == arg0.CharacterData);
        if (card == null) return;
        Destroy(card.gameObject);
        activeCards.Remove(card);
        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
        }
    }

    public void Setup(CharacterData characterData)
    {
        Reset();

        reviveTarget = characterData;
        soulPrice = characterData.SoulPrice();
        gameObject.SetActive(true);
        UpdateNeedSoulText();
        ClearLayout();
        LoadMercenariesData();
    }

    private void Reset()
    {
        activeCards.Clear();
        selectedCards.Clear();
        currentSoul = 0;
        reviveButton.interactable = false;
    }

    private void UpdateNeedSoulText()
    {
        if (currentSoul >= soulPrice)
        {
            needSoulText.text = "靈魂足夠，可以復活";
            return;
        }
        needSoulText.text = $"還需要 {soulPrice - currentSoul} 靈魂來復活";
    }

    private void LoadMercenariesData()
    {
        foreach (var p in MercenaryManager.Instance.GetAllMercenaries())
        {
            var card = Instantiate(sacrificeCandidateCardPrefab, sacrificeCandidateCardParent);
            card.SetupCard(p.CharacterData);
            card.SetupCharacterImage(p.RenderTexture);
            activeCards.Add(card);
        }
    }

    private void ClearLayout()
    {
        foreach (Transform child in sacrificeCandidateCardParent)
        {
            Destroy(child.gameObject);
        }
        activeCards.Clear();
    }
    public void UpdateCurrentSoul(SacrificeCandidateCard sacrificeCandidateCard)
    {
        if (selectedCards.Contains(sacrificeCandidateCard))
        {
            selectedCards.Remove(sacrificeCandidateCard);
            currentSoul -= sacrificeCandidateCard.CharacterData.SoulPrice();
        }
        else
        {
            selectedCards.Add(sacrificeCandidateCard);
            currentSoul += sacrificeCandidateCard.CharacterData.SoulPrice();
        }
        UpdateNeedSoulText();
        reviveButton.interactable = CheckCanRevive();
    }
    public bool IsCardSelected(SacrificeCandidateCard sacrificeCandidateCard)
    {
        return selectedCards.Contains(sacrificeCandidateCard);
    }
    public bool CheckCanRevive()
    {
        return currentSoul >= soulPrice;
    }
}
