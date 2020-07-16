using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
/// <summary>
/// HANDLES the movement of the hero --- WASD movement with the possibility to use a free orbit and a locked orbit camera. 
/// Free orbit -> camera goes around the hero without changing his direction; left and right movement rotates the hero.
/// Locked orbit -> camera goes around the hero but changes hero look direction as well; left and right movement moves the hero left and right.
/// Slope limit feature -> hero can't jump or climb slopes above a certain angle.
/// 
/// HANDLES hero states together with his animator.
/// Supports attack animation queueing -> if the player presses a certain attack while already attacking, that attack will play
/// automatically after the first one finishes. Supports only 1 animation queue slot, the last pressed attack takes priority.
/// </summary>
enum MovementState { None, Idle, Forward, Backward, Left, Right, Attack, Roll, SwitchingEquipment }

public class TPMovement_Controller : MonoBehaviour
{
    public bool debugMode = false;

    public static TPMovement_Controller instance;

    [Header("Prefabs")]
    public GameObject cameraObject;
    public CinemachineFreeLook cineFreeLookVcam;
    public GameObject weapon1visual, weapon2visual, bowVisual, arrowVisual;
    public GameObject arrowToShootPrefab, arrowSpawnPoint, arrowFirePrefab;
    public UnityEngine.UI.Slider rageBar;
    public Animator rageBarAnim;
    public GameObject alertPanel;
    private UnityEngine.UI.Text alertPanelText;
    public GameObject questPanel;
    public GameObject interactableAlertText;
    public GameObject hitObjectParticle;
    [Header("Cursor options")]
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    [HideInInspector]
    public Vector2 cursorHotSpot = Vector2.zero;

    [Header("Stats")]
    public float maxHealth;
    private float currentHealth;
    public float maxRage;
    private float currentRage;
    [Header("Movement")]
    [Tooltip("The maximum angle the player is allowed to walk and jump on. On a flat ground the angle is 90. Going up a hill is > 90, going downhill is < 90.")]
    public float maxAngle = 120f;
    private float groundAngle; //the calculated angle between the player and the direction he is moving to
    public float walkSpeed;
    public float sprintSpeed;
    public float rotationSpeed;
    public float jumpForce;
    public float rollForwardForce = 50f;
    public float rollCooldown = 4f;
    private float rollCooldownTimestamp;
    public float turnSmoothTime = 0.1f;
    [Tooltip("Roll animation and state will not last longer than this value after going into that state.")]
    public float maxRollDuration = 1.3f;
    private float elapsedRollDuration;
    private float turnSmoothVelocity;

    [Header("Attack")]
    public float autoAttackDamage = 10f;
    public float attackRange = 5f;
    public float autoKnockbackPower = 10f;
    [Tooltip("Attack animation and state will not last longer than this value after going into that state.")]
    public float attack1MaxDuration = 0.75f;
    public float attack1ForwardLungeForce = 25f;
    [Tooltip("Attack animation and state will not last longer than this value after going into that state.")]
    public float attack2MaxDuration = 0.8f;
    public float attack2RageRequirement = 350f;
    [Tooltip("Attack animation and state will not last longer than this value after going into that state.")]
    public float attack3MaxDuration = 0.8f;
    public float attack3RageRequirement = 750f;
    public float attack3Radius = 2f;
    private float internalAttack1Cooldown, internalAttack2Cooldown, internalAttack3Cooldown;
    private float internalAttack1Timestamp, internalAttack2Timestamp, internalAttack3Timestamp;
    public bool bowUnlocked = false;
    public float maxSwitchEquipmentDuration;
    private float elapsedSwitchedEquipmentDuration;


    private float elapsedAttackDuration;
    [Tooltip("Currently the attack raycast comes off the pivot of the model. If this is a bit into the ground it will not work so using some Y offset fixes this.")]
    public float attackYoffset = 0.5f;
    public LayerMask enemyLayer;


    [Header("Misc")]
    public float interactionRange;
    public LayerMask interactionLayer;
    public float checkForInteractableAlertEveryXSeconds = 2f;
    private float interactableCheckedTimestamp;

    private float currentMoveSpeed;
    private bool is_Grounded, is_Sprinting;
    private bool is_LockedOrbit = false, is_FreeOrbit = false;
    private bool is_SwitchedToBow = false;

    private ControlsGame controls;
    private MovementState movementState;
    private Animator anim;

    private Vector2 moveAxis;
    private Vector2 mouseMoveAxis;
    private Rigidbody rb;
    private CapsuleCollider playerCollider;
    private Vector3 direction = Vector3.zero; //the current direction of movement; changed constantly based on input and camera direction
    private float angle; //use this to adjust hero rotation based on camera rotation

    private int queuedAttackNum = 0; //the last queued attack that has to play after the current attack has finished playing
    InputAction.CallbackContext fakeContext; //entirely in order to pass it as a parameter to the Handle methods; serves no other purpose

    private readonly RaycastHit[] _groundCastResults = new RaycastHit[8]; //raycasts for checking if grounded

    #region InitialSetup
    void Awake()
    {
        controls = new ControlsGame();

        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        alertPanelText = alertPanel.GetComponentInChildren<UnityEngine.UI.Text>();

        Cursor.SetCursor(cursorTexture, cursorHotSpot, cursorMode);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ToggleFreeLook();
        currentMoveSpeed = walkSpeed;
        currentHealth = maxHealth;
        rageBar.maxValue = maxRage;
        SetAttackCooldowns();
        is_Sprinting = false;
        movementState = MovementState.Idle;
    }

    private void OnEnable()
    {
        controls.MainControls.Movement.performed += HandleMove;
        controls.MainControls.Movement.canceled += HandleMove;
        controls.MainControls.SprintToggle.performed += HandleSprint;
        controls.MainControls.LockedOrbit.performed += HandleLockedOrbit;
        controls.MainControls.LockedOrbit.canceled += HandleLockedOrbit;
        controls.MainControls.FreeOrbit.performed += HandleFreeOrbit;
        controls.MainControls.FreeOrbit.canceled += HandleFreeOrbit;
        controls.MainControls.Jump.performed += HandleJump;
        controls.MainControls.Attackone.performed += HandleAttack1;
        controls.MainControls.Attacktwo.performed += HandleAttack2;
        controls.MainControls.Attackthree.performed += HandleAttack3;
        controls.MainControls.RollForward.performed += HandleRollForward;
        controls.MainControls.RollBackward.performed += HandleRollBackward;
        controls.MainControls.MouseDelta.performed += HandleMouseDelta;
        controls.MainControls.MouseDelta.canceled += HandleMouseDelta;
        controls.MainControls.EquipmentSwitch.performed += HandleEquipmentSwitch;
        controls.MainControls.Interact.performed += HandleInteract;
        controls.MainControls.ToggleQuestPanel.performed += HandleToggleQuestPanel;
        controls.MainControls.Movement.Enable();
        controls.MainControls.SprintToggle.Enable();
        controls.MainControls.LockedOrbit.Enable();
        controls.MainControls.FreeOrbit.Enable();
        controls.MainControls.Jump.Enable();
        controls.MainControls.Attackone.Enable();     
        controls.MainControls.Attacktwo.Enable();
        controls.MainControls.Attackthree.Enable();
        controls.MainControls.RollForward.Enable();
        controls.MainControls.RollBackward.Enable();
        controls.MainControls.MouseDelta.Enable();
        controls.MainControls.EquipmentSwitch.Enable();
        controls.MainControls.Interact.Enable();
        controls.MainControls.ToggleQuestPanel.Enable();
    }

    private void OnDisable()
    {
        controls.MainControls.Movement.performed -= HandleMove;
        controls.MainControls.Movement.canceled -= HandleMove;
        controls.MainControls.SprintToggle.performed -= HandleSprint;
        controls.MainControls.LockedOrbit.performed -= HandleLockedOrbit;
        controls.MainControls.LockedOrbit.canceled -= HandleLockedOrbit;
        controls.MainControls.FreeOrbit.performed -= HandleFreeOrbit;
        controls.MainControls.FreeOrbit.canceled -= HandleFreeOrbit;
        controls.MainControls.Jump.performed -= HandleJump;
        controls.MainControls.Attackone.performed -= HandleAttack1;
        controls.MainControls.Attacktwo.performed -= HandleAttack2;
        controls.MainControls.Attackthree.performed -= HandleAttack3;
        controls.MainControls.RollForward.performed -= HandleRollForward;
        controls.MainControls.RollBackward.performed -= HandleRollBackward;
        controls.MainControls.MouseDelta.performed -= HandleMouseDelta;
        controls.MainControls.MouseDelta.canceled -= HandleMouseDelta;
        controls.MainControls.EquipmentSwitch.performed -= HandleEquipmentSwitch;
        controls.MainControls.Interact.performed -= HandleInteract;
        controls.MainControls.ToggleQuestPanel.performed -= HandleToggleQuestPanel;
        controls.MainControls.Movement.Disable();
        controls.MainControls.SprintToggle.Disable();
        controls.MainControls.LockedOrbit.Disable();
        controls.MainControls.FreeOrbit.Disable();
        controls.MainControls.Jump.Disable();
        controls.MainControls.Attackone.Disable();
        controls.MainControls.Attacktwo.Disable();
        controls.MainControls.Attackthree.Disable();
        controls.MainControls.RollForward.Disable();
        controls.MainControls.RollBackward.Disable();
        controls.MainControls.MouseDelta.Disable();
        controls.MainControls.EquipmentSwitch.Disable();
        controls.MainControls.Interact.Disable();
        controls.MainControls.ToggleQuestPanel.Disable();
    }
    #endregion

    #region Updates
    private void Update()
    {
        if (movementState == MovementState.Idle)
        {
            anim.SetBool("is_WalkBackward", false);
            anim.SetBool("is_WalkForward", false);
            anim.SetBool("is_Idle", true);
            anim.SetBool("is_WalkLeft", false);
            anim.SetBool("is_WalkRight", false);
            anim.SetBool("is_SprintForward", false);
            anim.SetBool("is_SprintBackward", false);
            anim.SetBool("is_RollForward", false);
            anim.SetBool("is_RollBackward", false);
            anim.SetBool("is_Attack1", false);
            anim.SetBool("is_Attack2", false);
            anim.SetBool("is_Attack3", false);
            anim.SetBool("is_BowAttack1", false);
            anim.SetBool("is_BowAttack2", false);
        }
        else if (movementState == MovementState.Forward)
        {
            anim.SetBool("is_WalkForward", true);
            if (is_Sprinting)
            {
                anim.SetBool("is_SprintForward", true);
            }
            else
            {
                anim.SetBool("is_SprintForward", false);
            }
            anim.SetBool("is_WalkBackward", false);
            anim.SetBool("is_Idle", false);
            anim.SetBool("is_WalkLeft", false);
            anim.SetBool("is_WalkRight", false);
            anim.SetBool("is_SprintBackward", false);
            anim.SetBool("is_RollForward", false);
            anim.SetBool("is_RollBackward", false);
            anim.SetBool("is_Attack1", false);
            anim.SetBool("is_Attack2", false);
            anim.SetBool("is_Attack3", false);
            anim.SetBool("is_BowAttack1", false);
            anim.SetBool("is_BowAttack2", false);
        }
        else if (movementState == MovementState.Backward)
        {
            anim.SetBool("is_WalkBackward", true);
            anim.SetBool("is_WalkForward", false);
            anim.SetBool("is_Idle", false);
            anim.SetBool("is_WalkLeft", false);
            anim.SetBool("is_WalkRight", false);
            anim.SetBool("is_SprintForward", false);
            anim.SetBool("is_RollForward", false);
            anim.SetBool("is_RollBackward", false);
            anim.SetBool("is_Attack1", false);
            anim.SetBool("is_Attack2", false);
            anim.SetBool("is_Attack3", false);
            anim.SetBool("is_BowAttack1", false);
            anim.SetBool("is_BowAttack2", false);
        }
        else if (movementState == MovementState.Left)
        {
            anim.SetBool("is_WalkBackward", false);
            anim.SetBool("is_WalkForward", false);
            anim.SetBool("is_Idle", false);
            anim.SetBool("is_WalkLeft", true);
            anim.SetBool("is_WalkRight", false);
            anim.SetBool("is_SprintForward", false);
            anim.SetBool("is_SprintBackward", false);
            anim.SetBool("is_RollForward", false);
            anim.SetBool("is_RollBackward", false);
            anim.SetBool("is_Attack1", false);
            anim.SetBool("is_Attack2", false);
            anim.SetBool("is_Attack3", false);
            anim.SetBool("is_BowAttack1", false);
            anim.SetBool("is_BowAttack2", false);
        }
        else if (movementState == MovementState.Right)
        {
            anim.SetBool("is_WalkBackward", false);
            anim.SetBool("is_WalkForward", false);
            anim.SetBool("is_Idle", false);
            anim.SetBool("is_WalkLeft", false);
            anim.SetBool("is_WalkRight", true);
            anim.SetBool("is_SprintForward", false);
            anim.SetBool("is_SprintBackward", false);
            anim.SetBool("is_RollForward", false);
            anim.SetBool("is_RollBackward", false);
            anim.SetBool("is_Attack1", false);
            anim.SetBool("is_Attack2", false);
            anim.SetBool("is_Attack3", false);
            anim.SetBool("is_BowAttack1", false);
            anim.SetBool("is_BowAttack2", false);
        }
        else if (movementState == MovementState.Attack)
        {
            anim.SetBool("is_WalkBackward", false);
            anim.SetBool("is_WalkForward", false);
            anim.SetBool("is_Idle", false);
            anim.SetBool("is_WalkLeft", false);
            anim.SetBool("is_WalkRight", false);
            anim.SetBool("is_RollForward", false);
            anim.SetBool("is_RollBackward", false);
            anim.SetBool("is_SprintForward", false);
            anim.SetBool("is_SprintBackward", false);
        //    Debug.Log(elapsedAttackDuration);
            elapsedAttackDuration += Time.deltaTime;

            if (elapsedAttackDuration >= attack1MaxDuration && (anim.GetBool("is_Attack1") || anim.GetBool("is_BowAttack1")))
            {
                //  Debug.Log("ATTACK ANIMATION CAUGHT BY TIMER");              
                GoToNoneState();
                DisableArrowVisual();
            }
            else if(elapsedAttackDuration >= attack2MaxDuration && (anim.GetBool("is_Attack2") || anim.GetBool("is_BowAttack2")))
            {
                GoToNoneState();
                DisableArrowVisual();
            }
            else if (elapsedAttackDuration >= attack3MaxDuration && (anim.GetBool("is_Attack3")))
            {
                GoToNoneState();
                DisableArrowVisual();
            }
        }
        else if(movementState == MovementState.Roll)
        {
         //   anim.SetBool("is_RollForward", true);
          //  anim.SetBool("is_RollBackward", true);
            anim.SetBool("is_WalkBackward", false);
            anim.SetBool("is_WalkForward", false);
            anim.SetBool("is_Idle", false);
            anim.SetBool("is_WalkLeft", false);
            anim.SetBool("is_WalkRight", false);
            anim.SetBool("is_SprintForward", false);
            anim.SetBool("is_SprintBackward", false);
            anim.SetBool("is_Attack1", false);
            anim.SetBool("is_Attack2", false);
            anim.SetBool("is_Attack3", false);
            anim.SetBool("is_BowAttack1", false);
            anim.SetBool("is_BowAttack2", false);
            //     Debug.Log(elapsedRollDuration);
            elapsedRollDuration += Time.deltaTime;
            if(elapsedRollDuration >= maxRollDuration)
            {
                Debug.Log("ROLL CAUGHT BY TIMER");
                GoToNoneState();
            }
        }
        else if(movementState == MovementState.SwitchingEquipment)
        {
            anim.SetBool("is_WalkBackward", false);
            anim.SetBool("is_WalkForward", false);
            anim.SetBool("is_Idle", false);
            anim.SetBool("is_WalkLeft", false);
            anim.SetBool("is_WalkRight", false);
            anim.SetBool("is_SprintForward", false);
            anim.SetBool("is_SprintBackward", false);
            anim.SetBool("is_RollForward", false);
            anim.SetBool("is_RollBackward", false);
            anim.SetBool("is_Attack1", false);
            anim.SetBool("is_Attack2", false);
            anim.SetBool("is_Attack3", false);
            anim.SetBool("is_BowAttack1", false);
            anim.SetBool("is_BowAttack2", false);
            elapsedSwitchedEquipmentDuration += Time.deltaTime;
            if (elapsedSwitchedEquipmentDuration >= maxSwitchEquipmentDuration)
            {
                Debug.Log("SWITCH CAUGHT BY TIMER");
                GoToNoneState();
            }
        }
        else if(movementState == MovementState.None)
        {
            anim.SetBool("is_WalkBackward", false);
            anim.SetBool("is_WalkForward", false);
            anim.SetBool("is_Idle", false);
            anim.SetBool("is_WalkLeft", false);
            anim.SetBool("is_WalkRight", false);
            anim.SetBool("is_SprintForward", false);
            anim.SetBool("is_SprintBackward", false);
            anim.SetBool("is_RollForward", false);
            anim.SetBool("is_RollBackward", false);
            anim.SetBool("is_Attack1", false);
            anim.SetBool("is_Attack2", false);
            anim.SetBool("is_Attack3", false);
            anim.SetBool("is_BowAttack1", false);
            anim.SetBool("is_BowAttack2", false);

            //animation queueing
            //if we enter this state with a queued attack this means that after we reset all animations here we have to play the proper queue attack
            if(queuedAttackNum != 0)
            {
                ExecuteQueuedAttack(queuedAttackNum);
            }
        }

        CheckForInteractables();
       
        if (moveAxis != Vector2.zero && movementState != MovementState.Roll && movementState != MovementState.Attack && movementState != MovementState.SwitchingEquipment)
        {
            direction = transform.forward * moveAxis.y + transform.right * moveAxis.x;
            direction.y = 0;

            if (moveAxis.x > 0)
            {
                movementState = MovementState.Right;
            }
            else if (moveAxis.x < 0)
            {
                movementState = MovementState.Left;
            }

            //these states are with priority if both forward and side movement keys are held (that's why they are after the side movement check); 
            //e.g A+W will trigger a forward state
            if (moveAxis.y > 0)
            {
                movementState = MovementState.Forward;
            }
            else if (moveAxis.y < 0)
            {
                movementState = MovementState.Backward;
            }

            if(is_LockedOrbit) //locked orbit movement
            {
                if (movementState == MovementState.Forward && groundAngle < maxAngle)
                {
                    rb.AddForce(direction * currentMoveSpeed * (groundAngle / 100) * Time.deltaTime);
                }
                else if (movementState == MovementState.Left || movementState == MovementState.Right || movementState == MovementState.Backward && groundAngle < maxAngle) //side movement always at walk speed
                {
                    rb.AddForce(direction * walkSpeed * (groundAngle / 100) * Time.deltaTime);
                }
            }
            else //free orbit movement
            {
                if (movementState == MovementState.Forward && groundAngle < maxAngle)
                {
                    rb.AddForce(direction * currentMoveSpeed * (groundAngle/100) * Time.deltaTime);

                    if(moveAxis.x < 0)
                    {
                        Vector3 velocity = new Vector3(0f, rotationSpeed, 0f);
                        Quaternion deltaRotation = Quaternion.Euler(-velocity * Time.deltaTime);
                        rb.MoveRotation(rb.rotation * deltaRotation);
                    }
                    else if(moveAxis.x > 0)
                    {
                        Vector3 velocity = new Vector3(0f, rotationSpeed, 0f);
                        Quaternion deltaRotation = Quaternion.Euler(velocity * Time.deltaTime);
                        rb.MoveRotation(rb.rotation * deltaRotation);
                    }
                }
                else if (movementState == MovementState.Backward && groundAngle < maxAngle)
                {
                    rb.AddForce(direction * walkSpeed * (groundAngle / 100) * Time.deltaTime);

                    if (moveAxis.x < 0)
                    {
                        Vector3 velocity = new Vector3(0f, rotationSpeed, 0f);
                        Quaternion deltaRotation = Quaternion.Euler(velocity * Time.deltaTime);
                        rb.MoveRotation(rb.rotation * deltaRotation);
                    }
                    else if (moveAxis.x > 0)
                    {
                        Vector3 velocity = new Vector3(0f, rotationSpeed, 0f);
                        Quaternion deltaRotation = Quaternion.Euler(-velocity * Time.deltaTime);
                        rb.MoveRotation(rb.rotation * deltaRotation);
                    }
                }
                else if(movementState == MovementState.Left && groundAngle < maxAngle)
                {
                    Vector3 velocity = new Vector3(0f, rotationSpeed, 0f);
                    Quaternion deltaRotation = Quaternion.Euler(-velocity * Time.deltaTime);
                    rb.MoveRotation(rb.rotation * deltaRotation);
                }
                else if (movementState == MovementState.Right && groundAngle < maxAngle)
                {
                    Vector3 velocity = new Vector3(0f, rotationSpeed, 0f);
                    Quaternion deltaRotation = Quaternion.Euler(velocity * Time.deltaTime);
                    rb.MoveRotation(rb.rotation * deltaRotation);
                }
            }
        }
        else
        {
            if (movementState != MovementState.Attack && movementState != MovementState.Roll && movementState != MovementState.SwitchingEquipment)
            {
                movementState = MovementState.Idle;
            }
        }
        is_Grounded = false; //by default assume that the body is not grounded; prove otherwise by checking upon collision or when jumping

        if (debugMode)
        {
            Debug.Log(movementState);
        }
    }
    #endregion

    private void FixedUpdate()
    {
        //in FixedUpdate to avoid camera jitter
        RotateHeroWithCamera();
    }

    /// Checks if the character is on the ground. Rigidbody goes to sleep after a while so this also stops working if rb is asleep. 
    /// Should forcefully CheckIfGrounded() if the rb is asleep (e.g when trying to jump after staying still for a while)
    private void OnCollisionStay()
    {
        CheckIfGrounded();
    }

    private void CheckIfGrounded()
    {
        var bounds = playerCollider.bounds;
        var extents = bounds.extents;
        var radius = extents.x - 0.01f;
        Physics.SphereCastNonAlloc(bounds.center, radius, Vector3.down, _groundCastResults, extents.y - radius * 0.5f, ~0, QueryTriggerInteraction.Ignore);
        if (!_groundCastResults.Any(hit => hit.collider != null && hit.collider != playerCollider)) { return; }

        //calculate the angle between the ground and the player; player should not be permitted to walk and jump on too steep hills
        groundAngle = Vector3.Angle(_groundCastResults[0].normal, direction);
        //Debug.Log("GROUND ANGLE: " + groundAngle);
        if (groundAngle > maxAngle)
        {
            return;
        }
        
        for (var i = 0; i < _groundCastResults.Length; i++)
        {
            _groundCastResults[i] = new RaycastHit();
        }

        is_Grounded = true;
    }

    public void UnlockBow()
    {
        bowUnlocked = true;
        alertPanel.SetActive(true);
        alertPanelText.text = "Bow unlocked!";
        StartCoroutine(DisableGameObjectAfter(alertPanel, 3f));
        Debug.Log("Reward acquired: bow");
    }

    private IEnumerator DisableGameObjectAfter(GameObject objectToDisable, float disableAfterSeconds)
    {
        yield return new WaitForSeconds(disableAfterSeconds);
        objectToDisable.SetActive(false);
    }

    private float GetAutoAttackDamage(float multiplier, Vector3 particleSpawnPos, bool spawnParticle)
    {
        if(spawnParticle)
        {
            Instantiate(hitObjectParticle, particleSpawnPos, Quaternion.identity);
            Debug.Log("SHOULD SPAWN");
        }      
        return autoAttackDamage * multiplier;
    }

    public void Attack1()
    {
        // Does the ray intersect any objects in the specified layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + new Vector3(0f, attackYoffset, 0f)), out RaycastHit hit, attackRange, enemyLayer))
        {
            if (hit.transform.CompareTag("Knockable"))
            {
                hit.transform.GetComponent<IKnockback>().SufferAttackWithKnockback(this.gameObject);
            }
            else if (hit.transform.CompareTag("AI"))
            {
                hit.transform.GetComponent<EnemyAI_Controller>().TakeDamage(GetAutoAttackDamage(0.65f, hit.transform.position + new Vector3(0f, 0.65f, 0f), true), 0, this.gameObject);
                UpdateRageAndRageBar(GetAutoAttackDamage(0.65f, Vector3.zero, false));
            }
        }
    }

    public void Attack2()
    {
        // Does the ray intersect any objects in the specified layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + new Vector3(0f, attackYoffset, 0f)), out RaycastHit hit, attackRange, enemyLayer))
        {
            if (hit.transform.CompareTag("Knockable"))
            {
                hit.transform.GetComponent<IKnockback>().SufferAttackWithKnockback(this.gameObject);
            }
            else if (hit.transform.CompareTag("AI"))
            {
                hit.transform.GetComponent<EnemyAI_Controller>().TakeDamage(GetAutoAttackDamage(1.1f, hit.transform.position + new Vector3(0f, 0.65f, 0f), true), autoKnockbackPower, this.gameObject);
                UpdateRageAndRageBar(GetAutoAttackDamage(1.1f, Vector3.zero, false));
            }
        }
    }

    public void Attack3()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + new Vector3(0f, attackYoffset, 0f), attack3Radius, enemyLayer);
        UpdateRageAndRageBar(-currentRage * 0.5f);
        foreach (Collider hit in hitColliders)
        {
            if(hit.CompareTag("AI"))
            {
                hit.GetComponent<EnemyAI_Controller>().TakeDamage(GetAutoAttackDamage(0.5f, hit.transform.position + new Vector3(0f, 0.65f, 0f), true), 0f, this.gameObject);             
            }
        }
    }

    //called at the end of the .None state IF the queueAttackNum is different than 0;
    //will force the .None state to transition directly again into the .Attack state with the proper attack based on the queuedAttackNum
    private void ExecuteQueuedAttack(int queuedNum)
    {
       // Debug.Log("QUEUE ATTACK");
        switch (queuedNum)
        {
            case 1:
                HandleAttack1(fakeContext);
                queuedAttackNum = 0;
                break;
            case 2:
                HandleAttack2(fakeContext);
                queuedAttackNum = 0;
                break;
            case 3:
                HandleAttack3(fakeContext);
                queuedAttackNum = 0;
                break;
            default:
                queuedAttackNum = 0;
                break;
        }         
    }

    public void ShootArrow1()
    {
        GameObject arrow = Instantiate(arrowToShootPrefab, arrowSpawnPoint.transform);
    }

    public void ShootArrow2()
    {
        GameObject arrow = Instantiate(arrowFirePrefab, arrowSpawnPoint.transform);

        //spawn arrow just a bit in front of the hero to avoid self-collision
        arrow.transform.position = arrowSpawnPoint.transform.position + arrowSpawnPoint.transform.forward;
    }

    public void TakeDamage(float damage, GameObject attacker)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            Debug.Log("got ya");
        }
    }

    //called during the animation
    public void SwitchEquipment()
    {
        is_SwitchedToBow = !is_SwitchedToBow;
        weapon1visual.SetActive(!is_SwitchedToBow);
        weapon2visual.SetActive(!is_SwitchedToBow);
        bowVisual.SetActive(is_SwitchedToBow);
    }

    //called on roll animation event
    public void RollForward()
    {
        rb.AddForce(transform.forward * rollForwardForce, ForceMode.Impulse);
    }

    //called on roll animation event
    public void RollBackward()
    {
        rb.AddForce(-transform.forward * rollForwardForce, ForceMode.Impulse);
    }

    //called at the end of certain animations /Roll, Attack, etc./
    public void GoToNoneState()
    {
        movementState = MovementState.None;   
    }

    public void DisableArrowVisual()
    {
        arrowVisual.SetActive(false); //disable the arrow if this state is called in case that it is active
    }

    //maybe a bit overdone name but this means I can put this useless comment here because it needs no explanation :)
    private bool HasAnimatorFinishedPlayingCurrentStateAnimation()
    {
        return anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0);
    }

    private void RotateHeroWithCamera()
    {
        //rotate hero to face the general camera direction
        if (is_LockedOrbit)
        {
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, cameraObject.transform.eulerAngles.y, ref turnSmoothVelocity, turnSmoothTime * Time.deltaTime);
            transform.rotation = Quaternion.Euler(transform.rotation.x, angle, transform.rotation.z);
        }
    }

    //enable rotation of the camera only while the proper buttons are currently held
    private void ToggleFreeLook()
    {
        if(!is_LockedOrbit && !is_FreeOrbit) //disable camera rotation
        {
            cineFreeLookVcam.m_XAxis.m_InputAxisName = "";
            cineFreeLookVcam.m_YAxis.m_InputAxisName = "";
            
            //reset input as well so that the camera does not keep rotating by itself
            cineFreeLookVcam.m_XAxis.m_InputAxisValue = 0;
            cineFreeLookVcam.m_YAxis.m_InputAxisValue = 0;
            
        }
        else //enable camera rotation
        {
            cineFreeLookVcam.m_XAxis.m_InputAxisName = "Mouse X";
            cineFreeLookVcam.m_YAxis.m_InputAxisName = "Mouse Y";
        }
    }

    public void SetQuestPanelState(bool stateValue)
    {
        questPanel.SetActive(stateValue);
    }

    private void CheckForInteractables()
    {
        if(interactableCheckedTimestamp <= Time.time)
        {
            interactableCheckedTimestamp = Time.time + checkForInteractableAlertEveryXSeconds;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange, interactionLayer);
            if(hitColliders.Length > 0)
            {
                interactableAlertText.SetActive(true);
            }
            else
            {
                interactableAlertText.SetActive(false);
            }                 
        }       
    }

    private void ResetAllElapsedAttackDurations()
    {
      //  Debug.Log("A1: " + elapsedAttackDuration);
        elapsedAttackDuration = 0f;
    }

    private void SetAttackCooldowns()
    {
        internalAttack1Cooldown = attack1MaxDuration + 0.1f;
        internalAttack2Cooldown = attack2MaxDuration + 0.1f;
    }

    public void UpdateRageAndRageBar(float valueToAdd)
    {
        currentRage = currentRage + valueToAdd > maxRage ? maxRage : currentRage + valueToAdd;
        rageBar.value = currentRage;
    }

    private bool CheckIfEnoughRageForSpell(float rageRequirement)
    {
        if(currentRage >= rageRequirement)
        {
            return true;
        }
        rageBarAnim.SetBool("notEnoughRage", true);
        return false;
    }
   

    #region HandleEvents
    private void HandleMove(InputAction.CallbackContext context)
    {
        moveAxis = context.ReadValue<Vector2>();
    }

    private void HandleSprint(InputAction.CallbackContext context)
    {
        is_Sprinting = !is_Sprinting;

        currentMoveSpeed = is_Sprinting ? sprintSpeed : walkSpeed;
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange, interactionLayer);
        foreach(Collider hit in hitColliders)
        {
            if(hit.gameObject.CompareTag("Interactable"))
            {
                hit.gameObject.GetComponent<IInteractable>().Interact();
                if(hit.gameObject.GetComponent<NPC_Controller>() != null)
                {
                    NPC_Controller npcContr = hit.gameObject.GetComponent<NPC_Controller>();
                    npcContr.StopCurrentWanderPointAndRotateTowardsTarget(gameObject);
                }
                break; //stop at the first interaction; prevent unexpected behaviour if multiple interactables are in one place
            }
        }
    }

    private void HandleToggleQuestPanel(InputAction.CallbackContext context)
    {
        questPanel.SetActive(!questPanel.activeSelf);
    }

    private void HandleLockedOrbit(InputAction.CallbackContext context)
    {
        is_LockedOrbit = !is_LockedOrbit;
        ToggleFreeLook();
    }

    private void HandleFreeOrbit(InputAction.CallbackContext context)
    {
        is_FreeOrbit = !is_FreeOrbit;
        ToggleFreeLook();
    }

    private void HandleJump(InputAction.CallbackContext context)
    {
        CheckIfGrounded();
        if (is_Grounded && movementState != MovementState.SwitchingEquipment && movementState != MovementState.Roll)
        {
            GoToNoneState(); //reset all when jumping
            anim.SetBool("is_Jump", true);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleAttack1(InputAction.CallbackContext context)
    {      
        if (movementState != MovementState.Attack && movementState != MovementState.SwitchingEquipment && internalAttack1Timestamp <= Time.time && !is_SwitchedToBow)
        {
            rb.AddForce(transform.forward * attack1ForwardLungeForce, ForceMode.Impulse);
            ResetAllElapsedAttackDurations();
            internalAttack1Timestamp = internalAttack1Cooldown + Time.time;
            
            movementState = MovementState.Attack;
            anim.SetBool("is_Attack1", true);
        }   
        else if(movementState != MovementState.Attack && movementState != MovementState.SwitchingEquipment && internalAttack1Timestamp <= Time.time && is_SwitchedToBow)
        {
            ResetAllElapsedAttackDurations();
            internalAttack1Timestamp = internalAttack1Cooldown + Time.time;

            movementState = MovementState.Attack;
            anim.SetBool("is_BowAttack1", true);
            arrowVisual.SetActive(is_SwitchedToBow);
        }
        else if (movementState == MovementState.Attack && queuedAttackNum != 1 /*&& elapsedAttackDuration >= attack2MaxDuration*/) //queue the attack if already attacking
        {
            queuedAttackNum = 1;
        }
    }

    private void HandleAttack2(InputAction.CallbackContext context)
    {     
        if (movementState != MovementState.Attack && movementState != MovementState.SwitchingEquipment && internalAttack2Timestamp <= Time.time && !is_SwitchedToBow && CheckIfEnoughRageForSpell(attack2RageRequirement))
        {
            ResetAllElapsedAttackDurations();
            internalAttack2Timestamp = internalAttack2Cooldown + Time.time;
            
            movementState = MovementState.Attack;
            anim.SetBool("is_Attack2", true);
        }    
        else if(movementState != MovementState.Attack && movementState != MovementState.SwitchingEquipment && internalAttack2Timestamp <= Time.time && is_SwitchedToBow)
        {
            ResetAllElapsedAttackDurations();
            internalAttack2Timestamp = internalAttack2Cooldown + Time.time;

            movementState = MovementState.Attack;
            anim.SetBool("is_BowAttack2", true);
            arrowVisual.SetActive(is_SwitchedToBow);
        }
        else if (movementState == MovementState.Attack && queuedAttackNum != 2 /*&& elapsedAttackDuration >= attack2MaxDuration*/) //queue the attack if already attacking
        {
            queuedAttackNum = 2;
        }
    }

    private void HandleAttack3(InputAction.CallbackContext context)
    {
        if (movementState != MovementState.Attack && movementState != MovementState.SwitchingEquipment && internalAttack3Timestamp <= Time.time && !is_SwitchedToBow && CheckIfEnoughRageForSpell(attack3RageRequirement))
        {
            ResetAllElapsedAttackDurations();
            internalAttack3Timestamp = internalAttack3Cooldown + Time.time;

            movementState = MovementState.Attack;
            anim.SetBool("is_Attack3", true);
        }
        else if (movementState == MovementState.Attack && queuedAttackNum != 3 /*&& elapsedAttackDuration >= attack3MaxDuration*/) //queue the attack if already attacking
        {
            queuedAttackNum = 3;
        }
    }


    private void HandleRollForward(InputAction.CallbackContext context)
    {
        if (movementState != MovementState.Attack && movementState != MovementState.Roll && movementState != MovementState.SwitchingEquipment && rollCooldownTimestamp <= Time.time)
        {
            rollCooldownTimestamp = Time.time + rollCooldown;
            elapsedRollDuration = 0f;
            movementState = MovementState.Roll;
            anim.SetBool("is_RollForward", true);
        }
    }

    private void HandleRollBackward(InputAction.CallbackContext context)
    {
        if (movementState != MovementState.Attack && movementState != MovementState.Roll && movementState != MovementState.SwitchingEquipment && rollCooldownTimestamp <= Time.time)
        {
            rollCooldownTimestamp = Time.time + rollCooldown;
            elapsedRollDuration = 0f;
            movementState = MovementState.Roll;
            anim.SetBool("is_RollBackward", true);
        }
    }

    private void HandleMouseDelta(InputAction.CallbackContext context)
    {
        mouseMoveAxis = context.ReadValue<Vector2>();
    }

    private void HandleEquipmentSwitch(InputAction.CallbackContext context)
    {
        if(bowUnlocked)
        {
            elapsedSwitchedEquipmentDuration = 0f;
            movementState = MovementState.SwitchingEquipment;
            anim.SetBool("is_EquipmentSwitch", true);
        }    
    }
    #endregion
}
