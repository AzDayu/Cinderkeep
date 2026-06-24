using UnityEngine;

// 5.00 direction: Handles one part of first-person player control, status, combat, gathering, or building.
// 5.01+ note: Keep input, state, and action effects separated so quickslots, tools, weapons, and tutorials remain maintainable.
public sealed class PlayerGroundCheck : MonoBehaviour
{
    private Collider _playerCollider;
    private Collider _rootCollider;
    private int _groundContactCount;

    private void Start()
    {
        _playerCollider = GetComponent<Collider>();
        _rootCollider = GetComponentInParent<Collider>();
    }

    public bool GetIsGrounded()
    {
        return _groundContactCount > 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsIgnoredCollider(other))
        {
            return;
        }

        _groundContactCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsIgnoredCollider(other))
        {
            return;
        }

        _groundContactCount = Mathf.Max(0, _groundContactCount - 1);
    }

    private bool IsIgnoredCollider(Collider other)
    {
        return other == _playerCollider || other == _rootCollider;
    }
}
