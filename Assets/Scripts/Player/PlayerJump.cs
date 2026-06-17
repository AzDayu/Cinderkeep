using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private float m_jumpForce = 5f;
    [SerializeField] private CharacterGroundCheck m_groundCheck;

    private Rigidbody m_rigidbody;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (m_groundCheck != null && m_groundCheck.GetIsGrounded() == true)
        {
            m_rigidbody.linearVelocity = new Vector3(m_rigidbody.linearVelocity.x, 0f, m_rigidbody.linearVelocity.z);
            m_rigidbody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }
    }
}