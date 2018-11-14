using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.GenericBehaviours
{
    [System.Serializable]
    public abstract class GBBase
    {

        public BattleEvent attachedBattleEvent;

        protected GBBase(BattleEvent _attachedBattleEvent)
        {
            attachedBattleEvent = _attachedBattleEvent;
        }

        /*

        public IEnumerator RunBehaviour()
        {
            AddToBehaviourList();
            yield return attachedBattleEvent.bEventMonoBehaviour.StartCoroutine(RunBehaviourImpl());
        }
        protected abstract IEnumerator RunBehaviourImpl();

        public void CancelBehaviour()
        {

            attachedBattleEvent.bEventMonoBehaviour.StopCoroutine(RunBehaviourImpl());
            RemoveFromBehaviourList();
        }

        protected void FinishBehaviour()
        {
            RemoveFromBehaviourList();
        }

        private void AddToBehaviourList()
        {
            TurnManager.instance.resolvingBehaviours.Add(this);
        }
        private void RemoveFromBehaviourList()
        {
            if (TurnManager.instance.resolvingBehaviours.Contains(this))
            {
                TurnManager.instance.resolvingBehaviours.Remove(this);
            }
        }
        */

    }

}
