using UnityEngine;
using TMPro;

public class FortressCurrentHPDisplay : MonoBehaviour
{
    [Header("체력을 참조할 바리케이드")]
    [SerializeField] private BarricadeHealth barricadeHealth;

    [Header("표시할 TMP 텍스트")]
    [SerializeField] private TextMeshProUGUI currentHpText;

    [Header("표시 형식")]
    [SerializeField] private string format = "FORTRESS CURRENT HP: {0}";

    private void Awake()
    {
        if (currentHpText == null)
        {
            currentHpText = GetComponent<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        if (barricadeHealth == null || currentHpText == null)
            return;

        int currentHpInt = Mathf.RoundToInt(barricadeHealth.currentHP);

        // 4자리, 빈 자리는 0
        string hpString = currentHpInt.ToString("0000");

        currentHpText.text = string.Format(format, hpString);
    }
}
