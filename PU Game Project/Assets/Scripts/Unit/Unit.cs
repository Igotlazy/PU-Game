//General methods a Unit should have access too.

using UnityEngine;
using System.Collections.Generic;
using Cinemachine;
using MHA.BattleBehaviours;
using MHA.Events;
using MHA.UserInterface;
using System.Collections;
using Kryz.CharacterStats;

public class Unit : GameEntity {

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

    public Node currentNode;

    [Space]
    [Header("STATS:")]
    [SerializeField]
    private int currentEnergy;
    public int CurrentEnergy
    {
        get
        {
            return currentEnergy;
        }
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            currentEnergy = value;
            if (currentEnergy <= 0)
            {
                RespondFinishToTurn();
            }
        }
    }
    public CharacterStat maxEnergy;
    public CharacterStat currentStrength;
    public CharacterStat currentDefense;
    public CharacterStat currentLuck;


    [Header("State Bools:")]
    public bool isInvincible;
    public bool amDead;

    [Header("MISC:")]
    public List<Buff> BuffList = new List<Buff>();
    public List<Buff> AddBuffList = new List<Buff>();
    public List<Buff> RemoveBuffList = new List<Buff>();

    public HealthBarControl healthBar;
    public GameObject damageIndicator;

    protected override void Awake()
    {
        base.Awake();
        entityType = EntityType.Unit;

        ReferenceObjects.AddToPlayerList(this.gameObject);
    }

    protected override void Start()
    {
        base.Start();
        PrepAbilities(out movementAbilitiesInsta, out passiveAbilitiesInsta, out activatableAbilitiesInsta);
        LoadStatData(givenCharData);
        healthBar.UpdateHealth(currentHealth, maxHealth.Value);

        if(team == Teams.Hero)
        {
            healthBar.currentHealthImage.color = heroColor;
        }
        else if (team == Teams.Villain)
        {
            healthBar.currentHealthImage.color = villainColor;
        }
        else if (team == Teams.Vigilante)
        {
            healthBar.currentHealthImage.color = vigilanteColor;
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


    public void LoadStatData(CharDataSO givenData)
    {
        currentHealth = givenData.baseHealth;
        maxHealth.BaseValue = givenData.baseHealth;

        currentEnergy = givenData.baseEnergy;
        maxEnergy.BaseValue = givenData.baseEnergy;

        currentStrength.BaseValue = givenData.baseStrength;
        currentDefense.BaseValue = givenData.baseDefense;
        currentLuck.BaseValue = givenData.baseLuck;
    }


    public void CreatureHit(Attack receivedAttack)
    {
        if (!isInvincible && !amDead)
        {
            float damage = CombatUtils.DamageCalculation(receivedAttack, this);
            currentHealth -= damage;

            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth.Value);

            new BBDealDamageAnim(this, this, currentHealth, damage);
            Debug.Log("DEAL DAMAGE");

            if (currentHealth <= 0 && !amDead)
            {
                Debug.Log("Creature Dead");
                CharAbility.totalCastIndex++;
                EffectDeath effectDeath = new EffectDeath(this, this);
                ResolutionManager.instance.LoadBattleEffect(effectDeath);
            }
        }
    }

    public void AddBuff(Buff buffToApply)
    {
        Debug.Log("Added Buff");
        AddBuffList.Add(buffToApply);
    }

    public void RemoveBuff(Buff buffToRemove)
    {
        RemoveBuffList.Add(buffToRemove);
    }

    public void ClearBuffs()
    {
        foreach (Buff currentBuff in BuffList)
        {
            currentBuff.RemoveSelf();
        }
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
    public IEnumerator AILogicMove()
    {
        SelectorData selectorData = movementAbilitiesInsta[0].selectorData[0][0];
        if (selectorData.SelectorName.Equals(AbilityPrefabRef.BasicMoveSelector))
        {
            //AbilityPrefabRef.BasicMoveSelectorData trueData = (AbilityPrefabRef.BasicMoveSelectorData)selectorData;
            int energy = CurrentEnergy;
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
        SelectorData selectorData = activatableAbilitiesInsta[0].selectorData[0][0];

        if (selectorData.SelectorName.Equals(AbilityPrefabRef.CircleSelector))
        {
            SelectorData.Circle trueData = (SelectorData.Circle)selectorData;
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
                AbilitySelection selectScript = activatableAbilitiesInsta[0].currentActiveSelector;
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
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.size = new Vector3(GridGen.instance.NodeDiameter, collider.size.y, GridGen.instance.NodeDiameter);
    }

    public void UnitPeekAnim(object sender, EventFlags.EPeekStart peekArgs)
    {
        if (peekArgs.peekingObject == this)
        {
            Debug.Log("Peek: " + peekArgs.peekingObject.name);
            peekArgs.peekPosition = new Vector3(peekArgs.peekPosition.x, spriteRig.transform.localPosition.y, peekArgs.peekPosition.z);

            new AnimMoveToPos(this, peekArgs.peekPosition, spriteRig, 3f, false);
            peekArgs.AddToPeek();
        }
    }

    public void UnitUnPeekAnim(object sender, EventFlags.EPeekEnd peekArgs)
    {
        if (peekArgs.peekingObject == this)
        {
            Debug.Log("UnPeek: " + peekArgs.peekingObject.name);
            new AnimMoveToPos(this, peekArgs.GiveOriginalPos(), spriteRig, 3f, false);
            peekArgs.RemoveFromPeek();
        }
    }
}
