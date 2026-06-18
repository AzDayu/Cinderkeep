using UnityEngine;
using UnityEngine.Serialization;

public sealed class PlayerJump : MonoBehaviour
{
    [FormerlySerializedAs("m_jumpForce")]
    [SerializeField] private float _jumpForce = 5f;
    [FormerlySerializedAs("PlayerMovement_PlayerMovement")]
    [SerializeField] private PlayerMovement _playerMovement;

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
        if (_playerMovement == null)
        {
            _playerMovement = GetComponent<PlayerMovement>();
        }
    }

    private void Jump()
    {
        if (_playerMovement == null)
        {
            return;
        }

        // 실제 점프 처리는 이동 컴포넌트가 CharacterController 기준으로 수행합니다.
        _playerMovement.Jump(_jumpForce);
    }
}
