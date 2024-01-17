public class WaveDisplay : DisplayTextUI
{
    protected override void Awake()
    {
        base.Awake();
        GameEvents.OnWaveSpawned += UpdateWaveDisplay;
    }

    private void OnDestroy()
    {
        GameEvents.OnWaveSpawned -= UpdateWaveDisplay;
    }

    public void UpdateWaveDisplay(int current, int final)
    {
        string text = $"Wave {current} / {final}";
        UpdateDisplayText(text);
    }
}
