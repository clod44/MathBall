using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketHoopController : MonoBehaviour
{
    private Vector2 originalPosition = Vector2.zero;
    private Quaternion originalRotation = Quaternion.identity;
    public Vector2 randomPositionAreaSize = Vector2.zero;
    public float randomRotationAmount = 10f;

    private Vector2 targetPosition = Vector2.zero;
    private Quaternion targetRotation = Quaternion.identity;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        newHoopCondition();
    }

    void Update()
    {
        Vector2 newPosition = Vector2.Lerp(transform.position, targetPosition, 0.5f * Time.deltaTime);
        transform.position = newPosition;

        Quaternion newRotation = Quaternion.Euler(0f, 0f, Mathf.LerpAngle(transform.rotation.eulerAngles.z, targetRotation.eulerAngles.z, 0.5f * Time.deltaTime));
        transform.rotation = newRotation;
    }

    public void newHoopCondition()
    {
        float randomX = Random.Range(originalPosition.x - randomPositionAreaSize.x / 2f, originalPosition.x + randomPositionAreaSize.x / 2f);
        float randomY = Random.Range(originalPosition.y - randomPositionAreaSize.y / 2f, originalPosition.y + randomPositionAreaSize.y / 2f);
        targetPosition = new Vector2(randomX, randomY);
        targetRotation = Quaternion.Euler(0f, 0f, Random.Range(-randomRotationAmount, randomRotationAmount));
    }
}
