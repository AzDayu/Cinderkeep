using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MainHub.CharacterSelect
{
    public sealed class MainHub_CharacterSelectCanvas : MonoBehaviour
    {
        private const int RequiredCharacterCount = 10;

        [FormerlySerializedAs("_detailPanel")]
        [SerializeField] private MainHub_CharacterDetailPanel CharacterDetailPanel_DetailPanel;

        private readonly List<MainHub_CharacterSlot> _characterSlots = new List<MainHub_CharacterSlot>();
        private MainHub_CharacterSlot _selectedSlot;

        public IReadOnlyList<MainHub_CharacterSlot> CharacterSlots
        {
            get
            {
                return _characterSlots;
            }
        }

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

        public void SetDetailPanel(MainHub_CharacterDetailPanel detailPanel)
        {
            CharacterDetailPanel_DetailPanel = detailPanel;
        }

        public void SelectCharacter(MainHub_CharacterSlot slot)
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
            foreach (MainHub_CharacterSlot characterSlot in _characterSlots)
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

            MainHub_CharacterSlot[] slots = GetComponentsInChildren<MainHub_CharacterSlot>(true);
            _characterSlots.Clear();
            _characterSlots.AddRange(slots);
            _characterSlots.Sort(CompareSlotOrder);

            foreach (MainHub_CharacterSlot slot in _characterSlots)
            {
                slot.Selected += SelectCharacter;
            }
        }

        private void UnregisterCharacterSlots()
        {
            foreach (MainHub_CharacterSlot slot in _characterSlots)
            {
                if (slot != null)
                {
                    slot.Selected -= SelectCharacter;
                }
            }
        }

        private int CompareSlotOrder(MainHub_CharacterSlot leftSlot, MainHub_CharacterSlot rightSlot)
        {
            if (leftSlot == null && rightSlot == null)
            {
                return 0;
            }

            if (leftSlot == null)
            {
                return 1;
            }

            if (rightSlot == null)
            {
                return -1;
            }

            return leftSlot.transform.GetSiblingIndex().CompareTo(rightSlot.transform.GetSiblingIndex());
        }

        private void ResolveDetailPanel()
        {
            if (CharacterDetailPanel_DetailPanel == null)
            {
                CharacterDetailPanel_DetailPanel = GetComponentInChildren<MainHub_CharacterDetailPanel>(true);
            }
        }

        private void HideDetailPanel()
        {
            _selectedSlot = null;

            foreach (MainHub_CharacterSlot slot in _characterSlots)
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
                Debug.Log($"MainHub_CharacterSelectCanvas: {_characterSlots.Count} character objects recognized.");
                return;
            }

            Debug.LogWarning($"MainHub_CharacterSelectCanvas: expected {RequiredCharacterCount} character objects, but recognized {_characterSlots.Count}.");
        }
    }
}
