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

    public List<BattleAnimation> animationQueue = new List<BattleAnimation>();
    private int animationTracker;
    public bool animationResolutionRunning;
    public bool resolutionRunning;


    public int currentResolutionCalls = 0;
    public int maxResolutionPerFrame;
    public int totalCalls;

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
            totalCalls = 0;
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
            currentEffect = resolvingEffects[resolvingEffects.Count - 1];
            currentEffect.RunEffect();

            currentResolutionCalls++;
            totalCalls++;

            if (currentResolutionCalls > maxResolutionPerFrame)
            {
                currentResolutionCalls = 0;
                yield return null;
            }       
        }

        CharAbility.totalCastIndex = 0; //Resets individual cast tracker when Resolution empties. 
        effectResolutionRunning = false;

        animationTracker = -1;
        NextQueuedAnimation();
        Debug.LogError("ANIM RESOLUTION START");
    }

    TurnManager.BattlePhase originalBattlePhase;


    public void QueueAnimation(BattleAnimation givenAnimation)
    {
        animationQueue.Add(givenAnimation);
    }

    public void NextQueuedAnimation()
    {
        animationTracker++;
        if (animationTracker > animationQueue.Count - 1)
        {
            animationResolutionRunning = false;
            animationTracker = -1;
            animationQueue.Clear();
            ReturnToOriginalBattlePhase();
            Debug.LogError("ANIM RESOLUTION DONE");
        }
        else
        {
            animationResolutionRunning = true;
            animationQueue[animationTracker].PlayBattleAnimation();
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
