using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Behavior/Wand")]
public class WandBehavior : WeaponBehavior
{
    [Header("Timing")]
    public float windupDuration  = 0.08f;
    public float thrustDuration  = 0.10f;
    public float returnDuration  = 0.22f;
    public float attackCooldown  = 0.55f;

    [Header("Distance")]
    public float pullBackDistance = 0.15f;
    public float thrustDistance   = 0.45f;
    public AnimationCurve thrustCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Flash")]
    public Color flashColor       = new Color(1f, 0.85f, 0.3f);
    public float tipFlashDuration = 0.18f;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float projectileSpeed  = 8f;
    public int   projectileDamage = 10;

    [Header("Audio")]
    public AudioClip shootClip;

    public override IEnumerator Perform(PlayerAttack ctx)
    {
        ctx.WeaponParent.IsAttacking = true;

        Vector3 baseLocalPos = ctx.WeaponParent.transform.localPosition;
        Vector3 aimDir       = ctx.WeaponParent.transform.right;

        float elapsed = 0f;
        while (elapsed < windupDuration)
        {
            elapsed += Time.deltaTime;
            ctx.WeaponParent.transform.localPosition = Vector3.Lerp(
                baseLocalPos,
                baseLocalPos - aimDir * pullBackDistance,
                Mathf.Clamp01(elapsed / windupDuration));
            yield return null;
        }

        Vector3 pulledPos = baseLocalPos - aimDir * pullBackDistance;
        Vector3 thrustPos  = baseLocalPos + aimDir * thrustDistance;

        ctx.StartCoroutine(FlashWeapon(ctx));

        elapsed = 0f;
        while (elapsed < thrustDuration)
        {
            elapsed += Time.deltaTime;
            ctx.WeaponParent.transform.localPosition = Vector3.Lerp(
                pulledPos, thrustPos,
                thrustCurve.Evaluate(Mathf.Clamp01(elapsed / thrustDuration)));
            yield return null;
        }

        // Pico: flash na ponta + projétil
        ctx.StartCoroutine(TipFlash(ctx));
        SpawnProjectile(ctx, aimDir);

        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            ctx.WeaponParent.transform.localPosition = Vector3.Lerp(
                thrustPos, baseLocalPos,
                Mathf.Clamp01(elapsed / returnDuration));
            yield return null;
        }

        ctx.WeaponParent.transform.localPosition = baseLocalPos;
        ctx.WeaponParent.IsAttacking = false;

        float remaining = attackCooldown - (windupDuration + thrustDuration + returnDuration);
        if (remaining > 0f)
            yield return new WaitForSeconds(remaining);
    }

    private IEnumerator FlashWeapon(PlayerAttack ctx)
    {
        if (ctx.WeaponRenderer == null) yield break;

        Color original = ctx.WeaponRenderer.color;
        ctx.WeaponRenderer.color = flashColor;

        float duration = thrustDuration + returnDuration * 0.5f;
        float elapsed  = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            ctx.WeaponRenderer.color = Color.Lerp(flashColor, original, elapsed / duration);
            yield return null;
        }

        ctx.WeaponRenderer.color = original;
    }

    private IEnumerator TipFlash(PlayerAttack ctx)
    {
        if (ctx.TipFlashAnimator == null) yield break;

        ctx.TipFlashAnimator.gameObject.SetActive(true);
        ctx.TipFlashAnimator.Play(0, -1, 0f);

        yield return new WaitForSeconds(tipFlashDuration);

        ctx.TipFlashAnimator.gameObject.SetActive(false);
    }

    private void SpawnProjectile(PlayerAttack ctx, Vector3 aimDir)
    {
        AudioManager.Instance?.PlaySFX(shootClip);

        Vector3 pos = ctx.ProjectileSpawnPoint != null
            ? ctx.ProjectileSpawnPoint.position
            : ctx.WeaponParent.transform.position;

        if (ProjectilePool.Instance != null)
        {
            ProjectilePool.Instance.Spawn(pos, aimDir, projectileSpeed, projectileDamage);
            return;
        }

        // Fallback (sem pool na cena): instancia direto.
        if (projectilePrefab == null) return;
        GameObject obj = Instantiate(projectilePrefab, pos, Quaternion.identity);
        obj.GetComponent<Projectile>()?.Initialize(aimDir, projectileSpeed, projectileDamage);
    }
}
