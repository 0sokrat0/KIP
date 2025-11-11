namespace RoguelikeGame.Models;

public abstract class Enemy
{
    public string Name { get; set; } = string.Empty;
    public int MaxHp { get; set; }
    public int CurrentHp { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int CriticalChance { get; set; }
    public int FreezeChance { get; set; }
    public bool IgnoresDefense { get; set; }

    protected Enemy(string name, int hp, int attack, int defense, int criticalChance = 0, int freezeChance = 0, bool ignoresDefense = false)
    {
        Name = name;
        MaxHp = hp;
        CurrentHp = hp;
        Attack = attack;
        Defense = defense;
        CriticalChance = criticalChance;
        FreezeChance = freezeChance;
        IgnoresDefense = ignoresDefense;
    }

    public void TakeDamage(int damage)
    {
        CurrentHp = Math.Max(0, CurrentHp - damage);
    }

    public bool IsAlive()
    {
        return CurrentHp > 0;
    }

    public abstract int AttackPlayer(Player player, Random random);
}

public class Goblin : Enemy
{
    public Goblin() : base("Гоблин", 50, 15, 8, 20)
    {
    }

    public override int AttackPlayer(Player player, Random random)
    {
        int damage = Attack;
        
        if (random.Next(1, 101) <= CriticalChance)
        {
            damage = (int)(damage * 1.5);
            Console.WriteLine($"{Name} наносит критический удар! Урон: {damage}");
        }
        else
        {
            Console.WriteLine($"{Name} атакует с уроном: {damage}");
        }

        return damage;
    }
}

public class Skeleton : Enemy
{
    public Skeleton() : base("Скелет", 60, 18, 6, 0, 0, true)
    {
    }

    public override int AttackPlayer(Player player, Random random)
    {
        int damage = Attack;
        Console.WriteLine($"{Name} атакует, игнорируя защиту! Урон: {damage}");
        return damage;
    }
}

public class Mage : Enemy
{
    public Mage() : base("Маг", 40, 20, 4, 0, 25)
    {
    }

    public override int AttackPlayer(Player player, Random random)
    {
        int damage = Attack;
        
        if (random.Next(1, 101) <= FreezeChance)
        {
            player.IsFrozen = true;
            Console.WriteLine($"{Name} замораживает игрока! Пропуск следующего хода.");
        }
        else
        {
            Console.WriteLine($"{Name} атакует с уроном: {damage}");
        }

        return damage;
    }
}




