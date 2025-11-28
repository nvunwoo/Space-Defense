using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefabOnTouch : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("생성할 프리팹 (에디터에서 할당)")]
    public GameObject prefabToSpawn;

    [Tooltip("스폰 위치 기준(없으면 이 오브젝트를 기준으로 함)")]
    public Transform spawnPoint;

    [Tooltip("버튼 기준 로컬 오프셋 (위쪽으로 얼마나 띄울지 등)")]
    public Vector3 localOffset = new Vector3(0f, 0.2f, 0f);

    [Header("Detection Settings")]
    [Tooltip("이 태그를 가진 오브젝트가 닿으면 스폰 (예: Controller)")]
    public string controllerTag = "Controller";

    private bool hasSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        // 이미 한 번 생성했다면 무시
        if (hasSpawned) return;

        // 컨트롤러 태그가 아니면 무시
        if (!other.CompareTag(controllerTag)) return;

        // 프리팹이 비어있으면 경고 후 종료
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("[SpawnPrefabOnTouch] prefabToSpawn이 설정되지 않았습니다.");
            return;
        }

        // 실제 스폰 위치 & 회전 계산
        Vector3 spawnPos;
        Quaternion spawnRot;

        if (spawnPoint != null)
        {
            // 별도의 스폰 포인트를 지정한 경우
            spawnPos = spawnPoint.position;
            spawnRot = spawnPoint.rotation;
        }
        else
        {
            // 버튼 위치 + 로컬 오프셋 기준
            spawnPos = transform.TransformPoint(localOffset);
            spawnRot = transform.rotation;
        }

        // 프리팹 생성
        Instantiate(prefabToSpawn, spawnPos, spawnRot);

        // 한 번만 생성되도록 플래그 설정
        hasSpawned = true;
    }

    // 필요하면 외부에서 다시 호출해서 리셋 가능
    public void ResetSpawn()
    {
        hasSpawned = false;
    }
}
