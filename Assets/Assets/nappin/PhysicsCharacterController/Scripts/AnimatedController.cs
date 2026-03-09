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

            // 👇 usamos el input del controller
            Vector2 input = characterManager.input.axisInput;
            float horizontalSpeed = input.magnitude;

            // 👇 parámetros del Blend Tree
            anim.SetFloat("moveX", input.x, 0.15f, Time.deltaTime);
            anim.SetFloat("moveY", input.y, 0.15f, Time.deltaTime);

            anim.SetBool("isGrounded", isGrounded);

            bool isJumping = !isGrounded && verticalVelocity > jumpThreshold;
            bool isFalling = !isGrounded && verticalVelocity < fallThreshold;
            bool isIdleFalling = false;

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