using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pokemon", fileName = "pokemon")]
public class Pokemon : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public int Hp;
    public int Attack;
    public int Defense;
    public int SpecialAttack;
    public int SpecialDefense;
    public int Speed;

    public List<PokemonMove> Moves = new List<PokemonMove>(4);
}
