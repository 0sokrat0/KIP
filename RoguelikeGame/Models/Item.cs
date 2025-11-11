namespace RoguelikeGame.Models;

public abstract class Item
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class Weapon : Item
{
    public int Attack { get; set; }
    public int CriticalChance { get; set; }

    public Weapon(string name, int attack, int criticalChance = 0)
    {
        Name = name;
        Attack = attack;
        CriticalChance = criticalChance;
    }
}

public class Armor : Item
{
    public int Defense { get; set; }

    public Armor(string name, int defense)
    {
        Name = name;
        Defense = defense;
    }
}

public class HealthPotion : Item
{
    public int HealAmount { get; set; }

    public HealthPotion(int healAmount = 100)
    {
        Name = "Лечебное зелье";
        Description = "Полностью восстанавливает здоровье";
        HealAmount = healAmount;
    }
}




