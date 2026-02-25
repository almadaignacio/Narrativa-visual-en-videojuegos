using UnityEngine;


namespace PhysicsCharacterController
{
    public class AnimatedController : MonoBehaviour
    {
        [Header("References")]
        public CharacterManager characterManager;
        public Rigidbody rigidbodyCharacter;

        [Header("Animation specifics")]
        public float fallThreshold = -0.2f;
        public float jumpThreshold = 0.15f;

        private Animator anim;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }
        private void Update()
        {
            bool isGrounded = characterManager.isGrounded;
            float verticalVelocity = rigidbodyCharacter.linearVelocity.y;

            Vector3 localVelocity = transform.InverseTransformDirection(rigidbodyCharacter.linearVelocity);
            float horizontalSpeed = new Vector3(localVelocity.x, 0, localVelocity.z).magnitude;

            // Blend Tree
            anim.SetFloat("moveX", localVelocity.x);
            anim.SetFloat("moveY", localVelocity.z);
            anim.SetBool("isGrounded", isGrounded);

            // Estados Aéreos
            bool isJumping = !isGrounded && verticalVelocity > jumpThreshold;
            bool isFalling = !isGrounded && verticalVelocity < fallThreshold;
            bool isIdleFalling = false;

            // Solo si querés diferenciar caída vertical
            if (isFalling)
            {
                isIdleFalling = horizontalSpeed <= 0.1f;
            }

            anim.SetBool("isJumping", isJumping);
            anim.SetBool("isFalling", isFalling);
            anim.SetBool("isIdleFalling", isIdleFalling);
        }
    }
}