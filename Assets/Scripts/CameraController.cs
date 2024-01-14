using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 originalPosition;
    private Camera cam;
    public float originalSize;

    [Header("constant camera shake")]
    public float slowMagnitude = 0.1f;
    public float slowSpeed = 0.5f;

    void Start()
    {
        originalPosition = transform.localPosition;
        cam = GetComponent<Camera>(); // Assuming this script is on the camera itself
        originalSize = cam.orthographicSize;
    }

    void Update()
    {
        // Apply slow continuous shake
        Vector3 slowOffset = CalculateSlowOffset(Time.time);
        transform.localPosition = originalPosition + slowOffset;
    }

    Vector3 CalculateSlowOffset(float time)
    {
        float noiseX = Mathf.PerlinNoise(time * slowSpeed, 0f) * 2 - 1;
        float noiseY = Mathf.PerlinNoise(0f, time * slowSpeed) * 2 - 1;

        Vector3 offset = new Vector3(noiseX * slowMagnitude, noiseY * slowMagnitude, 0f);
        return offset;
    }

    // Call this function to trigger a camera shake
    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float noiseX = Mathf.PerlinNoise(Time.time * 10f, 0f) * 2 - 1;
            float noiseY = Mathf.PerlinNoise(0f, Time.time * 10f) * 2 - 1;

            Vector3 offset = new Vector3(noiseX * magnitude, noiseY * magnitude, 0f);
            transform.localPosition = originalPosition + offset;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    // Function to zoom the camera to a specified size gradually over a duration
    public void ZoomTo(float newSize, float duration)
    {
        StartCoroutine(ChangeSize(cam.orthographicSize, newSize, duration));
    }

    public void ResetZoom(float duration)
    {
        StartCoroutine(ChangeSize(cam.orthographicSize, originalSize, duration));
    }



    IEnumerator ChangeSize(float startSize, float targetSize, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
            yield return null;
        }

        cam.orthographicSize = targetSize;
    }
}
