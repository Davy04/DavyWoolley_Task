using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/ConsumableBehavior")]
public class ConsumableBehavior : WeaponBehavior
{
    [SerializeField] private float useCooldown = 0.5f;

    public override IEnumerator Perform(PlayerAttack context)
    {
        Item item = InventoryManager.Instance?.CurrentItem;
        if (item == null) yield break;

        Health health = context.PlayerHealth;
        if (health == null || health.Current >= health.Max) yield break;

        health.Heal(item.healthRestore);
        InventoryManager.Instance.ConsumeCurrent();

        yield return new WaitForSeconds(useCooldown);
    }
}
