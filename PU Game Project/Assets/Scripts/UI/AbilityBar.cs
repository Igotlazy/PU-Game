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

        }

        // Update is called once per frame
        void Update()
        {
            MoveAbilityBar();
            AbilityBarActivation();
        }

        private void MoveAbilityBar()
        {
            if (TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.PlayerInput && ClickSelection.instance.selectedUnitObj != null)
            {
                currentPos = abilityBarLocations[1].position;
            }
            else if (TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.PlayerInput || (TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.ActionPhase && TurnManager.instance.teamTracker))
            {
                currentPos = abilityBarLocations[2].position;
            }
            else
            {
                currentPos = abilityBarLocations[0].position;
            }

            if (abilityGroup.transform.position != currentPos)
            {
                moveDirection = currentPos - abilityGroup.transform.position;

                abilityGroup.transform.Translate(moveDirection * Time.deltaTime * translationMoveSpeed);
            }
        }

        private void AbilityBarActivation()
        {
            if (TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.PlayerInput && ClickSelection.instance.hasSelection && !isActive)
            {
                moveButton.GetComponent<Button>().interactable = true;
                itemButton.GetComponent<Button>().interactable = true;
                foreach (GameObject currentButton in attackButtons)
                {
                    currentButton.GetComponent<Button>().interactable = true;
                }

                isActive = true;
            }

            if((TurnManager.instance.CurrentBattlePhase != TurnManager.BattlePhase.PlayerInput || !ClickSelection.instance.hasSelection) && isActive)
            {
                moveButton.GetComponent<Button>().interactable = false;
                itemButton.GetComponent<Button>().interactable = false;
                foreach (GameObject currentButton in attackButtons)
                {
                    currentButton.GetComponent<Button>().interactable = false;
                }

                isActive = false;
            }
        }
        bool isActive;



        public void MoveButtonPress()
        {
            GeneralSetUp();

            ClickSelection.instance.prepMoving = true;
            Debug.Log("Cast Ability");
            ClickSelection.instance.selectedUnitObj.GetComponent<HeroCharacter>().abilityList[0].InitiateAbility(0);
            ClickSelection.instance.DrawMoveZone();
        }

        public void ItemButtonPress()
        {
            GeneralSetUp();
        }

        public void BasicAButtonPress()
        {
            GeneralSetUp();

            ClickSelection.instance.prepAttack = true;
            //ClickSelection.instance.selectedUnitObj.GetComponent<HeroCharacter>().UnitBasicPrep();
        }

        public void A1ButtonPress()
        {
            GeneralSetUp();

            ClickSelection.instance.prepAttack = true;
            //ClickSelection.instance.selectedUnitObj.GetComponent<HeroCharacter>().UnitAttack1Prep();
        }

        public void A2ButtonPress()
        {
            GeneralSetUp();

            ClickSelection.instance.prepAttack = true;
            //ClickSelection.instance.selectedUnitObj.GetComponent<HeroCharacter>().UnitAttack2Prep();
        }

        public void A3ButtonPress()
        {
            GeneralSetUp();

            ClickSelection.instance.prepAttack = true;
            //ClickSelection.instance.selectedUnitObj.GetComponent<HeroCharacter>().UnitAttack3Prep();
        }



        private void GeneralSetUp()
        {
            ClickSelection.instance.ResetToDefault(); //Resets prepAttack and prepMove and cleans all tiles. 
            //ClickSelection.instance.selectedUnitObj.GetComponent<HeroCharacter>().UnitAbilityCleanup();
        }
    }
}
