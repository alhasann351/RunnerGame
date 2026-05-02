/*using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour
{
    public float forwardSpeed = 12f;
    public bool canMove = false;
    public float laneDistance = 1.5f;
    public float laneChangeSpeed = 12f;
    public float centerLaneX = -2f;
    public float jumpForce = 7f;
    public float swipeThreshold = 80f;
    public AudioClip gameOverSound;

    private Animator animator;
    private CharacterInputAction inputActions;
    private Rigidbody rb;

    private bool isGrounded = true;
    private bool isGameOver = false;
    private int targetLane = 1;

    // Touch tracking
    private Vector2 touchStartPos;
    private Vector2 currentTouchPos;
    private bool isTouching = false;
    private bool swipeHandled = false;
    private bool readyToDetect = false; // ← নতুন flag

    private const float MIN_SWIPE_TIME = 0.02f;
    private const float MAX_SWIPE_TIME = 0.5f;
    private float touchStartTime;

    private void Awake()
    {
        inputActions = new CharacterInputAction();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        inputActions.Character.Enable();
        inputActions.Character.TouchPress.started += OnTouchStarted;
        inputActions.Character.TouchPress.canceled += OnTouchEnded;
    }

    private void OnDisable()
    {
        inputActions.Character.TouchPress.started -= OnTouchStarted;
        inputActions.Character.TouchPress.canceled -= OnTouchEnded;
        inputActions.Character.Disable();
    }

    private void Start()
    {
        animator.SetBool("Grounded", true);
    }

    private void OnTouchStarted(InputAction.CallbackContext context)
    {
        if (isGameOver || !canMove) return;

        touchStartPos = inputActions.Character.TouchPosition.ReadValue<Vector2>();
        currentTouchPos = touchStartPos;
        touchStartTime = Time.realtimeSinceStartup;
        isTouching = true;
        swipeHandled = false;
        readyToDetect = false; // ← touch শুরুতে detect বন্ধ রাখো
    }

    private void OnTouchEnded(InputAction.CallbackContext context)
    {
        if (isGameOver || !canMove || !isTouching) return;

        if (!swipeHandled)
        {
            ProcessSwipe(currentTouchPos);
        }

        isTouching = false;
        swipeHandled = false;
        readyToDetect = false;
    }

    private void Update()
    {
        if (isGameOver || !canMove || !isTouching) return;

        currentTouchPos = inputActions.Character.TouchPosition.ReadValue<Vector2>();

        // ← প্রথম ফ্রেম skip করো, পরের ফ্রেম থেকে detect শুরু
        if (!readyToDetect)
        {
            // touchStartPos কে এই ফ্রেমের real position দিয়ে update করো
            touchStartPos = currentTouchPos;
            readyToDetect = true;
            return;
        }

        if (!swipeHandled)
        {
            Vector2 swipe = currentTouchPos - touchStartPos;
            float elapsed = Time.realtimeSinceStartup - touchStartTime;

            if (swipe.magnitude >= swipeThreshold && elapsed >= MIN_SWIPE_TIME)
            {
                ProcessSwipe(currentTouchPos);
                swipeHandled = true;
            }
        }
    }

    private void ProcessSwipe(Vector2 endPos)
    {
        float elapsed = Time.realtimeSinceStartup - touchStartTime;
        if (elapsed > MAX_SWIPE_TIME) return;

        Vector2 swipe = endPos - touchStartPos;
        if (swipe.magnitude < swipeThreshold) return;

        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
        {
            if (swipe.x > 0) MoveRight();
            else MoveLeft();
        }
        else
        {
            if (swipe.y > 0) Jump();
        }
    }

    private void MoveLeft()
    {
        if (targetLane > 0)
        {
            targetLane--;
            animator.ResetTrigger("Left");
            animator.SetTrigger("Left");
        }
    }

    private void MoveRight()
    {
        if (targetLane < 2)
        {
            targetLane++;
            animator.ResetTrigger("Right");
            animator.SetTrigger("Right");
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            isGrounded = false;
            animator.SetBool("Grounded", false);
            animator.ResetTrigger("Jump");
            animator.SetTrigger("Jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("Grounded", true);
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (isGameOver) return;

            isGameOver = true;

            if (gameOverSound != null)
            {
                AudioSource.PlayClipAtPoint(gameOverSound, transform.position);
                Invoke(nameof(RestartGame), gameOverSound.length);
            }
            else
            {
                RestartGame();
            }
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void FixedUpdate()
    {
        if (isGameOver || !canMove) return;

        float targetX = centerLaneX + (targetLane - 1) * laneDistance;

        float newX = Mathf.MoveTowards(
            rb.position.x,
            targetX,
            laneChangeSpeed * Time.fixedDeltaTime
        );

        Vector3 newPosition = new Vector3(
            newX,
            rb.position.y,
            rb.position.z + forwardSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);
    }

}*/




using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float forwardSpeed = 12f;
    public float laneDistance = 1.5f;
    public float laneChangeSpeed = 12f;
    public float centerLaneX = -2f;
    public float jumpForce = 7f;
    public float swipeThreshold = 100f;
    public AudioClip gameOverSound;

    private Animator animator;

    private CharacterInputAction inputActions;
    private Rigidbody rb;

    private bool isGrounded = true;
    private bool isGameOver = false;

    private int targetLane = 1;

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;

    private void Awake()
    {
        inputActions = new CharacterInputAction();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        inputActions.Character.Enable();

        inputActions.Character.TouchPress.started += OnTouchStarted;
        inputActions.Character.TouchPress.canceled += OnTouchEnded;
    }

    private void OnDisable()
    {
        inputActions.Character.TouchPress.started -= OnTouchStarted;
        inputActions.Character.TouchPress.canceled -= OnTouchEnded;

        inputActions.Character.Disable();
    }

    private void Start()
    {
        animator.SetBool("Grounded", true);
    }

    private void OnTouchStarted(InputAction.CallbackContext context)
    {
        if (isGameOver) return;

        touchStartPos = inputActions.Character.TouchPosition.ReadValue<Vector2>();
    }

    private void OnTouchEnded(InputAction.CallbackContext context)
    {
        if (isGameOver) return;

        touchEndPos = inputActions.Character.TouchPosition.ReadValue<Vector2>();

        Vector2 swipe = touchEndPos - touchStartPos;

        if (swipe.magnitude < swipeThreshold)
            return;

        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
        {
            if (swipe.x > 0)
                MoveRight();
            else
                MoveLeft();
        }
        else
        {
            if (swipe.y > swipeThreshold)
                Jump();
        }
    }

    private void MoveLeft()
    {
        if (targetLane > 0)
        {
            targetLane--;
            animator.ResetTrigger("Left");
            animator.SetTrigger("Left");
        }
    }

    private void MoveRight()
    {
        if (targetLane < 2)
        {
            targetLane++;
            animator.ResetTrigger("Right");
            animator.SetTrigger("Right");
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            isGrounded = false;
            animator.SetBool("Grounded", false);
            animator.ResetTrigger("Jump");
            animator.SetTrigger("Jump");

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("Grounded", true);
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (isGameOver) return;

            isGameOver = true;

            if (gameOverSound != null)
            {
                AudioSource.PlayClipAtPoint(gameOverSound, transform.position);
                Invoke(nameof(RestartGame), gameOverSound.length);
            }
            else
            {
                RestartGame();
            }
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void FixedUpdate()
    {
        if (isGameOver) return;

        float targetX = centerLaneX + (targetLane - 1) * laneDistance;

        float newX = Mathf.MoveTowards(
            rb.position.x,
            targetX,
            laneChangeSpeed * Time.fixedDeltaTime
        );

        Vector3 newPosition = new Vector3(
            newX,
            rb.position.y,
            rb.position.z + forwardSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);
    }
}
