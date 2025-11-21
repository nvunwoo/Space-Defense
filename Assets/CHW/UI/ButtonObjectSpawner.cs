using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonObjectSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("버튼 위에 생성할 임시 오브젝트 프리팹")]
    public GameObject prefabToSpawn;

    [Tooltip("버튼에서 얼마나 위로 띄워서 생성할지 (단위: 미터)")]
    public float heightOffset = 0.1f;

    [Tooltip("이전에 생성된 오브젝트를 지우고 새로 생성할지 여부")]
    public bool destroyPrevious = true;

    // 마지막으로 생성된 오브젝트 참조
    private GameObject lastSpawnedObject;

    // UI Button의 OnClick에 연결해서 호출할 함수
    public void SpawnAboveButton()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("[ButtonObjectSpawner] prefabToSpawn이 설정되지 않았습니다.");
            return;
        }

        // 이전 생성물 제거 옵션
        if (destroyPrevious && lastSpawnedObject != null)
        {
            Destroy(lastSpawnedObject);
        }

        // 버튼의 월드 위치 기준으로 위쪽 방향으로 heightOffset만큼 올린 위치 계산
        Transform buttonTransform = transform;
        Vector3 spawnPosition = buttonTransform.position + buttonTransform.up * heightOffset;
        Quaternion spawnRotation = buttonTransform.rotation;

        // 프리팹 생성
        lastSpawnedObject = Instantiate(prefabToSpawn, spawnPosition, spawnRotation);
    }
}
