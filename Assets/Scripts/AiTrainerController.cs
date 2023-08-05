using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiTrainerController : MonoBehaviour
{
    [SerializeField]
    private Trainer _trainer;

    [SerializeField]
    private BattleRoomManager _battleRoomManager;

    private void Start()
    {
        _battleRoomManager.ClientPushedEvent.AddListener(OnClientPushedEvent);
        _battleRoomManager.TurnStart.AddListener(OnTurnEnd);
    }

    public void GenerateAction()
    {
    }

    public void Act()
    {
        Pokemon currentPokemon = _trainer.CurrentPokemon;
        _trainer.QueueAttack(_trainer.BattleRoomManager.Trainer_1, currentPokemon.Moves[Random.Range(0, currentPokemon.Moves.Count - 1)]);
    }

    private void OnClientPushedEvent()
    { 
        Act();
    }

    private void OnTurnEnd()
    {
        // TODO: Present some dialogue
    }
}
