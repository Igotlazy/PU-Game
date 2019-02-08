using MHA.BattleBehaviours;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResolutionManager : MonoBehaviour {

    public static ResolutionManager instance;

    public List<BattleEffect> resolvingEffects = new List<BattleEffect>();
    public BattleEffect currentEffect;
    public bool effectResolutionRunning;

    public Queue<BattleAnimation> animationQueue = new Queue<BattleAnimation>();
    public bool animationResolutionRunning;
    public bool resolutionRunning;


    public int currentResolutionCalls = 0;
    public int maxResolutionPerFrame;

    private void Awake()
    {
        if(instance == null){instance = this; } else { Destroy(gameObject); }
    }


    void Start ()
    {
		
	}



    public void LoadBattleEffect(List<BattleEffect> givenEffects)
    {
        if (givenEffects.Count > 0)
        {
            if (effectResolutionRunning)
            {
                for (int i = givenEffects.Count - 1; i >= 0; i--)
                {
                    resolvingEffects.Add(givenEffects[i]);
                }
            }
            else
            {
                Debug.LogError("RESOLUTION START");
                resolvingEffects.Clear(); //Technically all of these should be clear, but just in case.

                for (int i = givenEffects.Count - 1; i >= 0; i--)
                {
                    resolvingEffects.Add(givenEffects[i]);
                }

                if (animationResolutionRunning)
                {
                    Debug.LogError("WARNING: ADDITION OF EFFECTS DURING ANIMATIONS. DID YOU NOT PROPERLY LOAD BATTLE EFFECTS?");
                }

                currentResolutionCalls = 0;
                StartCoroutine(EffectResolution());
            }
        }
    }

    public void LoadBattleEffect(BattleEffect givenEffect)
    {
        if (effectResolutionRunning)
        {
            resolvingEffects.Add(givenEffect);
        }
        else
        {
            Debug.LogError("RESOLUTION START");
            resolvingEffects.Clear(); //Technically this should be clear, but just in case.
            resolvingEffects.Add(givenEffect);

            if (animationResolutionRunning)
            {
                Debug.LogError("WARNING: ADDITION OF EFFECTS DURING ANIMATIONS. DID YOU NOT PROPERLY LOAD BATTLE EFFECTS?");
            }

            currentResolutionCalls = 0;
            StartCoroutine(EffectResolution());
        }
    }


    public IEnumerator EffectResolution()
    {
        originalBattlePhase = TurnManager.instance.CurrentBattlePhase;
        resolutionRunning = true;
        effectResolutionRunning = true;

        if(originalBattlePhase == TurnManager.BattlePhase.PlayerInput)
        {
            TurnManager.instance.CurrentBattlePhase = TurnManager.BattlePhase.ActionPhase;
        }

        while (resolvingEffects.Count > 0 )
        {
            currentEffect = resolvingEffects.Last();
            currentEffect.RunEffect();

            currentResolutionCalls++;

            if (currentResolutionCalls > maxResolutionPerFrame)
            {
                currentResolutionCalls = 0;
                yield return null;
            }       
        }

        CharAbility.totalCastIndex = 0; //Resets individual cast tracker when Resolution empties. 
        effectResolutionRunning = false;

        NextQueuedAnimation();
    }

    TurnManager.BattlePhase originalBattlePhase;


    public void QueueAnimation(BattleAnimation givenAnimation)
    {
        animationQueue.Enqueue(givenAnimation);
        /*
        if (!animQueueRunning)
        {
            animQueueRunning = true;
            NextQueueAnimation();
        }
        */
    }

    public void NextQueuedAnimation()
    {
        if(animationQueue.Count > 0)
        {
            animationResolutionRunning = true;
            Debug.LogWarning("PLAYED ANIM");
            animationQueue.Dequeue().PlayBattleAnimation();
        }
        else
        {
            animationResolutionRunning = false;
            ReturnToOriginalBattlePhase();
            Debug.LogError("ANIM RESOLUTION DONE");
        }
    }

    private void ReturnToOriginalBattlePhase()
    {
        if(!animationResolutionRunning && !effectResolutionRunning)
        {
            if(originalBattlePhase == TurnManager.BattlePhase.PlayerInput)
            {
                TurnManager.instance.CurrentBattlePhase = originalBattlePhase;
            }
            resolutionRunning = false;
        }
    }

}
