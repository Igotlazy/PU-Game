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

        public bool isInInput;
        public bool allFinished;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            ButtonSet();
        }

        void ButtonSet()
        {
            if(TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.PlayerInput)
            {
                if (!isInInput)
                {
                    if (TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.PlayerInput)
                    {
                        isInInput = true;
                        buttonComp.interactable = true;

                        ColorBlock colors = buttonComp.colors;
                        colors.normalColor = baseColor;
                        colors.highlightedColor = baseHighlight;
                        buttonComp.colors = colors;
                    }
                }
                if (allFinished /*&& TurnManager.instance.activePlayers.Count <= 0*/)
                {
                    allFinished = false;
                    ColorBlock colors = buttonComp.colors;
                    colors.normalColor = endTurnColor;
                    colors.highlightedColor = endTurnHighlight;
                    buttonComp.colors = colors;
                }
            }
        }

        public void ResetButton()
        {
            buttonComp.interactable = false;
            isInInput = false;
            allFinished = false;

            ColorBlock colors = buttonComp.colors;
            colors.normalColor = baseColor;
            colors.highlightedColor = baseHighlight;
            buttonComp.colors = colors;

            TurnManager.instance.NextMainBattlePhase();
        }
    }
}
