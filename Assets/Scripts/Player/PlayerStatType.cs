/// <summary>
/// Catalog of the player's modifiable stats. Lets data assets (evolution nodes,
/// upgrade cards) point at a specific stat without holding a direct reference to it.
/// </summary>
public enum PlayerStatType
{
    MaxHealth,
    HealthRegen,
    MoveSpeed,
    BulletSpeed,
    BulletDamage,
    BulletDistance,
    Reload
}
