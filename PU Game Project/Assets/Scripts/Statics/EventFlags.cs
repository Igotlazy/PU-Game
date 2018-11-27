using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MHA.Events
{
    public static class EventFlags
    {
        public const string TookDamageIdentifier = "Took Damage";
        public static event EventHandler<EventArgs> TookDamage;
        public static void EVENTTookDamage(object sender, EventArgs e) { if (TookDamage != null) { TookDamage(sender, e); } }

        public class ETookDamageArgs : EventArgs
        {
            public float damageValue;
            public LivingCreature source;
            public LivingCreature target;
        }
    }
}