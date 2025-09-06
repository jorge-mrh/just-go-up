using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer))]
public class PlatformBehavior : MonoBehaviour
{

    // [SerializeField] private GameObject dangerPlatform;
    // [SerializeField] private GameObject springPlatform;

    public float speed = 2f;
    public PlatformType currentPlatformType;
    public float springMultiplier = 1.8f;
    public float direction = 1f;
    private float leftBound;
    private float rightBound;
    private Rigidbody2D playerRb;
    private Transform player;
    private PlayerController playerController;
    private int platformNo;
    private float platformHalfWidth;

    [SerializeField] Color regularPlatformColor;
    [SerializeField] Color dangerPlatformColor;
    [SerializeField] Color springPlatformColor;

    [SerializeField] Light2D regularLightLeft;
    [SerializeField] Light2D regularLightRight;
    [SerializeField] Light2D centerLight;
    private AudioSource landAudio;

    public enum PlatformType
    {
        Regular,
        Danger,
        Spring
    }

    void Awake()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;
        if (playerRb == null)
            playerRb = player.GetComponent<Rigidbody2D>();
        if (playerController == null)
            playerController = player.GetComponent<PlayerController>();

        regularLightLeft.intensity = 0;
        regularLightRight.intensity = 0;
        centerLight.intensity = 0;
        GetComponent<SpriteRenderer>().color = regularPlatformColor;
    }

    void Update()
    {
        if (player.IsChildOf(transform))
        {
            centerLight.intensity = 0.3f;
            Vector3 centerLightPos = centerLight.transform.position;
            centerLightPos = new Vector3(player.position.x, centerLightPos.y, centerLightPos.z);
            centerLight.transform.position = centerLightPos;
        }
        else
        {
            centerLight.intensity = 0f;
        }

        transform.Translate(Vector2.right * speed * direction * Time.deltaTime);
        ApplyPlatformBehaviour(currentPlatformType);
    }

    void ApplyPlatformBehaviour(PlatformType platformType)
    {
        switch (platformType)
        {
            case PlatformType.Regular:
                RegularPlatform();
                break;
            case PlatformType.Danger:
                DangerPlatform();
                break;
            case PlatformType.Spring:
                SpringPlatform();
                break;
            default:
                RegularPlatform();
                break;

        }

    }

    //basic behaviours

    void Hide()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (direction == -1f)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 0, 0.7f);
            regularLightLeft.intensity = 0.2f; 
            regularLightRight.intensity = 0.2f;
            collider.excludeLayers |= (1 << 6);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = dangerPlatformColor;
            regularLightLeft.intensity = 1f;
            regularLightRight.intensity = 1f;
            collider.excludeLayers &= ~(1 << 6);
        }
    }

    void Blink()
    {
        float lefRand = Random.Range(0.2f, 1f);
        float RightRand = Random.Range(0.2f, 1f);
        regularLightLeft.intensity = lefRand;
        regularLightRight.intensity = RightRand;
    }

    //Platforms
    void RegularPlatform()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.excludeLayers &= ~(1 << 6);
        regularLightLeft.intensity = 1f;
        regularLightRight.intensity = 1f;
        GetComponent<SpriteRenderer>().color = regularPlatformColor;
    }

    void DangerPlatform()
    {
        GetComponent<SpriteRenderer>().color = dangerPlatformColor;
        Hide();
    }

    void SpringPlatform()
    {
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<SpriteRenderer>().color = springPlatformColor;
        Blink();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "WallLeft")
        {
            direction = 1f;
        }
        else if (collision.gameObject.tag == "WallRight")
        {
            direction = -1f;
        }
    }
    
}