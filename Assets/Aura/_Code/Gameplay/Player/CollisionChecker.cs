using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aura.Platformer2D.Gameplay
{
	public class CollisionChecker : MonoBehaviour
	{
        [Header("References")]
        [SerializeField]private Movement movement;

        [Header("Parameters")]
		[SerializeField] private float groundCheckDistance;
		[SerializeField] private float wallCheckDistance;
        [SerializeField] private LayerMask groundMask;

        //public property to make the results of ground check accessible to all but can only be set here.
        public bool IsGrounded {  get; private set; }

        //public property to make the results of wall check accessible to all but can only be set here.
        public bool IsWallDetected {  get; private set; }

        private void Awake()
        {
            movement = GetComponent<Movement>();
        }
        private void Update()
        {
            //Unity implicitly converts the RaycastHit2D object returned to a bool
            IsGrounded = Physics2D.Raycast(transform.position,Vector2.down,groundCheckDistance,groundMask);

            IsWallDetected = Physics2D.Raycast(transform.position, Vector2.right * movement.FacingDirection, wallCheckDistance, groundMask);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            //draw the ground check ray cast
            Gizmos.DrawLine(transform.position, new Vector2(0f,transform.position.y -groundCheckDistance));

            //draw the wall check ray cast
            Gizmos.DrawLine(transform.position, new Vector2((transform.position.x + wallCheckDistance) * movement.FacingDirection, transform.position.y));
        }
    } 
}
