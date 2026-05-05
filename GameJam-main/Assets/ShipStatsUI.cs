using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipStatsUI : MonoBehaviour
{
    [SerializeField] private Image shipHealthImage;
    [SerializeField] private Image madnessRateImage;
    [SerializeField] private TextMeshProUGUI coinsAmount;

    private void Start()
    {
        shipHealthImage.fillAmount = 1;
        madnessRateImage.fillAmount = 0;
        coinsAmount.text = "0";

        ShipManager.Instance.OnHealthChanged.AddListener(ChangeHealth);
        ShipManager.Instance.OnMadnessChanged.AddListener(ChangeMadnessRate);
        ShipManager.Instance.OnCoinsAmountChanged.AddListener(ChangeCoinsAmount);
    }

    private void ChangeHealth()
    {
        shipHealthImage.fillAmount = ShipManager.Instance.CurrentHealth/ShipManager.Instance.MaxHealth;
    }

    private void ChangeMadnessRate()
    {
        madnessRateImage.fillAmount = ShipManager.Instance.CurrentMadness/ShipManager.Instance.MaxMadnessValue;
    }

    private void ChangeCoinsAmount()
    {
        coinsAmount.text = ShipManager.Instance.CurrentCoins.ToString();
    }
}
