using UnityEngine;
using UnityEngine.Serialization;

public sealed class PlayerJump : MonoBehaviour
{
    [FormerlySerializedAs("m_jumpForce")]
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private PlayerMovement PlayerMovement_PlayerMovement;

    private void Start()
    {
        ResolveReferences();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void ResolveReferences()
    {
        if (PlayerMovement_PlayerMovement == null)
        {
            PlayerMovement_PlayerMovement = GetComponent<PlayerMovement>();
        }
    }

    private void Jump()
    {
        if (PlayerMovement_PlayerMovement == null)
        {
            return;
        }

        // 실제 점프 처리는 이동 컴포넌트가 CharacterController 기준으로 수행합니다.
        PlayerMovement_PlayerMovement.Jump(_jumpForce);
    }
}
