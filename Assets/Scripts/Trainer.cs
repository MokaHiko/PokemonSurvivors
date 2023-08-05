using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void Init()
    {
        if(Pokemons.Count > 0)
        {
            SwitchPokemon(0);
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

    public IEnumerator TakeDamage(PokemonMove move)
    {
		yield return StartCoroutine(ScriptableAnimations.Instance.Animations["SquashAndSqueeze"](PokemonSprite, PokemonSprite, 1.0f));

        CurrentPokemon.CurrentHp -= move.Damage;

        // Set death flag
        if(CurrentPokemon.CurrentHp <= 0)
        {
            CurrentPokemon.IsDead = true;
    	}
    }

    public IEnumerator Faint()
    {
        yield break;
    }

    public void SwitchPokemon(int index = -1)
    {
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
            return;
	    }

        // Change data and sprite
        CurrentPokemon = Pokemons[index];
        CurrentPokemonIndex = index;
        PokemonSprite.sprite = CurrentPokemon.Sprite;
    }
}
