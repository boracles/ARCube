using UnityEngine;
using Vuforia;
using System.Collections.Generic;

public class MultiTargetManager : MonoBehaviour
{
    public List<GameObject> multiTargetPrefabs;  // Multi Target 프리팹 목록
    public GameObject waterPrefab;  // Water 큐브에 대한 오브젝트 프리팹
    public GameObject grassPrefab;  // Grass 큐브에 대한 오브젝트 프리팹
    public GameObject sandPrefab;   // Sand 큐브에 대한 오브젝트 프리팹
    public GameObject furPrefab;    // Fur 큐브에 대한 오브젝트 프리팹
    public GameObject glassPrefab;  // Glass 큐브에 대한 오브젝트 프리팹

    private Dictionary<string, GameObject> instantiatedObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, MultiTargetBehaviour> multiTargetBehaviours = new Dictionary<string, MultiTargetBehaviour>();

    void Start()
    {
        // 모든 프리팹이 할당되었는지 확인
        if (waterPrefab == null || grassPrefab == null || sandPrefab == null || furPrefab == null || glassPrefab == null)
        {
            Debug.LogError("프리팹 참조가 인스펙터에서 할당되지 않았습니다!");
            return;
        }

        foreach (GameObject prefab in multiTargetPrefabs)
        {
            MultiTargetBehaviour multiTarget = prefab.GetComponent<MultiTargetBehaviour>();
            if (multiTarget)
            {
                Debug.Log("멀티타겟 발견: " + multiTarget.TargetName);

                GameObject instance = null;
                if (multiTarget.TargetName == "WaterCube")
                {
                    instance = Instantiate(waterPrefab);
                }
                else if (multiTarget.TargetName == "GrassCube")
                {
                    instance = Instantiate(grassPrefab);
                }
                else if (multiTarget.TargetName == "SandCube")
                {
                    instance = Instantiate(sandPrefab);
                }
                else if (multiTarget.TargetName == "FurCube")
                {
                    instance = Instantiate(furPrefab);
                }
                else if (multiTarget.TargetName == "GlassCube")
                {
                    instance = Instantiate(glassPrefab);
                }

                if (instance != null)
                {
                    instance.transform.SetParent(multiTarget.transform, false);
                    instance.SetActive(false); // 처음에 비활성화

                    instantiatedObjects[multiTarget.TargetName] = instance;
                    multiTargetBehaviours[multiTarget.TargetName] = multiTarget;
                    Debug.Log("인스턴스 생성됨: " + multiTarget.TargetName);

                    // 상태 변경 이벤트 구독
                    multiTarget.OnTargetStatusChanged += OnTargetStatusChanged;
                }
                else
                {
                    Debug.LogError("인스턴스 생성 실패: " + multiTarget.TargetName);
                }
            }
            else
            {
                Debug.LogError("프리팹에서 MultiTarget을 찾을 수 없습니다: " + prefab.name);
            }
        }
    }

    void Update()
    {
        // 모든 멀티타겟의 트래킹 상태를 업데이트
        foreach (var kvp in multiTargetBehaviours)
        {
            var targetName = kvp.Key;
            var behaviour = kvp.Value;
            var status = behaviour.TargetStatus.Status;

            if (instantiatedObjects.TryGetValue(targetName, out GameObject instance))
            {
                if (status == Status.TRACKED || status == Status.EXTENDED_TRACKED)
                {
                    if (!instance.activeSelf)
                    {
                        instance.SetActive(true);
                        Debug.Log("트래킹 중 (업데이트): " + targetName);
                    }
                }
                else
                {
                    if (instance.activeSelf)
                    {
                        instance.SetActive(false);
                        Debug.Log("트래킹 중 아님 (업데이트): " + targetName);
                    }
                }
            }
        }
    }

    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        Debug.Log($"OnTargetStatusChanged 호출됨 {behaviour.TargetName} 상태: {targetStatus.Status}");
        if (instantiatedObjects.TryGetValue(behaviour.TargetName, out GameObject instance))
        {
            Debug.Log($"타겟 {behaviour.TargetName} 상태 변경: {targetStatus.Status} - {targetStatus.StatusInfo}");
            if (targetStatus.Status == Status.TRACKED || targetStatus.Status == Status.EXTENDED_TRACKED)
            {
                if (!instance.activeSelf)
                {
                    instance.SetActive(true);
                    Debug.Log("트래킹 중 (이벤트): " + behaviour.TargetName);
                }
            }
            else
            {
                if (instance.activeSelf)
                {
                    instance.SetActive(false);
                    Debug.Log("트래킹 중 아님 (이벤트): " + behaviour.TargetName);
                }
            }
        }
        else
        {
            Debug.LogError("인스턴스를 찾을 수 없습니다: " + behaviour.TargetName);
        }
    }

    void OnDestroy()
    {
        foreach (var kvp in instantiatedObjects)
        {
            if (multiTargetBehaviours.TryGetValue(kvp.Key, out MultiTargetBehaviour multiTarget))
            {
                multiTarget.OnTargetStatusChanged -= OnTargetStatusChanged;
            }
        }
    }
}
