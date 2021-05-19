using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    public bool isJumping { get; set; }

    public Transform cameraSlot;

    [HideInInspector] public CharacterController controller;
    PlayerInputs playerInputs;
    CameraControls cameraControls;


    [Header("Player settings")]
    [SerializeField] float moveSpeed = 5f;

    [Header("Camera Settings")]
    [SerializeField] float cameraSensitivity = 5f;

    [Header("Jump Settings")]
    [SerializeField] float Gravity = 9.81f;
    [SerializeField] float jumpHeight = 4f;
    [SerializeField] float jumpForwardAppliedForce = .5f;
    [SerializeField] float airControl = 5f;
    [SerializeField] float stepDown = .2f;

    Vector3 velocity;
    AnimationController animationController;

    float cameraXrotation;
    float playerYrotation;

    [HideInInspector] public bool _jumpInput;

    private void Awake()
    {
        playerInputs = GetComponent<PlayerInputs>();
        controller = GetComponent<CharacterController>();
        cameraControls = GetComponent<CameraControls>();
        animationController = GetComponent<AnimationController>();
    }

    private void Update()
    {
        playerInputs.JumpInput();

        if (_jumpInput)
        {
            _jumpInput = false;
            animationController.JumpingAnimation(true);
            Jump();
        }
    }

    private void FixedUpdate()
    {
        PlayerMovement();
        LookCamera();
    }

    void Jump()
    {
        if (!isJumping)
        {
            isJumping = true;
            velocity = controller.velocity * jumpForwardAppliedForce;

            //calculating jump force
            velocity.y = Mathf.Sqrt(2 * Gravity * jumpHeight);
        }
    }

    private void LookCamera()
    {
        cameraXrotation = Mathf.Clamp(cameraXrotation - (cameraControls.mouseControls().y), -70f, 70f);
        cameraSlot.localRotation = Quaternion.Lerp(cameraSlot.localRotation,
                                    Quaternion.Euler(cameraXrotation, 0f, 0f),
                                    cameraSensitivity * Time.fixedDeltaTime);
                                    
        playerYrotation = Mathf.Lerp(playerYrotation,
                            cameraControls.mouseControls().x, cameraSensitivity * Time.fixedDeltaTime);
        transform.Rotate(transform.up, playerYrotation);
    }

    void PlayerMovement()
    {
        if (isJumping)
        {
            velocity.y -= Gravity * Time.fixedDeltaTime;

            Vector3 playerDisplacement = velocity * Time.fixedDeltaTime;
            playerDisplacement += CalculateAirControl();
            controller.Move(playerDisplacement);
            isJumping = !controller.isGrounded;
            animationController.isGruondedFunc(false);
        }
        else
        {
            animationController.isGruondedFunc(true);
            animationController.JumpingAnimation(false);
            Vector3 movement = move() * moveSpeed * Time.fixedDeltaTime;
            Vector3 _stepDown = Vector3.down * stepDown;

            Move(movement, _stepDown);
            animationController.playerMovementAnimation(playerInputs.Movement());

            //in case we are falling  
            if (!controller.isGrounded)
            {
                animationController.isGruondedFunc(false);
                isJumping = true;
                velocity = controller.velocity * jumpForwardAppliedForce;
                velocity.y = 0f;
            }
        }
    }

    Vector3 CalculateAirControl()
    {
        return move() * (airControl / 100f);
    }

    void Move(Vector3 move, Vector3 _stepdown)
    {
        controller.Move(move + _stepdown);
    }

    Vector3 move()
    {
        return (transform.forward *
            (playerInputs.Movement().y < 0 ? playerInputs.Movement().y / 2f : playerInputs.Movement().y)
                + transform.right * (playerInputs.Movement().x / 2f));
    }
}
