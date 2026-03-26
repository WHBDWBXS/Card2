using System;
using Godot;
using Card2.Player;

namespace Card2.Combat;

public partial class CombatController : Node2D
{
    [Export] public NodePath PlayerPath { get; set; }
    [Export] public PackedScene ProjectileScene { get; set; }

    private PlayerController _player;
    private float _attackCooldown;

    public override void _Ready()
    {
        _player = GetNode<PlayerController>(PlayerPath);
    }

    public override void _Process(double delta)
    {
        _attackCooldown = MathF.Max(0f, _attackCooldown - (float)delta);

        if (Input.IsActionPressed("attack") && _attackCooldown <= 0f)
        {
            if (_player.CurrentAttackMode == AttackMode.Melee)
            {
                DoMeleeAttack();
            }
            else
            {
                DoRangedAttack();
            }
        }
    }

    private void DoMeleeAttack()
    {
        var damage = _player.GetMeleeDamage();
        var range = _player.MeleeWeapon?.Range ?? 48f;
        var cooldown = _player.MeleeWeapon?.AttackCooldown ?? 0.45f;
        _attackCooldown = cooldown / _player.Stats.AttackSpeedScale;

        foreach (var node in GetTree().GetNodesInGroup("monster"))
        {
            if (node is not Monster monster) continue;
            if (monster.GlobalPosition.DistanceTo(_player.GlobalPosition) <= range)
            {
                monster.ReceiveDamage(damage);
            }
        }
    }

    private void DoRangedAttack()
    {
        var weapon = _player.GetActiveRangedWeapon();
        if (weapon == null || ProjectileScene == null) return;

        _attackCooldown = weapon.AttackCooldown / _player.Stats.AttackSpeedScale;
        var aim = (_player.GetGlobalMousePosition() - _player.GlobalPosition).Normalized();
        if (aim == Vector2.Zero) aim = Vector2.Right;

        for (var i = 0; i < weapon.Pellets; i++)
        {
            var p = ProjectileScene.Instantiate<Projectile>();
            AddChild(p);

            var spread = weapon.Pellets == 1 ? 0f : (i - (weapon.Pellets - 1) / 2f) * weapon.SpreadDegrees;
            var direction = aim.Rotated(Mathf.DegToRad(spread));
            p.Setup(direction, weapon.ProjectileSpeed, weapon.Damage);
            p.GlobalPosition = _player.GlobalPosition;
        }
    }
}
