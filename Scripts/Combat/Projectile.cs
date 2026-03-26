using Godot;

namespace Card2.Combat;

public partial class Projectile : Area2D
{
    private Vector2 _dir = Vector2.Right;
    private float _speed = 400f;
    private float _damage = 10f;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition += _dir * _speed * (float)delta;
    }

    public void Setup(Vector2 direction, float speed, float damage)
    {
        _dir = direction.Normalized();
        _speed = speed;
        _damage = damage;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Monster monster)
        {
            monster.ReceiveDamage(_damage);
            QueueFree();
        }
    }
}
