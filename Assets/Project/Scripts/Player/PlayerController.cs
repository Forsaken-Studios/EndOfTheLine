using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.CustomLogs;

namespace Player
{

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed; 
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
        
        [Header("Inputs")] 
        private KeyCode dashInput = KeyCode.LeftControl;
         
        // Start is called before the first frame update
        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            
            //LogManager.Log("GAME STATUS: " + GameManager.Instance.GameState.ToString(), FeatureType.General);
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
                HandleDashInputs();
                _animator.SetBool("running", true);
            }
            else
                _animator.SetBool("running", false);
            
           // _animator.SetInteger("SpeedInt", (int)speedX);
        }

        private void HandleDashInputs()
        {
            if (Input.GetKeyDown(dashInput) && canDash && PlayerOverheating.Instance.GetStamina() >= DASH_STAMINA_COST)
            {
                StartCoroutine(Dash());
                SoundManager.Instance.ActivateSoundByName(SoundAction.Movement_Dash);
            }
        }

        private IEnumerator Dash()
        {
            PlayerOverheating.Instance.DecreaseStamina(DASH_STAMINA_COST);
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


        public void SetIfPlayerCanMove(bool aux)
        {
            this.playerCanMove = aux;
        }
    }
}




