using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentShop : MonoBehaviour
{
    [SerializeField] Button closeButton;
    [SerializeField] EquipmentCard equipmentCardPrefab;
    [SerializeField] Transform equipmentListContainer;
    private List<EquipmentCard> equipmentCards = new List<EquipmentCard>();
    private int maxEquipmentCards = 4;
    private int currentLevel = 1;
    bool isFirstTime = true;
    void Start()
    {
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }
    public void Setup()
    {
        gameObject.SetActive(true);
        if (isFirstTime)
        {
            isFirstTime = false;
            RefreshEquipment();
        }
    }

    private void RefreshEquipment()
    {
        for(int i  =0; i < equipmentListContainer.childCount; i++)
        {
            Destroy(equipmentListContainer.GetChild(i).gameObject);
        }
        
        equipmentCards.Clear();

        for (int i = 0; i < maxEquipmentCards; i++)
        {
            EquipmentCard card = Instantiate(equipmentCardPrefab, equipmentListContainer);
            card.Setup(currentLevel);
            equipmentCards.Add(card);
        }

    }
    #region Test Methods
    [ContextMenu("Test SetupEquipmentShop")]
    public void TestSetupEquipmentShop()
    {
        currentLevel = 1; // Set the level for testing
        Setup();
        RefreshEquipment();
    }
    #endregion
}
