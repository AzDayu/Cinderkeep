using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Enemy Move To Target",
    story: "[Self] moves to [Target] when state is [RequiredState]",
    category: "Action/Cinderkeep/Enemy",
    id: "cinderkeep_enemy_move_to_target_action_v2")]
public partial class EnemyMoveToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    [SerializeReference] public BlackboardVariable<string> RequiredState = new BlackboardVariable<string>("MoveToCinderHeart");

    private EnemyMovement _enemyMovement;

    protected override Status OnStart()
    {
        GameObject selfObject = GetSelfObject();
        if (IsUnityObjectNull(selfObject))
        {
            Debug.LogWarning("EnemyMoveToTargetAction: Self가 없습니다.");
            return Status.Failure;
        }

        GameObject targetObject = Target == null ? null : Target.Value;
        if (IsUnityObjectNull(targetObject))
        {
            Debug.LogWarning("EnemyMoveToTargetAction: Target이 없습니다.");
            return Status.Failure;
        }

        _enemyMovement = selfObject.GetComponent<EnemyMovement>();
        if (_enemyMovement == null)
        {
            Debug.LogWarning("EnemyMoveToTargetAction: Self에 EnemyMovement가 없습니다. object=" + selfObject.name);
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        GameObject selfObject = GetSelfObject();
        GameObject targetObject = Target == null ? null : Target.Value;

        if (IsUnityObjectNull(selfObject) || IsUnityObjectNull(targetObject))
        {
            return Status.Failure;
        }

        if (IsRequiredStateMatched(selfObject) == false)
        {
            return Status.Failure;
        }

        if (_enemyMovement == null)
        {
            _enemyMovement = selfObject.GetComponent<EnemyMovement>();
        }

        if (_enemyMovement == null)
        {
            return Status.Failure;
        }

        _enemyMovement.MoveToTarget(targetObject.transform);

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }

    private bool IsRequiredStateMatched(GameObject selfObject)
    {
        string requiredStateName = RequiredState == null ? string.Empty : RequiredState.Value;
        if(string.IsNullOrWhiteSpace(requiredStateName))
        {
            return true;
        }
        EnemyBehaviorState behaviorState = selfObject.GetComponent<EnemyBehaviorState>();

        if(behaviorState == null)
        {
            Debug.LogWarning("EnemyMoveToTargetAction: EnemyBehaviorState가 없습니다. object=" + selfObject.name);
            return false;
        }

        return behaviorState.IsCurrentState(requiredStateName);
    }



    private GameObject GetSelfObject()
    {
        if (Self != null && IsUnityObjectNull(Self.Value) == false)
        {
            return Self.Value;
        }

        return GameObject;
    }

    private static bool IsUnityObjectNull(UnityEngine.Object targetObject)
    {
        return ReferenceEquals(targetObject, null) || targetObject == null;
    }
}