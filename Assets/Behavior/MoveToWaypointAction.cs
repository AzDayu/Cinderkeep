using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToWaypoint", story: "MoveToWaypoint", category: "Action", id: "d44444f48e79410a2d3d5c052ad1c511")]
public partial class MoveToWaypointAction : Action
{
    [SerializeReference]
    public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        EnemyBtPatrolController patrol =
            Self.Value.GetComponent<EnemyBtPatrolController>();

        if (patrol == null)
            return Status.Failure;

        patrol.MoveToCurrentWaypoint();

        if (patrol.HasReachedWaypoint())
            return Status.Success;

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}
