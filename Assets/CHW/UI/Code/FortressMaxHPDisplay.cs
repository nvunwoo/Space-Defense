using UnityEngine;
using TMPro;

public class FortressMaxHPDisplay : MonoBehaviour
{
    [Header("체력을 참조할 바리케이드")]
    [SerializeField] private BarricadeHealth barricadeHealth;

    [Header("표시할 TMP 텍스트")]
    [SerializeField] private TextMeshProUGUI maxHpText;

    [Header("표시 형식")]
    [SerializeField] private string format = "FORTRESS MAX HP: {0}";

    private void Awake()
    {
        if (maxHpText == null)
        {
            maxHpText = GetComponent<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        if (barricadeHealth == null || maxHpText == null)
            return;

        int maxHpInt = Mathf.RoundToInt(barricadeHealth.maxHP);

        string hpString = maxHpInt.ToString("0000");

        maxHpText.text = string.Format(format, hpString);
    }
}
