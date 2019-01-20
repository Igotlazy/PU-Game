using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MHA.UserInterface
{
    public class AbilityBar : MonoBehaviour
    {

        public GameObject abilityGroup;
        [Tooltip("0 = Disables, 1 = Input, 2 = Selected")]
        public List<Transform> abilityBarLocations = new List<Transform>();
        public Vector3 currentPos;
        Vector3 moveDirection;
        public float translationMoveSpeed = 10f;

        [Space]
        [Header("Abiltiy Buttons")]
        public GameObject moveButton;
        public GameObject itemButton;

        [Tooltip("0 = Basic, 1 = 1st, 2 = 2nd, 3 = 3rd")]
        public List<GameObject> attackButtons = new List<GameObject>();


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
            if (TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.PlayerInput && ClickSelection.instance.hasSelection)
            {
                currentPos = abilityBarLocations[1].position;
                AbilityBarActivation();
            }
            else if (TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.PlayerInput || (TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.ActionPhase && TurnManager.instance.teamTracker))
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
            moveButton.GetComponent<Button>().interactable = true;
            itemButton.GetComponent<Button>().interactable = true;
            List<CharAbility> acAb = ClickSelection.instance.selectedUnitScript.activatableAbilitiesInsta;
            int abilities = 0;
            foreach (GameObject currentButton in attackButtons)
            {
                currentButton.GetComponent<Image>().sprite = acAb[abilities].abilitySprite;
                currentButton.GetComponent<Button>().interactable = true;
                if (acAb.Count - 1 > abilities)
                {
                    abilities++;
                }
            }
        }
        private void AbilityBarDeactivation()
        {
            moveButton.GetComponent<Button>().interactable = false;
            itemButton.GetComponent<Button>().interactable = false;
            foreach (GameObject currentButton in attackButtons)
            {
                currentButton.GetComponent<Image>().sprite = null;
                currentButton.GetComponent<Button>().interactable = false;
            }
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

        public void BasicAButtonPress()
        {
            GeneralSetUp();

            ClickSelection.instance.selectedUnitObj.GetComponent<Unit>().activatableAbilitiesInsta[0].InitiateAbility(0);
        }

        public void A1ButtonPress()
        {
            GeneralSetUp();

            ClickSelection.instance.prepAttack = true;
        }

        public void A2ButtonPress()
        {
            GeneralSetUp();

            ClickSelection.instance.prepAttack = true;
        }

        public void A3ButtonPress()
        {
            GeneralSetUp();

            ClickSelection.instance.prepAttack = true;
        }



        private void GeneralSetUp()
        {
            ClickSelection.instance.ResetToDefault(); 

        }
    }
}
