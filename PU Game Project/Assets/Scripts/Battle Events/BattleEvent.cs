//Defines some Event to be resolved by the EffectResolutionTree on TurnManager. Any action (Attacks, Movements, Item use etc.) is derived from this class. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleEvent : IBattleEventControllable {

    protected MonoBehaviour bEventMonoBehaviour;

    private bool isFinished; //Defines whether the Event has completed.
    public bool IsFinished
    {
        get
        {
            return isFinished;
        }
        set
        {
            isFinished = value;
        }
    }
    private bool isDirty; //Defines whether the Event has been visited before.
    public bool IsDirty
    {
        get
        {
            return isDirty;
        }
        set
        {
            isDirty = value;
        }
    }
    private bool isPaused; //Defines whether the Event is currently paused and not running. 
    public bool IsPaused
    {
        get
        {
            return isPaused;
        }
        set
        {
            isPaused = value;
        }
    }

    //Gets reference to the TurnManager monobehavior. Need a monobehavior to call Coroutines. If either TurnManager or the GameObject that holds it is destroyed, this system won't work.
    public BattleEvent() 
    {
        this.bEventMonoBehaviour = TurnManager.instance;
    }





    public virtual void BattleEventRun()
    {
        IsDirty = true;
        BattleEventRunImpl();
    }
    protected abstract void BattleEventRunImpl();


    public virtual void BattleEventPause()
    {
        IsPaused = true;
        BattleEventPauseImpl();
    }  
    protected abstract void BattleEventPauseImpl();


    public virtual void BattleEventResume()
    {
        IsPaused = false;
        BattleEventResumeImpl();
    }
    protected abstract void BattleEventResumeImpl();


    public virtual void BattleEventCancel()
    {
        BattleEventCancelImpl();
    }
    protected abstract void BattleEventCancelImpl();






}
