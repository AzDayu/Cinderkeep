using UnityEngine;

namespace OODong.Muckhold
{
    public sealed class MuckholdPickaxeView : MonoBehaviour
    {
        [SerializeField] private Transform _viewRoot;
        [SerializeField] private float _swingDuration = 0.22f;
        [SerializeField] private Vector3 _swingEulerOffset = new Vector3(38f, -18f, -14f);
        [SerializeField] private Vector3 _swingPositionOffset = new Vector3(0.08f, -0.07f, 0.06f);

        private Vector3 _defaultLocalPosition;
        private Quaternion _defaultLocalRotation;
        private float _swingTimer;

        private void Awake()
        {
            if (_viewRoot == null)
            {
                _viewRoot = transform;
            }

            _defaultLocalPosition = _viewRoot.localPosition;
            _defaultLocalRotation = _viewRoot.localRotation;
        }

        private void Update()
        {
            if (_swingTimer <= 0f)
            {
                return;
            }

            _swingTimer -= Time.deltaTime;
            float progress = 1f - Mathf.Clamp01(_swingTimer / _swingDuration);
            float arc = Mathf.Sin(progress * Mathf.PI);
            _viewRoot.localPosition = _defaultLocalPosition + (_swingPositionOffset * arc);
            _viewRoot.localRotation = _defaultLocalRotation * Quaternion.Euler(_swingEulerOffset * arc);

            if (_swingTimer <= 0f)
            {
                _viewRoot.localPosition = _defaultLocalPosition;
                _viewRoot.localRotation = _defaultLocalRotation;
            }
        }

        public void PlaySwing()
        {
            _swingTimer = _swingDuration;
        }
    }
}
