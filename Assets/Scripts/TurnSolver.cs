using System;
using System.Collections;
using System.Collections.Generic;
using static PokemonMove;

[Flags]
public enum PokemonAttackMask: UInt32
{
    Normal = PokemonType.Fighting,
    Fire = PokemonType.Water | PokemonType.Ground | PokemonType.Rock,
    Water = PokemonType.Grass,
    Grass = PokemonType.Fire,
    Electric = PokemonType.Ground,
    Ice = PokemonType.Fire,
    Fighting = PokemonType.Fairy,
    Poison  = PokemonType.Ground | PokemonType.Psychic,
    Ground  = PokemonType.Ice,
    Flying  = PokemonType.Electric,
    Psychic = PokemonType.Dark,
    Bug = PokemonType.Fire,
    Rock = PokemonType.Fire,
    Ghost = PokemonType.Dark,
    Dark = PokemonType.Fighting,
    Dragon = PokemonType.Fairy,
    Steel = PokemonType.Fire,
    Fairy = PokemonType.Steel,
}

public enum BattleActionType
{
    Unknown,
    Attack,
    Switch,
    Faint
}

public class BattleAction
{
    public Trainer Actor;
    public BattleActionType Type = BattleActionType.Unknown;
    public bool Invalidated = false;

    public virtual IEnumerator Act(List<BattleAction> actions) { yield break; }

    public override string ToString() { return ""; }
    public List<String> Details = new List<String>();
}

public class ItemUseBattleAction
{ 
}

public class AttackBattleAction : BattleAction
{
    public Pokemon AttackerPokemon;
    public PokemonMove Move;

    public Trainer Defender;
    public Pokemon DefenderPokemon;

    public int Priority = 0;

    public AttackBattleAction(Trainer attacker, Trainer defender, PokemonMove move)
    {
        Actor = attacker;
        Type = BattleActionType.Attack;

        AttackerPokemon = Actor.CurrentPokemon;
        Move = move;

        Defender = defender;
        DefenderPokemon = Defender.CurrentPokemon;
    }

    public override string ToString()
    {
        return string.Format("{0} used {1}", AttackerPokemon.Name, Move.Name);
    }

    public override IEnumerator Act(List<BattleAction> actions)
    {
        yield return Actor.Attack(Defender, Move);

        float ePower = Move.Damage;
        float eAttackStat = Move.DamageClass == PokemonDamageClass.Special ? AttackerPokemon.SpecialAttack : AttackerPokemon.Attack;
        float eDefenseStat = Move.DamageClass == PokemonDamageClass.Special ? DefenderPokemon.SpecialDefense : DefenderPokemon.Defense;

        float damage = (((((2 * AttackerPokemon.Level) / 5.0f) + 2) * ePower * (eAttackStat/eDefenseStat)) / 50) + 2;

        bool isStab = false; 
        foreach(PokemonType attackerType in AttackerPokemon.Types)
        {
            if (attackerType == Move.Type)
            {
                isStab = true;
            }
        }

        float stabMultiplier = isStab ? 1.5f : 1.0f;
        damage *= stabMultiplier;

        // TODO: Type multiplier
        //float typeMultiplier = 1.0f;
        //foreach(PokemonType type in AttackerPokemon.Types) 
        //{
        //    if (Enum.TryParse<PokemonAttackMask>(DefenderPokemon.Types[0].ToString(), out var attackMask))
        //    {
        //        if(((UInt32)PokemonAttackMask.Fire | (UInt32)type) > 1)
        //        {
        //        }
        //    }
        //}
        //damage *= typeMultiplier;

        bool isCrit = UnityEngine.Random.Range(0.0f, 1.0f) <= (1.0f / 24.0f);
        float critMultiplier =  isCrit ? 1.5f : 1.0f;
        damage *= critMultiplier;

        if (isCrit)
        {
            Details.Add("It's a critical hit!");
        }

        float random = (float)((int)UnityEngine.Random.Range(86, 100) / 100.0f);
        damage *= random;

        yield return Defender.TakeDamage(AttackerPokemon, damage);

        if (Defender.CurrentPokemon.IsDead)
        {
            // Invalidate defender attack
            int invalidatedIndex = actions.FindIndex((action) => 
		    { 
                if(action.Actor == Defender && action.Type == BattleActionType.Attack)
                {
                    return true;
		        }
		        return false;
		    });

            if(invalidatedIndex > 0)
            {
                actions[invalidatedIndex].Invalidated = true;
	        }

            Defender.QueueFaint(Defender.CurrentPokemonIndex);
    	}
    }
}

public class SwitchBattleAction : BattleAction
{
    public int SwitchIndex;
    private readonly Pokemon _currentPokemon;

    public SwitchBattleAction(Trainer trainer, int switchIndex)
    {
        Actor = trainer;
        Type = BattleActionType.Switch;

        _currentPokemon = Actor.CurrentPokemon;
        SwitchIndex = switchIndex;
    }

    public override IEnumerator Act(List<BattleAction> actions)
    {
        yield return Actor.SwitchPokemon(SwitchIndex);
    }

    public override string ToString()
    { 
        return string.Format("Switched out {0} for {1}.", _currentPokemon.Name, Actor.Pokemons[SwitchIndex].Name);
    }
}

public class FaintBattleAction: BattleAction
{
    private int _faintedPokemonIndex;

    public FaintBattleAction(Trainer trainer, int faintedPokemonIndex)
    {
        Actor = trainer;
        Type = BattleActionType.Faint;

        _faintedPokemonIndex = faintedPokemonIndex;
    }

    public override IEnumerator Act(List<BattleAction> actions)
    {
        // Faintanimation
        yield return Actor.Faint();

        for(int i = 0; i < Actor.Pokemons.Count; i++)
        {
            if (!Actor.Pokemons[i].IsDead)
            {
                // TODO: Change to switch gui select if none ai
                Actor.QueueSwitch(i);
                break;
	        }
	    }

        // TODO: Queue Lose / New Trainer

        yield break;
    }

    public override string ToString()
    { 
        return string.Format("{0} fainted!.", Actor.Pokemons[_faintedPokemonIndex].Name);
    }
}


public class TurnSolver
{
    private List<SwitchBattleAction> _switchActions;
    private List<AttackBattleAction> _attackActions;
    private List<FaintBattleAction> _faintActions;

    public TurnSolver()
    {
        _switchActions = new List<SwitchBattleAction>();
        _attackActions = new List<AttackBattleAction>();
        _faintActions = new List<FaintBattleAction>();
    }

    // TODO: Make a single push function
    public void PushSwitch(SwitchBattleAction action)
    {
        _switchActions.Add(action);
    }

    public void PushAttack(AttackBattleAction action)
    {
        _attackActions.Add(action);
    }

    public void PushFaint(FaintBattleAction action)
    {
        _faintActions.Add(action);
    }

    public List<BattleAction> Solve()
    {
        List<BattleAction> _actions = new List<BattleAction>();

        // ~ Solve switches
        _actions.AddRange(_switchActions);

        // ~ Solve attacks

	    // Sort by speed
		_attackActions.Sort((AttackBattleAction a, AttackBattleAction b) => 
    	{ 
            // TODO: FIX SPEED ORDERING 
            if(a.Priority > b.Priority)
            {
                return 1;
	        }
            else if(a.Priority < b.Priority)
            {
                return -1;
	        }

            if(a.AttackerPokemon.Speed > b.AttackerPokemon.Speed)
            {
                return -1;
	        }
            else if(a.AttackerPokemon.Speed < b.AttackerPokemon.Speed)
            {
                return 1;
	        }

            return 0;
    	});

	    // Add to actions
		_actions.AddRange(_attackActions);

        // ~ Solve faints
        _actions.AddRange(_faintActions);

        // Clear actions
        _faintActions.Clear();
        _attackActions.Clear();
        _switchActions.Clear();

        return _actions;
    }
}
