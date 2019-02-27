using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomPropertyDrawer(typeof(SelectorData))]
public class SelectorDataEditor : PropertyDrawer
{
    /*
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);
    }
    */



    //[CustomPropertyDrawer(typeof(SelectorData.Circle))]
    public class Circle : SelectorDataEditor
    {
        /*
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
        }
        */
    }
}

