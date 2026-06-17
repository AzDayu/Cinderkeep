using UnityEngine;

public class CharacterGroundCheck : MonoBehaviour
{
    private bool m_isGrounded;
    private Collider m_playerCollider;
    private Collider m_rootCollider; 

    void Start()
    {
        m_playerCollider = GetComponent<Collider>();
        m_rootCollider = GetComponentInParent<Collider>();
    }

    public bool GetIsGrounded()
    {
        return m_isGrounded;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other != m_playerCollider && other != m_rootCollider)
        {
            m_isGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other != m_playerCollider && other != m_rootCollider))
        {
            m_isGrounded = false;
        }
    }

}