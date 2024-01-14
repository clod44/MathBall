using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.WSA;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class BallControl : MonoBehaviour
{
    public bool isPlayerControlling = true;
    public bool isDragging = false;
    public Vector2 dragStartPosition = Vector2.zero;
    public Vector2 dragReleasePosition = Vector2.zero;
    public Rigidbody2D rb;
    public float launchStrengthMultiplier = 5f;
    public float maxLaunchStrength = 15f;
    public float launchAccuracy = 1.0f;
    public float launchAccuracyFactor = 0.25f;
    private Camera cam;
    public CameraController camController;

    public int trajectoryPointCount = 100;
    public float trajectoryPointSpacing = 0.1f;
    public GameObject trajectoryPointPrefab;

    private GameObject[] trajectoryPoints;
    public VolumeExpoLerper globalVolume;
    public int score = 0;
    public HUDController hud;

    public Vector2 spawnPoint = Vector2.zero;
    public Vector2 spawnPointAreaSize = Vector2.one;
    //calculated with spawnpoint and spawnpointareasize
    public Vector2 newSpawnPoint = Vector2.zero;
    private bool isBeingResetted = false;
    public BasketHoopController basketHoop;
    AudioManager audioManager;


    void Start()
    {
        audioManager = AudioManager.GetInstance();
        audioManager.PlayLoopingSound("music_1");
        cam = Camera.main;
        rb.bodyType = RigidbodyType2D.Static;
        // Initialize trajectoryPoints array
        trajectoryPoints = new GameObject[trajectoryPointCount];

        // Instantiate trajectory point GameObjects
        for (int i = 0; i < trajectoryPointCount; i++)
        {
            trajectoryPoints[i] = Instantiate(trajectoryPointPrefab, Vector3.zero, Quaternion.identity);
            trajectoryPoints[i].SetActive(false);
        }
        globalVolume.ChangeFromTo(-10f, 0f, 1f);
    }


    float teleportTimer = 0f;
    float teleportDelay = 0.5f;  // Set your desired delay

    void Update()
    {
        if (!hud.isQuestionActive && isPlayerControlling)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                Collider2D hitCollider = Physics2D.OverlapPoint(mousePos);
                if (hitCollider != null && hitCollider.gameObject == gameObject)
                {
                    isDragging = true;
                    dragStartPosition = mousePos;
                    audioManager.PlaySound("stretching_1");

                }
            }
            if (isDragging)
            {
                // Draw the trajectory continuously while dragging
                Vector2 launchVector = dragStartPosition - mousePos;
                if (launchVector.sqrMagnitude < 0.01f) // Using sqrMagnitude for performance
                {
                    if (launchVector == Vector2.zero)
                    {
                        launchVector = new Vector2(1, 0) * 0.01f;
                    }
                    else
                    {
                        launchVector = launchVector.normalized * 0.01f;
                    }
                }

                launchVector = Vector2.ClampMagnitude(launchVector * launchStrengthMultiplier, maxLaunchStrength);

                DrawTrajectory(transform.position, launchVector);
            }
            if (isDragging && Input.GetMouseButtonUp(0))
            {
                DisableTrajectoryPoints();
                isDragging = false;
                dragReleasePosition = mousePos;
                isPlayerControlling = false;
                rb.bodyType = RigidbodyType2D.Dynamic;

                // Calculate launch vector
                Vector2 launchVector = dragStartPosition - dragReleasePosition;
                Vector2 randomLaunchVector = Random.insideUnitCircle.normalized * launchVector.magnitude;

                float accuracyRandomnessFactor = (1.0f - launchAccuracy) * launchAccuracyFactor;
                Vector2 finalLaunchVector = Vector2.Lerp(launchVector, randomLaunchVector, accuracyRandomnessFactor) * launchStrengthMultiplier;

                // Check if the launch vector magnitude is below a minimum threshold
                float minLaunchMagnitude = 0.01f; // Set your minimum launch magnitude here
                if (finalLaunchVector.sqrMagnitude < minLaunchMagnitude * minLaunchMagnitude)
                {
                    // Add a small force in a random direction if the launch vector is too small
                    finalLaunchVector = Random.insideUnitCircle.normalized * minLaunchMagnitude;
                }

                // Apply the force only if the launch vector is valid
                if (!float.IsNaN(finalLaunchVector.x) && !float.IsNaN(finalLaunchVector.y))
                {
                    finalLaunchVector = Vector2.ClampMagnitude(finalLaunchVector, maxLaunchStrength);
                    rb.AddForce(finalLaunchVector, ForceMode2D.Impulse);
                }
                audioManager.PlaySound("whoosh_1");
            }


        }
        if (isBeingResetted)
        {
            transform.position = Vector2.Lerp(transform.position, newSpawnPoint, 4f * Time.deltaTime);
            //keep generating new questions because it looks cool
            hud.ShowQuestion();
            if (Vector2.Distance(transform.position, newSpawnPoint) < 0.2f)
            {
                audioManager.PlaySound("explosion_1");
                isBeingResetted = false;
                isPlayerControlling = true;
                globalVolume?.ChangeFromTo(3f, 0f, 0.5f);
                camController.ResetZoom(0.5f);

            }
        }

        if (!IsInCameraView(transform.position) && !isBeingResetted)
        {
            teleportTimer += Time.deltaTime;
            if (teleportTimer >= teleportDelay)
            {
                ResetBall();
                teleportTimer = 0f;
            }
        }

    }
    void ResetBall()
    {
        rb.bodyType = RigidbodyType2D.Static;

        // Generate random position within the spawn point area
        float randomX = Random.Range(spawnPoint.x - spawnPointAreaSize.x / 2, spawnPoint.x + spawnPointAreaSize.x / 2);
        float randomY = Random.Range(spawnPoint.y - spawnPointAreaSize.y / 2, spawnPoint.y + spawnPointAreaSize.y / 2);
        newSpawnPoint = new Vector2(randomX, randomY);
        isBeingResetted = true;
        basketHoop.newHoopCondition();
        audioManager.PlaySound("reverb_" + Random.Range(1, 3));
    }

    public bool IsInCameraView(Vector2 pos)
    {
        Vector3 viewPos = cam.WorldToViewportPoint(pos);
        //ignore the top view
        return viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.z > 0;

    }

    void DrawTrajectory(Vector2 launchPosition, Vector2 launchVelocity)
    {
        float step = trajectoryPointSpacing;
        for (int i = 0; i < trajectoryPointCount; i++)
        {
            float t = i * step;
            Vector2 pointPosition = CalculatePointOnTrajectory(launchPosition, launchVelocity, t);
            trajectoryPoints[i].transform.position = pointPosition;

            // Calculate alpha based on the point's position in the trajectory
            float alpha = Mathf.Clamp01(1.0f - i / (float)trajectoryPointCount);
            Color pointColor = trajectoryPoints[i].GetComponent<SpriteRenderer>().color;
            pointColor.a = alpha;
            trajectoryPoints[i].GetComponent<SpriteRenderer>().color = pointColor;
            trajectoryPoints[i].SetActive(true);
        }
    }

    Vector2 CalculatePointOnTrajectory(Vector2 launchPosition, Vector2 launchVelocity, float time)
    {
        float gravity = Physics2D.gravity.y;  // Note: Physics2D.gravity.y is already negative
        float x = launchPosition.x + launchVelocity.x * time;
        float y = launchPosition.y + launchVelocity.y * time + 0.5f * gravity * time * time;

        return new Vector2(x, y);
    }



    void DisableTrajectoryPoints()
    {
        // Iterate through trajectoryPoints array and set each point's setActive to false
        foreach (GameObject point in trajectoryPoints)
        {
            point.SetActive(false);
        }
    }



    private bool isTopCollided = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ScoreAreaTop") && !isBeingResetted)
        {
            isTopCollided = true;
        }
        if (other.CompareTag("ScoreAreaBottom") && isTopCollided && !isBeingResetted)
        {
            if (isTopCollided && !isBeingResetted)
            {
                globalVolume?.ChangeFromTo(3f, 0f, 0.5f);
                AddScore(Random.Range(1, 5));
                audioManager.PlaySound("explosion_1");
                audioManager.PlaySound("whistle_1");
            }
            audioManager.PlaySound("net_swish_" + Random.Range(1, 9));
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ScoreAreaTop"))
        {
            isTopCollided = false;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (camController != null)
        {
            camController.ShakeCamera(0.3f, 0.1f);
            camController.ZoomTo(2f, 0.5f);
            camController.ResetZoom(0.5f);
        }
        audioManager.PlaySound("ball_hit_" + Random.Range(1, 5));
    }


    public void SetScore(int amount = 0)
    {
        score = amount;
        hud.SetScore(score);
    }
    public void AddScore(int amount = 1)
    {
        score += amount;
        hud.SetScore(score);
    }
}
