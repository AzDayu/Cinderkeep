using UnityEngine;
using UnityEngine.Serialization;

// 플레이어 점프 입력을 담당하는 컴포넌트입니다.
// 실제 이동 계산은 PlayerMovement가 가진 CharacterController 흐름을 사용합니다.
public sealed class PlayerJump : MonoBehaviour
{
    [FormerlySerializedAs("m_jumpForce")]
    [SerializeField] private float _jumpForce = 5f;

    [Header("Connected Components")]
    [FormerlySerializedAs("PlayerMovement_PlayerMovement")]
    [SerializeField] private PlayerMovement _playerMovement;

    private void Start()
    {
        ResolveReferences();
    }

    private void Update()
    {
        if (CinderkeepInput.WasKeyPressedThisFrame(KeyCode.Space))
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

        _playerMovement.Jump(_jumpForce);
    }
}
