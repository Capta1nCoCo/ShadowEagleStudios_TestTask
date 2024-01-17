using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public abstract class DisplayTextUI : MonoBehaviour
{
    private TextMeshProUGUI displayText;

    protected virtual void Awake()
    {
        displayText = GetComponent<TextMeshProUGUI>();
    }

    protected virtual void UpdateDisplayText(string text)
    {
        displayText.text = text;
    }
}
