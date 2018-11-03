//Need to change some of this. Came from MOBA. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MHA.UserInterface;
using Kryz.CharacterStats;

public class LivingCreature : GameEntity {

    [Header("[LIVING CREATURE]")]

    [Header("Base Stats:")]
    public float baseStrength;
    public int baseEnergy;
    public float baseQuirkPower;
    public float basePhysicalResistance;
    public float baseQuirkResistance;
    public float baseRange;

    [Header("Stats:")]
    public int currentLevel = 1;
    public CharacterStat strength;
    private int currentEnergy;
    public int CurrentEnergy
    {
        get
        {
            return currentEnergy;
        }
        set
        {
            currentEnergy = value;
            if(currentEnergy <= 0)
            {
                RespondFinishToTurn();
            }
        }
    }
    public CharacterStat maxEnergy;
    public CharacterStat quirkPower;
    public CharacterStat physicalResistance;
    public CharacterStat quirkResistance;
    public CharacterStat range;

    [Header("Stat Growth:")]
    public float healthGrowth;
    public int energyGrowth;
    public float strengthGrowth;
    public float attackSpeedGrowth;
    public float physicalResistanceGrowth;
    public float quirkResistanceGrowth;

    [SerializeField]
    protected float currentExperience;
    public float Experience {
        get
        {
            return currentExperience;
        }
        set
        {
            currentExperience = value;
            ExperienceChecker();
        }
    }

    [Header("State Bools:")]
    public bool amRanged;
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
    }

    protected override void Start ()
    {
        //Initial Setting of Stats. 
        strength.BaseValue = baseStrength;
        maxEnergy.BaseValue = baseEnergy;
        CurrentEnergy = Mathf.RoundToInt(maxEnergy.Value);
        quirkPower.BaseValue = baseQuirkPower;
        physicalResistance.BaseValue = basePhysicalResistance;
        quirkResistance.BaseValue = baseQuirkResistance;
        range.BaseValue = baseRange;

        base.Start();
    }
	

	protected override void Update ()
    {
        HandleBuffs();

        //Debug Functions
        if (Input.GetKeyDown(KeyCode.X))
        {
            currentExperience += 25;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearBuffs();
        }

        base.Update();
    }


    public void CreatureHit(Attack receivedAttack)
    {
        if (!isInvincible)
        {
            //Debug.Log(this.gameObject.name + ": Got Hit");
            currentHealth -= receivedAttack.damageValue;

            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth.Value);

            healthBar.UpdateHealth(currentHealth, maxHealth.Value); //Health Bar

            GameObject indicatorObj = Instantiate(damageIndicator, new Vector3 (transform.position.x, transform.position.y + 2f, transform.position.z), Quaternion.identity);
            indicatorObj.GetComponent<DamageIndicator>().SetText(receivedAttack.damageValue);

            if (currentHealth <= 0 && !amDead)
            {
                Debug.Log("Creature Dead");
                amDead = true;
                Destroy(this.gameObject);
            }
        }
    }


    protected virtual void LevelUp()
    {
        currentLevel += 1;

        float oldMaxHealth = maxHealth.Value;
        float oldMaxEnergy = maxEnergy.Value;

        strength.AddModifier(new StatModifier(strengthGrowth, StatModType.Flat));
        maxHealth.AddModifier(new StatModifier(healthGrowth, StatModType.Flat));
        maxEnergy.AddModifier(new StatModifier(energyGrowth, StatModType.Flat));
        physicalResistance.AddModifier(new StatModifier(physicalResistanceGrowth, StatModType.Flat));
        quirkResistance.AddModifier(new StatModifier(quirkResistanceGrowth, StatModType.Flat));

        //Makes it so changing max health/energy changes current. 
        currentHealth += maxHealth.Value - oldMaxHealth;

    }

    private void ExperienceChecker()
    {
        if (currentExperience >= 100)
        {
            if (currentLevel < 18)
            {
                LevelUp();
            }
            currentExperience -= 100;
        }
    }

    public void RespondFinishToTurn()
    {
        TurnManager.instance.SetPlayerAsFinished(this.gameObject);
    }


    public void AddBuff(Buff buffToApply)
    {
        Debug.Log("Added Buff");
        AddBuffList.Add(buffToApply);
    }

    public void RemoveBuff(Buff buffToRemove)
    {
        Debug.Log("Removed Buff");
        RemoveBuffList.Add(buffToRemove);
    }

    public void ClearBuffs()
    {
        foreach(Buff currentBuff in BuffList)
        {
            currentBuff.RemoveSelf();
        }
    }

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
}
