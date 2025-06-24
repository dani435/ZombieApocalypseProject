//shake della camera al lancio del missile con il lanciarazzi
using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 initialPosition;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0f;

    private void Awake()
    {
        initialPosition = transform.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        while (shakeDuration > 0f)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime;

            yield return null;
        }

        transform.localPosition = initialPosition;
    }
}
