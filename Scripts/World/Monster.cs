using System;
using Godot;
using Card2.Player;

namespace Card2.Combat;

public partial class Monster : CharacterBody2D
{
    [Export] public float Health { get; set; } = 30f;
    [Export] public float MoveSpeed { get; set; } = 80f;
    [Export] public float AttackDamage { get; set; } = 12f;
    [Export] public float KnockbackPower { get; set; } = 60f;
    [Export] public float AttackRange { get; set; } = 24f;
    [Export] public PackedScene CrystalDropScene { get; set; }

    private PlayerController _player;
    private float _attackCooldown;

    public override void _Ready()
    {
        AddToGroup("monster");
        _player = GetTree().GetFirstNodeInGroup("player") as PlayerController;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_player == null || _player.Stats.CurrentHealth <= 0f) return;

        var toPlayer = (_player.GlobalPosition - GlobalPosition);
        Velocity = toPlayer.Normalized() * MoveSpeed;
        MoveAndSlide();

        _attackCooldown = MathF.Max(0f, _attackCooldown - (float)delta);
        if (toPlayer.Length() <= AttackRange && _attackCooldown <= 0f)
        {
            _player.ReceiveDamage(AttackDamage);
            _player.ApplyKnockback((_player.GlobalPosition - GlobalPosition).Normalized(), KnockbackPower);
            _attackCooldown = 1f;
        }
    }

    public void ReceiveDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0f)
        {
            DropRewards();
            QueueFree();
        }
    }

    private void DropRewards()
    {
        if (CrystalDropScene == null) return;
        var crystal = CrystalDropScene.Instantiate<ExperienceCrystal>();
        crystal.GlobalPosition = GlobalPosition;
        GetTree().CurrentScene.AddChild(crystal);
    }
}
