using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.BattleBehaviours
{
    public class BBActivator
    {

        public List<string> eventStringArray = new List<string>();

        public BBActivator()
        {
            foreach (string currentString in eventStringArray)
            {
                EventFlagChecker(currentString);
            }
        }

        private void RunPassiveEvents()
        {
            ResolutionManager.instance.LoadBattleBehaviour(passiveBBehaviourControllerList);
        }

        private void EventFlagChecker(string givenString)
        {
            switch (givenString)
            {
                case (EventFlags.TookDamageIdentifier):
                    EventFlags.tookDamage += this.RunPassiveEvents;
                    break;
            }
        }

        public List<BattleBehaviourModel> passiveBBehaviorModelList = new List<BattleBehaviourModel>();
        public List<BattleBehaviourController> passiveBBehaviourControllerList = new List<BattleBehaviourController>();
    }
}
