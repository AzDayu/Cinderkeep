using UnityEngine;

namespace OODong.Muckhold
{
    public sealed class MuckholdMineableNode : MonoBehaviour, IMuckholdInteractable
    {
        [SerializeField] private string _displayName = "Ore";
        [SerializeField] private int _requiredHits = 3;
        [SerializeField] private MuckholdItemId _yieldItemId = MuckholdItemId.Ore;
        [SerializeField] private int _yieldAmount = 1;
        [SerializeField] private Renderer _targetRenderer;

        private int _hitsRemaining;
        private Color _defaultColor;

        private void Awake()
        {
            if (_targetRenderer == null)
            {
                _targetRenderer = GetComponentInChildren<Renderer>();
            }

            if (_targetRenderer != null)
            {
                _defaultColor = _targetRenderer.sharedMaterial.color;
            }
        }

        private void OnEnable()
        {
            _hitsRemaining = Mathf.Max(1, _requiredHits);
            RefreshView();
        }

        public string GetPrompt()
        {
            return $"[E] Mine {_displayName}";
        }

        public bool CanInteract(MuckholdFirstPersonPlayer player)
        {
            return player != null && _hitsRemaining > 0;
        }

        public void Interact(MuckholdFirstPersonPlayer player)
        {
            if (!CanInteract(player))
            {
                return;
            }

            if (!player.HasItem(MuckholdItemId.Pickaxe))
            {
                player.ShowStatus("Pickaxe required");
                return;
            }

            _hitsRemaining--;
            player.PlayPickaxeSwing();
            player.ShowMiningProgress(1f - ((float)_hitsRemaining / Mathf.Max(1, _requiredHits)));

            if (_hitsRemaining <= 0)
            {
                player.AddItem(_yieldItemId, _yieldAmount);
                player.ShowStatus($"Mined {_displayName}: +{_yieldAmount} {MuckholdItemCatalog.GetDisplayName(_yieldItemId)}");
                gameObject.SetActive(false);
                return;
            }

            player.ShowStatus($"{_displayName} hit {_requiredHits - _hitsRemaining}/{_requiredHits}");
            RefreshView();
        }

        public void SetMineable(string displayName, int requiredHits, MuckholdItemId yieldItemId, int yieldAmount)
        {
            _displayName = displayName;
            _requiredHits = Mathf.Max(1, requiredHits);
            _yieldItemId = yieldItemId;
            _yieldAmount = Mathf.Max(1, yieldAmount);
            _hitsRemaining = _requiredHits;
        }

        private void RefreshView()
        {
            if (_targetRenderer == null)
            {
                return;
            }

            float damageRate = 1f - ((float)_hitsRemaining / Mathf.Max(1, _requiredHits));
            Color targetColor = Color.Lerp(_defaultColor, Color.gray, damageRate);
            _targetRenderer.material.color = targetColor;
        }
    }
}
