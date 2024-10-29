using System.Collections;
using UnityEngine;
namespace Player
{

    public class PlayerController : MonoBehaviour
    {

        public static PlayerController Instance;
 
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 10f;
        [SerializeField] private float speedModifier = 0.5f;
        
        [SerializeField] private float moveSpeed;
        private float currentSpeedValue;
        private float speedX, speedY;

        [Header("Weight Properties")]
        [SerializeField] private int maxLimitWeight = 35;
        [SerializeField] private int overWeight = 25;
        [Range(0,1)]
        [Tooltip("A mas cercano a 0, mayor reducción")]
        [SerializeField] private float moveReducerByWeight = 0.6f;
        public bool isOverweight = false;
        //ID => 0 -> Desactivate | 1 -> Overweight Icon | 2 -> weight Icon | 
        public delegate void OnWeightChangedDelegate(int id);
        public event OnWeightChangedDelegate OnWeightChange;
        private float _currentWeight = 0;
        public float CurrentWeight
        {
            get
            {
                return _currentWeight;
            }
            set
            {
                _currentWeight = value;
                if (_currentWeight >= overWeight && _currentWeight < maxLimitWeight)
                {
                    playerCanMove = true;
                    isOverweight = true;
                    if (OnWeightChange != null)
                        OnWeightChange(1);
                }else if (_currentWeight >= maxLimitWeight)
                {
                    playerCanMove = false;
                    if (OnWeightChange != null)
                        OnWeightChange(2);
                }
                else
                {
                    playerCanMove = true;
                    isOverweight = false;
                    if (OnWeightChange != null)
                        OnWeightChange(0);
                }
                
            }
        }
        
        [Header("Properties")]
        private Rigidbody2D _rb;
        private Animator _animator;

        [Header("Dash Settings")] 
        [SerializeField] private float dashSpeed = 10f;
        [SerializeField] private float dashDuration = 1f;
        [SerializeField] private float dashCooldown;
        [SerializeField] private float dashStaminaCost = 40;
        private bool isDashing = false;
        private bool playerCanMove = true;
        private bool canDash = true;
        

        [Header("Noise Radius Properties")] 
        [SerializeField] private float walkRadius = 1f;
        [SerializeField] private float runRadius = 2f;
        private float currentRadius;
        [Range(1f, 25f)]
        [SerializeField] private float radiusDecay = 16f;
        
        [Header("Inputs")] 
        private KeyCode dashInput = KeyCode.Space;
        
        [Header("Noise Circle")] 
        [SerializeField] private NoiseCircle noise;

        [Header("Player Speed Bar")] 
        [SerializeField] private PlayerSpeedBarUI playerSpeedBar;
        
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
            currentSpeedValue = walkSpeed;
            playerSpeedBar.UpdateImage((currentSpeedValue / walkSpeed) - 1);

            SetWeightProperties();
        }

        private void SetWeightProperties()
        {
            if (PlayerPrefs.GetInt("UpgradeUnlocked_5") == 1)
            {
                maxLimitWeight += 7;
                overWeight += 7;
            }
            if (PlayerPrefs.GetInt("UpgradeUnlocked_6") == 1)
            {
                maxLimitWeight += 7;
                overWeight += 7;
            }
            if (PlayerPrefs.GetInt("UpgradeUnlocked_7") == 1)
            {
                maxLimitWeight += 7;
                overWeight += 7;
            }
        }

        private void Update()
        {
            if (!playerCanMove)
            { 
                return;
            }

            if (GameManager.Instance.GameState == GameState.OnGame)
            {
                CalculateNoiseRadius();
                playerSpeedBar.UpdateImage((currentSpeedValue / walkSpeed) - 1);
                HandleMouseWheelSpeed();
                if (GameManager.Instance.GameState == GameState.OnGame && playerCanMove)
                {
                    if (isDashing)
                    {
                        PlayerRadiation.Instance.SetCanRecoveryEnergy(false);
                        return;
                    }
                    HandleMovementInputs();
                }
                else
                {
                    if (GameManager.Instance.GameState == GameState.OnInventory ||
                        GameManager.Instance.GameState == GameState.onLoreView)
                    {
                        _animator.SetBool("isWalking", false);
                    }
                    speedX = 0;
                    speedY = 0;
                    moveSpeed = 0;
                }
            }
            else
            {
                speedX = 0;
                speedY = 0;
                moveSpeed = 0;
            }
        }

        void FixedUpdate()
        {
            if (isDashing || !playerCanMove)
            {
                return;
            }
            _rb.MovePosition(_rb.position + new Vector2(speedX, speedY) * (moveSpeed * Time.fixedDeltaTime));
        }

        private void HandleMouseWheelSpeed()
        {
            if (Input.mouseScrollDelta.y > 0f)
            {
                if (currentSpeedValue < runSpeed)
                {
                    currentSpeedValue += speedModifier;
                    moveSpeed += speedModifier;
                }
            }else if (Input.mouseScrollDelta.y < 0f)
            {
                if (currentSpeedValue > walkSpeed)
                {
                    currentSpeedValue -= speedModifier;
                    moveSpeed -= speedModifier;
                }
            }
        }
        
        private void HandleMovementInputs()
        {
            speedX = Input.GetAxisRaw("Horizontal");
            speedY = Input.GetAxisRaw("Vertical");
            if (isOverweight)
            {
                speedX *= moveReducerByWeight;
                speedY *= moveReducerByWeight;
            }
            

            if (speedX != 0 || speedY != 0)
            {
                HandleSprintInput();
                HandleDashInputs();
                // TODO: Change method to add variation between walking and running depending on player current speed
                if(moveSpeed >= runSpeed)
                {
                    _animator.SetBool("isRunning", true);
                    _animator.SetBool("isWalking", true); // false);
                } else
                {
                    _animator.SetBool("isRunning", false);
                    _animator.SetBool("isWalking", true);
                }
            }
            else
            {
                moveSpeed = 0;
                _animator.SetBool("isWalking", false);
                _animator.SetBool("isRunning", false);
            }
           // _animator.SetInteger("SpeedInt", (int)speedX);
        }

        private void HandleSprintInput()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveSpeed = runSpeed;
                playerSpeedBar.UpdateImage((moveSpeed / walkSpeed) - 1);
            }
            else
            {
                moveSpeed = currentSpeedValue;
                playerSpeedBar.UpdateImage((currentSpeedValue / walkSpeed) - 1);
            }
        }
        
        private void HandleDashInputs()
        {
            if (Input.GetKeyDown(dashInput) && canDash && PlayerOverheating.Instance.GetEnergy() >= dashStaminaCost)
            {
                StartCoroutine(Dash());
                _animator.SetBool("isDashPressed", true);
                SoundManager.Instance.ActivateSoundByName(SoundAction.Movement_Dash, null, true);
            }
        }

        private IEnumerator Dash()
        {
            PlayerOverheating.Instance.DecreaseEnergy(dashStaminaCost);
            PlayerOverheating.Instance.SetCanRecoveryEnergy(false);
            canDash = false;
            isDashing = true;

            if (speedX == 0)
            {
                _rb.velocity = new Vector2(speedX * dashSpeed, speedY * dashSpeed);
            }
            else
            {
                _rb.velocity = new Vector2(speedX * dashSpeed, speedY * dashSpeed / 3);
            }

            yield return new WaitForSeconds(dashDuration);
            isDashing = false;
            _animator.SetBool("isDashPressed", false);
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;

            yield return new WaitForSeconds(2f);
            PlayerOverheating.Instance.SetCanRecoveryEnergy(true);
        }
        
        private float CalculateNoiseRadius()
        {
            if (moveSpeed >= runSpeed)
            {
                currentRadius = ExpDecay(currentRadius, runRadius, radiusDecay, Time.deltaTime);
            }else if (moveSpeed < walkSpeed)
            {
                currentRadius = ExpDecay(currentRadius, 0, radiusDecay, Time.deltaTime);
            }
            else
            {
                float speedParam = (moveSpeed - walkSpeed) / (runSpeed - walkSpeed);

                float goalRadius = walkRadius + speedParam * (runRadius - walkRadius);
                
                currentRadius = ExpDecay(currentRadius, goalRadius, radiusDecay, Time.deltaTime);
            }
            
            return currentRadius;
        }

        public void PlayDeathAnimation()
        {
            if(_animator != null)
                _animator.SetBool("isDead", true);
        }

        public void PlayOpenInventoryAnimation()
        {
            if (_animator != null)
            {              
                _animator.SetBool("isOnInventory", true);
                _animator.SetTrigger("isOnInventoryTrigger");
            }

        }
        
        public void PlayCloseInventoryAnimation()
        {
            if(_animator != null)
                _animator.SetBool("isOnInventory", false);
        }
        //Advanced LERP function
        private float ExpDecay(float current, float goal, float decay, float dT)
        {
            return goal + (current - goal)* Mathf.Exp(-decay * dT);
        }
        
        public void SetIfPlayerCanMove(bool aux) => this.playerCanMove = aux;

        public float GetMoveSpeed() => moveSpeed;
        
        public float GetCurrentRadius() => currentRadius;

        public NoiseCircle GetNoiseScript() => noise;

        public static PlayerController PlayerControllerInstance => Instance;

        public int GetMaxWeight() => maxLimitWeight;

        public int GetOverweight() => overWeight;
        

    }
}




