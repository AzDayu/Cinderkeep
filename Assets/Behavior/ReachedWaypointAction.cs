using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ReachedWaypointAction ", story: "Reached", category: "Action", id: "c20e8162ef1e02d46379d151460f1c34")]
public partial class ReachedWaypointAction : Action
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

        if (patrol.HasReachedWaypoint())
        {
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}
