using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(AbstractDungeonGeneator), true)]
public class RandomDungeonGeneratorEditor : Editor
{
    AbstractDungeonGeneator generator;

    private void Awake()
    {
        generator = (AbstractDungeonGeneator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Create Dungeon"))
        {
            generator.GenerateDungeon();
        }
    }
}
