using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
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
        if (Time.timeScale == 0f) return; // jogo pausado / game over
        if (InventoryManager.Instance != null && InventoryManager.Instance.IsBagOpen) return;
        if (IsPointerOverUI()) return; // clique em botão de UI não deve atacar

        WeaponBehavior behavior = GetCurrentBehavior();
        if (behavior == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            StartCoroutine(PerformAttack(behavior));
    }

    private static bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    private IEnumerator PerformAttack(WeaponBehavior behavior)
    {
        _canAttack = false;
        yield return StartCoroutine(behavior.Perform(this));
        _canAttack = true;
    }

    private WeaponBehavior GetCurrentBehavior()
    {
        return InventoryManager.Instance != null ? InventoryManager.Instance.CurrentBehavior : null;
    }
}
