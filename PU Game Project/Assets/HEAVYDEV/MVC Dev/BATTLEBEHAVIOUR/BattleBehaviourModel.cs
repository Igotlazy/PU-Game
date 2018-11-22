using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MHA.BattleBehaviours
{
    [System.Serializable]
    public abstract class BattleBehaviourModel
    {
        public string identifierString;

        public CharAbilityModel associatedCharAbilityModel;

        public List<BattleBehaviourModel> auxBehaviourModels = new List<BattleBehaviourModel>();
        public List<BattleBehaviourController> auxBehaviourControllers = new List<BattleBehaviourController>();
    }
}
