using RoguelikeGame.Models;

namespace RoguelikeGame.Services;

public class GameService
{
    private readonly Random _random;
    private readonly Player _player;
    private int _turnCount;

    public GameService()
    {
        _random = new Random();
        _player = new Player();
        _turnCount = 0;
    }

    public void StartGame()
    {
        Console.WriteLine("=== ТЕКСТОВАЯ ИГРА-РОГАЛИК ===");
        Console.WriteLine("Добро пожаловать в подземелье!");
        Console.WriteLine("На каждом ходу вас ждет либо сундук с сокровищами, либо опасный враг.");
        Console.WriteLine("Каждые 10 ходов вас ждет встреча с боссом!");
        Console.WriteLine();

        while (_player.IsAlive())
        {
            _turnCount++;
            Console.WriteLine($"=== ХОД {_turnCount} ===");
            DisplayPlayerStatus();
            Console.WriteLine();

            if (_turnCount % 10 == 0)
            {
                Console.WriteLine("🔥 ВСТРЕЧА С БОССОМ! 🔥");
                var boss = GetRandomBoss();
                if (!FightBoss(boss))
                {
                    GameOver();
                    return;
                }
            }
            else
            {
                if (_random.Next(1, 3) == 1)
                {
                    Console.WriteLine("📦 Вы нашли сундук!");
                    OpenChest();
                }
                else
                {
                    Console.WriteLine("⚔️ Встреча с врагом!");
                    var enemy = GetRandomEnemy();
                    if (!FightEnemy(enemy))
                    {
                        GameOver();
                        return;
                    }
                }
            }

            if (_player.IsFrozen)
            {
                Console.WriteLine("❄️ Вы заморожены и пропускаете ход!");
                _player.IsFrozen = false;
            }

            Console.WriteLine();
            Console.WriteLine("Нажмите Enter для продолжения...");
            Console.ReadLine();
            Console.Clear();
        }
    }

    private void DisplayPlayerStatus()
    {
        Console.WriteLine($"❤️ Здоровье: {_player.CurrentHp}/{_player.MaxHp}");
        Console.WriteLine($"⚔️ Атака: {_player.GetAttack()}");
        Console.WriteLine($"🛡️ Защита: {_player.GetDefense()}");
        Console.WriteLine($"🗡️ Оружие: {(_player.Weapon?.Name ?? "Нет")}");
        Console.WriteLine($"🛡️ Доспехи: {(_player.Armor?.Name ?? "Нет")}");
    }

    private Enemy GetRandomEnemy()
    {
        var enemies = new List<Enemy> { new Goblin(), new Skeleton(), new Mage() };
        return enemies[_random.Next(enemies.Count)];
    }

    private Enemy GetRandomBoss()
    {
        var bosses = new List<Enemy> { new VVG(), new Kovalsky(), new ArchmageCpp(), new PestovC() };
        return bosses[_random.Next(bosses.Count)];
    }

    private bool FightEnemy(Enemy enemy)
    {
        Console.WriteLine($"Встречен враг: {enemy.Name}");
        Console.WriteLine($"Здоровье врага: {enemy.CurrentHp}");
        Console.WriteLine();

        while (_player.IsAlive() && enemy.IsAlive())
        {
            if (_player.IsFrozen)
            {
                Console.WriteLine("❄️ Вы заморожены и не можете действовать!");
                _player.IsFrozen = false;
            }
            else
            {
                PlayerTurn(enemy);
            }

            if (!enemy.IsAlive())
            {
                Console.WriteLine($"🎉 Вы победили {enemy.Name}!");
                return true;
            }

            EnemyTurn(enemy);
        }

        return _player.IsAlive();
    }

    private bool FightBoss(Enemy boss)
    {
        Console.WriteLine($"🔥 ВСТРЕЧА С БОССОМ: {boss.Name} 🔥");
        Console.WriteLine($"Здоровье босса: {boss.CurrentHp}");
        Console.WriteLine();

        while (_player.IsAlive() && boss.IsAlive())
        {
            if (_player.IsFrozen)
            {
                Console.WriteLine("❄️ Вы заморожены и не можете действовать!");
                _player.IsFrozen = false;
            }
            else
            {
                PlayerTurn(boss);
            }

            if (!boss.IsAlive())
            {
                Console.WriteLine($"🏆 ВЫ ПОБЕДИЛИ БОССА {boss.Name}! 🏆");
                return true;
            }

            EnemyTurn(boss);
        }

        return _player.IsAlive();
    }

    private bool _isDefending = false;

    private void PlayerTurn(Enemy enemy)
    {
        Console.WriteLine("Выберите действие:");
        Console.WriteLine("1. Атаковать");
        Console.WriteLine("2. Защищаться");

        while (true)
        {
            var choice = Console.ReadLine();
            if (choice == "1")
            {
                AttackEnemy(enemy);
                _isDefending = false;
                break;
            }
            else if (choice == "2")
            {
                Defend();
                break;
            }
            else
            {
                Console.WriteLine("Введите 1 или 2");
            }
        }
    }

    private void AttackEnemy(Enemy enemy)
    {
        int damage = _player.GetAttack();
        Console.WriteLine($"Вы атакуете на {damage} урона!");
        enemy.TakeDamage(damage);
        Console.WriteLine($"У врага осталось {enemy.CurrentHp} здоровья");
    }

    private void Defend()
    {
        Console.WriteLine("Вы принимаете защитную стойку!");
        _isDefending = true;
    }

    private void EnemyTurn(Enemy enemy)
    {
        int damage = enemy.AttackPlayer(_player, _random);
        
        if (_player.IsFrozen)
        {
            return;
        }

        int finalDamage = CalculateDamage(damage, enemy);
        if (finalDamage > 0)
        {
            _player.TakeDamage(finalDamage);
            Console.WriteLine($"Вы получаете {finalDamage} урона!");
            Console.WriteLine($"У вас осталось {_player.CurrentHp} здоровья");
        }
        else
        {
            Console.WriteLine("Вы уклонились от атаки!");
        }
    }

    private int CalculateDamage(int baseDamage, Enemy enemy)
    {
        if (_isDefending)
        {
            if (_random.Next(1, 101) <= 40)
            {
                Console.WriteLine("Вы полностью уклонились от атаки!");
                _isDefending = false;
                return 0;
            }
            else
            {
                Console.WriteLine("Уклонение не удалось, но защита сработала!");
                if (enemy.IgnoresDefense)
                {
                    Console.WriteLine("Но враг игнорирует защиту!");
                    _isDefending = false;
                    return baseDamage;
                }
                int defense = _player.GetDefense();
                int blockChanceDefend = _random.Next(70, 101);
                int blockAmountDefend = (int)(defense * blockChanceDefend / 100.0);
                _isDefending = false;
                return Math.Max(0, baseDamage - blockAmountDefend);
            }
        }

        if (enemy.IgnoresDefense)
        {
            return baseDamage;
        }

        int playerDefense = _player.GetDefense();
        int blockChance = _random.Next(70, 101);
        int blockAmount = (int)(playerDefense * blockChance / 100.0);
        
        return Math.Max(0, baseDamage - blockAmount);
    }

    private void OpenChest()
    {
        var item = GetRandomItem();
        
        if (item is HealthPotion potion)
        {
            Console.WriteLine($"Вы получили: {potion.Name}");
            _player.FullHeal();
            Console.WriteLine("Ваше здоровье полностью восстановлено!");
        }
        else if (item is Weapon weapon)
        {
            Console.WriteLine($"Вы получили оружие: {weapon.Name}");
            Console.WriteLine($"Атака: {weapon.Attack}, Крит. шанс: {weapon.CriticalChance}%");
            
            if (_player.Weapon != null)
            {
                Console.WriteLine($"Текущее оружие: {_player.Weapon.Name} (Атака: {_player.Weapon.Attack})");
                Console.WriteLine("Заменить оружие? (y/n)");
                
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    _player.Weapon = weapon;
                    Console.WriteLine($"Оружие заменено на {weapon.Name}!");
                }
                else
                {
                    Console.WriteLine("Оружие выброшено.");
                }
            }
            else
            {
                _player.Weapon = weapon;
                Console.WriteLine($"Оружие экипировано: {weapon.Name}!");
            }
        }
        else if (item is Armor armor)
        {
            Console.WriteLine($"Вы получили доспехи: {armor.Name}");
            Console.WriteLine($"Защита: {armor.Defense}");
            
            if (_player.Armor != null)
            {
                Console.WriteLine($"Текущие доспехи: {_player.Armor.Name} (Защита: {_player.Armor.Defense})");
                Console.WriteLine("Заменить доспехи? (y/n)");
                
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    _player.Armor = armor;
                    Console.WriteLine($"Доспехи заменены на {armor.Name}!");
                }
                else
                {
                    Console.WriteLine("Доспехи выброшены.");
                }
            }
            else
            {
                _player.Armor = armor;
                Console.WriteLine($"Доспехи экипированы: {armor.Name}!");
            }
        }
    }

    private Item GetRandomItem()
    {
        int itemType = _random.Next(1, 4);
        
        return itemType switch
        {
            1 => new HealthPotion(),
            2 => GetRandomWeapon(),
            3 => GetRandomArmor(),
            _ => new HealthPotion()
        };
    }

    private Weapon GetRandomWeapon()
    {
        var weapons = new List<Weapon>
        {
            new("Ржавый меч", 12, 5),
            new("Боевой топор", 18, 15),
            new("Магический посох", 15, 25),
            new("Кинжал убийцы", 20, 30),
            new("Легендарный клинок", 25, 40)
        };
        
        return weapons[_random.Next(weapons.Count)];
    }

    private Armor GetRandomArmor()
    {
        var armors = new List<Armor>
        {
            new("Кожаная броня", 8),
            new("Кольчуга", 12),
            new("Латная броня", 18),
            new("Магические доспехи", 15),
            new("Легендарная броня", 25)
        };
        
        return armors[_random.Next(armors.Count)];
    }

    private void GameOver()
    {
        Console.WriteLine();
        Console.WriteLine("💀 ИГРА ОКОНЧЕНА 💀");
        Console.WriteLine($"Вы продержались {_turnCount} ходов");
        Console.WriteLine("Спасибо за игру!");
    }
}
