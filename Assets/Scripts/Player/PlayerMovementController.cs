using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    #region Variables
    private CharacterController characterController;
    private Animator playerAnimator;
    private InputSystem inputSystem;

    private Vector3 movementInput;
    private bool sprintIsPerformed;
    public bool isSprinting; //public because can use from stamina class
    private bool isWalking;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeedForward = 5f;
    [SerializeField] private float moveSpeedBackward = 3f;
    [SerializeField] private float moveSpeedLeft = 4f;
    [SerializeField] private float moveSpeedRight = 4f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float currentMoveSpeed;

    [Header("Components")]
    [SerializeField] private GameObject playerCamera;

    [Header("Head Bob")]
    public bool enableHeadBob = true;
    public Transform joint;
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    private Vector3 jointOriginalPos;
    private float timer = 0;
    #endregion





    #region Unity Methods
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerAnimator = GetComponent<Animator>();
        if (characterController == null || playerAnimator == null)
            Debug.LogError("Missing Player Components");

        inputSystem = new InputSystem();
        inputSystem.Enable();

        inputSystem.Player.Sprint.performed += _ => sprintIsPerformed = true;
        inputSystem.Player.Sprint.canceled += _ => sprintIsPerformed = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //Call Read&Check Methods
        ReadMovementInput();
        CheckWalking();

        //Call Movement Methods
        Movement();
        Sprint();
        PlayerRotation();

        //Call Updaters
        SetSpeed();
        SetAnimations(movementInput);

        //Call other Methods
        HeadBob();
    }
    #endregion




    #region Read&Check Methods
    void ReadMovementInput()
    {
        Vector2 input = inputSystem.Player.Movement.ReadValue<Vector2>();
        movementInput = new Vector3(input.x, 0, input.y);
    }
    void CheckWalking()
    {
        if (movementInput != Vector3.zero) isWalking = true;
        else isWalking = false;
    }
    #endregion





    #region Custom Methods
    void Movement()
    {
        Vector3 move = playerCamera.transform.forward * movementInput.z + playerCamera.transform.right * movementInput.x;
        move.y = 0f;
        characterController.Move(move.normalized * currentMoveSpeed * Time.deltaTime);
    }

    void PlayerRotation()
    {
        transform.rotation = Quaternion.Euler(0f, playerCamera.transform.eulerAngles.y, 0f);
    }

    void Sprint()
    {
        bool canSprintForward = sprintIsPerformed && movementInput.z > 0.05f && Mathf.Abs(movementInput.x) < 0.05f;

        if (canSprintForward)
        {
            currentMoveSpeed = sprintSpeed;
            playerAnimator.SetBool("FRunning", true);
            isSprinting = true;
        }
        else
        {
            playerAnimator.SetBool("FRunning", false);
            isSprinting = false;
        }
    }
    private void HeadBob()
    {
        if (isWalking)
        {
            if (isSprinting)
            {
                timer += Time.deltaTime * (bobSpeed + sprintSpeed);
            }
            else
            {
                timer += Time.deltaTime * bobSpeed;
            }
            joint.localPosition = new Vector3(jointOriginalPos.x + Mathf.Sin(timer) * bobAmount.x, jointOriginalPos.y + Mathf.Sin(timer) * bobAmount.y, jointOriginalPos.z + Mathf.Sin(timer) * bobAmount.z);
        }
        else
        {
            timer = 0;
            joint.localPosition = new Vector3(Mathf.Lerp(joint.localPosition.x, jointOriginalPos.x, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.y, jointOriginalPos.y, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.z, jointOriginalPos.z, Time.deltaTime * bobSpeed));
        }
    }
    #endregion





    #region Setters
    void SetAnimations(Vector3 localInput)
    {
        float threshold = 0.05f;

        playerAnimator.SetBool("FWalking", false);
        playerAnimator.SetBool("BWalking", false);
        playerAnimator.SetBool("LWalking", false);
        playerAnimator.SetBool("RWalking", false);

        if (localInput.x < -threshold) playerAnimator.SetBool("LWalking", true);
        else if (localInput.x > threshold) playerAnimator.SetBool("RWalking", true);
        else if (localInput.z > threshold) playerAnimator.SetBool("FWalking", true);
        else if (localInput.z < -threshold) playerAnimator.SetBool("BWalking", true);
    }

    void SetSpeed()
    {
        if (playerAnimator.GetBool("FRunning")) return;

        float threshold = 0.05f;

        if (movementInput.z > threshold) currentMoveSpeed = moveSpeedForward;
        else if (movementInput.z < -threshold) currentMoveSpeed = moveSpeedBackward;
        else if (movementInput.x < -threshold) currentMoveSpeed = moveSpeedLeft;
        else if (movementInput.x > threshold) currentMoveSpeed = moveSpeedRight;
        else currentMoveSpeed = 0f;
    }
    #endregion
}