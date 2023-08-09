using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;

/// <summary>
/// Scriptable Animations are a set of coroutine functions 
/// </summary>
public class ScriptableAnimations : MonoBehaviour
{
    [SerializeField]
    private GameObject _shadowBallPrefab;

    public static ScriptableAnimations Instance = null;

    public delegate IEnumerator ScriptableAnimation<T0, T1, T2>(T0 arg0, T1 arg1, T2 arg2);
    public Dictionary<string, ScriptableAnimation<SpriteRenderer, SpriteRenderer, float>> Animations;

    private void Awake()
	{
		// If there is an instance, and it's not me, delete myself.
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
            Animations = new Dictionary<string, ScriptableAnimation<SpriteRenderer, SpriteRenderer, float>>()
            {
                {nameof(Faint), Faint},
                {nameof(MoveTowards), MoveTowards },
                {nameof(SquashAndSqueeze), SquashAndSqueeze},
                {nameof(ShadowBall), ShadowBall},
            };

			Instance = this;
		}
	}
    private IEnumerator Faint(SpriteRenderer p1, SpriteRenderer p2, float duration)
    {
        Vector3 originalPosition = p1.transform.position;
        Vector3 targetPosition = p1.transform.position - new Vector3(0, 1, 0);

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            p1.transform.position = Vector3.Lerp(p1.transform.position, targetPosition, (elapsedTime / duration));
            p1.color = Vector4.Lerp(p1.color, Vector4.zero, (elapsedTime /duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        p1.transform.position = originalPosition;
        p1.color = Vector4.one;
    }
    private IEnumerator MoveTowards(SpriteRenderer p1, SpriteRenderer p2, float duration)
    {
        float halfTripTime = duration / 2;
        Vector3 originalPosition = p1.transform.position;

        float elapsedTime = 0;
        while (elapsedTime < halfTripTime)
        {
            p1.transform.position = Vector3.Slerp(p1.transform.position, p2.transform.position, (elapsedTime / halfTripTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0;
        while (elapsedTime < halfTripTime)
        {
            p1.transform.position = Vector3.Lerp(p1.transform.position, originalPosition, (elapsedTime / halfTripTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator ShadowBall(SpriteRenderer p1, SpriteRenderer p2, float duration)
    {
        var shadowBall = Instantiate(_shadowBallPrefab);
        shadowBall.transform.position = p1.transform.position;

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            shadowBall.transform.position = Vector3.Lerp(p1.transform.position, p2.transform.position, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(shadowBall);
    }

    private IEnumerator SquashAndSqueeze(SpriteRenderer p1, SpriteRenderer p2, float duration)
    {
        float halfTripTime = duration / 2;
        Vector3 originalScale = p1.transform.localScale;
        Vector3 targetScale = p1.transform.localScale * 0.85f;

        float elapsedTime = 0;
        while (elapsedTime < halfTripTime)
        {
            p1.transform.localScale = Vector3.Slerp(p1.transform.localScale, targetScale, (elapsedTime / halfTripTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0;
        while (elapsedTime < halfTripTime)
        {
            p1.transform.localScale = Vector3.Lerp(p1.transform.localScale, originalScale, (elapsedTime / halfTripTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
