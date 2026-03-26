using Godot;
using Card2.Player;

namespace Card2.UI;

public partial class PlayerStatusPresenter : Node
{
    [Export] public NodePath PlayerPath { get; set; }
    [Export] public NodePath RightStaminaBarPath { get; set; }
    [Export] public NodePath HeadHealthBarPath { get; set; }
    [Export] public NodePath FootNameLabelPath { get; set; }

    private PlayerController _player;
    private ProgressBar _staminaBar;
    private ProgressBar _healthBar;
    private Label _nameLabel;

    public override void _Ready()
    {
        _player = GetNode<PlayerController>(PlayerPath);
        _staminaBar = GetNode<ProgressBar>(RightStaminaBarPath);
        _healthBar = GetNode<ProgressBar>(HeadHealthBarPath);
        _nameLabel = GetNode<Label>(FootNameLabelPath);

        _player.HealthChanged += OnHealthChanged;
        _player.StaminaChanged += OnStaminaChanged;
        _player.NameChanged += OnNameChanged;
    }

    private void OnHealthChanged(float current, float max)
    {
        _healthBar.MaxValue = max;
        _healthBar.Value = current;
    }

    private void OnStaminaChanged(float current, float max)
    {
        _staminaBar.MaxValue = max;
        _staminaBar.Value = current;
    }

    private void OnNameChanged(string displayName)
    {
        _nameLabel.Text = displayName;
    }
}
