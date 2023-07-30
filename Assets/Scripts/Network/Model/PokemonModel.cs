[System.Serializable]
public class PokemonMoveNetworkModel
{
    public string Name;
    public int Damage;
    public int Accuracy;
};

[System.Serializable]
public class PokemonNetworkModel
{
    public string Name;
    public string FrontSpriteUrl;
    public string BackSpriteUrl;
    public int Hp;
    public int Attack;
    public int Defense;
    public int SpecialAttack;
    public int SpecialDefense;
    public int Speed;

    public PokemonMoveNetworkModel[] Moves;
}
