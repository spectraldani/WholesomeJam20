using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody2D myRigidbody2D = null;
    [SerializeField] private Transform feetTransform = null;

    [SerializeField] private float runSpeed = 40f;
    [SerializeField] private float jumpForce = 400f;
    [Range(0, .3f)] [SerializeField] private float movimentSmoothing = .05f;
    private const float feetRadius = 0.2f;

    /// State Variables
    private Vector2 myVelocity = Vector2.zero;
    private float horizontalMove = 0f;
    private bool shouldJump = false;
    public bool onGround = false;
    //private bool isFacingLeft = false;
    private Collider2D[] overlapArray = new Collider2D[5];
    public LayerMask groundMask;

    private void Awake() {
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }


    private void move(float speed) {
        Vector2 targetVelocity = new Vector2(speed * 10f, myRigidbody2D.velocity.y);
        myRigidbody2D.velocity = Vector2.SmoothDamp(myRigidbody2D.velocity, targetVelocity, ref myVelocity, movimentSmoothing);
    }

    void Update() {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (Input.GetButtonDown("Jump") && onGround) {
            shouldJump = true;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(feetTransform.position, feetRadius);
    }

    void FixedUpdate() {
        onGround = false;
        int objs = Physics2D.OverlapCircleNonAlloc(feetTransform.position, feetRadius, overlapArray, groundMask);
        for (int i = 0; i < objs; i++) {
            var overlapped = overlapArray[i];
            if (overlapped.gameObject != gameObject) {
                onGround = true;
                break;
            }
        }
        move(horizontalMove * Time.fixedDeltaTime);
        if (shouldJump) {
            myRigidbody2D.AddForce(new Vector2(0f, jumpForce));
            shouldJump = false;
        }
    }
}
