using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChangeWaypointColor", story: "Change Waypoint Color", category: "Action", id: "e50d97473ce518b352da8d06c0876098")]
public partial class ChangeWaypointColorAction : Action
{
    [SerializeReference]
    public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        EnemyBtPatrolController patrol =
            Self.Value.GetComponent<EnemyBtPatrolController>();

        if (patrol == null)
        {
            return Status.Failure;
        }

        patrol.ChangeCurrentWaypointColor();

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
