using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventFlags
{
    public const string TookDamageIdentifier = "Took Damage";
    public delegate void TookDamage();
    public static event TookDamage tookDamage;
    public static void EVENTTookDamage() { if (tookDamage != null) { tookDamage(); } }
}
