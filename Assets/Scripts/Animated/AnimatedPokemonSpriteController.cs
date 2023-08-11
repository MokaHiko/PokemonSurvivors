using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AnimatedPokemonSpriteController : MonoBehaviour
{
    public enum AnimatedSpriteState
    {
        Unknown,
        Error,
        Loading,
        Ready,
    };

    [SerializeField]
    private List<Sprite> _spriteList;

    [SerializeField]
    private float _frameRate;

    [SerializeField]
    private int _spriteFrame = 0;

    [SerializeField]
    private AnimatedSpriteState _state;

    private List<UniGif.GifTexture> _gifTextureList;
    private int _loopCount = 0;
    private int _width;
    private int _height;

    FilterMode _filterMode;
    TextureWrapMode _wrapMode; 

    bool _outputDebugLog;

    private Coroutine _playCoroutine = null;

    private void Awake()
    {
        _spriteList = new List<Sprite>();
        _gifTextureList = new List<UniGif.GifTexture>();
    }

    public void Play(SpriteRenderer spriteRenderer)
    {
        if(_playCoroutine != null) 
        {
            StopCoroutine(_playCoroutine);
            _playCoroutine = null;
        }

        _playCoroutine = StartCoroutine(PlayGif(spriteRenderer));
    }

    public void Stop()
    {
        if( _playCoroutine != null)
        {
            StopCoroutine(_playCoroutine);
        }
    }

    private IEnumerator PlayGif(SpriteRenderer spriteRenderer)
    {
        while(true)
        {
            _spriteFrame = 0;
            while(_spriteFrame < _spriteList.Count) 
            {
                if(_state != AnimatedSpriteState.Ready)
                {
                    yield break;
                }

                spriteRenderer.sprite = _spriteList[_spriteFrame++];
                yield return new WaitForSeconds(1.0f / _frameRate);
            }
        }
    }

    public IEnumerator LoadGifFromUrlCoroutine(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("URL is nothing.");
            yield break;
        }

        string path;
        if (url.StartsWith("http"))
        {
            path = url;
        }
        else
        {
            // from StreamingAssets
            path = Path.Combine("file:///" + Application.streamingAssetsPath, url);
        }

        // Load file
        _state = AnimatedSpriteState.Loading;
        using (WWW www = new WWW(path))
        {
            yield return www;

            if (string.IsNullOrEmpty(www.error) == false)
            {
                Debug.LogError("File load error.\n" + www.error);
                yield break;
            }

            Clear();

            // Get GIF textures
            yield return StartCoroutine(UniGif.GetTextureListCoroutine(www.bytes, (gifTexList, loopCount, width, height) =>
            {
                if (gifTexList != null)
                {
                    _gifTextureList = gifTexList;
                    _loopCount = loopCount;
                    _width = width;
                    _height = height;

                    foreach(UniGif.GifTexture gifTexture in gifTexList)
                    {
                        _spriteList.Add(CreateSpriteFromTexture(gifTexture.m_texture2d, _width, _height));
                    }

                    _state = AnimatedSpriteState.Ready;
                }
                else
                {
                    Debug.LogError("Gif texture get error.");
                }
            },
            _filterMode, _wrapMode, _outputDebugLog));
        }
    }
    private Sprite CreateSpriteFromTexture(Texture2D texture, int width, int height)
    {
        Rect rect = new Rect(0, 0, width, height);
        Sprite newSprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        return newSprite;
    }

    /// <summary>
    /// Clear GIF texture
    /// </summary>
    private void Clear()
    {
        _spriteList.Clear();

        if (_gifTextureList != null)
        {
            for (int i = 0; i < _gifTextureList.Count; i++)
            {
                if (_gifTextureList[i] != null)
                {
                    if (_gifTextureList[i].m_texture2d != null)
                    {
                        Destroy(_gifTextureList[i].m_texture2d);
                        _gifTextureList[i].m_texture2d = null;
                    }
                    _gifTextureList[i] = null;
                }
            }
            _gifTextureList.Clear();
            _gifTextureList = null;
        }
    }
}
