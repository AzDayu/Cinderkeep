using System;
using UnityEngine;


public enum BTEnemyMoveState
{
    Wander, 
    MoveToCinderHeart
}


public class EnemyBehaviorState : MonoBehaviour
{
    [SerializeField] private BTEnemyMoveState _mMoveState = BTEnemyMoveState.MoveToCinderHeart;

    public BTEnemyMoveState MoveState
    {
        get
        {
            return _mMoveState;
        }
    }

    public void SetMoveState(BTEnemyMoveState moveState)
    {
        _mMoveState = moveState;
    }

    public void SteWanderState()
    {
        SetMoveState(BTEnemyMoveState.Wander);
    }

    public void SetMoveToCinderHeartState()
    {
        SetMoveState(BTEnemyMoveState.MoveToCinderHeart);
    }

    public bool IsCurrentState(string requiredStateName)
    {
        if(string.IsNullOrWhiteSpace(requiredStateName))
        {
            return true;
        }

        BTEnemyMoveState requiredState;
        bool canParse = Enum.TryParse(requiredStateName, true, out requiredState);
        if(canParse == false )
        {
            Debug.LogWarning("EnemyBehaviorState: 알 수 없는 상태 이름입니다. requiredStateName=" + requiredStateName);
            return false;
        }
        return _mMoveState == requiredState;
    }

#if UNITY_EDITOR
    [ContextMenu("Set State/Wander")]
    private void DebugSetWanderState()
    {
        DebugSetWanderState();
    }
    [ContextMenu("Set State/Move To CinderHeart")]
    private void DebugSetMoveToCinderHeartState()
    {
        SetMoveToCinderHeartState();
    }
#endif
}
