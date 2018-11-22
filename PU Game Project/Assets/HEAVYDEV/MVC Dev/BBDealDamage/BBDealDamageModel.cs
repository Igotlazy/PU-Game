using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.BattleBehaviours {
    [System.Serializable]
    public class BBDealDamageModel : BattleBehaviourModel {

        public BBDealDamageModel()
        {
            identifierString = "DealDamage";
        }

        public Attack attackToDeal;
        public GameObject targetObject;


    }
}
