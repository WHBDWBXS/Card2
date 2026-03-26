using System;
using Godot;
using Card2.Combat;
using Card2.Systems;

namespace Card2.Player;

public partial class PlayerController : CharacterBody2D
{
    // 定义三个信号
    [Signal] public delegate void HealthChangedEventHandler(float current, float max);
    [Signal] public delegate void StaminaChangedEventHandler(float current, float max);
    [Signal] public delegate void NameChangedEventHandler(string displayName);
    // 玩家名称
    [Export] public string DisplayName { get; set; } = "Hunter";
    // 玩家近战武器 和 远程武器组
    [ExportGroup("Combat")]
    [Export] public MeleeWeaponData MeleeWeapon { get; set; }
    [Export] public Godot.Collections.Array<RangedWeaponData> RangedWeapons { get; set; } = new();
    // Ui路径
    [ExportGroup("UI NodePath")]
    [Export] public NodePath NameLabelPath { get; set; }
    // 玩家数据  当前攻击模式 当前激活的远程武器
    public PlayerStats Stats { get; } = new();
    public AttackMode CurrentAttackMode { get; private set; } = AttackMode.Melee;
    public int ActiveRangedIndex { get; private set; }
    // 技能管理器实例
    private SkillManager _skillManager = new();
    // 无敌计时器 记录距离上一次受伤的时间  无敌效果的冷却时间
    private float _invincibleDuration = 0.2f;
    private float _invincibleTimer;
    private float _hurtWindowDuration = 5f;
    private float _hurtWindowTimer;
    // 最后位移方向
    private bool _isDashing;
    private Vector2 _dashDir = Vector2.Right;
    // 初始化
    public override void _Ready()
    {
        // 注册一个治疗技能
        _skillManager.RegisterSkill(new BasicHealSkill());
        // 安全检测 初始化 默认近战武器（如果编辑器没有配置）
        if (MeleeWeapon == null)
        {
            MeleeWeapon = WeaponLibrary.CreateDefaultMeleeWeapons()[0];
        }
        // 同上 远程武器
        if (RangedWeapons.Count == 0)
        {
            foreach (var ranged in WeaponLibrary.CreateDefaultRangedWeapons())
            {
                RangedWeapons.Add(ranged);
            }
        }
        // 触发信号 初始化UI
        EmitSignal(SignalName.NameChanged, DisplayName);
        EmitSignal(SignalName.HealthChanged, Stats.CurrentHealth, Stats.MaxHealth);
        EmitSignal(SignalName.StaminaChanged, Stats.CurrentStamina, Stats.MaxStamina);
    }
    // 物理帧运行时
    public override void _PhysicsProcess(double delta)
    {
        HandleTimers((float)delta);        // 更新无敌计时器和受伤时间计时器
        HandleMovement((float)delta);      // 处理移动逻辑
        HandleAttackSwitch();              // 处理近战攻击模式切换
        HandleWeaponSwitch();              // 处理远程武器切换
        _skillManager.Update(this, delta); // 更新技能系统
    }
    // 更新无敌计时器和受伤时间计时器
    private void HandleTimers(float delta)
    {
        _invincibleTimer = MathF.Max(0f, _invincibleTimer - delta);
        _hurtWindowTimer = MathF.Max(0f, _hurtWindowTimer - delta);
    }
    // 处理移动逻辑
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
    // 处理近战攻击模式切换
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
    // 处理远程武器切换
    private void HandleWeaponSwitch()
    {
        if (RangedWeapons.Count == 0) return;

        if (Input.IsActionJustPressed("next_gun"))
        {
            ActiveRangedIndex = (ActiveRangedIndex + 1) % RangedWeapons.Count;
        }
    }
    // 更新技能系统
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
    // 受伤方法
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
    // 治疗方法
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
    // 获取经验与金币的方法
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
    // 是否在无敌时间冷却中
    public bool IsInHurtWindow() => _hurtWindowTimer > 0f;
}
