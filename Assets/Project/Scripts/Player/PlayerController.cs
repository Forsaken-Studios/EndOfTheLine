using System.Collections;
using UnityEngine;
namespace Player
{

    public class PlayerController : MonoBehaviour
    {

        public static PlayerController Instance;
 
        [SerializeField] private float walkSpeed = 5f;
        public float GetMinSpeed => walkSpeed;
        [SerializeField] private float runSpeed = 10f;
        public float GetMaxSpeed => runSpeed;
        [SerializeField] private float speedModifier = 0.5f;
        
        [SerializeField] private float moveSpeed;
        [SerializeField, Range(0.8f, 1.25f)] private float animSpeedWalk = 0.8f;
        [SerializeField, Range(1f, 1.3f)] private float animSpeedRun = 1f;
        private float currentSpeedValue;
        private float speedX, speedY;
        private bool isSprinting;
        public bool IsSprinting => isSprinting;

        [Header("Animation Properties")]
        [SerializeField] private GameObject torsoModel;
        [SerializeField] private GameObject legsModel;
        [SerializeField] private float rotSpeed;
        [SerializeField] private float minAnimSpeedWalk;
        [SerializeField] private float maxAnimSpeedWalk;
        public bool isRunning = false;
        //[SerializeField] private float minAnimSpeedRun;
        //[SerializeField] private float maxAnimSpeedRun;

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
                    isOverweight = true;
                    _animTorso.SetBool("isWalking", false);
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
        [SerializeField] private Animator _animTorso;
        [SerializeField] private Animator _animLegs;
        private Vector2 moveVector;
        //private Quaternion walkDirection;
        //private Animator _animator;
        //[SerializeField] private Animator _animator;

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
            _animTorso = torsoModel.GetComponent<Animator>();
            _animLegs = legsModel.GetComponent<Animator>();
        }

        // Start is called before the first frame update
        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animTorso = GetComponentInChildren<Animator>();
            currentSpeedValue = walkSpeed;
            playerSpeedBar.UpdateImage(walkSpeed);

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
                playerSpeedBar.UpdateImage(currentSpeedValue);
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
                        _animTorso.SetBool("isWalking", false);
                        _animTorso.SetBool("isRunning", false);
                        _animLegs.SetBool("isWalking", false);
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
            moveVector = new Vector2(speedX, speedY);
            _rb.MovePosition(_rb.position + moveVector * (moveSpeed * Time.fixedDeltaTime));
            Rotate();
        }

        void Rotate() {

            //Vector3 filteredTransform = new Vector3(legsModel.transform.forward.x, legsModel.transform.forward.y, 0f);
            //Vector3 moveVector3D = new Vector3(moveVector.x, moveVector.y, 0f);

            //Vector3 desiredForward = Vector3.RotateTowards(filteredTransform, moveVector, rotSpeed * Time.deltaTime, 0f);
            //walkDirection = Quaternion.LookRotation(desiredForward);

            float vectorOrientation = Mathf.Atan2(-moveVector.x, moveVector.y) * Mathf.Rad2Deg;
            if (isRunning)
            {
                float currentOrientation = torsoModel.transform.eulerAngles.z;
                float orientation = Mathf.LerpAngle(currentOrientation, vectorOrientation, rotSpeed * Time.fixedDeltaTime);

                torsoModel.transform.rotation = Quaternion.Euler(0f, 0f, orientation);
            }
            else
            {
                float currentOrientation = legsModel.transform.eulerAngles.z;
                float orientation = Mathf.LerpAngle(currentOrientation, vectorOrientation, rotSpeed * Time.fixedDeltaTime);

                legsModel.transform.rotation = Quaternion.Euler(0f, 0f, orientation);
            }
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
            isRunning = false;

            if (isOverweight)
            {
                speedX *= moveReducerByWeight;
                speedY *= moveReducerByWeight;
            }

            if (speedX != 0 || speedY != 0)
            {
                HandleSprintInput();
                HandleDashInputs();

                if(moveSpeed >= runSpeed)
                {
                    isSprinting = true;
                    isRunning = true;
                    _animTorso.SetBool("isRunning", isRunning);
                    _animTorso.SetBool("isWalking", !isRunning);
                    _animTorso.SetFloat("animWalkSpeed", minAnimSpeedWalk);

                    _animLegs.SetBool("isWalking", !isRunning);
                    _animLegs.SetFloat("animWalkSpeed", minAnimSpeedWalk);
                }
                else
                {
                    float animSpeed = GetAnimationSpeed(moveSpeed, walkSpeed, runSpeed);
                    _animTorso.SetBool("isRunning", isRunning);
                    _animTorso.SetBool("isWalking", !isRunning);
                    _animTorso.SetFloat("animWalkSpeed", animSpeed);

                    _animLegs.SetBool("isWalking", !isRunning);
                    _animLegs.SetFloat("animWalkSpeed", animSpeed);
                }
            }
            else
            {
                isSprinting = false;
                moveSpeed = 0;
                _animTorso.SetBool("isRunning", false);
                _animTorso.SetBool("isWalking", false);
                _animTorso.SetFloat("animWalkSpeed", 0);

                _animLegs.SetBool("isWalking", false);
                _animLegs.SetFloat("animWalkSpeed", 0);
            }
        }

        private float GetAnimationSpeed(float currentSpeed, float minSpeed, float maxSpeed)
        {
            // Pillamos en que porcentaje esta la vel actual entre el min y max
            float percentage = Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed);
            float animSpeed = Mathf.Lerp(minAnimSpeedWalk, maxAnimSpeedWalk, percentage);
            return animSpeed;
        }

        private void HandleSprintInput()
        {
            if (Input.GetKey(KeyCode.LeftShift) && PlayerSprintStamina.Instance.PlayerCanSprint())
            {
                isSprinting = true;
                moveSpeed = runSpeed;
                playerSpeedBar.UpdateImage(moveSpeed);
            }
            else
            {
                isSprinting = false;
                moveSpeed = currentSpeedValue;
                playerSpeedBar.UpdateImage(moveSpeed);
            }
        }
        
        private void HandleDashInputs()
        {
            if (Input.GetKeyDown(dashInput) && canDash && PlayerOverheating.Instance.GetEnergy() >= dashStaminaCost)
            {
                StartCoroutine(Dash());
                _animTorso.SetBool("isDashPressed", true);
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
            _animTorso.SetBool("isDashPressed", false);
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
            if(_animTorso != null)
                _animTorso.SetBool("isDead", true);
        }

        public void PlayOpenInventoryAnimation()
        {
            if (_animTorso != null)
            {              
                _animTorso.SetBool("isOnInventory", true);
                _animTorso.SetTrigger("isOnInventoryTrigger");
            }

        }
        
        public void PlayCloseInventoryAnimation()
        {
            if(_animTorso != null)
                _animTorso.SetBool("isOnInventory", false);
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




