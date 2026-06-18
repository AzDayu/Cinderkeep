?using UnityEngine;
using TMPro;
using Cinderkeep.Gameplay;

public sealed class ResourceUI : MonoBehaviour
{
    [Header("자원 텍스트 UI 연결")]
    [SerializeField] private TMP_Text _woodText;
    [SerializeField] private TMP_Text _stoneText;
    [SerializeField] private TMP_Text _ironText;
    [SerializeField] private TMP_Text _mithrilText;
    [SerializeField] private TMP_Text _adamantiumText;

    private void Start()
    {
        if (GameManager.Inst != null && GameManager.Inst.PlayerModel != null)
        {
            GameManager.Inst.PlayerModel.OnResourceChanged += UpdateResourceUI;

            UpdateResourceUI();
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null && GameManager.Inst.PlayerModel != null)
        {
            GameManager.Inst.PlayerModel.OnResourceChanged -= UpdateResourceUI;
        }
    }

    private void UpdateResourceUI()
    {
        PlayerModel model = GameManager.Inst.PlayerModel;
        if (_woodText != null) _woodText.text = model.Wood.ToString();
        if (_stoneText != null) _stoneText.text = model.Stone.ToString();
        if (_ironText != null) _ironText.text = model.Iron.ToString();
        if (_mithrilText != null) _mithrilText.text = model.Mithril.ToString();
        if (_adamantiumText != null) _adamantiumText.text = model.Adamantium.ToString();
    }
}