using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OODong.CharacterSelect
{
    public sealed class CharacterSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private CharacterEntry _entry;
        [SerializeField] private Image _frame;
        [SerializeField] private Outline _outline;
        [SerializeField] private RectTransform _rect;
        [SerializeField] private Image _portrait;
        [SerializeField] private Text _placeholder;
        [SerializeField] private Image _hoverWash;
        [SerializeField] private Image _labelBack;
        [SerializeField] private Color _accentColor = Color.white;
        [SerializeField] private bool _loadPortraitFromResources = true;

        private bool _hovered;
        private bool _selected;
        private Coroutine _scaleRoutine;

        public event Action<CharacterSlot> Selected;

        public CharacterEntry Entry => _entry;

        private void Awake()
        {
            ResolveReferences();
            LoadPortrait();
            ApplyVisualState(false);
        }

        public void SetEntry(CharacterEntry entry)
        {
            _entry = entry;
        }

        public void SetAccentColor(Color accentColor)
        {
            _accentColor = accentColor;
        }

        public void SetViewReferences(
            Image frame,
            Outline outline,
            RectTransform rect,
            Image portrait,
            Text placeholder,
            Image hoverWash,
            Image labelBack)
        {
            _frame = frame;
            _outline = outline;
            _rect = rect;
            _portrait = portrait;
            _placeholder = placeholder;
            _hoverWash = hoverWash;
            _labelBack = labelBack;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;
            ApplyVisualState(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hovered = false;
            ApplyVisualState(true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Selected?.Invoke(this);
        }

        public void SetSelected(bool value)
        {
            _selected = value;
            ApplyVisualState(true);
        }

        private void ResolveReferences()
        {
            if (_rect == null)
            {
                _rect = GetComponent<RectTransform>();
            }

            if (_frame == null)
            {
                _frame = GetComponent<Image>();
            }

            if (_outline == null)
            {
                _outline = GetComponent<Outline>();
            }
        }

        private void LoadPortrait()
        {
            if (!_loadPortraitFromResources || _entry == null || !_entry.HasId || _entry.Portrait != null)
            {
                return;
            }

            Sprite sprite = Resources.Load<Sprite>($"CharacterPortraits/{_entry.Id}");
            if (sprite == null)
            {
                Texture2D texture = Resources.Load<Texture2D>($"CharacterPortraits/{_entry.Id}");
                if (texture != null)
                {
                    sprite = Sprite.Create(
                        texture,
                        new Rect(0f, 0f, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f),
                        100f);
                }
            }

            if (sprite == null)
            {
                return;
            }

            _entry.SetPortrait(sprite);
            if (_portrait != null)
            {
                _portrait.sprite = sprite;
                _portrait.type = Image.Type.Simple;
                _portrait.color = Color.white;
            }

            if (_placeholder != null)
            {
                _placeholder.gameObject.SetActive(false);
            }
        }

        private void ApplyVisualState(bool animate)
        {
            bool active = _hovered || _selected;

            if (_frame != null)
            {
                _frame.color = active ? Color.Lerp(_accentColor, Color.white, 0.34f) : new Color(0.08f, 0.085f, 0.1f, 1f);
            }

            if (_outline != null)
            {
                _outline.effectColor = _selected ? new Color(1f, 0.8f, 0.18f, 1f) : active ? Color.white : new Color(1f, 1f, 1f, 0.14f);
                _outline.effectDistance = active ? new Vector2(12f, -12f) : new Vector2(2f, -2f);
            }

            if (_hoverWash != null)
            {
                _hoverWash.color = active ? new Color(1f, 1f, 1f, 0.3f) : new Color(1f, 1f, 1f, 0f);
            }

            if (_labelBack != null)
            {
                _labelBack.color = active ? new Color(0f, 0f, 0f, 0.94f) : new Color(0f, 0f, 0f, 0.72f);
            }

            if (_placeholder != null)
            {
                _placeholder.color = active ? Color.white : new Color(1f, 1f, 1f, 0.78f);
            }

            if (_portrait != null && _portrait.sprite == null)
            {
                _portrait.color = active ? Color.Lerp(_accentColor, Color.white, 0.24f) : _accentColor;
            }

            ScaleCard(active ? 1.12f : 1f, animate);
        }

        private void ScaleCard(float targetScale, bool animate)
        {
            if (_rect == null)
            {
                return;
            }

            if (animate && isActiveAndEnabled)
            {
                if (_scaleRoutine != null)
                {
                    StopCoroutine(_scaleRoutine);
                }

                _scaleRoutine = StartCoroutine(AnimateScale(targetScale));
                return;
            }

            _rect.localScale = Vector3.one * targetScale;
        }

        private IEnumerator AnimateScale(float target)
        {
            Vector3 start = _rect.localScale;
            Vector3 end = Vector3.one * target;
            float elapsed = 0f;
            const float Duration = 0.12f;

            while (elapsed < Duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / Duration);
                _rect.localScale = Vector3.Lerp(start, end, t);
                yield return null;
            }

            _rect.localScale = end;
            _scaleRoutine = null;
        }
    }
}
