using UnityEngine;

namespace OODong.Workspace
{
    public sealed class MuckholdWorkspaceActor : MonoBehaviour
    {
        [SerializeField] private string _actorType;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(2, 5)] private string _description;

        public string ActorType => _actorType;
        public string DisplayName => _displayName;
        public string Description => _description;

        public void SetProfile(string actorType, string displayName, string description)
        {
            _actorType = actorType;
            _displayName = displayName;
            _description = description;
        }
    }
}
