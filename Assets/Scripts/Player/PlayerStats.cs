using BulletCrash.Core.Stats;
using SerializableMethods;
using UnityEngine;

/// <summary>
/// Holds every live, modifiable stat for the player in the current session.
/// Built from a CharacterData asset on Awake — the asset is never mutated.
/// All upgrades apply StatModifiers here; systems (movement, weapon, health)
/// read .Value from the relevant property.
/// </summary>
[RequireComponent(typeof(Health))]
public class PlayerStats : MonoBehaviour
{
    [SerializeField] private CharacterData character;

    public Stat MaxHealth      { get; private set; }
    public Stat HealthRegen    { get; private set; }
    public Stat MoveSpeed      { get; private set; }
    public Stat BulletSpeed    { get; private set; }
    public Stat BulletDamage   { get; private set; }
    public Stat BulletDistance { get; private set; }
    public Stat Reload         { get; private set; }

    private Health _health;
    private float  _regenAccumulator;

    private void Awake()
    {
        if (character == null)
        {
            Debug.LogError($"{nameof(PlayerStats)} is missing a {nameof(CharacterData)} asset.", this);
            enabled = false;
            return;
        }

        _health = GetComponent<Health>();

        MaxHealth      = new Stat(character.BaseMaxHealth);
        HealthRegen    = new Stat(character.BaseHealthRegen);
        MoveSpeed      = new Stat(character.BaseMoveSpeed);
        BulletSpeed    = new Stat(character.BaseBulletSpeed);
        BulletDamage   = new Stat(character.BaseBulletDamage);
        BulletDistance = new Stat(character.BaseBulletDistance);
        Reload         = new Stat(character.BaseReload);

        RefreshMaxHealth();
    }

    private void Update()
    {
        ApplyRegen();
    }
    
    public void RefreshMaxHealth()
    {
        _health.Initialize(Mathf.RoundToInt(MaxHealth.Value));
    }

    /// <summary>Resolves a <see cref="PlayerStatType"/> to its live <see cref="Stat"/>.</summary>
    public Stat GetStat(PlayerStatType type) => type switch
    {
        PlayerStatType.MaxHealth      => MaxHealth,
        PlayerStatType.HealthRegen    => HealthRegen,
        PlayerStatType.MoveSpeed      => MoveSpeed,
        PlayerStatType.BulletSpeed    => BulletSpeed,
        PlayerStatType.BulletDamage   => BulletDamage,
        PlayerStatType.BulletDistance => BulletDistance,
        PlayerStatType.Reload         => Reload,
        _ => null
    };

    /// <summary>
    /// Applies a data-authored bonus to the matching stat. The single entry point shared by
    /// the evolution tree and the stat-point system, so max-HP changes always resync Health.
    /// </summary>
    public void ApplyBonus(StatBonus bonus, object source)
    {
        GetStat(bonus.Stat).AddModifier(bonus.ToModifier(source));

        if (bonus.Stat == PlayerStatType.MaxHealth)
            RefreshMaxHealth();
    }

    private void ApplyRegen()
    {
        if (HealthRegen.Value <= 0f) return;

        _regenAccumulator += HealthRegen.Value * Time.deltaTime;
        int heal = Mathf.FloorToInt(_regenAccumulator);
        if (heal < 1) return;

        _health.Heal(heal);
        _regenAccumulator -= heal;
    }
    
    [SerializeMethod]
    private void AddMaxHealth(float amount)
    {
        MaxHealth.AddModifier(new StatModifier(amount, StatModifierType.Flat, this));
        RefreshMaxHealth();
        LogStats();
    }

    [SerializeMethod]
    private void AddHealthRegen(float amount)
    {
        HealthRegen.AddModifier(new StatModifier(amount, StatModifierType.Flat, this));
        LogStats();
    }

    [SerializeMethod]
    private void AddMoveSpeed(float percent)
    {
        MoveSpeed.AddModifier(new StatModifier(percent, StatModifierType.PercentAdd, this));
        LogStats();
    }

    [SerializeMethod]
    private void AddBulletDamage(float amount)
    {
        BulletDamage.AddModifier(new StatModifier(amount, StatModifierType.Flat, this));
        LogStats();
    }

    [SerializeMethod]
    private void AddBulletSpeed(float percent)
    {
        BulletSpeed.AddModifier(new StatModifier(percent, StatModifierType.PercentAdd, this));
        LogStats();
    }

    [SerializeMethod]
    private void AddBulletDistance(float amount)
    {
        BulletDistance.AddModifier(new StatModifier(amount, StatModifierType.Flat, this));
        LogStats();
    }

    [SerializeMethod]
    private void AddFireRate(float percent)
    {
        // Reload é cooldown em segundos: percent positivo deixa o tiro mais rápido,
        // por isso o modifier é aplicado com o sinal invertido.
        Reload.AddModifier(new StatModifier(-percent, StatModifierType.PercentAdd, this));
        LogStats();
    }

    [SerializeMethod]
    private void ResetAllModifiers()
    {
        MaxHealth.RemoveAllFromSource(this);
        HealthRegen.RemoveAllFromSource(this);
        MoveSpeed.RemoveAllFromSource(this);
        BulletDamage.RemoveAllFromSource(this);
        BulletSpeed.RemoveAllFromSource(this);
        BulletDistance.RemoveAllFromSource(this);
        Reload.RemoveAllFromSource(this);
        RefreshMaxHealth();
        LogStats();
    }

    [SerializeMethod]
    private void LogStats()
    {
        Debug.Log(
            $"[PlayerStats]\n" +
            $"  MaxHealth:      {MaxHealth.Value} (base {MaxHealth.BaseValue})\n" +
            $"  HealthRegen:    {HealthRegen.Value:F2} HP/s\n" +
            $"  MoveSpeed:      {MoveSpeed.Value:F2}\n" +
            $"  BulletDamage:   {BulletDamage.Value:F1}\n" +
            $"  BulletSpeed:    {BulletSpeed.Value:F1}\n" +
            $"  BulletDistance: {BulletDistance.Value:F1}\n" +
            $"  Reload:         {Reload.Value:F3}s"
        );
    }
}
