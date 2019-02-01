//Need to change some of this. Came from MOBA. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MHA.UserInterface;
using MHA.BattleBehaviours;
using Kryz.CharacterStats;

public class LivingCreature : MonoBehaviour {


    [Header("Stats:")]
    public float currentHealth;
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
            if(value < 0)
            {
                value = 0;
            }
            currentEnergy = value;
            if(currentEnergy <= 0)
            {
                attachedUnit.RespondFinishToTurn();
            }
        }
    }
    public CharacterStat maxHealth;
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

    public Unit attachedUnit;

    protected void Awake()
    {
        attachedUnit = GetComponent<Unit>();
    }

    protected void Start ()
    {

    }
	

	protected void Update ()
    {
        //HandleBuffs();
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
        if (!isInvincible)
        {
            float damage = CombatUtils.DamageCalculation(receivedAttack, attachedUnit);
            currentHealth -= damage;

            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth.Value);

            new BBDealDamageAnim(this, currentHealth, damage);

            if (currentHealth <= 0 && !amDead)
            {
                Debug.Log("Creature Dead");
                CharAbility.totalCastIndex++;
                EffectDataPacket packet = new EffectDataPacket(attachedUnit, null);               
                EffectDeath effectDeath = new EffectDeath(packet);
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
        foreach(Buff currentBuff in BuffList)
        {
            currentBuff.RemoveSelf();
        }
    }

    /*
    public void HandleBuffs()
    {
        if (AddBuffList.Count > 0)
        {
            BuffList.AddRange(AddBuffList);

            foreach(Buff buffToAdd in AddBuffList)
            {
                buffToAdd.Start();
            }
            AddBuffList.Clear();
        }

        foreach (Buff buffToRemove in RemoveBuffList)
        {
            BuffList.Remove(buffToRemove);
        }

        foreach (Buff currentBuff in BuffList)
        {
           currentBuff.Update();
        }
    }
    */
}
