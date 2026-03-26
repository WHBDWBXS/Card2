using System.Collections.Generic;
using Godot;
using Card2.Player;

namespace Card2.Systems;

public interface IPlayerSkill
{
    string Id { get; }
    string Name { get; }
    string Description { get; }
    bool CanTrigger(PlayerController player);
    void Trigger(PlayerController player, double delta);
}

public partial class SkillManager
{
    private readonly List<IPlayerSkill> _skills = new();

    public void RegisterSkill(IPlayerSkill skill) => _skills.Add(skill);

    public void Update(PlayerController player, double delta)
    {
        foreach (var skill in _skills)
        {
            if (skill.CanTrigger(player))
            {
                skill.Trigger(player, delta);
            }
        }
    }
}

public partial class BasicHealSkill : IPlayerSkill
{
    public string Id => "basic_heal";
    public string Name => "Vital Surge";
    public string Description => "缓慢恢复少量生命值，为后续组合技能预留接口";

    public bool CanTrigger(PlayerController player)
    {
        return player.IsInsideTree() && player.Stats.CurrentHealth > 0f && player.Stats.CurrentHealth < player.Stats.MaxHealth;
    }

    public void Trigger(PlayerController player, double delta)
    {
        player.Heal((float)delta * 1.2f);
    }
}
