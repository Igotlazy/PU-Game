//General methods a Unit should have access too.

using UnityEngine;
using System.Collections.Generic;
using Cinemachine;
using MHA.BattleBehaviours;
using MHA.Events;

public class Unit : MonoBehaviour {

    public CharDataSO givenCharData;

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
        UnPackCharData();

        creatureScript = GetComponent<LivingCreature>();
        if(creatureScript == null)
        {
            Debug.LogWarning("WARNING: No LivingCreature Script on: " + gameObject.name);
        }
        ReferenceObjects.AddToPlayerList(this.gameObject);
    }

    void Start()
    {
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
        this.name = "Unit - " + givenCharData.heroName;
        if(spriteRigProper != null)
        {
            Destroy(spriteRigProper);
        }

        spriteRigProper = Instantiate(givenCharData.spriteRig, spriteRig.transform);

    }
    private GameObject spriteRigProper;

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
