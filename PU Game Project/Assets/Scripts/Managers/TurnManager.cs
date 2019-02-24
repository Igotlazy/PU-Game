//Manages turn progression.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq; //Gives List.Last
using MHA.UserInterface;
using MHA.BattleBehaviours;

public class TurnManager : MonoBehaviour {

    public enum BattlePhase
    {
        MatchStart,
        TurnStart,
        PlayerInput,
        AIInput,
        ActionPhase,
        TurnEnd,
    }

    [SerializeField]
    private BattlePhase currentBattlePhase;
    public BattlePhase CurrentBattlePhase
    {
        get
        {
            return currentBattlePhase;
        }
        set
        {
            currentBattlePhase = value;
            BattlePhaseInitiation(currentBattlePhase);

            if(BattlePhaseResponseEVENT != null)
            {
                BattlePhaseResponseEVENT(currentBattlePhase);
            }
        }
    }
    public delegate void BattlePhaseResponse(BattlePhase currentPhase);
    public event BattlePhaseResponse BattlePhaseResponseEVENT;

    public List<GameObject> activeUnits = new List<GameObject>();
    public List<GameObject> finishedPlayers = new List<GameObject>();

    public Unit.Teams teamTracker;
    public int turnCounter = 1;

    public static TurnManager instance;


    private void Awake()
    {
        if(instance == null){ instance = this;} else{ Destroy(this);}
    }

    void Start()
    {
        CurrentBattlePhase = BattlePhase.MatchStart;
    }

    void Update()
    {
        //DEBUG
        if (Input.GetKeyDown(KeyCode.N))
        {
            NextMainBattlePhase();
        }
    }



    public void BattlePhaseInitiation(BattlePhase chosenState) //Skeleton for method calls upon specific Phase change. 
    {
        switch (chosenState)
        {
            case (BattlePhase.MatchStart):
                StartCoroutine(MatchStartManager());
                break;

            case (BattlePhase.TurnStart):
                StartCoroutine(TurnStartManager());
                break;

            case (BattlePhase.PlayerInput):
                break;

            case (BattlePhase.AIInput):
                Debug.Log("Requesting AI Input");
                StartCoroutine(TurnAIControl());
                break;

            case (BattlePhase.ActionPhase):
                break;

            case (BattlePhase.TurnEnd):
                StartCoroutine(TurnEndManager());
                break;
        }
    }

    public void NextMainBattlePhase() //Progresses through the Main Battle Phases. Action Phase is a separate branch coming off and then returns to the Input Phases.
    {
        switch (CurrentBattlePhase)
        {
            case (BattlePhase.MatchStart):
                CurrentBattlePhase = BattlePhase.TurnStart;
                break;

            case (BattlePhase.TurnStart):
                CurrentBattlePhase = BattlePhase.PlayerInput;
                break;

            case (BattlePhase.PlayerInput):
                CurrentBattlePhase = BattlePhase.AIInput;
                break;

            case (BattlePhase.AIInput):
                CurrentBattlePhase = BattlePhase.TurnEnd;
                break;

            case (BattlePhase.ActionPhase):
                CurrentBattlePhase = BattlePhase.PlayerInput;
                break;

            case (BattlePhase.TurnEnd):
                CurrentBattlePhase = BattlePhase.TurnStart;
                break;
        }
    }

    public IEnumerator MatchStartManager()
    {
        yield return new WaitForSeconds(1f);
        NextMainBattlePhase();
    }


    public IEnumerator TurnStartManager()
    {
        TeamSwitch();
        EnergyReset();
        CooldownDrop();

        BattleUIReferences.instance.heroTurnIntro.GetComponent<Animator>().SetTrigger("Animate");

        yield return new WaitForSeconds(2f);

        if(teamTracker == Unit.Teams.Hero)
        {
            NextMainBattlePhase();
        }
        else
        {
            CurrentBattlePhase = BattlePhase.AIInput;
        }
    }
    public void TeamSwitch()
    {
        activeUnits.Clear();

        if(teamTracker == Unit.Teams.Hero)
        {
            foreach (GameObject currentAlly in ReferenceObjects.HeroList)
            {
                activeUnits.Add(currentAlly);
            }
        }
        else if (teamTracker == Unit.Teams.Villain)
        {
            foreach (GameObject currentAlly in ReferenceObjects.VillainList)
            {
                activeUnits.Add(currentAlly);
            }

        }
        else if(teamTracker == Unit.Teams.Vigilante)
        {
            foreach (GameObject currentAlly in ReferenceObjects.VigilanteList)
            {
                activeUnits.Add(currentAlly);
            }
        }


        finishedPlayers.Clear();
    }
    public void EnergyReset()
    {
        foreach (GameObject currentUnit in activeUnits)
        {
            Unit creatureScript = currentUnit.GetComponent<Unit>();
            creatureScript.CurrentEnergy = Mathf.RoundToInt(creatureScript.maxEnergy.Value);
        }
    }
    public void CooldownDrop()
    {
        foreach(GameObject currentUnit in activeUnits)
        {
            Unit unitScript = currentUnit.GetComponent<Unit>();
            foreach(CharAbility currAbility in unitScript.activatableAbilitiesInsta)
            {
                currAbility.CooldownDecrease();
            }
            foreach (CharAbility currAbility in unitScript.movementAbilitiesInsta)
            {
                currAbility.CooldownDecrease();
            }
            foreach (CharAbility currAbility in unitScript.passiveAbilitiesInsta)
            {
                currAbility.CooldownDecrease();
            }
        }
    }


    public void SetPlayerAsFinished(GameObject finishedPlayer) //Keeps track of when a player runs out of moves.
    {
        if (activeUnits.Contains(finishedPlayer))
        {
            activeUnits.Remove(finishedPlayer);
        }
        if (!finishedPlayers.Contains(finishedPlayer))
        {
            finishedPlayers.Add(finishedPlayer);
        }
        if(activeUnits.Count == 0)
        {
            Debug.Log("TURN END - ALL PLAYERS FINISHED");
        }
    }

    public IEnumerator TurnAIControl()
    {
        Debug.Log("Turn AI START");

        yield return null; //Phase change cleared selector without this.

        List<Unit> aiUnits = new List<Unit>();
        foreach (GameObject obj in activeUnits)
        {
            Unit unitScript = obj.GetComponent<Unit>();
            if(unitScript != null && unitScript.isAIControlled)
            {
                aiUnits.Add(unitScript);
            }
        }

        foreach(Unit currUnit in aiUnits)
        {
            yield return StartCoroutine(currUnit.AIResponder());
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("Turn AI END");
        NextMainBattlePhase();
    }

    public IEnumerator TurnEndManager()
    {
        //Add Buff Resolution.
        yield return new WaitForSeconds(1f);

        EndTurnBuffHandling();
        yield return null;
        yield return new WaitUntil(() => !ResolutionManager.instance.resolutionRunning);

        switch (teamTracker)
        {
            case (Unit.Teams.Hero):
                teamTracker = Unit.Teams.Villain;
                break;

            case (Unit.Teams.Villain):
                if (ReferenceObjects.VigilanteList.Count > 0)
                {
                    teamTracker = Unit.Teams.Vigilante;
                }
                else
                {
                    teamTracker = Unit.Teams.Hero;
                    turnCounter++;
                }
                break;

            case (Unit.Teams.Vigilante):
                teamTracker = Unit.Teams.Hero;
                turnCounter++;
                break;
        }

        NextMainBattlePhase();
    }

    private void EndTurnBuffHandling()
    {
        AddBuffsToMainList();

        EffectCustomAction callEndBuffs = new EffectCustomAction(null, new Action(CallEndBuffs));
        EffectCustomAction callCooldownBuffs = new EffectCustomAction(null, new Action(CallBuffCooldown));
        EffectCustomAction callRemoveBuffs = new EffectCustomAction(null, new Action(RemoveBuffs));

        List<BattleEffect> effects = new List<BattleEffect>() { callEndBuffs, callCooldownBuffs, callRemoveBuffs };
        ResolutionManager.instance.LoadBattleEffect(effects);
    }

    public void AddBuffsToMainList()
    {
        Debug.Log("Hello?");
        foreach(GameObject currentUnit in activeUnits)
        {
            Unit unitScript = currentUnit.GetComponent<Unit>();
            foreach(Buff currentBuff in unitScript.AddBuffList)
            {
                unitScript.BuffList.Add(currentBuff);
            }
            unitScript.AddBuffList.Clear();
        }
    }
    public void CallEndBuffs()
    {
        foreach (GameObject currentUnit in activeUnits)
        {
            Unit unitScript = currentUnit.GetComponent<Unit>();
            foreach (Buff currentBuff in unitScript.BuffList)
            {
                currentBuff.BuffEndTurnApplication();
            }
        }
    }
    public void CallBuffCooldown()
    {
        foreach (GameObject currentUnit in activeUnits)
        {
            Unit unitScript = currentUnit.GetComponent<Unit>();
            foreach (Buff currentBuff in unitScript.BuffList)
            {
                currentBuff.CooldownReduce();
            }
        }
    }
    public void RemoveBuffs()
    {
        foreach (GameObject currentUnit in activeUnits)
        {
            Unit unitScript = currentUnit.GetComponent<Unit>();
            foreach (Buff buffToRemove in unitScript.RemoveBuffList)
            {
                if (unitScript.BuffList.Contains(buffToRemove))
                {
                    unitScript.BuffList.Remove(buffToRemove);
                }
            }
            unitScript.RemoveBuffList.Clear();
        }
    }
}
