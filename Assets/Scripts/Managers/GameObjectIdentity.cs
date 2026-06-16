using UnityEngine;

namespace Cinderkeep.Gameplay
{
    // GameObjectManager가 만든 오브젝트에 붙는 식별 컴포넌트입니다.
    // 몬스터, 구조물, 채집물은 이 id를 통해 수정/제거 요청을 구분할 수 있습니다.
    public sealed class GameObjectIdentity : MonoBehaviour
    {
        [SerializeField] private int _instanceId;
        [SerializeField] private bool _isInitialized;

        public int InstanceId
        {
            get
            {
                return _instanceId;
            }
        }

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        public void Initialize(int instanceId)
        {
            _instanceId = instanceId;
            _isInitialized = true;
        }
    }
}
