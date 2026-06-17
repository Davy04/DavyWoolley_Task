using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private WeaponParent weaponParent;
    [SerializeField] private SpriteRenderer weaponRenderer;
    [SerializeField] private Animator tipFlashAnimator;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Health playerHealth;

    public WeaponParent   WeaponParent        => weaponParent;
    public SpriteRenderer WeaponRenderer      => weaponRenderer;
    public Animator       TipFlashAnimator    => tipFlashAnimator;
    public Transform      ProjectileSpawnPoint => projectileSpawnPoint;
    public Health         PlayerHealth        => playerHealth;

    private bool _canAttack = true;

    private void Start()
    {
        if (weaponRenderer == null && InventoryManager.Instance != null)
            weaponRenderer = InventoryManager.Instance.weaponRenderer;

        if (tipFlashAnimator != null)
            tipFlashAnimator.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_canAttack || Mouse.current == null) return;
        if (InventoryManager.Instance != null && InventoryManager.Instance.IsBagOpen) return;

        WeaponBehavior behavior = GetCurrentBehavior();
        if (behavior == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            StartCoroutine(PerformAttack(behavior));
    }

    private IEnumerator PerformAttack(WeaponBehavior behavior)
    {
        _canAttack = false;
        yield return StartCoroutine(behavior.Perform(this));
        _canAttack = true;
    }

    private WeaponBehavior GetCurrentBehavior()
    {
        InventorySlot slot = InventoryManager.Instance?.GetSelectedSlot();
        if (slot == null || slot.transform.childCount == 0) return null;
        InventoryItem held = slot.transform.GetChild(0).GetComponent<InventoryItem>();
        return held?.item?.weaponBehavior;
    }
}
