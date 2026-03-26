using System;

namespace Card2.Player;

public partial class PlayerStats
{
    public float MaxHealth { get; private set; } = 100f;
    public float CurrentHealth { get; private set; } = 100f;

    public float BaseAttack { get; private set; } = 15f;
    public float MoveSpeed { get; private set; } = 170f;
    public float RunMultiplier { get; private set; } = 1.8f;

    public float MaxStamina { get; private set; } = 100f;
    public float CurrentStamina { get; private set; } = 100f;
    public float StaminaRunCostPerSecond { get; private set; } = 18f;
    public float StaminaDashCost { get; private set; } = 20f;
    public float StaminaRecoverPerSecond { get; private set; } = 10f;

    public float DashDistance { get; private set; } = 140f;
    public float AttackSpeedScale { get; private set; } = 1f;

    public int Level { get; private set; } = 1;
    public int Gold { get; private set; }
    public int Experience { get; private set; }
    public int NextLevelExp { get; private set; } = 50;

    public void RecoverStamina(float delta) => CurrentStamina = MathF.Min(CurrentStamina + StaminaRecoverPerSecond * delta, MaxStamina);

    public bool TryConsumeRunStamina(float delta)
    {
        var cost = StaminaRunCostPerSecond * delta;
        if (CurrentStamina < cost) return false;
        CurrentStamina -= cost;
        return true;
    }

    public bool TryConsumeDashStamina()
    {
        if (CurrentStamina < StaminaDashCost) return false;
        CurrentStamina -= StaminaDashCost;
        return true;
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth = MathF.Max(CurrentHealth - amount, 0f);
    }

    public void Heal(float amount)
    {
        CurrentHealth = MathF.Min(CurrentHealth + amount, MaxHealth);
    }

    public void GainCurrency(int exp, int gold)
    {
        Experience += exp;
        Gold += gold;
    }

    public bool CanLevelUp() => Experience >= NextLevelExp;

    public void LevelUpBase()
    {
        Experience -= NextLevelExp;
        Level++;
        NextLevelExp = (int)(NextLevelExp * 1.35f);
        MaxHealth += 15f;
        CurrentHealth = MaxHealth;
        BaseAttack += 4f;
    }

    public void AddMoveSpeed(float value) => MoveSpeed += value;
    public void AddAttack(float value) => BaseAttack += value;
    public void AddAttackSpeed(float value) => AttackSpeedScale += value;
    public void AddDashDistance(float value) => DashDistance += value;
}
