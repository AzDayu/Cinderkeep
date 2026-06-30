using UnityEngine;

// 화살 에셋이 없을 때도 타워 공격을 읽을 수 있게 해주는 폴백 투사체입니다.
public sealed class TowerProjectileView : MonoBehaviour
{
    private const float DefaultDuration = 0.18f;
    private const float ProjectileScale = 0.18f;

    private static Material _projectileMaterial;
    private static Material _trailMaterial;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _duration = DefaultDuration;
    private float _elapsedTime;

    public static void Play(Vector3 startPosition, Vector3 targetPosition)
    {
        GameObject projectileObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectileObject.name = "VFX_TowerProjectile_Fallback";
        projectileObject.transform.position = startPosition;
        projectileObject.transform.localScale = Vector3.one * ProjectileScale;

        Collider projectileCollider = projectileObject.GetComponent<Collider>();
        if (projectileCollider != null)
        {
            Destroy(projectileCollider);
        }

        MeshRenderer renderer = projectileObject.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = GetProjectileMaterial();
        }

        TrailRenderer trailRenderer = projectileObject.AddComponent<TrailRenderer>();
        trailRenderer.time = 0.16f;
        trailRenderer.startWidth = 0.12f;
        trailRenderer.endWidth = 0.02f;
        trailRenderer.sharedMaterial = GetTrailMaterial();
        trailRenderer.startColor = new Color(1f, 0.86f, 0.2f, 0.95f);
        trailRenderer.endColor = new Color(1f, 0.35f, 0.05f, 0f);

        TowerProjectileView projectileView = projectileObject.AddComponent<TowerProjectileView>();
        projectileView.Initialize(startPosition, targetPosition, DefaultDuration);
    }

    private void Initialize(Vector3 startPosition, Vector3 targetPosition, float duration)
    {
        _startPosition = startPosition;
        _targetPosition = targetPosition;
        _duration = Mathf.Max(0.05f, duration);
        _elapsedTime = 0f;
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(_elapsedTime / _duration);
        transform.position = Vector3.Lerp(_startPosition, _targetPosition, progress);

        if (progress >= 1f)
        {
            Destroy(gameObject);
        }
    }

    private static Material GetProjectileMaterial()
    {
        if (_projectileMaterial != null)
        {
            return _projectileMaterial;
        }

        _projectileMaterial = CreateMaterial(new Color(1f, 0.78f, 0.05f, 1f));
        return _projectileMaterial;
    }

    private static Material GetTrailMaterial()
    {
        if (_trailMaterial != null)
        {
            return _trailMaterial;
        }

        _trailMaterial = CreateMaterial(new Color(1f, 0.45f, 0.05f, 0.85f));
        return _trailMaterial;
    }

    private static Material CreateMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }

        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        Material material = new Material(shader);
        material.color = color;
        return material;
    }
}
