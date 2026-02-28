using UnityEngine;

public class FootGroundChecker : MonoBehaviour
{
    public float rayDistance = 0.3f;
    public LayerMask groundLayer;

    private bool wasGrounded;
    private FootStepSound footstepManager;

    void Start()
    {
        footstepManager = GetComponentInParent<FootStepSound>();
    }

    void Update()
    {
        RaycastHit hit;

        bool isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            out hit,
            rayDistance,
            groundLayer
        );

        if (isGrounded && !wasGrounded)
        {
            footstepManager.PlayFootstep(hit.collider.tag);
        }

        wasGrounded = isGrounded;
    }
}
