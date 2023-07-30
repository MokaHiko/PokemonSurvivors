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

    private EventMenu _eventMenu;

    public void Init(EventMenu eventMenu)
    {
        _eventMenu = eventMenu;
        if(Pokemons.Count > 0)
        {
            SwitchPokemon(0);
        }
    }

    public void Attack(Trainer target, PokemonMove move)
    {
		// Push to event string queue
		_eventMenu.PushBattleEvent(new BattleEvent(string.Format("{0} used {1}", CurrentPokemon.Name, move.Name), 0));

        // Calculate effects
		_eventMenu.PushBattleEvent(new BattleEvent(string.Format("It was super effective!"), 0));

		// Animation
		StartCoroutine(ScriptableAnimations.Instance.Animations[move.AnimationName](PokemonSprite, target.PokemonSprite, move.AnimationDuration));

        // Invoke attack event
        BattleRoomManager.TrainerAttack.Invoke(this, move);
    }

    public void TakeDamage(PokemonMove move)
    {
		StartCoroutine(ScriptableAnimations.Instance.Animations["SquashAndSqueeze"](PokemonSprite, PokemonSprite, 1.0f));
    }

    private void SwitchPokemon(int index)
    {
        CurrentPokemon = Pokemons[index];
        PokemonSprite.sprite = CurrentPokemon.Sprite;
    }
}
