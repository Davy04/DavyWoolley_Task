using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/ConsumableBehavior")]
public class ConsumableBehavior : WeaponBehavior
{
    [SerializeField] private float useCooldown = 0.5f;

    public override IEnumerator Perform(PlayerAttack context)
    {
        InventorySlot slot = InventoryManager.Instance?.GetSelectedSlot();
        if (slot == null || slot.transform.childCount == 0) yield break;

        InventoryItem invItem = slot.transform.GetChild(0).GetComponent<InventoryItem>();
        if (invItem == null || invItem.item == null) yield break;

        Health health = context.PlayerHealth;
        if (health == null || health.Current >= health.Max) yield break;

        health.Heal(invItem.item.healthRestore);
        invItem.ConsumeOne();

        yield return new WaitForSeconds(useCooldown);
    }
}
