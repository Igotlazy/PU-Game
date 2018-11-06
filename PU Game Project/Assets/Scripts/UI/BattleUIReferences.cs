using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MHA.UserInterface
{
    public class BattleUIReferences : MonoBehaviour
    {
        public GameObject endTurnButton;
        public GameObject heroTurnIntro;
        public GameObject abilitySelection;

        public static BattleUIReferences instance;

        private void Awake()
        {
            instance = this;
        }
    }
}
