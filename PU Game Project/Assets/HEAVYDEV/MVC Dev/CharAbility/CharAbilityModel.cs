using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAbilityModel {

    public GameObject associatedObject;
    public List<GameObject> relevantTargets = new List<GameObject>();

    public string abilityName;
    public string abilityDescription;
    public Sprite abilitySprite;

    public int energyCost;
    public int turnCooldown;

    public List<BattleBehaviourModel> bBehaviourList;

}
