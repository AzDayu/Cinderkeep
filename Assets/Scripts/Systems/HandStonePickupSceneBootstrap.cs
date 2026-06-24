using UnityEngine;

public static class HandStonePickupSceneBootstrap
{
    private const string HandStonePickupName = "Pickup_HandStone_5_00";
    private static readonly Vector3 PickupScale = new Vector3(0.42f, 0.24f, 0.42f);
    private static readonly Color PickupColor = new Color(0.42f, 0.44f, 0.46f, 1f);

    public static void EnsureHandStonePickup()
    {
        HandStonePickup pickup = FindScenePickup();
        if (pickup == null)
        {
            CreatePickup();
            return;
        }

        pickup.ResetPickup();
    }

    private static HandStonePickup FindScenePickup()
    {
        HandStonePickup[] pickups = Object.FindObjectsByType<HandStonePickup>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        for (int i = 0; i < pickups.Length; i++)
        {
            HandStonePickup pickup = pickups[i];
            if (pickup == null)
            {
                continue;
            }

            if (pickup.gameObject.scene.IsValid() == false)
            {
                continue;
            }

            if (pickup.gameObject.name == HandStonePickupName)
            {
                return pickup;
            }
        }

        return null;
    }

    private static void CreatePickup()
    {
        GameObject pickupObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pickupObject.name = HandStonePickupName;
        pickupObject.transform.position = GetPickupPosition();
        pickupObject.transform.localScale = PickupScale;

        ApplyPickupMaterial(pickupObject);
        pickupObject.AddComponent<HandStonePickup>();
    }

    private static Vector3 GetPickupPosition()
    {
        Transform playerTransform = FindPlayerTransform();
        if (playerTransform == null)
        {
            return new Vector3(0f, 0.18f, 2.5f);
        }

        Vector3 rawPosition = playerTransform.position + playerTransform.forward * 2f + playerTransform.right * 0.8f;
        return ProjectToGround(rawPosition);
    }

    private static Transform FindPlayerTransform()
    {
        PlayerToolController toolController = Object.FindFirstObjectByType<PlayerToolController>();
        if (toolController != null)
        {
            return toolController.transform;
        }

        GameObject playerObject = GameObject.Find("Player");
        if (playerObject == null)
        {
            return null;
        }

        return playerObject.transform;
    }

    private static Vector3 ProjectToGround(Vector3 rawPosition)
    {
        Ray ray = new Ray(rawPosition + Vector3.up * 6f, Vector3.down);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 20f))
        {
            return hitInfo.point + Vector3.up * 0.14f;
        }

        rawPosition.y = 0.18f;
        return rawPosition;
    }

    private static void ApplyPickupMaterial(GameObject pickupObject)
    {
        Renderer renderer = pickupObject.GetComponent<Renderer>();
        if (renderer == null)
        {
            return;
        }

        Shader shader = Shader.Find("Standard");
        if (shader == null)
        {
            renderer.material.color = PickupColor;
            return;
        }

        Material material = new Material(shader);
        material.name = "MAT_Runtime_HandStone";
        material.color = PickupColor;
        renderer.material = material;
    }
}
