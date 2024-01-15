using TMPro;
using UnityEngine;

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

    protected override void Awake()
    {
        base.Awake();
        buttonText = displayText.text;
        GameEvents.OnCooldown += UpdateCooldownDisplay;
        GameEvents.OnPlayerInit += InitByAttackType;
    }

    private void OnDestroy()
    {
        GameEvents.OnCooldown -= UpdateCooldownDisplay;
        GameEvents.OnPlayerInit -= InitByAttackType;
    }

    private void UpdateCooldownDisplay()
    {
        float time = Time.time;
        float cooldown = type == AttackType.Normal ? time - _player.LastAttackTime : time - _player.LastSuperTime;
        displayText.text = skillCooldown >= cooldown ? cooldown.ToString() : buttonText;
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