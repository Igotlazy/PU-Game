using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MHA.Events
{
    public static class EventFlags
    {
        //----------------Gameplay Events---------------//

        public static event EventHandler<ETookDamageArgs> TookDamage;
        public static void EVENTTookDamage(object sender, ETookDamageArgs e) { if (TookDamage != null) { TookDamage(sender, e); } }

        public class ETookDamageArgs : EventArgs
        {
            public ETookDamageArgs(float _damageValue, LivingCreature _source, LivingCreature _target)
            {
                this.damageValue = _damageValue;
                this.source = _source;
                this.target = _target;
            }

            public float damageValue;
            public LivingCreature source;
            public LivingCreature target;
        }

        //----------------Animation Events---------------//


        public static event EventHandler<EPeekStart> ANIMStartPeek;
        public static void ANIMStartPeekCALL(object sender, EPeekStart e)
        {
            if (ANIMStartPeek != null)
            {
                ANIMStartPeek(sender, e);
            }
        }
        public static event EventHandler<EPeekEnd> ANIMEndPeek;
        public static void ANIMEndPeekCALL(object sender, EPeekEnd e)
        {
            if (ANIMEndPeek != null)
            {
                ANIMEndPeek(sender, e);
            }
        }

        public abstract class EPeekArgs : EventArgs
        {
            protected static List<object> peekingObjects = new List<object>();
            protected static List<Vector3> peekingObjOriginalLocations = new List<Vector3>();
        }
        public class EPeekStart : EPeekArgs
        {
            public Unit peekingObject;
            public Vector3 peekPosition;
            public Vector3 startPosition;

            public EPeekStart(Unit _peekingObject, Vector3 _peekPosition, Vector3 _startPosition)
            {
                this.peekingObject = _peekingObject;
                this.peekPosition = _peekPosition;
                this.startPosition = _startPosition;
            }

            public void AddToPeek()
            {
                peekingObjects.Add(peekingObject);
                peekingObjOriginalLocations.Add(startPosition);
            }

        }
        public class EPeekEnd : EPeekArgs
        {
            public Unit peekingObject;

            public EPeekEnd(Unit _peekingObject)
            {
                this.peekingObject = _peekingObject;
            }

            public void RemoveFromPeek()
            {
                if (peekingObjects.Contains(peekingObject))
                {
                    int index = peekingObjects.IndexOf(peekingObject);
                    peekingObjOriginalLocations.RemoveAt(index);
                    peekingObjects.Remove(peekingObject);
                }
            }
            public Vector3 GiveOriginalPos()
            {
                if (peekingObjects.Contains(peekingObject))
                {
                    int index = peekingObjects.IndexOf(peekingObject);
                    return peekingObjOriginalLocations[index];
                }
                else
                {
                    Debug.LogWarning("Hey you didn't get the originalPos for Peeking");
                    return Vector3.zero;
                }
            }
        }

        public static event EventHandler<ECastAnim> ANIMStartCast;
        public static void ANIMStartCastCALL(object sender, ECastAnim e)
        {
            if (ANIMStartCast != null)
            {
                ANIMStartCast(sender, e);
            }
        }
        public static event EventHandler<ECastAnim> ANIMFinishCast;
        public static void ANIMFinishCastCALL(object sender, ECastAnim e)
        {
            if (ANIMFinishCast != null)
            {
                ANIMFinishCast(sender, e);
            }
        }

        public static event EventHandler<ECastAnim> ANIMFinishProj;
        public static void ANIMFinishProjCALL(object sender, ECastAnim e)
        {
            Debug.Log("HIT Anim Event");
            if (ANIMFinishProj != null)
            {
                ANIMFinishProj(sender, e);
            }
        }

        public class ECastAnim : EventArgs
        {
            public ECastAnim()
            {

            }
        }
    }
}