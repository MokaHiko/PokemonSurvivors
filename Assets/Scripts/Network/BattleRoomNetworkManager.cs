using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class BattleRoomNetworkManager : MonoBehaviour
{
	public static BattleRoomNetworkManager Instance = null;
	private Dictionary<string, PokemonNetworkModel> _pokemonCache = new Dictionary<string, PokemonNetworkModel>();
	public const string PokemonAPIString = "http://localhost:5259/pokemon/";

	private void Awake()
	{
		// If there is an instance, and it's not me, delete myself.
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}
	public IEnumerator FetchPokemonData(string pokemonName, UnityAction<PokemonNetworkModel> callback)
	{
		if(_pokemonCache.ContainsKey(pokemonName))
		{
			callback(_pokemonCache[pokemonName]);
			yield break;
		}

        using UnityWebRequest request = UnityWebRequest.Get(PokemonAPIString + pokemonName);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error + " [Endpoint]: " + PokemonAPIString + pokemonName);
        }
        else
        {
            // Add to pokemon cache
            PokemonNetworkModel pokemon;
            string rawJson = request.downloadHandler.text;
            pokemon = JsonUtility.FromJson<PokemonNetworkModel>(rawJson);

            _pokemonCache[pokemonName] = pokemon;

            callback(pokemon);
        }
    }

	public IEnumerator FetchImage(string imageUrl, UnityAction<Texture2D> callback)
	{
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error + " [Endpoint]: " + imageUrl);
        }
        else
        {
            var texture = DownloadHandlerTexture.GetContent(request);
            callback(texture);
        }
    }
	public IEnumerator FetchImageRaw(string imageUrl, UnityAction<Texture2D> callback)
	{
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error + " [Endpoint]: " + imageUrl);
        }
        else
        {
            var texture = DownloadHandlerTexture.GetContent(request);
            callback(texture);
        }
    }
}
