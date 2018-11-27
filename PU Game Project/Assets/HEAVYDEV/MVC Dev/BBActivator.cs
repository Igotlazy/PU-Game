using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MHA.Events;

namespace MHA.BattleBehaviours
{
    public class BBActivator
    {
        public EventArgs eventArgs;

        public string eventString;


        public BBActivator()
        {
            EventFlagChecker(eventString);
        }


        private void RunPassiveEvents(object source, EventArgs givenArgs)
        {
            eventArgs = givenArgs;
            //ResolutionManager.instance.LoadBattleBehaviour(passiveBBehaviourControllerList);
        }

        private void EventFlagChecker(string givenString)
        {
            switch (givenString)
            {
                case (EventFlags.TookDamageIdentifier):
                    EventFlags.TookDamage += this.RunPassiveEvents;
                    break;
            }
        }

        //public List<BattleBehaviourModel> passiveBBehaviorModelList = new List<BattleBehaviourModel>();
        //public List<BattleBehaviourController> passiveBBehaviourControllerList = new List<BattleBehaviourController>();
    }
}
