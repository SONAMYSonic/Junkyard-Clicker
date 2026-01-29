using System.Collections;
using UnityEngine;

namespace JunkyardClicker.Feedback
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteFlash : MonoBehaviour
    {
        [SerializeField]
        private Color _flashColor = Color.white;

        [SerializeField]
        private float _flashDuration = 0.05f;

        [SerializeField]
        private Material _flashMaterial;

        private SpriteRenderer _spriteRenderer;
        private Material _originalMaterial;
        private Coroutine _flashCoroutine;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _originalMaterial = _spriteRenderer.material;
        }

        public void Flash()
        {
            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
            }

            _flashCoroutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            if (_flashMaterial != null)
            {
                _spriteRenderer.material = _flashMaterial;
                _flashMaterial.color = _flashColor;
            }
            else
            {
                _spriteRenderer.color = _flashColor;
            }

            yield return new WaitForSeconds(_flashDuration);

            if (_flashMaterial != null)
            {
                _spriteRenderer.material = _originalMaterial;
            }
            else
            {
                _spriteRenderer.color = Color.white;
            }

            _flashCoroutine = null;
        }

        public void FlashWithColor(Color color)
        {
            _flashColor = color;
            Flash();
        }

        private void OnDisable()
        {
            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
                _flashCoroutine = null;
            }

            if (_spriteRenderer != null && _originalMaterial != null)
            {
                _spriteRenderer.material = _originalMaterial;
                _spriteRenderer.color = Color.white;
            }
        }
    }
}
