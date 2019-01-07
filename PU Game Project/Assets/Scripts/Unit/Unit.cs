//General methods a Unit should have access too.

using UnityEngine;
using System.Collections.Generic;
using Cinemachine;
using MHA.BattleBehaviours;
using MHA.Events;

public class Unit : MonoBehaviour {

    [Header("CHARACTER DATA:")]
    [SerializeField]
    private CharDataSO givenCharData;
    [Space]

    [Header("Abilities")]
    [SerializeField]
    private List<CharAbility> movementAbilities = new List<CharAbility>();
    [SerializeField]
    private List<CharAbility> passiveAbilities = new List<CharAbility>();
    [SerializeField]
    private List<CharAbility> activatableAbilities = new List<CharAbility>();
    [Space]

    [HideInInspector]
    public List<CharAbility> movementAbilitiesInsta = new List<CharAbility>();
    [HideInInspector]
    public List<CharAbility> passiveAbilitiesInsta = new List<CharAbility>();
    [HideInInspector]
    public List<CharAbility> activatableAbilitiesInsta = new List<CharAbility>();
    [HideInInspector]

    [Header("Other")]    
    public Node currentNode;
    public int teamValue;


    [Space]
    [Header("SHOT RESPONDERS:")]
    public GameObject shotConnecter;
    public GameObject partialCoverCheck;
    public Transform centerPoint;

    [Space]
    [Header("REFERENCES:")]
    [SerializeField]
    private GameObject spriteRig;
    public GameObject SpriteRig
    {
        get
        {
            return spriteRig;
        }
    }
    private LivingCreature creatureScript;
    public LivingCreature CreatureScript
    {
        get
        {
            return creatureScript;
        }
    }

    private void Awake()
    {
        creatureScript = GetComponent<LivingCreature>();
        if(creatureScript == null)
        {
            Debug.LogWarning("WARNING: No LivingCreature Script on: " + gameObject.name);
        }
        ReferenceObjects.AddToPlayerList(this.gameObject);
    }

    void Start()
    {
        UnPackCharData();
        PrepAbilities(out movementAbilitiesInsta, out passiveAbilitiesInsta, out activatableAbilitiesInsta);
        creatureScript.LoadStatData(givenCharData);
        creatureScript.healthBar.UpdateHealth(creatureScript.currentHealth, creatureScript.maxHealth.Value);
        StartNodeFind();

        EventFlags.StartPeek += UnitPeekAnim;
        EventFlags.EndPeek += UnitUnPeekAnim;
    }

    private void OnDestroy()
    {
        EventFlags.StartPeek -= UnitPeekAnim;
        EventFlags.EndPeek -= UnitUnPeekAnim;
    }

    private void UnPackCharData()
    { 
        
    }

    public void PrepAbilities(out List<CharAbility> movementAb, out List<CharAbility> passiveAb, out List<CharAbility> activatableAb)
    {
        movementAb = new List<CharAbility>();
        passiveAb = new List<CharAbility>();
        activatableAb = new List<CharAbility>();
        foreach (CharAbility currentChar in movementAbilities)
        {
            CharAbility newOne = Instantiate(currentChar);
            movementAb.Add(newOne);
            newOne.Initialize(this);
        }
        foreach (CharAbility currentChar in passiveAbilities)
        {
            CharAbility newOne = Instantiate(currentChar);
            passiveAb.Add(newOne);
            newOne.Initialize(this);
        }
        foreach (CharAbility currentChar in activatableAbilities)
        {
            CharAbility newOne = Instantiate(currentChar);
            activatableAb.Add(newOne);
            newOne.Initialize(this);
        }
    }


    /*
    private void OnValidate()
    {
        if(givenCharData != null)
        {
            UnPackCharData();
        }
        else
        {
            Debug.Log("Hello?");
            this.name = "Unit - Model";
            if (spriteRigProper != null)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(spriteRigProper);
                };
            }
        }
    }
    */

    private void StartNodeFind() //Gives the unit reference to the Node below it.
    {
        Node foundNode = GridGen.instance.NodeFromWorldPoint(transform.position);
        if(foundNode == null)
        {
            Debug.Log("WARNING: Unit was not able to initialize itself to the Grid");
        }
        currentNode = foundNode;
        if (foundNode.IsWalkable)
        {
            foundNode.IsOccupied = true;
            foundNode.occupant = this.gameObject;
        }
    }

    public void UnitPeekAnim(object sender, EventFlags.EPeekStart peekArgs)
    {
        if (peekArgs.peekingObject == this)
        {
            Debug.Log("Peek: " + peekArgs.peekingObject.name);
            peekArgs.peekPosition = new Vector3(peekArgs.peekPosition.x, spriteRig.transform.localPosition.y, peekArgs.peekPosition.z);

            new AnimMoveToPos(peekArgs.peekPosition, spriteRig, 3f, false);
            peekArgs.AddToPeek();
        }
    }

    public void UnitUnPeekAnim(object sender, EventFlags.EPeekEnd peekArgs)
    {
        if (peekArgs.peekingObject == this)
        {
            Debug.Log("UnPeek: " + peekArgs.peekingObject.name);
            new AnimMoveToPos(peekArgs.GiveOriginalPos(), spriteRig, 3f, false);
            peekArgs.RemoveFromPeek();
        }
    }
}
