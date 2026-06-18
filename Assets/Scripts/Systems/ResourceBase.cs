using UnityEngine;

// 자원 오브젝트가 공통으로 가질 기본 정보입니다.
// 채집량, 내구도, 도구 조건은 후속 작업에서 데이터 기반으로 확장합니다.
public sealed class ResourceBase : MonoBehaviour
{
    [SerializeField] private string _resourceId = "wood";
    [SerializeField] private int _amount = 1;

    public string ResourceId
    {
        get
        {
            return _resourceId;
        }
    }

    public int Amount
    {
        get
        {
            return _amount;
        }
    }

    public void Initialize(string resourceId, int amount)
    {
        _resourceId = resourceId;
        _amount = Mathf.Max(0, amount);
    }
}
