using System.Collections.Generic;
using UnityEngine;

namespace OODong.CharacterSelect
{
    public sealed class CharacterSelectCanvas : MonoBehaviour
    {
        private const int RequiredCharacterCount = 10;

        [SerializeField] private CharacterDetailPanel _detailPanel;

        private readonly List<CharacterSlot> _characterSlots = new List<CharacterSlot>();
        private CharacterSlot _selectedSlot;

        public IReadOnlyList<CharacterSlot> CharacterSlots => _characterSlots;

        private void OnEnable()
        {
            RegisterCharacterSlots();
            HideDetailPanel();
            ReportCharacterCount();
        }

        private void OnDisable()
        {
            UnregisterCharacterSlots();
        }

        public void SetDetailPanel(CharacterDetailPanel detailPanel)
        {
            _detailPanel = detailPanel;
        }

        public void SelectCharacter(CharacterSlot slot)
        {
            if (slot == null)
            {
                return;
            }

            if (_selectedSlot == slot && _detailPanel != null && _detailPanel.IsOpen)
            {
                HideDetailPanel();
                return;
            }

            _selectedSlot = slot;
            foreach (CharacterSlot characterSlot in _characterSlots)
            {
                characterSlot.SetSelected(characterSlot == _selectedSlot);
            }

            if (_detailPanel != null)
            {
                _detailPanel.Show(slot.Entry);
            }
        }

        private void RegisterCharacterSlots()
        {
            UnregisterCharacterSlots();
            ResolveDetailPanel();

            CharacterSlot[] slots = GetComponentsInChildren<CharacterSlot>(true);
            _characterSlots.Clear();
            _characterSlots.AddRange(slots);
            _characterSlots.Sort((left, right) => left.transform.GetSiblingIndex().CompareTo(right.transform.GetSiblingIndex()));

            foreach (CharacterSlot slot in _characterSlots)
            {
                slot.Selected += SelectCharacter;
            }
        }

        private void UnregisterCharacterSlots()
        {
            foreach (CharacterSlot slot in _characterSlots)
            {
                if (slot != null)
                {
                    slot.Selected -= SelectCharacter;
                }
            }
        }

        private void ResolveDetailPanel()
        {
            if (_detailPanel == null)
            {
                _detailPanel = GetComponentInChildren<CharacterDetailPanel>(true);
            }
        }

        private void HideDetailPanel()
        {
            _selectedSlot = null;

            foreach (CharacterSlot slot in _characterSlots)
            {
                slot.SetSelected(false);
            }

            if (_detailPanel != null)
            {
                _detailPanel.Hide();
            }
        }

        private void ReportCharacterCount()
        {
            if (_characterSlots.Count == RequiredCharacterCount)
            {
                Debug.Log($"CharacterSelectCanvas: {_characterSlots.Count} character objects recognized.");
                return;
            }

            Debug.LogWarning($"CharacterSelectCanvas: expected {RequiredCharacterCount} character objects, but recognized {_characterSlots.Count}.");
        }
    }
}
