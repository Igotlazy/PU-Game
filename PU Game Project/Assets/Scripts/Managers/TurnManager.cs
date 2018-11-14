//Manages turn progression.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq; //Gives List.Last
using MHA.UserInterface;
using MHA.GenericBehaviours;

public class TurnManager : MonoBehaviour {

    public enum BattlePhase
    {
        MatchStart,

        TurnStart,

        PlayerInput,

        EnemyInput,

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
            //Debug.Log("BattleStateSet: " + value);
            if (value != BattlePhase.ActionPhase) //Action Phase CANNOT be set directly. Must use the set up function as it requires the Resolve Group.
            {
                //put the stuff on the top back in here later. 
            }
        }
    }

    public List<GameObject> activePlayers = new List<GameObject>();
    public List<GameObject> finishedPlayers = new List<GameObject>();

    [Tooltip("True = Ally Team, False = Enemy Team")]
    public bool teamTracker;
    public int turnCounter = 1;

    public List<CinemachineVirtualCamera> unitCameraList = new List<CinemachineVirtualCamera>();
    public CinemachineVirtualCamera currentCamera;

    public static TurnManager instance;

    public List<BattleEvent> battleEventResolutionGroup = new List<BattleEvent>();
    public BattleEvent currentBattleEvent;

    public List<BattleBehaviourController> resolvingBehaviours = new List<BattleBehaviourController>();

    public bool eventResolutionRunning;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        foreach (GameObject currentPlayer in ReferenceObjects.UnitList)
        {
            unitCameraList.Add(currentPlayer.GetComponent<Unit>().unitCamera);
        }

        CurrentBattlePhase = BattlePhase.MatchStart;
    }

    void Update()
    {
        //DEBUG
        if (Input.GetKeyDown(KeyCode.Return))
        {
            NextMainBattlePhase();
        }
    }



    public void SetCameraTargetBasic(CinemachineVirtualCamera selectedCamera) //Controls Cinemachine camera movement between Units. Should honestly be on another script. 
    {
        foreach (CinemachineVirtualCamera vCam in unitCameraList)
        {
            if (vCam != selectedCamera)
            {
                vCam.Priority = 10;
            }
        }

        selectedCamera.Priority = 11;
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

            case (BattlePhase.EnemyInput):
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
                if (teamTracker)
                {
                    CurrentBattlePhase = BattlePhase.PlayerInput;
                }
                else
                {
                    CurrentBattlePhase = BattlePhase.EnemyInput;
                }
                break;

            case (BattlePhase.PlayerInput):
                CurrentBattlePhase = BattlePhase.TurnEnd;
                break;

            case (BattlePhase.EnemyInput):
                CurrentBattlePhase = BattlePhase.TurnEnd;
                break;

            case (BattlePhase.ActionPhase):
                if (teamTracker)
                {
                    CurrentBattlePhase = BattlePhase.PlayerInput;
                }
                else
                {
                    CurrentBattlePhase = BattlePhase.EnemyInput;
                }
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

        BattleUIReferences.instance.heroTurnIntro.GetComponent<Animator>().SetTrigger("Animate");

        yield return new WaitForSeconds(2f);

        NextMainBattlePhase();
    }
    public void TeamSwitch()
    {
        teamTracker = !teamTracker;

        if (teamTracker)
        {
            foreach (GameObject currentAlly in ReferenceObjects.UnitAllyList)
            {
                activePlayers.Add(currentAlly);
            }
        }
        else
        {
            foreach (GameObject currentEnemy in ReferenceObjects.UnitEnemyList)
            {
                activePlayers.Add(currentEnemy);
            }
        }

        finishedPlayers.Clear();
    }
    public void EnergyReset()
    {
        if (teamTracker)
        {
            foreach(GameObject currentAlly in ReferenceObjects.UnitAllyList)
            {
                LivingCreature creatureScript = currentAlly.GetComponent<LivingCreature>();
                creatureScript.CurrentEnergy = Mathf.RoundToInt(creatureScript.maxEnergy.Value); 
            }
        }
        else
        {
            foreach (GameObject currentEnemy in ReferenceObjects.UnitEnemyList)
            {
                LivingCreature creatureScript = currentEnemy.GetComponent<LivingCreature>();
                creatureScript.CurrentEnergy = (int)Mathf.RoundToInt(creatureScript.maxEnergy.Value + 0.5f); //Need to do + 0.5 in order to guarentee normal Rounding behavior. 
            }
        }
    }


    public void SetPlayerAsFinished(GameObject finishedPlayer) //Keeps track of when a player runs out of moves.
    {
        if (activePlayers.Contains(finishedPlayer))
        {
            activePlayers.Remove(finishedPlayer);
        }
        if (!finishedPlayers.Contains(finishedPlayer))
        {
            finishedPlayers.Add(finishedPlayer);
        }
        if(activePlayers.Count == 0)
        {
            Debug.Log("TURN END - ALL PLAYERS FINISHED");
        }
    }

    public IEnumerator TurnEndManager()
    {
        //Add Buff Resolution.

        if (!teamTracker)
        {
            turnCounter += 1;
        }

        yield return new WaitForSeconds(1f);
        NextMainBattlePhase();
    }

    public void EventResolutionReceiver(BattleEvent receivedEvent) //Takes in one BattleEvent.
    {
        if (eventResolutionRunning)
        {
            battleEventResolutionGroup.Add(receivedEvent);
        }
        else
        {
            battleEventResolutionGroup.Clear(); //Technically this should be clear, but just in case.
            battleEventResolutionGroup.Add(receivedEvent);

            CurrentBattlePhase = BattlePhase.ActionPhase;

            eventResolutionRunning = true;

            StartCoroutine(EffectResolutionTree());
        }
    }

    public void EventResolutionReceiver(List<BattleEvent> receivedEvents) //Takes in many. Use for buff resolution and other predetermined lists of Events.
    {
        if (eventResolutionRunning)
        {
            for (int i = receivedEvents.Count - 1; i >= 0; i--)
            {
                battleEventResolutionGroup.Add(receivedEvents[i]);
            }
        }
        else
        {          
            battleEventResolutionGroup.Clear(); //Technically all of these should be clear, but just in case.

            for (int i = receivedEvents.Count - 1; i >= 0; i--)
            {
                battleEventResolutionGroup.Add(receivedEvents[i]);
            }

            CurrentBattlePhase = BattlePhase.ActionPhase;

            eventResolutionRunning = true;

            StartCoroutine(EffectResolutionTree());
        }
    }

    private IEnumerator EffectResolutionTree() //Literally handles the ordering and resolution of every BattleEvent. 
    {
        Debug.Log("Size of Stack: " + battleEventResolutionGroup.Count);

        if (battleEventResolutionGroup.Count <= 0)
        {
            eventResolutionRunning = false;
            CurrentBattlePhase = BattlePhase.PlayerInput; //Create a function to handle finishing (currently just works through Action Phase, probably want it available for End and Start Phase).

            yield break; //End of Resolution Tree.
        }
        else
        {
            currentBattleEvent = battleEventResolutionGroup.Last();
        }

        if (currentBattleEvent.IsDirty)
        {
            Debug.Log("Resumed Event");
            currentBattleEvent.BattleEventResume();
        }
        else
        {
            Debug.Log("Run Event");
            currentBattleEvent.BattleEventRun();
        }

        yield return null; 
        //Wait until the current BattleEvent either finishes or is interrupted by a new Event on the stack.
        while(!currentBattleEvent.IsFinished && battleEventResolutionGroup.Last() == currentBattleEvent)
        {
            yield return null;
        }

        if (currentBattleEvent.IsFinished) //If Event HAS finished.
        {
            Debug.Log("Finished Event");
            battleEventResolutionGroup.Remove(currentBattleEvent); //Removes the last BattleEvent in the list. Allows resolution to work like a Stack. 
  
            StartCoroutine(EffectResolutionTree());
        }
        if(!currentBattleEvent.IsFinished && battleEventResolutionGroup.Last() != currentBattleEvent) //If the Event HASN'T finished and IT ISN'T at the top of the "Stack".
        {
            Debug.Log("Paused Event");
            currentBattleEvent.BattleEventPause();

            StartCoroutine(EffectResolutionTree());
        }

            //Needs to be a check where if the current running BattleEvent is not on the top of the stack. If so, pause that BattleEvent and start the one on top. 
            //isFinished to represent it being done.
            //isDirty to represent if it's already started and needs to be resumed as opposed to started fresh.
    }
}
