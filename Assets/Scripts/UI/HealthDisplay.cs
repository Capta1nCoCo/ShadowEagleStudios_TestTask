using UnityEngine;

public class HealthDisplay : DisplayTextUI
{
    protected override void Awake()
    {
        base.Awake();
        GameEvents.OnHealthChanged += UpdateHealthDisplay;
    }

    private void OnDestroy()
    {
        GameEvents.OnHealthChanged -= UpdateHealthDisplay;
    }

    private void UpdateHealthDisplay(float health)
    {
        int roundedHealth = Mathf.Sign(health) == -1 ? 0 : Mathf.RoundToInt(health);
        string text = $"HP: {roundedHealth}";
        UpdateDisplayText(text);
    }
}
