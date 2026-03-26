using Godot;

namespace Card2.Combat;

public partial class WeaponLibrary : Node
{
    public static MeleeWeaponData[] CreateDefaultMeleeWeapons()
    {
        return new[]
        {
            new MeleeWeaponData { WeaponName = "战术匕首", Damage = 8f, Range = 52f, AttackCooldown = 0.35f },
            new MeleeWeaponData { WeaponName = "消防斧", Damage = 14f, Range = 60f, AttackCooldown = 0.6f },
            new MeleeWeaponData { WeaponName = "合金长刀", Damage = 11f, Range = 75f, AttackCooldown = 0.45f }
        };
    }

    public static RangedWeaponData[] CreateDefaultRangedWeapons()
    {
        return new[]
        {
            new RangedWeaponData { WeaponType = RangedWeaponType.Pistol, WeaponName = "P200 手枪", Damage = 12f, ProjectileSpeed = 620f, AttackCooldown = 0.24f },
            new RangedWeaponData { WeaponType = RangedWeaponType.Rifle, WeaponName = "AR-7 步枪", Damage = 18f, ProjectileSpeed = 800f, AttackCooldown = 0.14f },
            new RangedWeaponData { WeaponType = RangedWeaponType.Shotgun, WeaponName = "SG 霰弹枪", Damage = 9f, ProjectileSpeed = 560f, AttackCooldown = 0.85f, Pellets = 6, SpreadDegrees = 8f }
        };
    }
}
