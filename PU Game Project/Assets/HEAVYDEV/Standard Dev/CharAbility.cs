using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharAbility{

    public LivingCreature associatedCreature;

    public CharAbility(LivingCreature _associatedCreature)
    {
        associatedCreature = _associatedCreature;
    }

    public static int charIndex;

    public string abilityName;
    public string abilityDescription;
    public Sprite abilitySprite;

    public int energyCost;
    public int turnCooldown;

    protected List<List<GameObject>> targetCollectors = new List<List<GameObject>>();
    public List<List<Node>> targets = new List<List<Node>>();

    public List<Action<int>> castableAbilities = new List<Action<int>>();

    public void InitiateAbility(int abilityIndex)
    {
        associatedCreature.StartCoroutine(CollectTargets(abilityIndex));
    }

    private IEnumerator CollectTargets(int abilityIndex)
    {
        int collectorIndex = 0;

        while(collectorIndex < targetCollectors.Count)
        {
            GameObject.Instantiate(targetCollectors[abilityIndex][collectorIndex], associatedCreature.gameObject.transform.position, Quaternion.identity);
            yield return new WaitUntil(() => targetCollectors[abilityIndex][collectorIndex].GetComponent<AttackSelection>().hasSentTargets);
            collectorIndex++;
        }

        CastAbility(abilityIndex);
    }

    private void CastAbility(int abilityIndex)
    {
        if(targets.Count > 0)
        {
            charIndex += 1;
            castableAbilities[abilityIndex].Invoke(charIndex);
        }

    }
}
