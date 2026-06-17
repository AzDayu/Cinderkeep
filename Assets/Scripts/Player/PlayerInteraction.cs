using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("상호작용 세팅")]
    [SerializeField] private float _interactionDistance = 3f; // 상호작용 거리
    [SerializeField] private LayerMask _interactionLayerMask; // 상호작용 레이어 마스크
    [SerializeField] private Transform Transform_Camera; // 플레이어 카메라


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        Ray ray = new Ray(Transform_Camera.position, Transform_Camera.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, _interactionDistance, _interactionLayerMask))
        {
            //GameObject hitObject = hitInfo.collider.gameObject;
            //var fieldObject = hitObject.LayerMask.NameToLayer("Resources");

        }
    }
}
