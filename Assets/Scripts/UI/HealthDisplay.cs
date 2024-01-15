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
        string text = $"HP: {Mathf.RoundToInt(health)}";
        UpdateDisplayText(text);
    }
}
