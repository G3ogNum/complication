using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AttackData",menuName ="Attack/Attak Data")]
public class AttackData_SO : ScriptableObject
{
    public float health = 100f;
    public float elementShield = 100f;
    public float damage=0f;
    public float elementDamage=0f;

    //all below are Buff percent of
    public float attackBuff = 0f;
    public float attackElementDamageBuff = 0f;
    public float healthBuff = 0f;
    public float shieldBuff = 0f;
    public float fireRateBuff = 0f;

    public void ApplyWeaponData(AttackData_SO item)
    {
        health += item.health;
        elementShield += item.elementShield;
        
        //remember to make it all up
    }
    public void DisapplyWeaponData(AttackData_SO item)
    {
        health -= item.health;
        elementShield -= item.elementShield;

        //remember to make it all up
    }
}
