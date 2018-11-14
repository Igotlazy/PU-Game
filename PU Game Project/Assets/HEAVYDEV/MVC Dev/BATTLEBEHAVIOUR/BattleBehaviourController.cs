using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleBehaviourController : IBattleBehaviour {

    public CharAbilityController charAbilityController;
    public BattleEvent attachedBattleEvent;



    public IEnumerator RunBehaviour()
    {
        AddToBehaviourList();
        yield return attachedBattleEvent.bEventMonoBehaviour.StartCoroutine(RunBehaviourImpl());
    }
    protected abstract IEnumerator RunBehaviourImpl();

    public void CancelBehaviour()
    {
        TurnManager.instance.StopCoroutine(RunBehaviourImpl());
        RemoveFromBehaviourList();
    }
    protected abstract void CancelBehaviourImpl();

    public void FinishBehaviour()
    {
        RemoveFromBehaviourList();
    }
    protected abstract void FinishBehaviourImpl();

    public void AddToBehaviourList()
    {
        TurnManager.instance.resolvingBehaviours.Add(this);
    }

    public void RemoveFromBehaviourList()
    {
        if (TurnManager.instance.resolvingBehaviours.Contains(this))
        {
            TurnManager.instance.resolvingBehaviours.Remove(this);
        }
    }
} 



