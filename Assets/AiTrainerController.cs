using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiTrainerController : MonoBehaviour
{
    [SerializeField]
    private Trainer _trainer;

    private void Start()
    {
    }
    public void GenerateAction()
    {
    }

    public void Act()
    {
        Pokemon currentPokemon = _trainer.CurrentPokemon;
        _trainer.Attack(_trainer.BattleRoomManager.Trainer_1, currentPokemon.Moves[Random.Range(0, currentPokemon.Moves.Count - 1)]);
    }
}
