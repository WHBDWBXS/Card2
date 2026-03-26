using System;
using Godot;
using Card2.Combat;
using Card2.Systems;

namespace Card2.Player;

public partial class PlayerController : CharacterBody2D
{
    [Signal] public delegate void HealthChangedEventHandler(float current, float max);
    [Signal] public delegate void StaminaChangedEventHandler(float current, float max);
    [Signal] public delegate void NameChangedEventHandler(string displayName);

    [Export] public string DisplayName { get; set; } = "Hunter";

    [ExportGroup("Combat")]
    [Export] public MeleeWeaponData MeleeWeapon { get; set; }
    [Export] public Godot.Collections.Array<RangedWeaponData> RangedWeapons { get; set; } = new();

    [ExportGroup("UI NodePath")]
    [Export] public NodePath NameLabelPath { get; set; }

    public PlayerStats Stats { get; } = new();
    public AttackMode CurrentAttackMode { get; private set; } = AttackMode.Melee;
    public int ActiveRangedIndex { get; private set; }

    private SkillManager _skillManager = new();

    private float _invincibleDuration = 0.2f;
    private float _invincibleTimer;
    private float _hurtWindowDuration = 5f;
    private float _hurtWindowTimer;

    private bool _isDashing;
    private Vector2 _dashDir = Vector2.Right;

    public override void _Ready()
    {
        _skillManager.RegisterSkill(new BasicHealSkill());
        if (MeleeWeapon == null)
        {
            MeleeWeapon = WeaponLibrary.CreateDefaultMeleeWeapons()[0];
        }

        if (RangedWeapons.Count == 0)
        {
            foreach (var ranged in WeaponLibrary.CreateDefaultRangedWeapons())
            {
                RangedWeapons.Add(ranged);
            }
        }

        EmitSignal(SignalName.NameChanged, DisplayName);
        EmitSignal(SignalName.HealthChanged, Stats.CurrentHealth, Stats.MaxHealth);
        EmitSignal(SignalName.StaminaChanged, Stats.CurrentStamina, Stats.MaxStamina);
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleTimers((float)delta);
        HandleMovement((float)delta);
        HandleAttackSwitch();
        HandleWeaponSwitch();
        _skillManager.Update(this, delta);
    }

    private void HandleTimers(float delta)
    {
        _invincibleTimer = MathF.Max(0f, _invincibleTimer - delta);
        _hurtWindowTimer = MathF.Max(0f, _hurtWindowTimer - delta);
    }

    private void HandleMovement(float delta)
    {
        Vector2 input = Input.GetVector("move_left", "move_right", "move_up", "move_down").Normalized();
        if (input != Vector2.Zero)
        {
            _dashDir = input;
        }

        var speed = Stats.MoveSpeed;

        if (Input.IsActionPressed("run") && input != Vector2.Zero)
        {
            if (Stats.TryConsumeRunStamina(delta))
            {
                speed *= Stats.RunMultiplier;
            }
        }
        else
        {
            Stats.RecoverStamina(delta);
        }

        if (Input.IsActionJustPressed("dash") && Stats.TryConsumeDashStamina())
        {
            _isDashing = true;
            GlobalPosition += _dashDir * Stats.DashDistance;
        }

        Velocity = input * speed;
        MoveAndSlide();

        _isDashing = false;
        EmitSignal(SignalName.StaminaChanged, Stats.CurrentStamina, Stats.MaxStamina);
    }

    private void HandleAttackSwitch()
    {
        if (Input.IsActionPressed("switch_mode"))
        {
            CurrentAttackMode = AttackMode.Ranged;
        }
        if (Input.IsActionJustReleased("switch_mode"))
        {
            CurrentAttackMode = AttackMode.Melee;
        }
    }

    private void HandleWeaponSwitch()
    {
        if (RangedWeapons.Count == 0) return;

        if (Input.IsActionJustPressed("next_gun"))
        {
            ActiveRangedIndex = (ActiveRangedIndex + 1) % RangedWeapons.Count;
        }
    }

    public float GetMeleeDamage()
    {
        var weaponDamage = MeleeWeapon?.Damage ?? 0f;
        return Stats.BaseAttack + weaponDamage;
    }

    public RangedWeaponData GetActiveRangedWeapon()
    {
        if (RangedWeapons.Count == 0) return null;
        return RangedWeapons[ActiveRangedIndex];
    }

    public void ApplyKnockback(Vector2 direction, float power)
    {
        GlobalPosition += direction.Normalized() * power;
    }

    public void ReceiveDamage(float amount)
    {
        if (_invincibleTimer > 0f) return;

        Stats.TakeDamage(amount);
        EmitSignal(SignalName.HealthChanged, Stats.CurrentHealth, Stats.MaxHealth);

        _invincibleTimer = _invincibleDuration;
        if (_hurtWindowTimer <= 0f)
        {
            _hurtWindowTimer = _hurtWindowDuration;
        }
    }

    public void Heal(float value)
    {
        if (value <= 0f) return;
        var old = Stats.CurrentHealth;
        Stats.Heal(value);
        if (Stats.CurrentHealth > old)
        {
            EmitSignal(SignalName.HealthChanged, Stats.CurrentHealth, Stats.MaxHealth);
        }
    }

    public void GainRewards(int exp, int gold)
    {
        Stats.GainCurrency(exp, gold);
        if (Stats.CanLevelUp())
        {
            Stats.LevelUpBase();
            EmitSignal(SignalName.HealthChanged, Stats.CurrentHealth, Stats.MaxHealth);
            GetTree().CallGroup("upgrade_ui", "ShowUpgradePanel");
        }
    }

    public bool IsInHurtWindow() => _hurtWindowTimer > 0f;
}
