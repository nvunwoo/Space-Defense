using UnityEngine;

public class TurretSelectButton : MonoBehaviour
{
    [Header("이 버튼이 선택할 터렛 타입")]
    [SerializeField] private TurretType turretType = TurretType.A;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Hand"))
            return; // 컨트롤러(Hand 태그)만 반응

        if (TurretBuildManager.Instance == null)
        {
            Debug.LogWarning("TurretBuildManager 인스턴스가 없습니다.");
            return;
        }

        TurretBuildManager.Instance.SelectTurret(turretType);

        // 여기에 버튼 눌릴 때 효과(사운드, 애니메이션 등) 추가하면 됨
    }
}
