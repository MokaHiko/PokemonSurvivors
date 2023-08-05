using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleRoomManager : MonoBehaviour
{
	public UnityEvent ClientPushedEvent;
	public UnityEvent TurnStart;
	public UnityEvent TurnEnd;

	[SerializeField]
	public Trainer Trainer_1;

	[SerializeField]
	public Trainer Trainer_2;

	[SerializeField]
	public MenuManager MenuManager;

	private TurnSolver _solver;
	private bool Trainer_1_ActedFlag = false;
	private bool Trainer_2_ActedFlag = false;

	// Start is called before the first frame update
	void Start()
	{
		SpawnRandomTrainer(Trainer_2);
		SpawnRandomTrainer(Trainer_1, OnFinishedLoad);

		// Create turn solver
		_solver = new TurnSolver();
	}

	private IEnumerator LoadTrainerPokemonData(Trainer trainer, UnityAction callback = null)
	{
		// Load pokemon data and sprite URLS
		List<string> spriteUrls = new List<string>();

		for(int i = 0; i < trainer.Pokemons.Count; i++)
		{
			if (trainer.Pokemons[i] == null)
			{
				continue;
			}

			yield return StartCoroutine(BattleRoomNetworkManager.Instance.FetchPokemonData(trainer.Pokemons[i].Name, (PokemonNetworkModel pokemonData) =>
            {
				// Create pokemon scriptable object
                Pokemon newPokemon = trainer.Pokemons[i];
                newPokemon.Name = pokemonData.Name;
                newPokemon.Hp = pokemonData.Hp;
                newPokemon.Attack = pokemonData.Attack;
                newPokemon.Defense = pokemonData.Defense;
                newPokemon.SpecialDefense = pokemonData.SpecialDefense;
                newPokemon.SpecialAttack = pokemonData.SpecialAttack;
                newPokemon.Speed = pokemonData.Speed;

				// EVS & IVS
				newPokemon.CurrentHp = newPokemon.Hp;

				// Create move scriptable object
                newPokemon.Moves.Clear();
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

				if(trainer == Trainer_1)
				{ 
					spriteUrls.Add(pokemonData.BackSpriteUrl);
				}
				else
				{ 
					spriteUrls.Add(pokemonData.FrontSpriteUrl);
				}
            }));
		};

		// Load Sprites
		for(int i = 0; i < spriteUrls.Count; i++)
		{ 
			yield return StartCoroutine(BattleRoomNetworkManager.Instance.FetchImage(spriteUrls[i], (Texture2D texture) =>
			{
				Rect rect = new Rect(0, 0, texture.width, texture.height);
				trainer.Pokemons[i].Sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
			}));
		}

		if(callback != null)
		{ 
			callback();
		}
	}

	private void OnFinishedLoad()
	{
        Trainer_1.Init();

		MenuManager.OpenMenu<AttackMenu>();
		MenuManager.OpenMenu<SwitchMenu>();
	}

	private void SpawnRandomTrainer(Trainer trainer, UnityAction callback = null)
	{
		foreach(Pokemon pokemon in trainer.Pokemons)
		{
			DestroyImmediate(pokemon, true);
		}


		// TODO: Add Gym Leader Spawn animation
		int pokemonCount = Random.Range(0, 7);
		trainer.Pokemons = new List<Pokemon>();
		for(int i = 0;  i < pokemonCount; i++)
	    {
            Pokemon newPokemon = ScriptableObject.CreateInstance(nameof(Pokemon)) as Pokemon;
			newPokemon.Name = Random.Range(1, 1001).ToString();
			trainer.Pokemons.Add(newPokemon);
		}

		StartCoroutine(LoadTrainerPokemonData(trainer, () => {
			trainer.Init();

			if(callback != null)
			{
				callback();
			}
		}));
    }


	public void QueueAttack(Trainer attacker, PokemonMove move)
	{
		if(attacker == Trainer_1)
		{
			Trainer_1_ActedFlag = true;
			_solver.PushAttack(new AttackBattleAction(Trainer_1, Trainer_2, move));
		}
		else if(attacker == Trainer_2)
		{
			Trainer_2_ActedFlag = true;
			_solver.PushAttack(new AttackBattleAction(Trainer_2, Trainer_1, move));
		}

		if(Trainer_1_ActedFlag && Trainer_2_ActedFlag)
		{
			StartCoroutine(FlushActions());
		}
	}

	public void QueueSwitch(Trainer trainer, int switchIndex)
	{
		if(trainer == Trainer_1)
		{
			Trainer_1_ActedFlag = true;
			_solver.PushSwitch(new SwitchBattleAction(Trainer_1, switchIndex));
		}
		else if(trainer == Trainer_2)
		{
			Trainer_2_ActedFlag = true;
			_solver.PushSwitch(new SwitchBattleAction(Trainer_2, switchIndex));
		}

		if(Trainer_1_ActedFlag && Trainer_2_ActedFlag)
		{
			StartCoroutine(FlushActions());
		}
	}

	public void QueueFaint(Trainer trainer, int faintIndex)
	{
		if(trainer == Trainer_1)
		{
			Trainer_1_ActedFlag = true;
			_solver.PushFaint(new FaintBattleAction(Trainer_1, faintIndex));
		}
		else if(trainer == Trainer_2)
		{
			Trainer_2_ActedFlag = true;
			_solver.PushFaint(new FaintBattleAction(Trainer_2, faintIndex));
		}

		if(Trainer_1_ActedFlag && Trainer_2_ActedFlag)
		{
			StartCoroutine(FlushActions());
		}
	}

	private IEnumerator FlushActions()
	{
		// Open event menu
		EventMenu eventMenu = MenuManager.FindMenu<EventMenu>();
		MenuManager.OpenMenu(eventMenu);

		// Recursively solve turn	
		List<BattleAction> actions = _solver.Solve();
		while(actions.Count > 0)
		{ 
			// Perform solved trun
			foreach(BattleAction action in actions)
			{
				// Reset actions flags ahead
				Trainer_1_ActedFlag = false;
				Trainer_2_ActedFlag = false;

				// Check if invalidated 
				if(action.Invalidated)
				{
					continue;
				}

				// Push to Log queue
				eventMenu.PushBattleAction(action);

				yield return action.Act(actions);
			}

			yield return StartCoroutine(eventMenu.FlushAndPresentBattleActionQueue());

			actions = _solver.Solve();
		}

		OnTurnExecuted();
    }

	private void OnTurnExecuted()
	{
        MenuManager.OpenMenu<AttackMenu>(true);
		MenuManager.OpenMenu<SwitchMenu>();
	}
}
