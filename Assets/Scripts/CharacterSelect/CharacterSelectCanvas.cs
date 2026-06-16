using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace OODong.CharacterSelect
{
    public sealed class CharacterSelectCanvas : MonoBehaviour
    {
        private const int RequiredCharacterCount = 10;

        [FormerlySerializedAs("_detailPanel")]
        [SerializeField] private CharacterDetailPanel CharacterDetailPanel_DetailPanel;

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
            CharacterDetailPanel_DetailPanel = detailPanel;
        }

        public void SelectCharacter(CharacterSlot slot)
        {
            if (slot == null)
            {
                return;
            }

            if (_selectedSlot == slot && CharacterDetailPanel_DetailPanel != null && CharacterDetailPanel_DetailPanel.IsOpen)
            {
                HideDetailPanel();
                return;
            }

            _selectedSlot = slot;
            foreach (CharacterSlot characterSlot in _characterSlots)
            {
                characterSlot.SetSelected(characterSlot == _selectedSlot);
            }

            if (CharacterDetailPanel_DetailPanel != null)
            {
                CharacterDetailPanel_DetailPanel.Show(slot.Entry);
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
            if (CharacterDetailPanel_DetailPanel == null)
            {
                CharacterDetailPanel_DetailPanel = GetComponentInChildren<CharacterDetailPanel>(true);
            }
        }

        private void HideDetailPanel()
        {
            _selectedSlot = null;

            foreach (CharacterSlot slot in _characterSlots)
            {
                slot.SetSelected(false);
            }

            if (CharacterDetailPanel_DetailPanel != null)
            {
                CharacterDetailPanel_DetailPanel.Hide();
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
