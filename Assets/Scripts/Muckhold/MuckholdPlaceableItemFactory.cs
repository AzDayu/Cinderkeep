using UnityEngine;

namespace OODong.Muckhold
{
    public sealed class MuckholdPlaceableItemFactory : MonoBehaviour
    {
        [SerializeField] private Transform _placedItemRoot;
        [SerializeField] private Material _stoneMaterial;
        [SerializeField] private Material _oreMaterial;
        [SerializeField] private float _placeDistance = 3.2f;

        public void SetReferences(Transform placedItemRoot, Material stoneMaterial, Material oreMaterial)
        {
            _placedItemRoot = placedItemRoot;
            _stoneMaterial = stoneMaterial;
            _oreMaterial = oreMaterial;
        }

        public bool TryPlaceItem(MuckholdFirstPersonPlayer player, Camera playerCamera, MuckholdItemId itemId)
        {
            if (player == null || playerCamera == null)
            {
                return false;
            }

            if (itemId != MuckholdItemId.Stone && itemId != MuckholdItemId.Ore)
            {
                return false;
            }

            if (!player.Inventory.TryRemoveItem(itemId, 1))
            {
                player.ShowStatus($"{MuckholdItemCatalog.GetDisplayName(itemId)} is empty");
                return false;
            }

            Vector3 placePosition = GetPlacePosition(playerCamera);
            GameObject placedItem = GameObject.CreatePrimitive(PrimitiveType.Cube);
            placedItem.name = $"Placed_{MuckholdItemCatalog.GetDisplayName(itemId)}";
            placedItem.transform.SetParent(_placedItemRoot != null ? _placedItemRoot : transform, true);
            placedItem.transform.position = placePosition;
            placedItem.transform.localScale = itemId == MuckholdItemId.Ore
                ? new Vector3(0.8f, 0.8f, 0.8f)
                : new Vector3(0.6f, 0.35f, 0.6f);

            Renderer renderer = placedItem.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = itemId == MuckholdItemId.Ore ? _oreMaterial : _stoneMaterial;
            }

            player.ShowStatus($"Placed {MuckholdItemCatalog.GetDisplayName(itemId)}");
            return true;
        }

        private Vector3 GetPlacePosition(Camera playerCamera)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, _placeDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                return hit.point + (hit.normal * 0.35f);
            }

            Vector3 forwardPoint = playerCamera.transform.position + playerCamera.transform.forward * _placeDistance;
            forwardPoint.y = Mathf.Max(0.35f, forwardPoint.y - 0.6f);
            return forwardPoint;
        }
    }
}
