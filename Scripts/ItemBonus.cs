using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBonus : MonoBehaviour
{
    public enum BonusType
    {
        MaxHealth,
        MaxMana,
        DamagePercent,
        DamageBase,
        DefensePercent,
        DefenseBase,
        HealingPercent,
        HealingBase
    }
    public BonusType type;
    public float amount;

    private BonusType Type { get => type; set => type = value; }
    public float Amount { get => amount; set => amount = value; }

    public void initialize(BonusType t, float a)
    {
        type = t;
        amount = a;
    }
}
