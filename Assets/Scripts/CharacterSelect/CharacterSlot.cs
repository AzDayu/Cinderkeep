using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace OODong.CharacterSelect
{
    public sealed class CharacterSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [FormerlySerializedAs("_entry")]
        [SerializeField] private CharacterEntry CharacterEntry_Entry;
        [FormerlySerializedAs("_frame")]
        [SerializeField] private Image Image_Frame;
        [FormerlySerializedAs("_outline")]
        [SerializeField] private Outline Outline_Outline;
        [FormerlySerializedAs("_rect")]
        [SerializeField] private RectTransform RectTransform_Rect;
        [FormerlySerializedAs("_portrait")]
        [SerializeField] private Image Image_Portrait;
        [FormerlySerializedAs("_placeholder")]
        [SerializeField] private Text Text_Placeholder;
        [FormerlySerializedAs("_hoverWash")]
        [SerializeField] private Image Image_HoverWash;
        [FormerlySerializedAs("_labelBack")]
        [SerializeField] private Image Image_LabelBack;
        [SerializeField] private Color _accentColor = Color.white;
        [SerializeField] private bool _loadPortraitFromResources = true;

        private bool _hovered;
        private bool _selected;
        private Coroutine _scaleRoutine;

        public event Action<CharacterSlot> Selected;

        public CharacterEntry Entry => CharacterEntry_Entry;

        private void Awake()
        {
            ResolveReferences();
            LoadPortrait();
            ApplyVisualState(false);
        }

        public void SetEntry(CharacterEntry entry)
        {
            CharacterEntry_Entry = entry;
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
            Image_Frame = frame;
            Outline_Outline = outline;
            RectTransform_Rect = rect;
            Image_Portrait = portrait;
            Text_Placeholder = placeholder;
            Image_HoverWash = hoverWash;
            Image_LabelBack = labelBack;
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
            if (RectTransform_Rect == null)
            {
                RectTransform_Rect = GetComponent<RectTransform>();
            }

            if (Image_Frame == null)
            {
                Image_Frame = GetComponent<Image>();
            }

            if (Outline_Outline == null)
            {
                Outline_Outline = GetComponent<Outline>();
            }
        }

        private void LoadPortrait()
        {
            if (!_loadPortraitFromResources || CharacterEntry_Entry == null || !CharacterEntry_Entry.HasId || CharacterEntry_Entry.Portrait != null)
            {
                return;
            }

            Sprite sprite = Resources.Load<Sprite>($"CharacterPortraits/{CharacterEntry_Entry.Id}");
            if (sprite == null)
            {
                Texture2D texture = Resources.Load<Texture2D>($"CharacterPortraits/{CharacterEntry_Entry.Id}");
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

            CharacterEntry_Entry.SetPortrait(sprite);
            if (Image_Portrait != null)
            {
                Image_Portrait.sprite = sprite;
                Image_Portrait.type = Image.Type.Simple;
                Image_Portrait.color = Color.white;
            }

            if (Text_Placeholder != null)
            {
                Text_Placeholder.gameObject.SetActive(false);
            }
        }

        private void ApplyVisualState(bool animate)
        {
            bool active = _hovered || _selected;

            if (Image_Frame != null)
            {
                Image_Frame.color = active ? Color.Lerp(_accentColor, Color.white, 0.34f) : new Color(0.08f, 0.085f, 0.1f, 1f);
            }

            if (Outline_Outline != null)
            {
                Outline_Outline.effectColor = _selected ? new Color(1f, 0.8f, 0.18f, 1f) : active ? Color.white : new Color(1f, 1f, 1f, 0.14f);
                Outline_Outline.effectDistance = active ? new Vector2(12f, -12f) : new Vector2(2f, -2f);
            }

            if (Image_HoverWash != null)
            {
                Image_HoverWash.color = active ? new Color(1f, 1f, 1f, 0.3f) : new Color(1f, 1f, 1f, 0f);
            }

            if (Image_LabelBack != null)
            {
                Image_LabelBack.color = active ? new Color(0f, 0f, 0f, 0.94f) : new Color(0f, 0f, 0f, 0.72f);
            }

            if (Text_Placeholder != null)
            {
                Text_Placeholder.color = active ? Color.white : new Color(1f, 1f, 1f, 0.78f);
            }

            if (Image_Portrait != null && Image_Portrait.sprite == null)
            {
                Image_Portrait.color = active ? Color.Lerp(_accentColor, Color.white, 0.24f) : _accentColor;
            }

            ScaleCard(active ? 1.12f : 1f, animate);
        }

        private void ScaleCard(float targetScale, bool animate)
        {
            if (RectTransform_Rect == null)
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

            RectTransform_Rect.localScale = Vector3.one * targetScale;
        }

        private IEnumerator AnimateScale(float target)
        {
            Vector3 start = RectTransform_Rect.localScale;
            Vector3 end = Vector3.one * target;
            float elapsed = 0f;
            const float Duration = 0.12f;

            while (elapsed < Duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / Duration);
                RectTransform_Rect.localScale = Vector3.Lerp(start, end, t);
                yield return null;
            }

            RectTransform_Rect.localScale = end;
            _scaleRoutine = null;
        }
    }
}
