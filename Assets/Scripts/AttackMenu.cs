using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AttackMenu : Menu
{
    [SerializeField]
    BattleRoomManager _battleRoomManger;

    [SerializeField]
    public GameObject AttackButtonPrefab;

    private List<Button> _buttons;

	public override string GetName ()
	{
        return name;
	}

	public override void OnClose()
	{
        foreach (var button in _buttons)
        {
            Destroy(button.gameObject);
        }

        _buttons.Clear();
	}
	public override void OnOpen()
	{
        if(_buttons.Count > 0)
        {
            return;
	    }

        InitAttackButtons();
	}
	public override void OnFirstOpen()
    {
        _buttons = new List<Button>();
        InitAttackButtons();
    }

    private void InitAttackButtons()
    {
        _battleRoomManger.Trainer_1.CurrentPokemon.Moves.ForEach((move) =>
        {
            var button = Instantiate(AttackButtonPrefab, gameObject.transform).GetComponent<Button>();
            button.GetComponentInChildren<Text>().text = move.Name;

            button.onClick.AddListener(() => 
            {
                _battleRoomManger.Trainer_1.QueueAttack(_battleRoomManger.Trainer_2, move);
                _battleRoomManger.ClientPushedEvent.Invoke();
            });

            _buttons.Add(button);
        });
    }
}
