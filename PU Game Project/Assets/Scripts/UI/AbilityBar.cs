using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MHA.UserInterface
{
    public class AbilityBar : MonoBehaviour
    {

        public GameObject abilityGroup;
        public GameObject abilityButtonGroup;
        [Tooltip("0 = Disables, 1 = Input, 2 = Selected")]
        public List<Transform> abilityBarLocations = new List<Transform>();
        public Vector3 currentPos;
        Vector3 moveDirection;
        public float translationMoveSpeed = 10f;

        [Space]
        [Header("Abiltiy Buttons")]
        public GameObject energyIndicator;
        public GameObject statsIndicator;
        public GameObject moveButton;
        public GameObject itemButton;
        public GameObject attackButton;
        public GameObject abilityButtonPrefab;
        List<GameObject> abButtonList = new List<GameObject>();

        [Space]
        [Header("Audio")]
        public AudioSource audSource;

        public delegate void AbilityButtonClick();
        public static event AbilityButtonClick AbilityButtonClickEVENT;

        private void Awake()
        {
            audSource = GetComponent<AudioSource>();
        }


        void Start()
        {
            abilityGroup.transform.position = abilityBarLocations[0].position;

            TurnManager.instance.BattlePhaseResponseEVENT += BattlePhaseReceiver;
            ClickSelection.instance.NewSelectionEvent += NewSelectionReceiver;
            ClickSelection.instance.ClearSelectionEvent += ClearSelectionReceiver;
        }
        private void OnDestroy()
        {
            TurnManager.instance.BattlePhaseResponseEVENT -= BattlePhaseReceiver;
            ClickSelection.instance.NewSelectionEvent -= NewSelectionReceiver;
            ClickSelection.instance.ClearSelectionEvent -= ClearSelectionReceiver;
        }


        void BattlePhaseReceiver(TurnManager.BattlePhase givenPhase)
        {
            UpdateAbilityBar();
        }
        void NewSelectionReceiver(GameObject newObj)
        {
            UpdateAbilityBar();
        }
        void ClearSelectionReceiver()
        {
            UpdateAbilityBar();
        }

        // Update is called once per frame
        void Update()
        {
            if (abilityGroup.transform.position != currentPos)
            {
                moveDirection = currentPos - abilityGroup.transform.position;

                abilityGroup.transform.Translate(moveDirection * Time.deltaTime * translationMoveSpeed);
            }
        }

        private void UpdateAbilityBar()
        {
            if (TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.PlayerInput && ClickSelection.instance.hasSelection && TurnManager.instance.teamTracker == Unit.Teams.Hero)
            {
                currentPos = abilityBarLocations[1].position;
                AbilityBarActivation();
            }
            else if ((TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.PlayerInput || (TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.ActionPhase) && TurnManager.instance.teamTracker == Unit.Teams.Hero))
            {
                currentPos = abilityBarLocations[2].position;
                AbilityBarDeactivation();
            }
            else
            {
                currentPos = abilityBarLocations[0].position;
                AbilityBarDeactivation();
            }
        }

        private void AbilityBarActivation()
        {
            GeneralSetUp();

            attackButton.GetComponent<Button>().interactable = true;
            moveButton.GetComponent<Button>().interactable = true;
            itemButton.GetComponent<Button>().interactable = true;
            energyIndicator.GetComponent<TextMeshProUGUI>().text = ClickSelection.instance.selectedUnitScript.CreatureScript.CurrentEnergy.ToString();
            energyIndicator.transform.parent.gameObject.SetActive(true);

            statsIndicator.GetComponent<StatsIndicator>().AssociatedUnit = ClickSelection.instance.selectedUnitScript;
            statsIndicator.SetActive(true);
        }
        private void AbilityBarDeactivation()
        {
            GeneralSetUp();
            attackButton.GetComponent<Button>().interactable = false;
            moveButton.GetComponent<Button>().interactable = false;
            itemButton.GetComponent<Button>().interactable = false;
            energyIndicator.GetComponent<TextMeshProUGUI>().text = string.Empty;
            energyIndicator.transform.parent.gameObject.SetActive(false);

            statsIndicator.SetActive(false);
        }



        public void MoveButtonPress()
        {
            GeneralSetUp();

            ClickSelection.instance.selectedUnitObj.GetComponent<Unit>().movementAbilitiesInsta[0].InitiateAbility(0);
        }

        public void ItemButtonPress()
        {
            GeneralSetUp();
        }

        public void AttackButtonPress()
        {
            GeneralSetUp();
            List<CharAbility> acAb = ClickSelection.instance.selectedUnitScript.activatableAbilitiesInsta;

            float[] xPositions = new float[acAb.Count];
            float distance = 56f;
            if(acAb.Count % 2 != 0) //Odd
            {
                for (int i = 0; i < xPositions.Length; i++)
                {
                    float num = (i - ((xPositions.Length - 1f) / 2f));
                    xPositions[i] = distance * num;
                }
            }
            else //Even
            {
                for (int i = 0; i < xPositions.Length; i++)
                {
                    float num = (i - (xPositions.Length / 2f)) + 0.5f;
                    xPositions[i] = distance * num;
                }
            }

            for (int i = 0; i < acAb.Count; i++)
            {
                GameObject spawnedABButton = Instantiate(abilityButtonPrefab, abilityButtonGroup.transform);
                RectTransform rectTransABButton = spawnedABButton.GetComponent<RectTransform>();
                RectTransform rectAttack = attackButton.GetComponent<RectTransform>();
                rectTransABButton.anchoredPosition = new Vector2(rectAttack.anchoredPosition.x + xPositions[i], rectAttack.anchoredPosition.y + 60f);

                AbilityButton buttonScript = spawnedABButton.GetComponent<AbilityButton>();
                buttonScript.AssociatedAbility = acAb[i];
                buttonScript.abilityBar = this;

                abButtonList.Add(spawnedABButton);
            }
        }

        public void GeneralSetUp()
        {
            foreach (GameObject button in abButtonList)
            {
                Destroy(button);
            }
            abButtonList.Clear();

            ClickSelection.instance.ResetToDefault(); 
            if(AbilityButtonClickEVENT != null)
            {
                AbilityButtonClickEVENT.Invoke();
            }

        }
    }
}
