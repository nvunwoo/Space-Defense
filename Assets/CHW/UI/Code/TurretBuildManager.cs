using UnityEngine;

public class TurretBuildManager : MonoBehaviour
{
    public static TurretBuildManager Instance { get; private set; }

    [Header("터렛 프리팹들")]
    [SerializeField] private GameObject turretAPrefab;
    [SerializeField] private GameObject turretBPrefab;
    [SerializeField] private GameObject turretCPrefab;

    // 현재 선택된 터렛 타입 (버튼 A/B/C 누르면 세팅)
    private TurretType selectedTurretType = TurretType.None;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // 필요하면 DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 터렛 선택 버튼에서 호출
    /// </summary>
    public void SelectTurret(TurretType type)
    {
        selectedTurretType = type;
        // 나중에 UI 하이라이트 같은 거 여기서 처리하면 됨
        // Debug.Log($"Turret Selected: {selectedTurretType}");
    }

    /// <summary>
    /// 스폰 버튼에서 호출
    /// </summary>
    public void OnSpawnButtonPressed(TurretSpawnButton spawnButton)
    {
        // 1) 나중에 "업그레이드" 기능을 넣을 자리
        if (selectedTurretType == TurretType.None)
        {
            // 여기서 나중에:
            // if (spawnButton.HasTurret) spawnButton.UpgradeTurret();
            return; // 지금은 아무 것도 안 함
        }

        // 2) 이미 터렛이 있는 자리는 지금은 무시
        //    (나중에 여기서 업그레이드 기능 넣어도 됨)
        if (spawnButton.HasTurret)
        {
            // 나중에: spawnButton.UpgradeTurret();
            return;
        }

        // 3) 선택된 타입에 맞는 프리팹 가져오기
        GameObject prefab = GetPrefabByType(selectedTurretType);
        if (prefab == null)
        {
            Debug.LogWarning("선택된 타입에 해당하는 터렛 프리팹이 없습니다.");
            return;
        }

        // 4) 실제 터렛 생성
        Transform spawnPoint = spawnButton.SpawnPoint;
        GameObject turretInstance = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        // 스폰 버튼에 "현재 터렛" 등록
        spawnButton.RegisterTurret(turretInstance);

        // 5) 선택 초기화 (원하면 유지해도 됨)
        selectedTurretType = TurretType.None;
    }

    private GameObject GetPrefabByType(TurretType type)
    {
        switch (type)
        {
            case TurretType.A:
                return turretAPrefab;
            case TurretType.B:
                return turretBPrefab;
            case TurretType.C:
                return turretCPrefab;
            default:
                return null;
        }
    }
}
