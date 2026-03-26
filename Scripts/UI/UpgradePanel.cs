using Godot;
using Card2.Player;

namespace Card2.UI;

public partial class UpgradePanel : Control
{
    [Export] public NodePath PlayerPath { get; set; }

    private PlayerController _player;

    public override void _Ready()
    {
        AddToGroup("upgrade_ui");
        _player = GetNode<PlayerController>(PlayerPath);
        Hide();
    }

    public void ShowUpgradePanel()
    {
        Show();
        GetTree().Paused = true;
    }

    public void SelectMoveSpeed()
    {
        _player.Stats.AddMoveSpeed(20f);
        Close();
    }

    public void SelectAttackPower()
    {
        _player.Stats.AddAttack(6f);
        Close();
    }

    public void SelectAttackSpeed()
    {
        _player.Stats.AddAttackSpeed(0.2f);
        Close();
    }

    public void SelectDashDistance()
    {
        _player.Stats.AddDashDistance(30f);
        Close();
    }

    private void Close()
    {
        Hide();
        GetTree().Paused = false;
    }
}
