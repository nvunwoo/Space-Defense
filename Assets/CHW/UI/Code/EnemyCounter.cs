using UnityEngine;
using TMPro;

public class EnemyCounter : MonoBehaviour
{
    [Header("표시할 TMP 텍스트")]
    [SerializeField] private TextMeshProUGUI enemyCountText;

    [Header("표시 형식")]
    [SerializeField] private string format = "ENEMIES LEFT: {0}";

    [Header("갱신 간격 (초)")]
    [SerializeField] private float updateInterval = 1f;

    private float timer = 0f;

    private void Start()
    {
        // 같은 오브젝트에 TMP 텍스트가 붙어 있을 경우 자동으로 가져오기
        if (enemyCountText == null)
        {
            enemyCountText = GetComponent<TextMeshProUGUI>();
        }

        UpdateEnemyCount();
    }

    private void Update()
    {
        // 실시간(매 프레임) 갱신
        if (updateInterval <= 0f)
        {
            UpdateEnemyCount();
            return;
        }

        // 지정한 간격마다 갱신
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            UpdateEnemyCount();
        }
    }

    private void UpdateEnemyCount()
    {
        int enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemyCountText != null)
        {
            enemyCountText.text = string.Format(format, enemyCount);
        }
    }
}
