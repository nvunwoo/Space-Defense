using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(XRGrabInteractable))]
public class SnappableToSpawnButton : MonoBehaviour
{
    [Header("고정 위치 설정 (SpawnButton 기준 로컬 오프셋)")]
    [SerializeField] private Vector3 snapOffset = new Vector3(0f, 0.2f, 0f);
    [SerializeField] private bool resetLocalRotationOnSnap = true;

    [Header("Spawn 버튼 태그")]
    [SerializeField] private string spawnButtonTag = "SpawnButton";

    private XRGrabInteractable grabInteractable;
    private Transform currentSpawnButton; // 지금 겹쳐 있는 SpawnButton

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // 이 오브젝트는 물리 오브젝트(그랩/충돌용)
        var col = GetComponent<Collider>();
        col.isTrigger = false; // SpawnButton 쪽 콜라이더를 Trigger로 사용

        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnSelectEntered);
            grabInteractable.selectExited.AddListener(OnSelectExited);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
            grabInteractable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    // 그랩 시작
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // 만약 이전에 SpawnButton에 붙어 있었으면 부모만 끊어준다.
        if (transform.parent != null && transform.parent.CompareTag(spawnButtonTag))
        {
            transform.SetParent(null, true);
        }

        // XRGrabInteractable가 알아서 손에 붙여주기 때문에
        // 여기에서 따로 SetParent(null) 같은 건 하면 안 됨.
    }

    // 그랩 끝(손에서 놓음)
    private void OnSelectExited(SelectExitEventArgs args)
    {
        // 놓는 순간, SpawnButton 위에 있다면 그 버튼에 스냅
        if (currentSpawnButton != null)
        {
            SnapToCurrentButton();
        }
    }

    // SpawnButton에 들어갔을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(spawnButtonTag))
        {
            currentSpawnButton = other.transform;
        }
    }

    // SpawnButton에서 나왔을 때
    private void OnTriggerExit(Collider other)
    {
        if (currentSpawnButton != null && other.transform == currentSpawnButton)
        {
            currentSpawnButton = null;
        }
    }

    private void SnapToCurrentButton()
    {
        // 버튼을 부모로 삼고, 로컬 위치를 오프셋으로 고정
        transform.SetParent(currentSpawnButton, false);
        transform.localPosition = snapOffset;

        if (resetLocalRotationOnSnap)
            transform.localRotation = Quaternion.identity;

        // 물리 속도 제거 (고정된 느낌 주기)
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // 필요하면 고정 상태에서는 키네마틱으로
        }

        Debug.Log("[SnappableToSpawnButton] " + currentSpawnButton.name + " 위에 스냅됨");
    }

    // 만약 스냅된 상태에서 다시 잡으면, OnSelectEntered에서 부모를 끊고
    // rb.isKinematic = false로 돌려줘야 한다면 이렇게 추가:

    public void UnsnapIfNeeded()
    {
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}
