using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MHA.UserInterface
{
    public class NextPhaseButton : MonoBehaviour
    {
        public Button buttonComp;
        public Image imageComp;
        public Text textComp;

        [Space]
        public Color baseColor;
        public Color baseHighlight;
        public Color endTurnColor;
        public Color endTurnHighlight;

        public bool hasSetToFinish;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.PlayerInput)
            {
                if(!buttonComp.interactable)
                {
                    ButtonSet();
                    buttonComp.interactable = true;
                }
                if(TurnManager.instance.activePlayers.Count <= 0 && !hasSetToFinish)
                {
                    hasSetToFinish = true;
                    ButtonFinish();
                }
            }
            else if(buttonComp.interactable)
            {
                ResetButton();
                buttonComp.interactable = false;
            }

        }

        void ButtonSet()
        {
            ColorBlock colors = buttonComp.colors;
            colors.normalColor = baseColor;
            colors.highlightedColor = baseHighlight;
            buttonComp.colors = colors;
        }

        public void ButtonFinish()
        {
            ColorBlock colors = buttonComp.colors;
            colors.normalColor = endTurnColor;
            colors.highlightedColor = endTurnHighlight;
            buttonComp.colors = colors;
        }

        public void ResetButton()
        {
            hasSetToFinish = false;

            ColorBlock colors = buttonComp.colors;
            colors.normalColor = baseColor;
            colors.highlightedColor = baseHighlight;
            buttonComp.colors = colors;
        }
    } 
}
