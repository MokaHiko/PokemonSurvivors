using System.Collections;
using UnityEngine;

/// <summary>
/// Attach to a game object with a RectTransform and will 
/// </summary>
public class UIEffect : MonoBehaviour
{
    enum Animation
    {
        BobUpAndDown,
        Rotate
    };

    [SerializeField]
    private float _animationSpeed;

    [SerializeField]
    private float _animationAmplitude = 10f;

    [SerializeField]
    private Animation _animation;

    private RectTransform _rectTransform;
    private Vector2 _originalAnchorPosition;
    private Vector3 _originalRotation;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();

        StartCoroutine(_animation.ToString(), _animationSpeed);
    }

    private void OnDisable()
    {
        StopCoroutine(_animation.ToString());
    }

    private IEnumerator BobUpAndDown(float rate)
    {
        float timeElapsed = 0;
        _originalAnchorPosition = _rectTransform.anchoredPosition;

        while (true)
        {
            timeElapsed += Time.deltaTime;
            _rectTransform.anchoredPosition = new Vector2(_originalAnchorPosition.x,
                                                          _originalAnchorPosition.y + Mathf.Sin(timeElapsed * rate) * _animationAmplitude);
            yield return null;
        }
    }

    private IEnumerator Rotate(float rate)
    {
        float timeElapsed = 0;
        _originalRotation = _rectTransform.rotation.eulerAngles;

        while (true)
        {
            timeElapsed += Time.deltaTime;
            _rectTransform.rotation = Quaternion.Euler(0, 0, _rectTransform.rotation.eulerAngles.z + (Time.deltaTime * rate));
            yield return null;
        }
    }
}
