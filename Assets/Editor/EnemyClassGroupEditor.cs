using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;

[CustomEditor(typeof(EnemyClassGroup))]
public class EnemyClassGroupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // This button saves having to fill all groups manually
        GUILayout.Label("\nFill the group types with all types specified in Group Class");
        if (GUILayout.Button("Fill Group", GUILayout.Width(200)))
        {
            PopulateGroup(target as EnemyClassGroup);
        }
        GUILayout.Label("\n");
    }

    private void PopulateGroup(EnemyClassGroup group)
    {
        // Clear group list and load all enemy types using addressables
        group.groupTypes.Clear();
        Addressables.LoadAssetsAsync<EnemyType>("EnemyTypes", result => { EnemyTypeLoaded(group, result); });
    }

    private void EnemyTypeLoaded(EnemyClassGroup group, EnemyType result)
    {
        // Check if this enemy is part of the class group we are trying to fill
        if (group.groupClass == result.enemyClass)
        {
            group.groupTypes.Add(result);
        }
    }
}