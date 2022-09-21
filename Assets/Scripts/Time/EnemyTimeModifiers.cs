using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyTimeModifiers
{
    public List<EnemyType.EnemyStatModifier> morningModifiers;
    public List<EnemyType.EnemyStatModifier> noonModifiers;
    public List<EnemyType.EnemyStatModifier> nightModifiers;
}
