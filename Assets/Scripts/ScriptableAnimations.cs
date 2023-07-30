using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Scriptable Animations are a set of coroutine functions 
/// </summary>
public class ScriptableAnimations : MonoBehaviour
{
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
                {nameof(MoveTowards), MoveTowards },
                {nameof(SquashAndSqueeze), SquashAndSqueeze}
            };

			Instance = this;
		}
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
