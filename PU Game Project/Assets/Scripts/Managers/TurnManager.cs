//Manages turn progression.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq; //Gives List.Last

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
            BattlePhaseChange(currentBattlePhase);
            Debug.Log("BattleStateSet: " + value);
            if (value != BattlePhase.ActionPhase) //Action Phase CANNOT be set directly. Must use the set up function as it requires the Resolve Group.
            {
                //put the stuff on the top back in here later. 
            }
        }
    }

    public List<GameObject> activePlayers = new List<GameObject>();
    public List<GameObject> finishedPlayers = new List<GameObject>();
    public int activeSizeCount;
    private bool teamTracker;
    public int turnCounter = 1;

    public List<CinemachineVirtualCamera> unitCameraList = new List<CinemachineVirtualCamera>();
    public CinemachineVirtualCamera currentCamera;

    public static TurnManager instance;
    
    public List<List<BattleEvent>> battleResolveBranches = new List<List<BattleEvent>>();
    public List<BattleEvent> currentResolveBranch = new List<BattleEvent>();
    private BattleEvent currentBattleEvent; 
    public BattleEvent CurrentBattleEvent { get { return currentBattleEvent; } } //For other things to be able to know what's currently being resolved. 

    public List<BattleEvent> battleResolveAddList = new List<BattleEvent>();

    public bool eventResolutionRunning;


    private void Awake()
    {
        instance = this;
    }

    void Start ()
    {
        foreach(GameObject currentPlayer in ReferenceObjects.UnitList)
        {
            unitCameraList.Add(currentPlayer.GetComponent<Unit>().unitCamera);
        }

        CurrentBattlePhase = BattlePhase.MatchStart;
	}
	
	void Update ()
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
            if(vCam != selectedCamera)
            {
                vCam.Priority = 10;
            }
        }

        selectedCamera.Priority = 11;
    }



    public void BattlePhaseChange(BattlePhase chosenState) //Skeleton for method calls upon specific Phase change. 
    {
        switch (chosenState)
        {
            case (BattlePhase.MatchStart):
                break;

            case (BattlePhase.TurnStart):
                StartStateSetup();
                break;

            case (BattlePhase.PlayerInput):
                break;

            case (BattlePhase.EnemyInput):
                break;

            case (BattlePhase.ActionPhase):

                break;

            case (BattlePhase.TurnEnd):
                break;
        }
    }


    public void StartStateSetup() 
    {
        TeamSwitch();
    }

    public void TeamSwitch()
    {
        teamTracker = !teamTracker;

        if (teamTracker)
        {
            activePlayers = ReferenceObjects.UnitAllyList;
        }
        else
        {
            activePlayers = ReferenceObjects.UnitEnemyList;
        }

        finishedPlayers.Clear();
        activeSizeCount = activePlayers.Count;
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
        if(finishedPlayers.Count == activeSizeCount)
        {
            Debug.Log("TURN END - ALL PLAYERS FINISHED");
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


    public void EventResolutionReceiver(BattleEvent receivedEvent) //Takes in one BattleEvent.
    {
        if (eventResolutionRunning)
        {
            battleResolveAddList.Add(receivedEvent);
        }
        else
        {
            battleResolveAddList.Clear(); //Technically all of these should be clear, but just in case.
            currentResolveBranch.Clear();
            battleResolveBranches.Clear();
            battleResolveBranches.Add(new List<BattleEvent> {receivedEvent});

            CurrentBattlePhase = BattlePhase.ActionPhase;

            eventResolutionRunning = true;

            StartCoroutine(EffectResolutionTree());
        }
    }

    public void EventResolutionReceiver(List<BattleEvent> receivedEvents) //Takes in many. Use for buff resolution and other predetermined lists of Events.
    {
        if (eventResolutionRunning)
        {
            foreach(BattleEvent currentEvent in receivedEvents)
            {
                battleResolveAddList.Add(currentEvent);
            }
        }
        else
        {
            battleResolveAddList.Clear(); //Technically all of these should be clear, but just in case.
            currentResolveBranch.Clear();
            battleResolveBranches.Clear();
            battleResolveBranches.Add(receivedEvents);

            CurrentBattlePhase = BattlePhase.ActionPhase;

            eventResolutionRunning = true;

            StartCoroutine(EffectResolutionTree());
        }
    }

    private IEnumerator EffectResolutionTree() //Literally handles the ordering and resolution of every BattleEvent. 
    {   
        if(battleResolveBranches.Count <= 0)
        {
            if (currentResolveBranch.Count <= 0)
            {
                if (battleResolveBranches.Count <= 0)
                {
                    eventResolutionRunning = false;
                    CurrentBattlePhase = BattlePhase.PlayerInput; //Create a function to handle finishing (currently just works through Action Phase, probably want it available for End and Start Phase).

                    yield break; //End of Resolution Tree.
                }
                else
                {
                    currentResolveBranch = battleResolveBranches[0];
                    currentBattleEvent = currentResolveBranch.Last();
                }
            }
            else
            {
                currentBattleEvent = currentResolveBranch.Last();
            }
        }
        else
        {
            currentResolveBranch = battleResolveBranches[0];
            currentBattleEvent = currentResolveBranch.Last();
        }


        if (currentBattleEvent.IsDirty)
        {
            currentBattleEvent.BattleEventResume();
        }
        else
        {
            currentBattleEvent.BattleEventRun();
        }

        yield return null; //Allows Events to react to instantenous Events by being able to load on before it gets Popped out of the stack. 

        //Wait until the current BattleEvent either finishes or is interrupted by a new Event on the stack.
        while (!currentBattleEvent.IsFinished && battleResolveBranches[0].Last() == currentBattleEvent)
        {
            if(battleResolveAddList.Count > 0)
            {
                Debug.Log("ADD SIZE: " + battleResolveAddList.Count);
                for(int i = battleResolveAddList.Count - 1; i >= 0; i--)
                {
                    if(i == 0)
                    {
                        currentResolveBranch.Add(battleResolveAddList[i]); //Puts the last one to react into the current branch.
                    }
                    else
                    {
                        battleResolveBranches.Insert(0, new List<BattleEvent> { battleResolveAddList[i]}); //Puts the others in order of who reacted first into new Branches at the top.
                    }
                }
                battleResolveAddList.Clear();
            }
            yield return null; //Allows Events to react midway through other Events. 
        }

        if (currentBattleEvent.IsFinished) //If Event HAS finished.
        {
            currentResolveBranch.Remove(currentBattleEvent); //Basically removes the last BattleEvent in the list. Allows resolution within a branch to work like a Stack. 

            if(currentResolveBranch.Count <= 0) 
            {
                battleResolveBranches.RemoveAt(0); //Removes the first Branch in the branch list. Allows resolution between branches to work like a Queue. 
            }
  
            StartCoroutine(EffectResolutionTree());
        }
        if(!currentBattleEvent.IsFinished && battleResolveBranches[0].Last() != currentBattleEvent) //If the Event HASN'T finished and IT ISN'T at the top of the "Stack".
        {
            currentBattleEvent.BattleEventPause();

            StartCoroutine(EffectResolutionTree());
        }

            //Needs to be a check where if the current running BattleEvent is not on the top of the stack. If so, pause that BattleEvent and start the one on top. 
            //isFinished to represent it being done.
            //isDirty to represent if it's already started and needs to be resumed as opposed to started fresh.
    }
    
}
