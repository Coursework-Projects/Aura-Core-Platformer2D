using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aura.Platformer2D.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CollisionChecker))]
    public class Movement : MonoBehaviour
    {

        [Header("Settings")]
        [SerializeField] private float runSpeed;
        [SerializeField] private float jumpHeight;
        [SerializeField] private float doubleJumpHeight;
        [SerializeField] private float wallSlideSpeedModifier;
        [SerializeField] private float wallJumpDuration;
        [SerializeField] private float knockBackDuration;
        [SerializeField] private Vector2 wallJumpDirection;
        [SerializeField] private Vector2 knockBackDirection;

        //References
        private CollisionChecker collisionChecker;
        private Rigidbody2D playerRb;
        private AnimationController playerAnimator;

        //Cache of Inputs
        float xInput;
        float yInput;

        //public property accessible to other components but set here
        [field: SerializeField]
        public int FacingDirection { get; private set; } = 1;

        //flags 
        //ToDo:Serialized for testing
        [SerializeField] private bool isFacingRight = true;
        [SerializeField] bool isAirborne = false;
        [SerializeField] bool canDoubleJump = false;
        [SerializeField] bool isWallJumping = false;
        [SerializeField] bool isKnockedBack = false;
        [SerializeField] bool canBeKnockedBack = true;

        #region Monobehaviour Callbacks
        private void Awake()
        {
            playerAnimator = GetComponentInChildren<AnimationController>();
            collisionChecker = GetComponent<CollisionChecker>();
            playerRb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            CheckAirborneStatus();

            CheckWallDetectionStatus();

            if (isKnockedBack)
                return;

            HandleInput();


            //re-enable double jump on successful wall landing

            HandleWallSlide();
        }

        private void CheckWallDetectionStatus()
        {
            if (collisionChecker.IsWallDetected)
            {
                canDoubleJump = true;
            }
        }

        private void FixedUpdate()
        {

            HandleMovement();
        }
        #endregion

        #region Public API
        public void KnockBack()
        {
            Debug.Log("Inside KnockBack()");
            if (canBeKnockedBack == false)
                return;

            playerAnimator.PlayKnockBackAnimation();
            playerRb.velocity = new Vector2(knockBackDirection.x * -FacingDirection, knockBackDirection.y);

            StartCoroutine(KnockBackRoutine());

        }
        #endregion

        #region Private Utility Methods

        private void HandleWallSlide()
        {
            if (collisionChecker.IsWallDetected == false)
                return;

            wallSlideSpeedModifier = yInput < 0 ? 1f : 0.5f;

            if (playerRb.velocity.y < 0)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, playerRb.velocity.y * wallSlideSpeedModifier);
            }
        }
        private void HandleInput()
        {
            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(KeyCode.Space))
            {
                HandleJumpInput();
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                KnockBack();
            }
        }
        private void HandleMovement()
        {
            if (isWallJumping)
                return;

            if (isKnockedBack)
                return;

            playerRb.velocity = new Vector2(xInput * runSpeed, playerRb.velocity.y);
            HandleFlip();
        }

        private void HandleFlip()
        {
            if (xInput < 0 && isFacingRight || xInput > 0 && !isFacingRight)
            {
                Flip();
            }
        }

        private void HandleJumpInput()
        {

            if (!isAirborne)
            {
                HandleGroundedJumpLogic();
            }
            else if (isAirborne)
            {
                HandleAirborneJumpLogic();
            }
        }

        private void HandleGroundedJumpLogic()
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, jumpHeight);
        }

        private void HandleAirborneJumpLogic()
        {
            if (collisionChecker.IsWallDetected)
            {
                playerRb.velocity = new Vector2(wallJumpDirection.x * -FacingDirection, wallJumpDirection.y);
                Flip();

                StopAllCoroutines();
                StartCoroutine(WallJumpRoutine());

            }
            if (canDoubleJump)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, doubleJumpHeight);
                canDoubleJump = false;

            }
        }


        private void CheckAirborneStatus()
        {
            if (!collisionChecker.IsGrounded && isAirborne == false)
            {
                HandleBecomingAirborne();
            }
            else if (collisionChecker.IsGrounded && isAirborne == true)
            {
                HandleBecomingGrounded();
            }
        }

        private void HandleBecomingAirborne()
        {
            isAirborne = true;
        }

        private void HandleBecomingGrounded()
        {
            isAirborne = false;
            canDoubleJump = true;
        }

        private void Flip()
        {
            //turn character to face opposite direction
            transform.Rotate(0, 180, 0);

            //set flag to track if facing right or left
            isFacingRight = !isFacingRight;

            //cache facing direction to use in other calculations e.g knock back
            FacingDirection *= -1;
        }
        #endregion

        #region CoRoutines
        private IEnumerator WallJumpRoutine()
        {
            isWallJumping = true;

            yield return new WaitForSeconds(wallJumpDuration);

            isWallJumping = false;
        }
        private IEnumerator KnockBackRoutine()
        {
            Debug.Log("inside KnockBackRoutine");
            isKnockedBack = true;
            canBeKnockedBack = false;//avoid double knock back
            yield return new WaitForSeconds(knockBackDuration);
            canBeKnockedBack = true;
            isKnockedBack = false;
        }
        #endregion
    }
}
