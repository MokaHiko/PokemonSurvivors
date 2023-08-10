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
        if (ScriptableAnimations.Instance.Animations.ContainsKey(move.AnimationName))
        {
		    yield return StartCoroutine(ScriptableAnimations.Instance.Animations[move.AnimationName](PokemonSprite, target.PokemonSprite, move.AnimationDuration));
        }
        else
        {
            if (move.DamageClass == PokemonDamageClass.Physical)
            {
		        yield return StartCoroutine(ScriptableAnimations.Instance.Animations["MoveTowards"](PokemonSprite, target.PokemonSprite, move.AnimationDuration));
            }
            else if (move.DamageClass == PokemonDamageClass.Special)
            {
		        yield return StartCoroutine(ScriptableAnimations.Instance.Animations["ShadowBall"](PokemonSprite, target.PokemonSprite, move.AnimationDuration));
            }
            else
            {
		        yield return StartCoroutine(ScriptableAnimations.Instance.Animations["SquashAndSqueeze"](PokemonSprite, PokemonSprite, 0.50f));
            }
        }
    }

    public IEnumerator TakeDamage(Pokemon attacker, float damage)
    {
		yield return StartCoroutine(ScriptableAnimations.Instance.Animations["SquashAndSqueeze"](PokemonSprite, PokemonSprite, 0.50f));

        // TODO: Health bar animation

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
