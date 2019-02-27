using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MHA.UserInterface
{
    public class BattleUIReferences : MonoBehaviour
    {
        public NextPhaseButton endTurnButton;
        public GameObject heroTurnIntro;
        public AbilityBar mainButtonSelection;

        public static BattleUIReferences instance;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.Log("WARNING: Two Copies of BattleUIReferences");
                Destroy(this.gameObject);
            }
        }
    }
}
