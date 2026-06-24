using UnityEngine;

public class EnemyBtPatrolController : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] _patrolPoints;
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _stopDistance = 0.5f;

    [Header("Waypoint Color Settings")]
    [SerializeField]
    private Color[] _waypointReachedColors =
    {
        Color.red,
        Color.yellow,
        Color.green,
        Color.cyan,
        Color.magenta
    };

    private int _currentIndex;

    public void MoveToCurrentWaypoint()
    {
        if (_patrolPoints == null || _patrolPoints.Length == 0)
        {
            return;
        }

        Vector3 target = _patrolPoints[_currentIndex].position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            _moveSpeed * Time.deltaTime);
    }

    public bool HasReachedWaypoint()
    {
        if (_patrolPoints == null || _patrolPoints.Length == 0)
        {
            return false;
        }

        float distance = Vector3.Distance(
            transform.position,
            _patrolPoints[_currentIndex].position);

        return distance <= _stopDistance;
    }

    public void ChangeCurrentWaypointColor()
    {
        if (_patrolPoints == null || _patrolPoints.Length == 0)
        {
            return;
        }

        Transform currentWaypoint = _patrolPoints[_currentIndex];

        Renderer renderer = currentWaypoint.GetComponent<Renderer>();

        if (renderer == null)
        {
            renderer = currentWaypoint.GetComponentInChildren<Renderer>();
        }

        if (renderer == null)
        {
            Debug.LogWarning(currentWaypoint.name + "에 Renderer가 없습니다.");
            return;
        }

        Color targetColor = GetColorByCurrentIndex();

        renderer.material.color = targetColor;
    }

    public void NextWaypoint()
    {
        if (_patrolPoints == null || _patrolPoints.Length == 0)
        {
            return;
        }

        _currentIndex++;

        if (_currentIndex >= _patrolPoints.Length)
        {
            _currentIndex = 0;
        }
    }

    private Color GetColorByCurrentIndex()
    {
        if (_waypointReachedColors == null || _waypointReachedColors.Length == 0)
        {
            return Color.white;
        }

        int colorIndex = _currentIndex % _waypointReachedColors.Length;
        return _waypointReachedColors[colorIndex];
    }
}