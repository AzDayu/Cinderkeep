using UnityEngine;

namespace OODong.Muckhold
{
    public sealed class MuckholdChestNode : MonoBehaviour, IMuckholdInteractable
    {
        [SerializeField] private MuckholdItemId _rewardItemId = MuckholdItemId.Apple;
        [SerializeField] private int _rewardAmount = 1;
        [SerializeField] private Renderer _targetRenderer;

        private bool _isOpened;

        private void Awake()
        {
            if (_targetRenderer == null)
            {
                _targetRenderer = GetComponentInChildren<Renderer>();
            }
        }

        public string GetPrompt()
        {
            return "[E] Open Chest";
        }

        public bool CanInteract(MuckholdFirstPersonPlayer player)
        {
            return player != null && !_isOpened;
        }

        public void Interact(MuckholdFirstPersonPlayer player)
        {
            if (!CanInteract(player))
            {
                return;
            }

            _isOpened = true;
            player.AddItem(_rewardItemId, _rewardAmount);
            player.ShowStatus($"Chest opened: +{_rewardAmount} {MuckholdItemCatalog.GetDisplayName(_rewardItemId)}");

            if (_targetRenderer != null)
            {
                _targetRenderer.material.color = new Color(0.45f, 0.38f, 0.22f, 1f);
            }
        }

        public void SetReward(MuckholdItemId rewardItemId, int rewardAmount)
        {
            _rewardItemId = rewardItemId;
            _rewardAmount = Mathf.Max(1, rewardAmount);
        }
    }
}
