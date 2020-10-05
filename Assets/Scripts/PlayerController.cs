using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class PlayerController : MonoBehaviour {
    [SerializeField] private new Rigidbody2D rigidbody2D;
    [SerializeField] private Transform feetTransform = null;
    [SerializeField] private Transform cameraTargetTransform = null;
    [SerializeField] private Animator animator = null;
    [SerializeField] private LayerMask groundMask = 0;
    public bool CanDrag = false;

    [Header("Constants")] [SerializeField] private float runSpeed = 1f;
    [SerializeField] private float baseJumpForce = 400f;

    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;

    // private const float feetRadius = 0.2f;
    private readonly Vector2 feetBox = new Vector2(1.7f, 0.1f);
    private readonly float dragFollowDistance = Mathf.Pow(0.6f, 2);

    /// State Variables
    private Vector2 acceleration = Vector2.zero;

    private Vector2 computedAcceleration = Vector2.zero;
    private Vector2 lastVelocity = Vector2.zero;

    public bool CanJump = true;

    private float horizontalMove = 0f;
    private bool shouldJump = false;
    private bool onGround = false;
    private float jumpCharge = 0f;
    private bool isCharging = false;
    private bool isFacingLeft = false;
    private bool isFrozen = false;
    private bool isDragging = false;
    private readonly Collider2D[] overlapArray = new Collider2D[5];
    private Vector2 cameraBounds;

    [UsedImplicitly]
    private void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();
        cameraBounds = new Vector2(Camera.main.orthographicSize * Screen.width / Screen.height,
            Camera.main.orthographicSize);
        cameraTargetTransform.position = rigidbody2D.position;
    }

    private void OnMouseDown() {
        if (!CanDrag) return;
        isDragging = true;
        SetFreeze(true);
    }

    private void OnMouseUp() {
        if (!CanDrag) return;
        isDragging = false;
        SetFreeze(false);
    }

    private void Move(float speed, float smoothTime) {
        Vector2 targetVelocity = new Vector2(speed, rigidbody2D.velocity.y);
        rigidbody2D.velocity = Vector2.SmoothDamp(
            rigidbody2D.velocity, targetVelocity, ref acceleration,
            smoothTime, Mathf.Infinity, Time.deltaTime
        );
    }

    private void Move(float speed) {
        Move(speed, movementSmoothing);
    }

    public void SetFreeze(bool freeze) {
        acceleration = Vector2.zero;
        if (freeze) {
            isFrozen = true;
            var velocity = rigidbody2D.velocity;
            velocity.x = 0;
            rigidbody2D.velocity = velocity;
            isCharging = false;
            jumpCharge = 0f;
        } else {
            isFrozen = false;
        }
    }

    [UsedImplicitly]
    void Update() {
        if (!isFrozen) {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
            if (Input.GetButtonDown("Jump") && onGround && CanJump) {
                isCharging = true;
            }

            if (Input.GetButtonUp("Jump") && isCharging) {
                shouldJump = true;
                isCharging = false;
            }

            if (isCharging) {
                jumpCharge += Time.deltaTime;
            }
        } else {
            horizontalMove = 0;
        }


        if ((horizontalMove > 0 && isFacingLeft) || (horizontalMove < 0 && !isFacingLeft)) {
            Flip();
        }

        animator.SetBool("OnGround", onGround);
        animator.SetFloat("SpeedY", rigidbody2D.velocity.y);
        animator.SetFloat("SpeedX", Mathf.Abs(rigidbody2D.velocity.x));
        animator.SetFloat("Charge", jumpCharge);
    }

    private void Flip() {
        isFacingLeft = !isFacingLeft;
        GetComponent<SpriteRenderer>().flipX = isFacingLeft;
        // var localScale = transform.localScale;
        // localScale.x *= -1;
        // transform.localScale = localScale;
    }

    [UsedImplicitly]
    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        // Gizmos.DrawSphere(feetTransform.position, FeetRadius);
        Gizmos.DrawCube(feetTransform.position, feetBox);
        var text = rigidbody2D != null ? $"Acc: {computedAcceleration.y}\nVel: {rigidbody2D.velocity.y}" : "";

        var position = transform.position;
        position.x -= 0.6f;
        position.y += 3f;
        Handles.Label(position, text);
    }

    [UsedImplicitly]
    void FixedUpdate() {
        int numberOfHits =
            Physics2D.OverlapBoxNonAlloc(feetTransform.position, feetBox, 0, overlapArray, groundMask);
        // Physics2D.OverlapCircleNonAlloc(feetTransform.position, FeetRadius, overlapArray, groundMask);
        onGround = (numberOfHits > 0);

        if (onGround) {
            var plat = overlapArray[0].gameObject.GetComponent<PlatformHealth>();
            if (plat != null) {
                plat.TakeHit(Time.fixedDeltaTime);
            }
        }

        if (!isCharging && !isFrozen) {
            Move(horizontalMove);
        } else if (isCharging || (isFrozen && !isDragging)) {
            Move(0, movementSmoothing * 10);
        }

        if (isDragging) {
            Vector2 cameraTargetPosition = cameraTargetTransform.transform.position;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.MovePosition(mousePosition);

            rigidbody2D.position =
                Vector2.SmoothDamp(rigidbody2D.position, mousePosition, ref acceleration, Time.fixedDeltaTime);

            cameraBounds *= dragFollowDistance * cameraBounds;
            var delta = mousePosition - cameraTargetPosition;
            if (delta.x * delta.x > cameraBounds.x || delta.y * delta.y > cameraBounds.y) {
                cameraTargetTransform.position +=
                    (Vector3)(delta).normalized * Time.fixedDeltaTime * 10f;
            }

            // cameraTargetTransform.position = 5f*Vector3.up;
        }

        if (shouldJump && !isFrozen) {
            float jumpForce = baseJumpForce;
            if (jumpCharge > 0.15f) {
                jumpForce *= 1f + Mathf.Clamp(jumpCharge - 0.5f, 0, 0.5f);
            }

            var velocity = rigidbody2D.velocity;
            velocity.y = 0;
            rigidbody2D.velocity = velocity;
            rigidbody2D.AddForce(new Vector2(0f, jumpForce));
            shouldJump = false;
            jumpCharge = 0f;
        }

        if (!isDragging) {
            Vector2 position = cameraTargetTransform.position;
            Vector2 delta = Vector2.zero;
            if (Mathf.Abs(horizontalMove) > 0.01) {
                delta.x = ((Mathf.Sign(horizontalMove) * 4 + rigidbody2D.position.x) - position.x);
            } else {
                delta.x = (rigidbody2D.position.x - position.x);
            }

            if (rigidbody2D.velocity.y < 3 && computedAcceleration.y < 0 && !isCharging && !onGround) {
                delta.y = (-3 + rigidbody2D.position.y - position.y);
            } else {
                delta.y = (5 + rigidbody2D.position.y - position.y);
            }

            position += delta * Time.fixedDeltaTime * runSpeed;
            cameraTargetTransform.position = position;
        }

        computedAcceleration = (rigidbody2D.velocity - lastVelocity) / Time.fixedDeltaTime;
        lastVelocity = rigidbody2D.velocity;
    }

    [UsedImplicitly]
    void OnTriggerStay2D(Collider2D aCol) {
        if ((groundMask & 1 << aCol.gameObject.layer) == 0) return;
        acceleration.x = 0;
    }
}
