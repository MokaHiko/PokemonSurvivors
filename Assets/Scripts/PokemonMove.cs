using UnityEngine;

[CreateAssetMenu(menuName = "PokemonMove", fileName = "move")]
public class PokemonMove: ScriptableObject
{
    public enum PokemonDamageClass
    {
        Unknown,
        Special,
        Physical,
        Status
    }

    public string Name;
    public PokemonType Type;
    public PokemonDamageClass DamageClass;

    public int Damage;
    public float Accuracy;

    public string AnimationName;
    public float AnimationDuration;
}
