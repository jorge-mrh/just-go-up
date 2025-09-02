using UnityEngine;
using System.Collections.Generic;
using static PlatformBehavior;

public class PlatformGenerator : MonoBehaviour
{
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private GameObject firstPlatform;
    [SerializeField] private float platformSpeedMin = 1f;
    [SerializeField] private float platformSpeedMax = 3f;
    [SerializeField] private float platformWidthMin = 1f;
    [SerializeField] private float platformWidthMax = 3f;
    [SerializeField] private float spacingMin = 2f;
    [SerializeField] private float spacingMax = 3.2f;
    [SerializeField] private Transform player;
    [SerializeField] private float generateAheadDistance = 10f;
    [SerializeField] private float optimizeDistance = 30f;

    private float highestPlatformY;
    public static float firstPlatformY;
    private float lastPlatformSpeed = -0.5f;
    private float lastPlatformWidth;
    private List<GameObject> activePlatforms = new List<GameObject>();
    public int platformCounter = 0;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }

        // Initialize first platform
        firstPlatform.GetComponent<PlatformBehavior>().speed = 0;
        activePlatforms.Add(firstPlatform);
        Renderer renderer = firstPlatform.GetComponent<SpriteRenderer>();
        float width = renderer.bounds.size.x;
        lastPlatformWidth = width;

        firstPlatformY = firstPlatform.transform.position.y;
        highestPlatformY = firstPlatform.transform.position.y;

        // Fill the initial range
        SpawnPlatformAbove();
    }

    void Update()
    {
        SpawnPlatformAbove();
        GenerateBellowAsPlayerFalls();
        OptimizePlatforms();
    }

    void GenerateBellowAsPlayerFalls()
    {
        // Always fill from the lowest active platform above firstPlatformY, down to firstPlatformY, within the safe distance
        float fillToY = Mathf.Max(player.position.y - generateAheadDistance, firstPlatformY);

        //find the current lowest platform Y among all active platforms excluding firstPlatformY
        float trueLowestY = highestPlatformY;
        foreach (GameObject platform in activePlatforms)
        {
            if (platform == null) continue;
            if (platform != firstPlatform)
            {
                float y = platform.transform.position.y;
                if (y < trueLowestY) trueLowestY = y;
            }
        }

        float spawnY = trueLowestY;
        while (spawnY > fillToY + 0.01f)
        {
            float spacing = Random.Range(spacingMin, spacingMax);
            float nextPlatformY = spawnY - spacing;

            // avoid spawn below or at the same spot as the first platform
            if (nextPlatformY <= firstPlatformY + 0.01f) break;
            if (nextPlatformY <= firstPlatformY + spacingMin) break;

            SpawnAtY(nextPlatformY);
            spawnY = nextPlatformY;
        }
    }

    void SpawnPlatformAbove()
    {
        while (player.position.y + generateAheadDistance > highestPlatformY)
        {
            float spacing = Random.Range(spacingMin, spacingMax);
            highestPlatformY += spacing;
            SpawnAtY(highestPlatformY);
        }
    }

    //Main spawn function
    void SpawnAtY(float y)
    {
        platformCounter++;

        Vector2 position = new Vector2(Random.Range(-1f, 3f), y);
        GameObject platform = ObjectPoolManager.SpawnObject(platformPrefab, position, Quaternion.identity);
        float speed = GenerateDifferentSpeed(lastPlatformSpeed);
        float width = GenerateDifferentWidth(lastPlatformWidth);
        float currentWidth = platform.GetComponent<SpriteRenderer>().bounds.size.x;
        float scaleRatio = width / currentWidth;
        Vector3 newScale = platform.transform.localScale;
        newScale.x *= scaleRatio;
        platform.transform.localScale = newScale;
        platform.GetComponent<PlatformBehavior>().speed = speed;
        lastPlatformSpeed = speed;

        activePlatforms.Add(platform);
    }

    float GenerateDifferentWidth(float previousWidth)
    {
        float width;
        do
        {
            width = Random.Range(platformWidthMin, platformWidthMax);
        } while (width == previousWidth);
        return width;        
    }

    //try not to repeat platform speeds to avoid awkward shit
    float GenerateDifferentSpeed(float previousSpeed)
    {
        float speed;
        do
        {
            speed = Random.Range(platformSpeedMin, platformSpeedMax);
        } while (Mathf.Abs(speed - previousSpeed) < 0.4f);
        return speed;
    }

    void OptimizePlatforms()
    {
        if (player == null) return;
        List<GameObject> platformsToRemove = new List<GameObject>();

        foreach (GameObject platform in activePlatforms)
        {
            if (platform == null) continue;

            float y = platform.transform.position.y;
            if (platform == firstPlatform) continue;

            // Remove platforms too far above or below the player
            if (y > player.position.y + optimizeDistance || y < player.position.y - optimizeDistance)
            {
                // // Detach player
                // player.SetParent(null);
                ObjectPoolManager.ReturnObjectPool(platform);
                platformsToRemove.Add(platform);
            }
        }

        foreach (GameObject platform in platformsToRemove)
        {
            activePlatforms.Remove(platform);
        }

        highestPlatformY = firstPlatformY;
        foreach (GameObject platform in activePlatforms)
        {
            if (platform == null) continue;
            float y = platform.transform.position.y;
            if (y > highestPlatformY) highestPlatformY = y;
        }
    }

}