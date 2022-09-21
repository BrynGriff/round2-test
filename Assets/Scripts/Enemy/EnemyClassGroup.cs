using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyClassGroup", menuName = "Enemies/EnemyClassGroup")]
public class EnemyClassGroup : ScriptableObject
{
    public enum EnemyClass
    {
        Grunt,
        Archer,
        Assassin,
    }

    // The class that this class group contains
    public EnemyClass groupClass;

    // All types in the group
    public List<EnemyType> groupTypes;

    // Stat modifiers based on time to be applied to every class member
    public EnemyTimeModifiers classModifiers;

    // Contain a reference to enemy class modifiers on the type
    public void OnEnable()
    {
        AssignModifiersToTypes();
    }

    public void OnValidate()
    {
        AssignModifiersToTypes();
    }

    public void AssignModifiersToTypes()
    {
        // Assign class modifiers to each group member
        foreach(EnemyType type in groupTypes)
        {
            type.classModifiers = classModifiers;
        }
    }
}
