namespace RoguelikeGame.Models;

public class Player
{
    public int MaxHp { get; set; }
    public int CurrentHp { get; set; }
    public Weapon? Weapon { get; set; }
    public Armor? Armor { get; set; }
    public bool IsFrozen { get; set; }

    public Player(int maxHp = 100)
    {
        MaxHp = maxHp;
        CurrentHp = maxHp;
        IsFrozen = false;
    }

    public int GetAttack()
    {
        return Weapon?.Attack ?? 10;
    }

    public int GetDefense()
    {
        return Armor?.Defense ?? 5;
    }

    public void TakeDamage(int damage)
    {
        CurrentHp = Math.Max(0, CurrentHp - damage);
    }

    public void Heal(int amount)
    {
        CurrentHp = Math.Min(MaxHp, CurrentHp + amount);
    }

    public void FullHeal()
    {
        CurrentHp = MaxHp;
    }

    public bool IsAlive()
    {
        return CurrentHp > 0;
    }
}
