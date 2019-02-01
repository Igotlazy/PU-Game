//General methods a Unit should have access too.

using UnityEngine;
using System.Collections.Generic;
using Cinemachine;
using MHA.BattleBehaviours;
using MHA.Events;
using System.Collections;

public class Unit : MonoBehaviour {

    [Header("CHARACTER DATA:")]
    public CharDataSO givenCharData;

    [Space]
    [Header("Abilities")]
    [SerializeField]
    private List<CharAbility> passiveAbilities = new List<CharAbility>();
    [SerializeField]
    private List<CharAbility> movementAbilities = new List<CharAbility>();
    [SerializeField]
    private List<CharAbility> activatableAbilities = new List<CharAbility>();
    [Space]

    //[HideInInspector]
    public List<CharAbility> passiveAbilitiesInsta = new List<CharAbility>();
    //[HideInInspector]
    public List<CharAbility> movementAbilitiesInsta = new List<CharAbility>();
    //[HideInInspector]
    public List<CharAbility> activatableAbilitiesInsta = new List<CharAbility>();

    [Space]
    [Header("Turn Management:")]
    public Teams team;
    public enum Teams
    {
        Hero,
        Villain,
        Vigilante
    }

    public Color heroColor;
    public Color villainColor;
    public Color vigilanteColor;
    public bool isAIControlled;


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
    public Node currentNode;

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

        if(team == Teams.Hero)
        {
            creatureScript.healthBar.currentHealthImage.color = heroColor;
        }
        else if (team == Teams.Villain)
        {
            creatureScript.healthBar.currentHealthImage.color = villainColor;
        }
        else if (team == Teams.Vigilante)
        {
            creatureScript.healthBar.currentHealthImage.color = vigilanteColor;
        }

        StartNodeFind();

        EventFlags.ANIMStartPeek += UnitPeekAnim;
        EventFlags.ANIMEndPeek += UnitUnPeekAnim;
    }

    private void OnDestroy()
    {
        EventFlags.ANIMStartPeek -= UnitPeekAnim;
        EventFlags.ANIMEndPeek -= UnitUnPeekAnim;
    }

    private void UnPackCharData()
    { 
        
    }

    public void PrepAbilities(out List<CharAbility> movementAb, out List<CharAbility> passiveAb, out List<CharAbility> activatableAb)
    {
        movementAb = new List<CharAbility>();
        passiveAb = new List<CharAbility>();
        activatableAb = new List<CharAbility>();

        PrepAbilAux(passiveAbilities, passiveAb);
        PrepAbilAux(movementAbilities, movementAb);
        PrepAbilAux(activatableAbilities, activatableAb);
    }

    private void PrepAbilAux(List<CharAbility> baseAbilities, List<CharAbility> copyList)
    {
        int givenSlotValue = 0;
        foreach (CharAbility currentChar in baseAbilities)
        {
            CharAbility abilityCopy = Instantiate(currentChar);
            copyList.Add(abilityCopy);
            abilityCopy.Initialize(this);
            givenSlotValue++;
        }
    }

    public void RespondFinishToTurn()
    {
        TurnManager.instance.SetPlayerAsFinished(this.gameObject);
    }

    public IEnumerator AIResponder()
    {
        if (isAIControlled)
        {
            Debug.Log(name + ": AI Pinged");
            yield return StartCoroutine(AILogicMove());
            yield return StartCoroutine(AILogicAttack());
        }
    }

    /*
    public AIProceed()
    {

    }
    */

    public IEnumerator AILogicMove()
    {
        AbilityPrefabRef.SelectorData selectorData = movementAbilitiesInsta[0].selectorPacketBaseData[0][0].selectorData;
        if (selectorData.SelectorName.Equals(AbilityPrefabRef.BasicMoveSelector))
        {
            AbilityPrefabRef.BasicMoveSelectorData trueData = (AbilityPrefabRef.BasicMoveSelectorData)selectorData;
            int energy = CreatureScript.CurrentEnergy;
            List<Node> foundNodes = Pathfinding.instance.DisplayAvailableMoves(currentNode, energy);
            yield return null;
            List<Node> coverNodes = new List<Node>();
            foreach(Node currNode in foundNodes)
            {
                foreach(Node neighbor in currNode.nodeNeighbors)
                {
                    if (!neighbor.IsWalkable && !coverNodes.Contains(currNode) && currNode != currentNode)
                    {
                        coverNodes.Add(currNode);
                        continue;
                    }
                }
            }

            Node chosenNode = coverNodes[Mathf.RoundToInt(Random.Range(0, coverNodes.Count - 1))];

            movementAbilitiesInsta[0].InitiateAbility(0);
            BasicMoveSelector selectScript = (BasicMoveSelector)movementAbilitiesInsta[0].currentActiveSelector;
            selectScript.isAIControlled = true;

            yield return new WaitForSeconds(0.5f);

            selectScript.SetMovePath(chosenNode);

            yield return new WaitForSeconds(1f);
            selectScript.MadeSelection();
            yield return null;
            yield return new WaitUntil(() => ResolutionManager.instance.resolutionRunning == false);
            yield return new WaitForSeconds(0.2f);
        }
    }
    public IEnumerator AILogicAttack()
    {
        AbilityPrefabRef.SelectorData selectorData = activatableAbilitiesInsta[0].selectorPacketBaseData[0][0].selectorData;

        if (selectorData.SelectorName.Equals(AbilityPrefabRef.CircleSelector))
        {
            AbilityPrefabRef.CircleSelectorData trueData = (AbilityPrefabRef.CircleSelectorData)selectorData;
            float range = trueData.radius;
            //Debug.Log(trueData.radius);

            GameObject hitObject = null;
            Collider[] hits = Physics.OverlapSphere(transform.position, range);
            //Debug.Log(hits.Length);
            foreach (Collider hit in hits)
            {
                if (hit.gameObject.CompareTag("Tile"))
                {
                    //Debug.Log(hit.gameObject.transform.parent.name);
                    Node node = GridGen.instance.NodeFromWorldPoint(hit.gameObject.transform.position);
                    if (node.occupant != null && node.occupant != gameObject)
                    {
                        hitObject = node.occupant;
                        break;
                    }
                }
            }

            if (hitObject != null)
            {
                activatableAbilitiesInsta[0].InitiateAbility(0);
                AttackSelection selectScript = activatableAbilitiesInsta[0].currentActiveSelector;
                selectScript.isAIControlled = true;

                yield return new WaitForSeconds(1f);

                bool matchFound = false;
                foreach (TargetSpecs currSpecs in selectScript.allSpecs)
                {
                    //Debug.Log("Specs Length: " + currSpecs.targetObj.name);
                    if (currSpecs.targetObj == hitObject)
                    {
                        selectScript.Gather(hitObject);
                        matchFound = true;
                        break;
                    }
                }

                //Debug.Log("Match Found: " + matchFound);

                if (matchFound)
                {
                    yield return new WaitForSeconds(0.5f);
                    selectScript.MadeSelection();
                    yield return null;
                    yield return new WaitUntil(() => ResolutionManager.instance.resolutionRunning == false);
                    yield return new WaitForSeconds(0.2f);


                    Debug.Log("FINISHED AI TURN");
                }
                else
                {
                    selectScript.CancelSelection();
                    Debug.Log("AI Cancelled");

                }
            }
            else
            {
                Debug.Log("AI Found: Null");
            }
        }
    }

    private void StartNodeFind() //Gives the unit reference to the Node below it.
    {
        Node foundNode = GridGen.instance.NodeFromWorldPoint(transform.position);
        if(foundNode == null)
        {
            Debug.LogWarning("WARNING: Unit was not able to initialize itself to the Grid");
        }
        currentNode = foundNode;
        if (foundNode.IsWalkable)
        {
            foundNode.IsOccupied = true;
            foundNode.occupant = this.gameObject;
            transform.position = new Vector3(currentNode.worldPosition.x, transform.position.y, currentNode.worldPosition.z);
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
