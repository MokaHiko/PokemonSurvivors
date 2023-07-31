using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;

public class BattleRoomManager : MonoBehaviour
{
	[SerializeField]
	public Trainer Trainer_1;

	[SerializeField]
	public Trainer Trainer_2;

	[SerializeField]
	public MenuManager MenuManager;

	// Trainer : attacker , PokemonMove : Moved used by attacker
	public UnityEvent<Trainer, PokemonMove> TrainerAttack;

	// Start is called before the first frame update
	async void Start()
	{
		await LoadPokemonData();

		// Subscribe to events
		TrainerAttack.AddListener(OnTrainerAttack);
	}
	private async Task LoadPokemonData()
	{
		int ctr = 0;
		Trainer_1.Pokemons.ForEach((Pokemon pokemon) =>
		{
			if (pokemon == null)
			{
				return;
			}
			
            StartCoroutine(BattleRoomNetworkManager.Instance.FetchPokemonData(pokemon.Name, (PokemonNetworkModel pokemonData) =>
            {
                Pokemon newPokemon = ScriptableObject.CreateInstance(nameof(Pokemon)) as Pokemon;
                newPokemon.Name = pokemonData.Name;
                newPokemon.Attack = pokemonData.Attack;
                newPokemon.Hp = pokemonData.Hp;
                newPokemon.Speed = pokemonData.Speed;
                newPokemon.SpecialAttack = pokemonData.SpecialAttack;

                for (int i = 0; i < 4; i++)
                {
                    PokemonMove move = ScriptableObject.CreateInstance(nameof(PokemonMove)) as PokemonMove;
                    move.Name = pokemonData.Moves[i].Name;
                    move.Damage = pokemonData.Moves[i].Damage;
                    move.Accuracy = pokemonData.Moves[i].Accuracy;
                    move.AnimationName = "MoveTowards";
                    move.AnimationDuration = 1;

                    newPokemon.Moves.Add(move);
                }

				// Load Pokemon Sprite
				StartCoroutine(BattleRoomNetworkManager.Instance.FetchImage(pokemonData.BackSpriteUrl, (Texture2D texture) =>
				{
					Rect rect = new Rect(0, 0, texture.width, texture.height);
					Trainer_1.Pokemons[0].Sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));

                    OnFinishedLoad();
				}));

                Trainer_1.Pokemons[ctr++] = newPokemon;
            }));
		});

		// Todo: Do the same for trainer 2 
		await Task.Yield();
	}

	private void OnFinishedLoad()
	{
		EventMenu eventMenu = MenuManager.FindMenu<EventMenu>();
        Trainer_1.Init(eventMenu);

		MenuManager.Init();
	}

	private void OnTrainerAttack(Trainer attacker, PokemonMove move)
	{
		// Open event menu
		EventMenu eventMenu = MenuManager.FindMenu<EventMenu>();
		MenuManager.OpenMenu(eventMenu);

		if(attacker == Trainer_1)
		{
			Trainer_2.TakeDamage(move);
		}
		else if(attacker == Trainer_2)
		{
			Trainer_1.TakeDamage(move);
		}

		// Flush event queue
		StartCoroutine(eventMenu.FlushBattleEventQueue(OnBattleEventsPresented));
	}
	private void OnBattleEventsPresented()
	{
        Menu attackMenu = MenuManager.FindMenu<AttackMenu>();
        MenuManager.OpenMenu(attackMenu, true);
	}
}
