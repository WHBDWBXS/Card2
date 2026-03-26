using Godot;
using Card2.Player;
using Card2.Combat;

namespace Card2.World;

public partial class MonsterSpawner : Node2D
{
    [Export] public PackedScene MonsterScene { get; set; }
    [Export] public NodePath PlayerPath { get; set; }
    [Export] public Rect2 MapBounds { get; set; } = new(new Vector2(-1200f, -700f), new Vector2(2400f, 1400f));
    [Export] public float SpawnRadiusMin { get; set; } = 320f;
    [Export] public float SpawnRadiusMax { get; set; } = 520f;
    [Export] public float SpawnInterval { get; set; } = 1.2f;

    private PlayerController _player;
    private float _timer;
    private readonly RandomNumberGenerator _rng = new();

    public override void _Ready()
    {
        _player = GetNode<PlayerController>(PlayerPath);
    }

    public override void _Process(double delta)
    {
        _timer += (float)delta;
        if (_timer >= SpawnInterval)
        {
            _timer = 0f;
            TrySpawnOne();
        }
    }

    private void TrySpawnOne()
    {
        if (_player == null || MonsterScene == null) return;

        var angle = _rng.RandfRange(0f, Mathf.Tau);
        var radius = _rng.RandfRange(SpawnRadiusMin, SpawnRadiusMax);
        var pos = _player.GlobalPosition + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

        pos.X = Mathf.Clamp(pos.X, MapBounds.Position.X, MapBounds.End.X);
        pos.Y = Mathf.Clamp(pos.Y, MapBounds.Position.Y, MapBounds.End.Y);

        var monster = MonsterScene.Instantiate<Monster>();
        monster.GlobalPosition = pos;
        AddChild(monster);
    }
}
