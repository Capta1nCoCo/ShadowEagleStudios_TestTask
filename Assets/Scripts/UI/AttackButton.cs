using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttackButton : ButtonBase
{
    public enum AttackType
    {
        None,
        Normal,
        Super
    }

    [SerializeField] private AttackType type;
    [SerializeField] private TextMeshProUGUI displayText;

    private string buttonText;
    private float skillCooldown;

    private delegate void AttackHandler();
    private AttackHandler attackHandler;

    private ISuperAttacker _player;
    private Image _buttonBackground;

    protected override void Awake()
    {
        base.Awake();
        _buttonBackground = GetComponent<Image>();
        GameEvents.OnCooldown += UpdateCooldownDisplay;
        GameEvents.OnPlayerInit += InitByAttackType;
    }

    private void OnDestroy()
    {
        GameEvents.OnCooldown -= UpdateCooldownDisplay;
        GameEvents.OnPlayerInit -= InitByAttackType;
    }

    private void Start()
    {
        buttonText = displayText.text;
    }

    private void UpdateCooldownDisplay()
    {
        float time = Time.time;
        float cooldown = type == AttackType.Normal ? time - _player.LastAttackTime : time - _player.LastSuperTime;
        bool isOnCooldown = skillCooldown >= cooldown;
        displayText.text = isOnCooldown ? cooldown.ToString() : buttonText;
        ChangeButtonTransparency(isOnCooldown);
    }

    private void ChangeButtonTransparency(bool isOnCooldown)
    {
        Color cdColor = _buttonBackground.color;
        cdColor.a = isOnCooldown ? 0.3f : 1f;
        _buttonBackground.color = cdColor;
    }

    protected override void Execute()
    {
        attackHandler();
    }

    private void InitByAttackType()
    {
        if (Player.Instance is ISuperAttacker)
        {
            _player = Player.Instance;
            switch (type)
            {
                case AttackType.Normal:
                    attackHandler = _player.Attack;
                    skillCooldown = _player.AttackSpeed;
                    break;

                case AttackType.Super:
                    attackHandler = _player.SuperAttack;
                    skillCooldown = _player.SuperAttackSpeed;
                    break;

                default:
                    Debug.Log("Assign an attack type!");
                    return;
            }
        }
    }
}