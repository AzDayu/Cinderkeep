using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "NextWaypointAction ", story: "NextPoint", category: "Action", id: "ae9dab0f804acb72b3019163cecc40a1")]
public partial class NextWaypointAction : Action
{
    [SerializeReference]
    public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        EnemyBtPatrolController patrol =
            Self.Value.GetComponent<EnemyBtPatrolController>();

        patrol.NextWaypoint();

        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}