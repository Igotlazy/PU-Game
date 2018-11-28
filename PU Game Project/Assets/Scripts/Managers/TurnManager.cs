//Manages turn progression.

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
        if (Input.GetKeyDown(KeyCode.N))
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
                Debug.Log("Add Player");
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
}
