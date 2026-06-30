// ==================================================
// FILE: Assets/Behavior/Enemy/EnemyDetectorDebugProbe.cs
// ==================================================

using System.Reflection;
using UnityEngine;

// EnemyDetector가 왜 Player를 감지하지 못하는지 확인하는 디버그 전용 스크립트입니다.
// 테스트가 끝나면 비활성화하거나 제거하면 됩니다.

public sealed class EnemyDetectorDebugProbe : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool _enableLog = true;
    [SerializeField] private float _logInterval = 0.5f;
    [SerializeField] private string _playerTag = "Player";

    [Header("Physics Probe")]
    [SerializeField] private int _maxOverlapCount = 32;
    [SerializeField] private float _fallbackProbeDistance = 12f;

    private EnemyDetector _enemyDetector;
    private Collider[] _overlapColliders;
    private float _nextLogTime;

    private const BindingFlags PrivateInstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic;

    private void Awake()
    {
        _enemyDetector = GetComponent<EnemyDetector>();
        _overlapColliders = new Collider[Mathf.Max(1, _maxOverlapCount)];
    }

    private void Update()
    {
        if (_enableLog == false)
        {
            return;
        }

        if (Time.time < _nextLogTime)
        {
            return;
        }

        _nextLogTime = Time.time + _logInterval;
        LogDetectorSnapshot();
    }

    private void LogDetectorSnapshot()
    {
        if (_enemyDetector == null)
        {
            Debug.LogWarning("[DetectorProbe] EnemyDetector가 없습니다. object=" + gameObject.name);
            return;
        }

        Transform detectedPlayer = _enemyDetector.DetectedPlayer;
        bool hasDetectedPlayer = _enemyDetector.HasDetectedPlayer;

        float detectorDistance = GetPrivateFloat("_detectorDistance", -1f);
        float viewAngle = GetPrivateFloat("_viewAngle", -1f);
        float dayDetectorDistance = GetPrivateFloat("_dayDetectorDistance", -1f);
        float nightDetectorDistance = GetPrivateFloat("_nightDetectorDistance", -1f);
        float detectionInterval = GetPrivateFloat("_detectionInterval", -1f);
        bool isNightDetectionEnabled = GetPrivateBool("_isNightDetectionEnabled", false);

        float probeDistance = detectorDistance > 0f ? detectorDistance : _fallbackProbeDistance;

        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            probeDistance,
            _overlapColliders);

        Debug.Log(
            "[DetectorProbe] object=" + gameObject.name +
            " | hasDetected=" + hasDetectedPlayer +
            " | detected=" + GetObjectName(detectedPlayer) +
            " | detectorDistance=" + detectorDistance +
            " | dayDistance=" + dayDetectorDistance +
            " | nightDistance=" + nightDetectorDistance +
            " | viewAngle=" + viewAngle +
            " | halfViewAngle=" + (viewAngle * 0.5f) +
            " | nightMode=" + isNightDetectionEnabled +
            " | interval=" + detectionInterval +
            " | overlapHitCount=" + hitCount +
            " | enemyForward=" + transform.forward);

        bool foundPlayerCandidate = false;

        for (int i = 0; i < hitCount; i++)
        {
            Collider targetCollider = _overlapColliders[i];
            if (targetCollider == null)
            {
                continue;
            }

            bool colliderTagIsPlayer = targetCollider.CompareTag(_playerTag);

            Transform rootTransform = targetCollider.transform.root;
            bool rootTagIsPlayer = rootTransform != null && rootTransform.CompareTag(_playerTag);

            if (colliderTagIsPlayer == false && rootTagIsPlayer == false)
            {
                continue;
            }

            foundPlayerCandidate = true;

            Vector3 directionToTarget = targetCollider.transform.position - transform.position;
            float distance = directionToTarget.magnitude;

            float angle = -1f;
            bool inViewAngle = false;

            if (directionToTarget.sqrMagnitude > 0.0001f && viewAngle > 0f)
            {
                angle = Vector3.Angle(transform.forward, directionToTarget.normalized);
                inViewAngle = angle <= viewAngle * 0.5f;
            }

            Debug.Log(
                "[DetectorProbe][PLAYER_CANDIDATE] enemy=" + gameObject.name +
                " | collider=" + targetCollider.name +
                " | colliderObject=" + targetCollider.gameObject.name +
                " | colliderTag=" + targetCollider.tag +
                " | root=" + GetObjectName(rootTransform) +
                " | rootTag=" + (rootTransform == null ? "NULL" : rootTransform.tag) +
                " | colliderTagIsPlayer=" + colliderTagIsPlayer +
                " | rootTagIsPlayer=" + rootTagIsPlayer +
                " | colliderEnabled=" + targetCollider.enabled +
                " | isTrigger=" + targetCollider.isTrigger +
                " | distance=" + distance +
                " | angle=" + angle +
                " | inViewAngle=" + inViewAngle +
                " | detectorWouldAcceptByCurrentCode=" + colliderTagIsPlayer);
        }

        if (foundPlayerCandidate == false)
        {
            Debug.LogWarning(
                "[DetectorProbe] OverlapSphere 안에서 Player 태그 후보를 찾지 못했습니다. " +
                "Player가 실제 감지 반경 안에 있는지, Player Collider가 켜져 있는지, Collider가 Player root가 아닌 자식에 붙어 있는지 확인하세요. object=" + gameObject.name);
        }
    }

    private float GetPrivateFloat(string fieldName, float fallbackValue)
    {
        FieldInfo fieldInfo = typeof(EnemyDetector).GetField(fieldName, PrivateInstanceFlags);
        if (fieldInfo == null)
        {
            return fallbackValue;
        }

        object value = fieldInfo.GetValue(_enemyDetector);
        if (value is float floatValue)
        {
            return floatValue;
        }

        return fallbackValue;
    }

    private bool GetPrivateBool(string fieldName, bool fallbackValue)
    {
        FieldInfo fieldInfo = typeof(EnemyDetector).GetField(fieldName, PrivateInstanceFlags);
        if (fieldInfo == null)
        {
            return fallbackValue;
        }

        object value = fieldInfo.GetValue(_enemyDetector);
        if (value is bool boolValue)
        {
            return boolValue;
        }

        return fallbackValue;
    }

    private static string GetObjectName(Object targetObject)
    {
        if (targetObject == null)
        {
            return "NULL";
        }

        return targetObject.name;
    }
}