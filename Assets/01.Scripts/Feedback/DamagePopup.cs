using UnityEngine;
using TMPro;

namespace JunkyardClicker.Feedback
{
    public class DamagePopup : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro _text;

        [SerializeField]
        private float _lifetime = 0.8f;

        [SerializeField]
        private float _floatSpeed = 2f;

        [SerializeField]
        private AnimationCurve _scaleCurve;

        [SerializeField]
        private AnimationCurve _alphaCurve;

        [SerializeField]
        private Color _normalColor = Color.white;

        [SerializeField]
        private Color _criticalColor = Color.yellow;

        [SerializeField]
        private int _criticalThreshold = 20;

        private float _elapsedTime;
        private Vector3 _startScale;

        private void Awake()
        {
            _startScale = transform.localScale;

            if (_scaleCurve == null || _scaleCurve.length == 0)
            {
                _scaleCurve = CreateDefaultScaleCurve();
            }

            if (_alphaCurve == null || _alphaCurve.length == 0)
            {
                _alphaCurve = CreateDefaultAlphaCurve();
            }
        }

        public void Initialize(int damage)
        {
            if (_text == null)
            {
                return;
            }

            _text.text = damage.ToString();

            bool isCritical = damage >= _criticalThreshold;
            _text.color = isCritical ? _criticalColor : _normalColor;

            if (isCritical)
            {
                _startScale *= 1.5f;
            }

            transform.localScale = _startScale;
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            float progress = _elapsedTime / _lifetime;

            UpdatePosition();
            UpdateScale(progress);
            UpdateAlpha(progress);

            if (_elapsedTime >= _lifetime)
            {
                Destroy(gameObject);
            }
        }

        private void UpdatePosition()
        {
            transform.position += Vector3.up * (_floatSpeed * Time.deltaTime);
        }

        private void UpdateScale(float progress)
        {
            float scaleMultiplier = _scaleCurve.Evaluate(progress);
            transform.localScale = _startScale * scaleMultiplier;
        }

        private void UpdateAlpha(float progress)
        {
            if (_text == null)
            {
                return;
            }

            float alpha = _alphaCurve.Evaluate(progress);
            Color currentColor = _text.color;
            currentColor.a = alpha;
            _text.color = currentColor;
        }

        private AnimationCurve CreateDefaultScaleCurve()
        {
            return new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(0.1f, 1.2f),
                new Keyframe(0.2f, 1f),
                new Keyframe(1f, 1f)
            );
        }

        private AnimationCurve CreateDefaultAlphaCurve()
        {
            return new AnimationCurve(
                new Keyframe(0f, 1f),
                new Keyframe(0.7f, 1f),
                new Keyframe(1f, 0f)
            );
        }
    }
}
