using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MHA.BattleBehaviours;

public class ResolutionManager : MonoBehaviour {

    public static ResolutionManager instance;

    public List<BattleBehaviourController> resolvingBehaviours = new List<BattleBehaviourController>();
    public BattleBehaviourController currentBehaviour;
    public bool eventResolutionRunning;

    private Queue<BattleAnimation> animationQueue = new Queue<BattleAnimation>();
    public bool animQueueRunning;


    public int currentResolutionCalls = 0;
    public int maxResolutionPerTurn;

    private void Awake()
    {
        if(instance == null){instance = this;}
    }


    void Start ()
    {
		
	}



    public void LoadBattleBehaviour(List<BattleBehaviourController> givenControllers)
    {
        if (eventResolutionRunning)
        {
            for (int i = givenControllers.Count - 1; i >= 0; i--)
            {
                resolvingBehaviours.Add(givenControllers[i]);
            }
        }
        else
        {
            resolvingBehaviours.Clear(); //Technically all of these should be clear, but just in case.

            for (int i = givenControllers.Count - 1; i >= 0; i--)
            {
                resolvingBehaviours.Add(givenControllers[i]);
            }

            currentResolutionCalls = 0;
            StartCoroutine(EffectResolution());
        }
    }

    public void LoadBattleBehaviour(BattleBehaviourController givenController)
    {
        if (eventResolutionRunning)
        {
            resolvingBehaviours.Add(givenController);
        }
        else
        {
            resolvingBehaviours.Clear(); //Technically this should be clear, but just in case.
            resolvingBehaviours.Add(givenController);

            currentResolutionCalls = 0;
            StartCoroutine(EffectResolution());
        }
    }


    public IEnumerator EffectResolution()
    {
        originalBattlePhase = TurnManager.instance.CurrentBattlePhase;

        TurnManager.instance.CurrentBattlePhase = TurnManager.BattlePhase.ActionPhase;
        eventResolutionRunning = true;

        while (resolvingBehaviours.Count > 0 )
        {
            currentBehaviour = resolvingBehaviours.Last();
            currentBehaviour.RunBehaviour();

            currentResolutionCalls++;

            if (currentResolutionCalls > maxResolutionPerTurn)
            {
                currentResolutionCalls = 0;
                yield return null;
            }
            
        }

        eventResolutionRunning = false;
    }

    TurnManager.BattlePhase originalBattlePhase;


    public void QueueAnimation(BattleAnimation givenAnimation)
    {
        animationQueue.Enqueue(givenAnimation);
        if (!animQueueRunning)
        {
            animQueueRunning = true;
            NextQueueAnimation();
        }
    }

    public void NextQueueAnimation()
    {
        if(animationQueue.Count > 0)
        {
            BattleAnimation currentAnim = animationQueue.Dequeue();
            currentAnim.PlayBattleAnimation();
        }
        else
        {
            animQueueRunning = false;
            TurnManager.instance.CurrentBattlePhase = originalBattlePhase;
        }
    }

}
