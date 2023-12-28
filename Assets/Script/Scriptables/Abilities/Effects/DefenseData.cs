using System;
using UnityEditor;
using UnityEngine;
/*[CreateAssetMenu(fileName = "Raise Defense", menuName = "Abilities/Effects/Raise Defense")]
public class DefenseData : ModifierData
{
    public AttackDefines.DamageFlag[] BlockedAttacks = new AttackDefines.DamageFlag[0];
    public AttackDefines.DamageElement[] BlockedElements = new AttackDefines.DamageElement[(int)AttackDefines.DamageElement.Recognised];
    public propertyDefense.DefenseType Type;
    public float TypeData;
    public propertyDefense.Direction BlockDirection = propertyDefense.Direction.retroactive;
    public MainAttackData RetaliationAttack;
    public DefenseData() { }
    public DefenseData(string Name, ModifierDefines.Alignment a, AttackDefines.DamageElement e)
    {
        ModifierName = Name;
        alignment = a;
        element = e;
        expireType = ModifierDefines.ExpireType.permanent;
    }
    public DefenseData(string Name, float t, ModifierDefines.Alignment a, AttackDefines.DamageElement e)
    {
        ModifierName = Name;
        Duration = t;
        alignment = a;
        element = e;
        expireType = ModifierDefines.ExpireType.time;
    }
    public void CastOnActor(PropertyAbility ability, Mob target,float delay)
    {
        if (target.IsObject())
            return;
        propertyDefense defense = new propertyDefense(this, ability);
        entityActor actorTarget = (entityActor)target;
        actorTarget.Modifiers.New(defense, ModifierDefines.Type.Replace, ModifierEffects, AbilityDefines.anim_delay);

    }
    public override AttackDefines.DamageElement GetEffectElement()
    {
        return AttackDefines.DamageElement.Null;
    }
}*/