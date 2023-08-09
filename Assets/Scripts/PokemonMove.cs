using UnityEngine;

[CreateAssetMenu(menuName = "PokemonMove", fileName = "move")]
public class PokemonMove: ScriptableObject
{
    public enum MoveType
    {
        Unknown,
        Special,
        Physical
    }

    public string Name;
    public int Damage;
    public float Accuracy;
    public MoveType Type = MoveType.Unknown;

    public string AnimationName;
    public float AnimationDuration;
}
