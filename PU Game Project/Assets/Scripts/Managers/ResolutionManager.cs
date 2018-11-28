using MHA.BattleBehaviours;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResolutionManager : MonoBehaviour {

    public static ResolutionManager instance;

    public List<BattleEffect> resolvingEffects = new List<BattleEffect>();
    public BattleEffect currentEffect;
    public bool eventResolutionRunning;

    private Queue<BattleAnimation> animationQueue = new Queue<BattleAnimation>();
    public bool animQueueRunning;


    public int currentResolutionCalls = 0;
    public int maxResolutionPerTurn;

    private void Awake()
    {
        if(instance == null){instance = this; } else { Destroy(this); }
    }


    void Start ()
    {
		
	}



    public void LoadBattleEffect(List<BattleEffect> givenEffects)
    {
        if (givenEffects.Count > 0)
        {
            Debug.Log(givenEffects.Count);
            if (eventResolutionRunning)
            {
                for (int i = givenEffects.Count - 1; i >= 0; i--)
                {
                    resolvingEffects.Add(givenEffects[i]);
                }
            }
            else
            {
                resolvingEffects.Clear(); //Technically all of these should be clear, but just in case.

                for (int i = givenEffects.Count - 1; i >= 0; i--)
                {
                    resolvingEffects.Add(givenEffects[i]);
                }

                currentResolutionCalls = 0;
                eventResolutionRunning = true;
                StartCoroutine(EffectResolution());
            }
        }
    }

    public void LoadBattleEffect(BattleEffect givenEffect)
    {
        if (eventResolutionRunning)
        {
            resolvingEffects.Add(givenEffect);
        }
        else
        {
            resolvingEffects.Clear(); //Technically this should be clear, but just in case.
            resolvingEffects.Add(givenEffect);

            currentResolutionCalls = 0;
            StartCoroutine(EffectResolution());
        }
    }


    public IEnumerator EffectResolution()
    {
        originalBattlePhase = TurnManager.instance.CurrentBattlePhase;
        eventResolutionRunning = true;

        TurnManager.instance.CurrentBattlePhase = TurnManager.BattlePhase.ActionPhase;

        while (resolvingEffects.Count > 0 )
        {
            currentEffect = resolvingEffects.Last();
            currentEffect.RunEffect();
            Debug.Log("Effect Ran");

            currentResolutionCalls++;

            if (currentResolutionCalls > maxResolutionPerTurn)
            {
                currentResolutionCalls = 0;
                yield return null;
            }
            
        }

        CharAbility.charIndex = 0; //Resets individual cast tracker when Resolution empties. 
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
            Debug.Log("Return to Input");
            animQueueRunning = false;
            TurnManager.instance.CurrentBattlePhase = originalBattlePhase;
            Debug.Log(originalBattlePhase.ToString());
        }
    }

}
