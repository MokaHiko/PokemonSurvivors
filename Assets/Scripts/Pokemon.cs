using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Flags]
[Serializable]
public enum PokemonType : UInt32
{
    Unknown = 0,
    Normal = 1 << 1,  
    Fire = 1 << 2, 
    Water = 1 << 3,  
    Grass = 1 << 4,  
    Electric = 1 << 5, 
    Ice = 1 << 6, 
    Fighting = 1 << 7,  
    Poison = 1 << 8, 
    Ground = 1 << 9, 
    Flying = 1 << 10, 
    Psychic = 1 << 11,  
    Bug = 1 << 12, 
    Rock = 1 << 13, 
    Ghost = 1 << 14, 
    Dark = 1 << 15, 
    Dragon = 1 << 16, 
    Steel = 1 << 17, 
    Fairy = 1 << 18
}

[Serializable]
public class IVS
{
    public int Hp;
    public int Attack;
    public int Defense;
    public int SpecialAttack;
    public int SpecialDefense;
    public int Speed;
}

[Serializable]
public class EVS
{
    public int Hp;
    public int Attack;
    public int Defense;
    public int SpecialAttack;
    public int SpecialDefense;
    public int Speed;
}

[Serializable]
public class Pokemon
{
    public string Name;
    public int Level = 100;
    public Sprite Sprite;

    public IVS Ivs = new IVS();
    public EVS Evs = new EVS();

    public List<PokemonType> Types = new List<PokemonType>();
    public int Hp
    {
        get { return (2 * _hp + Ivs.Hp + (Evs.Hp/4) * Level) + Level + 10; }
        set { _hp = value; }
    }

    public int Attack
    {
        get { return ((2 * _attack + Ivs.Attack + (Evs.Attack/4) * Level) / 100) + 5; }
        set { _attack = value; }
    }

    public int Defense
    {
        get { return ((2 * _defense + Ivs.Defense + (Evs.Defense/4) * Level) / 100) + 5; }
        set { _defense = value; }
    }

    public int SpecialAttack
    {
        get { return ((2 * _specialAttack + Ivs.SpecialAttack + (Evs.SpecialAttack/4) * Level) / 100) + 5; }
        set { _specialAttack = value; }
    }

    public int SpecialDefense
    {
        get { return ((2 * _specialDefense + Ivs.SpecialDefense + (Evs.SpecialDefense/4) * Level) / 100) + 5; }
        set { _specialDefense = value; }
    }
    public int Speed
    {
        get { return ((2 * _speed + Ivs.Speed + (Evs.Speed/4) * Level) / 100) + 5; }
        set { _speed = value; }
    }

    public float CurrentHp;
    public bool IsDead = false;

    public List<PokemonMove> Moves = new List<PokemonMove>(4);

    // Base stats
    private int _hp;
    private int _attack;
    private int _defense;
    private int _specialAttack;
    private int _specialDefense;
    private int _speed;
}

