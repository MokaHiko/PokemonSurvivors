using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PokemonMove;

public class Trainer : MonoBehaviour
{
    [SerializeField]
    public BattleRoomManager BattleRoomManager;

    [SerializeField]
    public List<Pokemon> Pokemons;

    [SerializeField]
    public SpriteRenderer PokemonSprite;

    [SerializeField]
    public Pokemon CurrentPokemon;

    [SerializeField]
    public int CurrentPokemonIndex;

    public IEnumerator Init()
    {
        if(Pokemons.Count > 0)
        {
            yield return StartCoroutine(SwitchPokemon(0));
        }
    }

    public void QueueAttack(Trainer target, PokemonMove move)
    { 
        BattleRoomManager.QueueAttack(this, move);
    }

    public void QueueSwitch(int pokemonIndex)
    { 
        BattleRoomManager.QueueSwitch(this, pokemonIndex);
    }

    public void QueueFaint(int pokemonIndex) 
    {
        BattleRoomManager.QueueFaint(this, pokemonIndex);
    }

    public IEnumerator Attack(Trainer target, PokemonMove move)
    {
		yield return StartCoroutine(ScriptableAnimations.Instance.Animations[move.AnimationName](PokemonSprite, target.PokemonSprite, move.AnimationDuration));
    }

    public IEnumerator TakeDamage(Pokemon attacker, PokemonMove move)
    {
		yield return StartCoroutine(ScriptableAnimations.Instance.Animations["SquashAndSqueeze"](PokemonSprite, PokemonSprite, 0.50f));

        Pokemon defender = CurrentPokemon;

        float ePower = move.Damage;
        float eAttackStat = move.DamageClass == PokemonDamageClass.Special ? attacker.SpecialAttack : attacker.Attack;
        float eDefenseStat = move.DamageClass == PokemonDamageClass.Special ? defender.SpecialDefense : defender.Defense;

        float damage = (((((2 * attacker.Level) / 5.0f) + 2) * ePower * (eAttackStat/eDefenseStat)) / 50) + 2;

        float critMultiplier = Random.Range(0.0f, 1.0f) <= (1.0f/24.0f) ? 1.5f : 1.0f;
        damage *= critMultiplier;

        float random = (float)((int)Random.Range(86, 100) / 100.0f);
        damage *= random;

        CurrentPokemon.CurrentHp -= damage;

        // Set death flag
        if(CurrentPokemon.CurrentHp <= 0)
        {
            CurrentPokemon.IsDead = true;
    	}
    }

    public IEnumerator Faint()
    {
        yield return StartCoroutine(ScriptableAnimations.Instance.Animations["Faint"](PokemonSprite, PokemonSprite, 1.0f));
    }

    public IEnumerator SwitchPokemon(int index = -1)
    {
        yield return ScriptableAnimations.Instance.Animations["PokeballThrow"](PokemonSprite, PokemonSprite, 0.5f);

        if(index < 0)
        {
            for(int i = 0; i < Pokemons.Count; i++)
            {
                if (!Pokemons[i].IsDead)
                { 
					CurrentPokemon = Pokemons[i];
					PokemonSprite.sprite = CurrentPokemon.Sprite;
                    break;
	        	}
	        }

            yield break;
	    }

        // Change data and sprite
        CurrentPokemon = Pokemons[index];
        CurrentPokemonIndex = index;
        PokemonSprite.sprite = CurrentPokemon.Sprite;
    }
}
