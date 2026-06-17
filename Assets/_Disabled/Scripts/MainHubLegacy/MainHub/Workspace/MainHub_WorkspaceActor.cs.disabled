using UnityEngine;

namespace MainHub.Workspace
{
    public sealed class MainHub_WorkspaceActor : MonoBehaviour
    {
        [SerializeField] private string _actorType;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(2, 5)] private string _description;

        public string ActorType
        {
            get
            {
                return _actorType;
            }
        }

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public void SetProfile(string actorType, string displayName, string description)
        {
            _actorType = actorType;
            _displayName = displayName;
            _description = description;
        }
    }
}
