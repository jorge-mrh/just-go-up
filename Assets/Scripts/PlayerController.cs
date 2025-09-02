using UnityEngine;
using UnityEngine.Rendering.Universal;
using static PlatformBehavior;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    private Touch touch;
    private Rigidbody2D rb;
    private int numOfJumps;
    private bool canJump;
    public static float currentScore = 0f;
    private float startingY;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float scoreMultiplier = 10f;
    [SerializeField] private Light2D playerLight;

    private CinemachineImpulseSource impulseSource;


    void Start()
    {
        startingY = transform.position.y;
        rb = GetComponent<Rigidbody2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Update()
    {
        float rawScore = (transform.position.y - startingY) * scoreMultiplier;
        currentScore = Mathf.Max(0, Mathf.RoundToInt(rawScore));

        bool canJump =
            ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) ||
            Input.GetMouseButtonDown(0)) && numOfJumps > 0;
        if (canJump)
        {
            Jump();
        }
    }

    void Jump()
    {
        numOfJumps--;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        SoundManager.instance.PlayJumpSound();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    float platformY = collision.gameObject.transform.position.y;
                    if (collision.gameObject.GetComponent<PlatformBehavior>().currentPlatformType == PlatformType.Spring)
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10f);
                    }

                    CameraShakeManager.instance.CameraShake(impulseSource);
                    SoundManager.instance.PlayLandingSound();
                    transform.SetParent(collision.transform, true);
                    numOfJumps = 2;
                    return;
                }
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            // Unparent when leaving platform
            transform.SetParent(null);
        }
    }

}
