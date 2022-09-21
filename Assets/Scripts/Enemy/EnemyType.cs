using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyType", menuName = "Enemies/EnemyType")]
public class EnemyType : ScriptableObject
{
    // Serializable reference to stat types
    public enum StatType
    {
        AttackPower,
        Health,
        Speed,
        SpawnRate
    }

    // Generic serializable class for modifiers
    [System.Serializable]
    public class EnemyStatModifier
    {
        // Does this modifier add to the range or set it
        public enum ModifierType
        {
            Modify,
            Set
        }

        public StatType statType;
        public ModifierType modifierType;

        // Range to select between
        public float range1;
        public float range2;
    }

    public EnemyClassGroup.EnemyClass enemyClass;

    // Stats for this type
    public EnemyStats stats;
    public float spawnRate;

    // Prefab to spawn in the spawner
    public GameObject enemyPrefab;

    // Modifiers for the type and modifiers for overall class
    public EnemyTimeModifiers typeModifiers;
    [HideInInspector] public EnemyTimeModifiers classModifiers;

    [System.NonSerialized] public float modifiedSpawnRate;
    [System.NonSerialized] private bool assignedUpdateListener = false;
    [System.NonSerialized] private bool modifiedSpawnRateDirty = true;

    // Gets the modified spawn rate on this time category
    public float GetModifiedSpawnRate()
    {
        if (!assignedUpdateListener)
        {
            assignedUpdateListener = true;
            DayNightCycle.preTimeCategoryChanged.AddListener(UpdateModifiedSpawnRate);
        }
        if (modifiedSpawnRateDirty)
        {
            modifiedSpawnRateDirty = false;
            UpdateModifiedSpawnRate();
        }

        return modifiedSpawnRate;
    }

    // Updates the spawn rate for this time category
    public void UpdateModifiedSpawnRate()
    {
        // Gets modifiers from the type and class
        List<EnemyStatModifier> spawnModifiers = GetSpawnModifiers(GetModifiersByTime(typeModifiers));
        if (classModifiers != null)
        {
            spawnModifiers.AddRange(GetSpawnModifiers(GetModifiersByTime(classModifiers)));
        }

        // Sets initial spawn rate then modifies it
        float newModifiedSpawnRate = spawnRate;
        foreach (EnemyStatModifier modifier in spawnModifiers)
        {
            ModifyVariable(modifier, ref newModifiedSpawnRate);
        }

        modifiedSpawnRate = newModifiedSpawnRate;
    }

    // Iterates through the given stat modifiers to get spawn modifiers, prevents having to list them separately
    private List<EnemyStatModifier> GetSpawnModifiers(List<EnemyStatModifier> enemyStats)
    {
        List<EnemyStatModifier> spawnModifiers = new List<EnemyStatModifier>();
        foreach(EnemyStatModifier modifier in enemyStats)
        {
            if (modifier.statType == StatType.SpawnRate)
            {
                spawnModifiers.Add(modifier);
            }
        }
        return spawnModifiers;
    }

    public EnemyStats CreateModifiedStats()
    {
        // Copy the base stats to the modified ones
        EnemyStats modifiedStats = new EnemyStats();
        modifiedStats.attackPower = stats.attackPower;
        modifiedStats.speed = stats.speed;
        modifiedStats.health = stats.health;

        // Applies class modifiers then type modifiers to prioritise type specifity
        if (classModifiers != null)
        {
            ApplyTimeModifiers(classModifiers, modifiedStats);
        }
        ApplyTimeModifiers(typeModifiers, modifiedStats);
        return modifiedStats;
    }

    // Applies time modifiers to the given stats
    private void ApplyTimeModifiers(EnemyTimeModifiers timeModifiers, EnemyStats modifiedStats)
    {
        List<EnemyStatModifier> modifiersToApply = GetModifiersByTime(timeModifiers);

        // Apply each modifier individually
        foreach(EnemyStatModifier modifier in modifiersToApply)
        {
            ApplyModifier(modifier, modifiedStats);
        }
    }

    // Gets the relevant modifier list based on the global time of day
    public List<EnemyStatModifier> GetModifiersByTime(EnemyTimeModifiers timeModifiers)
    {
        switch (DayNightCycle.currentTimeCategory)
        {
            case DayNightCycle.TimeCategory.Morning:
                {
                    return timeModifiers.morningModifiers;
                }
            case DayNightCycle.TimeCategory.Noon:
                {
                    return timeModifiers.noonModifiers;
                }
            case DayNightCycle.TimeCategory.Night:
                {
                    return timeModifiers.nightModifiers;
                }
        }

        Debug.LogError("Attempting to apply unaccounted time category");
        return timeModifiers.morningModifiers;
    }

    // Applies an individual stat modifier to the given stats
    private void ApplyModifier(EnemyStatModifier modifier, EnemyStats modifiedStats)
    {

        // Get the correct stat reference based on serialized stat enum
        switch (modifier.statType)
        {
            case StatType.AttackPower:
                {
                    ModifyVariable(modifier, ref modifiedStats.attackPower);
                    break;
                }
            case StatType.Health:
                {
                    ModifyVariable(modifier, ref modifiedStats.health);
                    break;
                }
            case StatType.Speed:
                {
                    ModifyVariable(modifier, ref modifiedStats.speed);
                    break;
                }
        }
    }

    // Modifiers the stat reference by the given modifier
    private void ModifyVariable(EnemyStatModifier modifier, ref float modifiedVariable)
    {
        switch (modifier.modifierType)
        {
            // Add by range if modifying
            case EnemyStatModifier.ModifierType.Modify:
                {
                    modifiedVariable += Random.Range(modifier.range1, modifier.range2);
                    modifiedVariable = Mathf.Max(modifiedVariable, 0);
                    break;
                }

            // Set by range if setting
            case EnemyStatModifier.ModifierType.Set:
                {
                    modifiedVariable = Random.Range(modifier.range1, modifier.range2);
                    break;
                }
        }
    }
}
