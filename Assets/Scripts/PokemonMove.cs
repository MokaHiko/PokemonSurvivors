using UnityEngine;

[CreateAssetMenu(menuName = "PokemonMove", fileName = "move")]
public class PokemonMove: ScriptableObject
{
    public string Name;
    public int Damage;
    public float Accuracy;

    public string AnimationName;
    public float AnimationDuration;
}
