using UnityEngine;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour
{
    /// <summary>
    /// The Index of the pokemon the button switches to 
    /// </summary>
    [SerializeField]
    private int _index;

    [SerializeField]
    private Image _pokemonImage;

    [SerializeField]
    private Button _button;

    private Trainer _client;

    public void Init(Trainer client, int index, Sprite sprite, BattleRoomManager battleRoomManager)
    {
        _client = client;
        _index = index;
        _pokemonImage.sprite = sprite;

        _button.onClick.AddListener(() =>
        {
            SwitchPokemon();
            battleRoomManager.ClientPushedEvent.Invoke();
        });
    }

    private void SwitchPokemon()
    {
        _client.QueueSwitch(_index);
    }
}
