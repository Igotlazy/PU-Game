using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.GenericBehaviours
{
    public class GBBase
    {

        protected BattleEvent attachedBattleEvent;
        public bool behaviourDone;

        protected GBBase(BattleEvent _attachedBattleEvent)
        {
            attachedBattleEvent = _attachedBattleEvent;
        }

    }
}
