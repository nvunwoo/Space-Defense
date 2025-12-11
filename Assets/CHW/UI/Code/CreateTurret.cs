using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CreateTurret : MonoBehaviour
{
    [Header("생성할 프리팹 설정")]
    [SerializeField] private GameObject turretPrefab;

    [Header("스폰 위치 설정")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 0.2f, 0f);

    [Header("Hand 인식용 태그")]
    [SerializeField] private string handTag = "Hand";

    [Header("생성 횟수 설정")]
    [SerializeField] private bool spawnOnlyOnce = true;
    private bool hasSpawned = false;

    private Collider _col;

    private void Reset()
    {
        // 이 스크립트를 붙이는 순간 자동으로 콜라이더를 보장
        _col = GetComponent<Collider>();
        if (_col == null)
        {
            _col = gameObject.AddComponent<BoxCollider>();
            _col.isTrigger = true;
            Debug.Log($"[CreateTurret] Reset()에서 BoxCollider 자동 추가 + isTrigger = true 로 설정함. ({name})", this);
        }
        else
        {
            _col.isTrigger = true;
            Debug.Log($"[CreateTurret] Reset()에서 기존 Collider isTrigger = true 로 설정함. ({name})", this);
        }
    }

    private void Awake()
    {
        _col = GetComponent<Collider>();

        Debug.Log(
            $"[CreateTurret] Awake on '{name}'. " +
            $"활성상태(ActiveInHierarchy)={gameObject.activeInHierarchy}, " +
            $"enabled={enabled}, " +
            $"Collider존재={_col != null}",
            this
        );

        if (_col != null)
        {
            Debug.Log(
                $"[CreateTurret] Collider 정보 -> isTrigger={_col.isTrigger}, layer={gameObject.layer}, tag={tag}",
                this
            );
        }
        else
        {
            Debug.LogError(
                $"[CreateTurret] Collider가 없습니다. OnTriggerEnter는 절대 호출되지 않습니다. ({name})",
                this
            );
        }

        if (turretPrefab == null)
        {
            Debug.LogWarning("[CreateTurret] turretPrefab이 비어 있습니다.", this);
        }
    }

    private void OnEnable()
    {
        Debug.Log($"[CreateTurret] OnEnable 호출됨. ({name})", this);
    }

    private void Start()
    {
        Debug.Log($"[CreateTurret] Start 호출됨. ({name})", this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(
            $"[CreateTurret] OnTriggerEnter on '{name}' with '{other.name}' (tag='{other.tag}', layer={other.gameObject.layer})",
            this
        );

        if (!other.CompareTag(handTag))
        {
            Debug.Log(
                $"[CreateTurret] '{other.name}' 태그 '{other.tag}' != handTag('{handTag}') → 무시",
                this
            );
            return;
        }

        if (spawnOnlyOnce && hasSpawned)
        {
            Debug.Log("[CreateTurret] 이미 한 번 생성했으므로 무시.", this);
            return;
        }

        if (turretPrefab == null)
        {
            Debug.LogWarning("[CreateTurret] turretPrefab이 비어 있어 생성 불가.", this);
            return;
        }

        SpawnTurret();
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(
            $"[CreateTurret] OnTriggerStay on '{name}' with '{other.name}' (tag='{other.tag}')",
            this
        );
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(
            $"[CreateTurret] OnTriggerExit on '{name}' with '{other.name}' (tag='{other.tag}')",
            this
        );
    }

    private void SpawnTurret()
    {
        Vector3 pos;
        Quaternion rot;

        if (spawnPoint != null)
        {
            pos = spawnPoint.position;
            rot = spawnPoint.rotation;
        }
        else
        {
            pos = transform.position + spawnOffset;
            rot = Quaternion.identity;
        }

        GameObject spawned = Instantiate(turretPrefab, pos, rot);
        hasSpawned = true;

        Debug.Log(
            $"[CreateTurret] Turret '{turretPrefab?.name}' 생성됨 at {pos}. 생성된 오브젝트: '{spawned.name}'",
            this
        );
    }
}
