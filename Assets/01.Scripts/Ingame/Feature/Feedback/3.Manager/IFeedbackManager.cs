using UnityEngine;

namespace JunkyardClicker.Feedback
{
    /// <summary>
    /// 피드백 매니저 인터페이스
    /// </summary>
    public interface IFeedbackManager
    {
        void PlayFeedback(FeedbackType type, int value = 0);
        void SpawnDamagePopup(int damage, Vector3 position);
    }
}
