using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.CustomLogs;

namespace Player
{

    public class PlayerController : MonoBehaviour
    {

        public static PlayerController Instance;
        
        [SerializeField] private float walkSpeed = 18f;
        [SerializeField] private float runSpeed = 26f;
        
        private float moveSpeed; 
        private float speedX, speedY; 
        [Header("Properties")]
        private Rigidbody2D _rb;
        private Animator _animator;

        [Header("Dash Settings")] 
        [SerializeField] private float dashSpeed = 10f;
        [SerializeField] private float dashDuration = 1f;
        [SerializeField] private float dashCooldown;
        private bool isDashing = false;
        private bool playerCanMove = true;
        private bool canDash = true;
        private float DASH_STAMINA_COST = 40;

        [Header("Noise Radius Properties")] 
        [SerializeField] private float walkRadius = 1f;
        [SerializeField] private float runRadius = 2f;
        private float currentRadius;
        [Range(1f, 25f)]
        [SerializeField] private float radiusDecay = 16f;
        
        [Header("Inputs")] 
        private KeyCode dashInput = KeyCode.LeftControl;


        [Header("Noise Circle")] 
        [SerializeField] private NoiseCircle noise;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("[PlayerController.cs] : There is already a PlayerController Instance");
                Destroy(this);
            }
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            CalculateNoiseRadius();
           
            if (GameManager.Instance.GameState == GameState.OnGame && playerCanMove)
            {
                if (isDashing)
                {
                    PlayerStamina.Instance.SetCanRecoveryEnergy(false);
                    return;
                }
                HandleMovementInputs();
            }
            else
            {
                if (GameManager.Instance.GameState == GameState.OnInventory || 
                    GameManager.Instance.GameState == GameState.onLoreView)
                {
                    _animator.SetBool("running", false);
                }
                speedX = 0;
                speedY = 0; 
            }
        }

        void FixedUpdate()
        {
            if (isDashing)
            {
                return;
            }
            _rb.MovePosition(_rb.position + new Vector2(speedX, speedY) * moveSpeed * Time.fixedDeltaTime);
        }

        private void HandleMovementInputs()
        {
            speedX = Input.GetAxisRaw("Horizontal");
            speedY = Input.GetAxisRaw("Vertical");

            if (speedX != 0 || speedY != 0)
            {
                HandleSprintInput();
                HandleDashInputs();
                _animator.SetBool("running", true);
            }
            else
            {
                moveSpeed = 0;
                _animator.SetBool("running", false);
            }
                
            
           // _animator.SetInteger("SpeedInt", (int)speedX);
        }

        private void HandleSprintInput()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveSpeed = runSpeed;
            }
            else
            {
                moveSpeed = walkSpeed;
            }
        }
        
        private void HandleDashInputs()
        {
            if (Input.GetKeyDown(dashInput) && canDash && PlayerOverheating.Instance.GetEnergy() >= DASH_STAMINA_COST)
            {
                StartCoroutine(Dash());
                SoundManager.Instance.ActivateSoundByName(SoundAction.Movement_Dash);
            }
        }

        private IEnumerator Dash()
        {
            PlayerOverheating.Instance.DecreaseEnergy(DASH_STAMINA_COST);
            PlayerOverheating.Instance.SetCanRecoveryEnergy(false);
            canDash = false;
            isDashing = true;
            _rb.velocity = new Vector2(speedX * dashSpeed, speedY * dashSpeed / 2);
            yield return new WaitForSeconds(dashDuration);
            isDashing = false;
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;

            yield return new WaitForSeconds(2f);
            PlayerOverheating.Instance.SetCanRecoveryEnergy(true);
        }
        private float CalculateNoiseRadius()
        {
            if (moveSpeed >= runSpeed)
            {
                currentRadius = expDecay(currentRadius, runRadius, radiusDecay, Time.deltaTime);
            }else if (moveSpeed < walkSpeed)
            {
                currentRadius = expDecay(currentRadius, 0, radiusDecay, Time.deltaTime);
            }
            else
            {
                float speedParam = (moveSpeed - walkSpeed) / (runSpeed - walkSpeed);

                float goalRadius = walkRadius + speedParam * (runRadius - walkRadius);
                
                currentRadius = expDecay(currentRadius, goalRadius, radiusDecay, Time.deltaTime);
            }
            
            return currentRadius;
        }

        float expDecay(float current, float goal, float decay, float dT)
        {
            //Advanced LERP function
            return goal + (current - goal)* Mathf.Exp(-decay * dT);
        }
        
        public void SetIfPlayerCanMove(bool aux)
        {
            this.playerCanMove = aux;
        }

        public float GetMoveSpeed()
        {
            return moveSpeed;
        }
        
        public float GetWalkSpeed()
        {
            return walkSpeed;
        }
        
        public float GetRunSpeed()
        {
            return runSpeed;
        }

        public NoiseCircle GetNoiseScript()
        {
            return noise;
        }

        public float GetCurrentRadius()
        {
            return currentRadius;
        }
        
    }
}




