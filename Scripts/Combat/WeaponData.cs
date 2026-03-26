using Godot;

namespace Card2.Combat;

public enum AttackMode
{
    Melee,
    Ranged
}

public enum RangedWeaponType
{
    Pistol,
    Rifle,
    Shotgun
}

[GlobalClass]
public partial class MeleeWeaponData : Resource
{
    [Export] public string WeaponName { get; set; } = "Rusty Knife";
    [Export] public float Damage { get; set; } = 8f;
    [Export] public float Range { get; set; } = 56f;
    [Export] public float AttackCooldown { get; set; } = 0.45f;
}

[GlobalClass]
public partial class RangedWeaponData : Resource
{
    [Export] public RangedWeaponType WeaponType { get; set; } = RangedWeaponType.Pistol;
    [Export] public string WeaponName { get; set; } = "Pistol";
    [Export] public float Damage { get; set; } = 12f;
    [Export] public float ProjectileSpeed { get; set; } = 560f;
    [Export] public float AttackCooldown { get; set; } = 0.25f;
    [Export] public int Pellets { get; set; } = 1;
    [Export] public float SpreadDegrees { get; set; } = 0f;
}
