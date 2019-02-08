using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace MHA.UserInterface
{
    public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public CharAbility AssociatedAbility
        {
            get
            {
                return associatedAbility;
            }
            set
            {
                associatedAbility = value;
                SetUpAbilityIcon();

            }
        }
        [SerializeField]
        private CharAbility associatedAbility;
        [HideInInspector]
        public AbilityBar abilityBar;
        Image abilityIcon;

        [Space]
        [Header("Cooldown")]
        [SerializeField]
        private Image cooldownIndicator;
        [SerializeField]
        private TextMeshProUGUI cooldownValue;

        [Header("Description")]
        [SerializeField]
        private GameObject descriptionGroup;
        [SerializeField]
        private TextMeshProUGUI abilityNameInd;
        [SerializeField]
        private TextMeshProUGUI abilityDescriptionInd;
        [SerializeField]
        private TextMeshProUGUI abilityCDInd;
        [SerializeField]
        private TextMeshProUGUI abilityCostInd;
        public Color badColor;

        [Space]
        [Header("Audio")]
        [SerializeField]
        private AudioClip hover;
        [SerializeField]
        private AudioClip click;
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            abilityIcon = GetComponent<Image>();
            descriptionGroup.SetActive(false);
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void SetUpAbilityIcon()
        {
            abilityIcon.sprite = associatedAbility.abilitySprite;

            //Cooldown
            if(associatedAbility.currentCooldown > 0)
            {
                cooldownIndicator.fillAmount = (float) associatedAbility.currentCooldown / associatedAbility.turnCooldown;

                cooldownValue.text = associatedAbility.currentCooldown.ToString();
                cooldownIndicator.gameObject.SetActive(true);
            }
            else
            {
                cooldownIndicator.gameObject.SetActive(false);
            }

            //Description
            abilityNameInd.text = associatedAbility.abilityName;
            abilityDescriptionInd.text = associatedAbility.abilityDescription;
            abilityCDInd.text = "CD: " + associatedAbility.turnCooldown;
            abilityCostInd.text = "Cost: " + associatedAbility.energyCost.ToString();
            if(associatedAbility.energyCost > associatedAbility.associatedUnit.CreatureScript.CurrentEnergy)
            {
                abilityCostInd.color = badColor;
            }
            else
            {
                abilityCostInd.color = new Color(1, 1, 1);
            }
            if (associatedAbility.currentCooldown > 0)
            {
                abilityCDInd.color = badColor;
            }
            else
            {
                abilityCDInd.color = new Color(1, 1, 1);
            }
        }

        public void CastAbilityWithButton()
        {
            if(associatedAbility.currentCooldown <= 0 && associatedAbility.associatedUnit.CreatureScript.CurrentEnergy >= associatedAbility.energyCost)
            {
                abilityBar.audSource.clip = click;
                abilityBar.audSource.Play();

                abilityBar.GeneralSetUp();
                associatedAbility.InitiateAbility(0);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            descriptionGroup.SetActive(true);
            abilityBar.audSource.clip = hover;
            abilityBar.audSource.Play();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            descriptionGroup.SetActive(false);
        }
    }
}
