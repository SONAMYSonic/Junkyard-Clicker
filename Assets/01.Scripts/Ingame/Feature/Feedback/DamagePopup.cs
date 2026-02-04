using UnityEngine;
using TMPro;

namespace JunkyardClicker.Feedback
{
    /// <summary>
    /// 데미지 팝업 UI
    /// </summary>
    public class DamagePopup : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _damageText;

        [SerializeField]
        private float _lifetime = 1f;

        [SerializeField]
        private float _floatSpeed = 1f;

        private float _timer;
        private Color _originalColor;

        public void Initialize(int damage)
        {
            if (_damageText != null)
            {
                _damageText.text = damage.ToString();
                _originalColor = _damageText.color;
            }

            _timer = _lifetime;
        }

        private void Update()
        {
            // 위로 이동
            transform.position += Vector3.up * _floatSpeed * Time.deltaTime;

            // 페이드 아웃
            _timer -= Time.deltaTime;
            float alpha = _timer / _lifetime;

            if (_damageText != null)
            {
                Color color = _originalColor;
                color.a = alpha;
                _damageText.color = color;
            }

            // 수명 종료
            if (_timer <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
