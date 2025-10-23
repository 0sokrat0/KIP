namespace RoguelikeGame.Models;

public class VVG : Enemy
{
    public VVG() : base("ВВГ (Гоблин-Босс)", 100, 22, 9, 30)
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

public class Kovalsky : Enemy
{
    public Kovalsky() : base("Ковальский (Скелет-Босс)", 150, 23, 8, 0, 0, true)
    {
    }

    public override int AttackPlayer(Player player, Random random)
    {
        int damage = Attack;
        Console.WriteLine($"{Name} атакует, игнорируя защиту! Урон: {damage}");
        return damage;
    }
}

public class ArchmageCpp : Enemy
{
    public ArchmageCpp() : base("Архимаг C++ (Маг-Босс)", 72, 32, 4, 0, 35)
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

public class PestovC : Enemy
{
    public PestovC() : base("Пестов С-- (Скелет-Босс)", 78, 32, 3, 0, 40, true)
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
            Console.WriteLine($"{Name} атакует, игнорируя защиту! Урон: {damage}");
        }

        return damage;
    }
}
