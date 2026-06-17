using System.Collections;
using UnityEngine;

public abstract class WeaponBehavior : ScriptableObject
{
    public abstract IEnumerator Perform(PlayerAttack context);
}
