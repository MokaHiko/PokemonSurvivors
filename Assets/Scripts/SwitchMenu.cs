using System.Collections.Generic;
using UnityEngine;

public class SwitchMenu : Menu
{
    [SerializeField]
    private BattleRoomManager _battleRoomManager;

    [SerializeField]
    private GameObject _switchButtonPrefab;

    [SerializeField]
    private List<SwitchButton> _switchButtons;

    public override string GetName() {return "SwitchMenu";}

    public override void OnFirstOpen() 
    {
        OnOpen();
    }

    public override void OnClose()
	{
        foreach(SwitchButton button in _switchButtons)
        {
            Destroy(button.gameObject);
    	}

        _switchButtons.Clear();
	}

    public override void OnOpen() 
    {
        InitSwitchButtons();
    }

    private void InitSwitchButtons()
    {
        Trainer client = _battleRoomManager.Trainer_1;
        for(int i = 0; i < client.Pokemons.Count; i++)
        {
            if (client.CurrentPokemon == client.Pokemons[i])
            {
                continue;
            }

            var switchButton = Instantiate(_switchButtonPrefab, gameObject.transform).GetComponent<SwitchButton>();
            switchButton.Init(client, i, client.Pokemons[i].Sprite, _battleRoomManager);
            _switchButtons.Add(switchButton);
        }
    }
}
