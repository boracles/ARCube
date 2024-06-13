using UnityEngine;
using Vuforia;

public class MultiTargetTracker : MonoBehaviour
{
    void Start()
    {
        VuforiaApplication.Instance.OnVuforiaStarted += OnVuforiaStarted;
    }

    void OnVuforiaStarted()
    {
        VuforiaBehaviour.Instance.SetMaximumSimultaneousTrackedImages(6); // 동시에 추적할 최대 이미지 타겟 수 설정
    }
}