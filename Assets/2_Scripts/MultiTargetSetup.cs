using UnityEngine;
using Vuforia;

public class MultiTargetSetup : MonoBehaviour
{
    public GameObject cubePrefab;  // 오브젝트 프리팹

    void Start()
    {
        // MultiTarget Behaviour를 통해 Vuforia 타겟에 접근
        MultiTargetBehaviour multiTarget = GetComponent<MultiTargetBehaviour>();
        if (multiTarget)
        {
            GameObject cube = Instantiate(cubePrefab, multiTarget.transform);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localRotation = Quaternion.identity;
        }
    }
}