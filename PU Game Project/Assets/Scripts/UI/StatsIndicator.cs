using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class StatsIndicator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Unit AssociatedUnit
    {
        get
        {
            return associatedUnit;
        }
        set
        {
            associatedUnit = value;
            SetUpStatsIndicator();

        }
    }
    [SerializeField]
    private Unit associatedUnit;

    [Header("Description")]
    [SerializeField]
    private GameObject statsGroup;
    [SerializeField]
    private TextMeshProUGUI unitName;
    [SerializeField]
    private TextMeshProUGUI unitTotalHealth;
    [SerializeField]
    private TextMeshProUGUI unitTotalEnergy;
    [SerializeField]
    private TextMeshProUGUI unitStrength;
    [SerializeField]
    private TextMeshProUGUI unitDefense;
    [SerializeField]
    private TextMeshProUGUI unitLuck;

    private void Awake()
    {
        statsGroup.SetActive(false);
    }

    public void SetUpStatsIndicator()
    {
        unitName.text = associatedUnit.givenCharData.heroName;
        unitTotalHealth.text = "HP: " + associatedUnit.maxHealth.Value.ToString();
        unitTotalEnergy.text = "ENG: " + associatedUnit.maxEnergy.Value.ToString();
        unitStrength.text = "STR: " + associatedUnit.currentStrength.Value.ToString();
        unitDefense.text = "DEF: " + associatedUnit.currentDefense.Value.ToString();
        unitLuck.text = "LUC: " + associatedUnit.currentLuck.Value.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        statsGroup.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        statsGroup.SetActive(false);
    }
}
