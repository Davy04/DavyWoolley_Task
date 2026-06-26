/// <summary>
/// Implemented by whatever owns the player's active firing behavior (the future PlayerShooter).
/// Lets the evolution system swap the weapon when a class node is chosen without depending on
/// the shooter's concrete type — the tree just asks the holder to equip a new behavior.
/// </summary>
public interface IWeaponHolder
{
    void SetWeapon(WeaponBehavior weapon);
}
