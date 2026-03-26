using Godot;
using Card2.Player;

namespace Card2.Combat;

public partial class ExperienceCrystal : Area2D
{
    [Export] public int ExpValue { get; set; } = 8;
    [Export] public int GoldValue { get; set; } = 3;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is PlayerController player)
        {
            player.GainRewards(ExpValue, GoldValue);
            QueueFree();
        }
    }
}
