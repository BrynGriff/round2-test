using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;

[CustomEditor(typeof(EnemySpawner))]
public class EnemySpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        // This button adds all classes to the inspector
        if (GUILayout.Button("Spawn Any Class", GUILayout.Width(200)))
        {
            PopulateGroup(target as EnemySpawner);
        }
    }

    private void PopulateGroup(EnemySpawner group)
    {
        // Clear all classes and types and load classes from addressables
        group.spawnableClasses.Clear();
        group.spawnableTypes.Clear();
        Addressables.LoadAssetsAsync<EnemyClassGroup>("EnemyClasses", result => { EnemyClassLoaded(group, result); });
    }

    // When class loads add it to the spawnables
    private void EnemyClassLoaded(EnemySpawner group, EnemyClassGroup result)
    {
        group.spawnableClasses.Add(result);
        PrefabUtility.RecordPrefabInstancePropertyModifications(group);
        if (Application.isPlaying)
        {
            group.Initialize();
        }
    }
}