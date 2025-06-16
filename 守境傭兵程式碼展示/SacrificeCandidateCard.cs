using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SacrificeCandidateCard : CharacterCardBase , IPointerClickHandler
{
    [SerializeField] RawImage characterImage;
    [SerializeField] TextMeshProUGUI soulText;
    [SerializeField] Image chooseImage;

    public void OnPointerClick(PointerEventData eventData)
    {
        var isSelected = SacrificeSelectionUI.Instance.IsCardSelected(this);

        if (SacrificeSelectionUI.Instance.CheckCanRevive() && !isSelected)
            return;

        SacrificeSelectionUI.Instance.UpdateCurrentSoul(this);
        chooseImage.gameObject.SetActive(!isSelected);
    }


    public override void SetupCard(CharacterData data)
    {
        base.SetupCard(data);
        soulText.text = characterData.SoulPrice().ToString();
        chooseImage.gameObject.SetActive(false);
    }
    public void SetupCharacterImage(RenderTexture renderTexture)
    {
        characterImage.texture = renderTexture;
    }
    void OnDestroy()
    {
        if (characterImage.texture != null && characterImage.texture is RenderTexture rt)
        {
            rt.Release();
            characterImage.texture = null;
        }
    }
}
