using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aura.Platformer2D.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CollisionChecker))]
    public class AnimationController : MonoBehaviour
	{
		//references
		private Animator playerAnimator;
        private CollisionChecker collisionChecker;
        private Rigidbody2D playerRb;

        //Animator hashes
        private int speedHash = Animator.StringToHash("speed");
        private int yVelocity = Animator.StringToHash("yVelocity");
        private int isGrounded = Animator.StringToHash("IsGrounded");
        private int isWallDetected = Animator.StringToHash("isWallDetected");
        private int knockBackTrigger = Animator.StringToHash("knockBack");
        internal void PlayKnockBackAnimation()
        {
           playerAnimator.SetTrigger(knockBackTrigger);
        }

        private void Awake()
        {
            playerAnimator = GetComponentInChildren<Animator>();
            collisionChecker = GetComponent<CollisionChecker>();
            playerRb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            playerAnimator.SetFloat(speedHash,playerRb.velocity.x);
            playerAnimator.SetBool(isGrounded,collisionChecker.IsGrounded);
            playerAnimator.SetFloat(yVelocity,playerRb.velocity.y);
            playerAnimator.SetBool(isWallDetected,collisionChecker.IsWallDetected);
        }
    } 
}
