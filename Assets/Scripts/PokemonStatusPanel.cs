using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PokemonStatusPanel : MonoBehaviour
{
    [SerializeField]
    public Trainer _trainer;

    [SerializeField]
    public TMP_Text _pokemonName;

    [SerializeField]
    public Slider _healthBarSlider;

    [SerializeField]
    public TMP_Text _healthBarValue;

    private void FixedUpdate()
    {
        // TODO: Change to animated coroutine or event driven
        if(_trainer.CurrentPokemon != null)
        { 
            _healthBarSlider.value = ((float)_trainer.CurrentPokemon.CurrentHp / (float)_trainer.CurrentPokemon.Hp);
            _healthBarValue.text = String.Format("{0:0.00}%", _healthBarSlider.value * 100.0f);
			_pokemonName.text = _trainer.CurrentPokemon.Name;
	    }
    }
}
