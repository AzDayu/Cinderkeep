using UnityEngine;

// 5.13 MVP 함정을 CinderHeart 기준 동서남북에 자동 배치합니다.
// 정식 건축 함정이 붙으면 이 스크립트는 테스트/기본 배치 용도로만 남깁니다.
public static class TrapZoneSceneBootstrap
{
    private const string RootName = "TrapZones_5_13";

    public static void EnsureTrapZones()
    {
        Transform root = GetOrCreateRoot();
        Vector3 center = ResolveCenterPosition();

        CreateOrKeepTrap(root, "Trap_North", center + new Vector3(0f, 0.08f, 5.2f), new Vector3(4.0f, 0.18f, 2.2f));
        CreateOrKeepTrap(root, "Trap_South", center + new Vector3(0f, 0.08f, -5.2f), new Vector3(4.0f, 0.18f, 2.2f));
        CreateOrKeepTrap(root, "Trap_East", center + new Vector3(5.2f, 0.08f, 0f), new Vector3(2.2f, 0.18f, 4.0f));
        CreateOrKeepTrap(root, "Trap_West", center + new Vector3(-5.2f, 0.08f, 0f), new Vector3(2.2f, 0.18f, 4.0f));
    }

    private static Transform GetOrCreateRoot()
    {
        GameObject root = GameObject.Find(RootName);
        if (root == null)
        {
            root = new GameObject(RootName);
        }

        return root.transform;
    }

    private static void CreateOrKeepTrap(Transform root, string trapName, Vector3 position, Vector3 scale)
    {
        Transform existing = root.Find(trapName);
        if (existing != null)
        {
            return;
        }

        GameObject trapObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        trapObject.name = trapName;
        trapObject.transform.SetParent(root);
        trapObject.transform.position = position;
        trapObject.transform.localScale = scale;

        BoxCollider boxCollider = trapObject.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true;
        }

        Renderer renderer = trapObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(0.95f, 0.18f, 0.08f, 0.55f);
        }

        trapObject.AddComponent<TrapCrowdControlReporter>();
        trapObject.AddComponent<TrapSlowZone>();
    }

    private static Vector3 ResolveCenterPosition()
    {
        CinderHeart cinderHeart = Object.FindFirstObjectByType<CinderHeart>();
        if (cinderHeart != null)
        {
            return cinderHeart.transform.position;
        }

        return Vector3.zero;
    }
}
