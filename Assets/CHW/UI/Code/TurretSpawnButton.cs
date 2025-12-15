using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TurretSpawnButton : MonoBehaviour
{
    [Header("터렛 생성 위치 (비워두면 자기 Transform 사용)")]
    [SerializeField] private Transform spawnPoint;

    private GameObject currentTurret;

    // 외부에서 읽기용
    public Transform SpawnPoint => spawnPoint != null ? spawnPoint : transform;
    public bool HasTurret => currentTurret != null;

    private void Reset()
    {
        // 자동으로 트리거 설정
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Hand 태그 가진 컨트롤러만 버튼으로 인정
        if (!other.CompareTag("Hand"))
            return;

        // 안전하게 SpawnButton 태그인지 확인 (실수 방지용)
        if (!CompareTag("SpawnButton"))
        {
            Debug.LogWarning($"SpawnButton 스크립트가 붙었는데 태그가 SpawnButton이 아닙니다. ({name})");
            return;
        }

        if (TurretBuildManager.Instance == null)
        {
            Debug.LogWarning("TurretBuildManager 인스턴스가 없습니다.");
            return;
        }

        TurretBuildManager.Instance.OnSpawnButtonPressed(this);
    }

    /// <summary>
    /// 새로 생성된 터렛을 이 버튼에 연결
    /// </summary>
    public void RegisterTurret(GameObject turret)
    {
        currentTurret = turret;
    }

    /// <summary>
    /// 나중에 업그레이드 로직 넣을 자리
    /// </summary>
    public void UpgradeTurret()
    {
        if (currentTurret == null)
            return;

        // TODO: 나중에 포탑 업그레이드 기능 구현
    }
}
