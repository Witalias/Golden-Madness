using UnityEngine;

public class Minecart : MonoBehaviour
{
    private const string leaveAnimationName = "Leave";

    [SerializeField] private bool canBeUsed = true;
    [SerializeField] private GameObject minecart;

    private Animation minecartAnimation;
    private TriggerZone trigger;
    private PressActionKey pressActionKey;

    public bool CanBeUsed
    {
        get => canBeUsed;
        set
        {
            canBeUsed = value;
            pressActionKey.SetActive(value);

            minecartAnimation.Play(leaveAnimationName);
            if (value)
                minecartAnimation.Stop(leaveAnimationName);
        }
    }

    private void Awake()
    {
        minecartAnimation = minecart.GetComponent<Animation>();
        trigger = GetComponent<TriggerZone>();
        pressActionKey = GetComponent<PressActionKey>();
    }

    private void Update()
    {
        if (trigger.IsTriggered && Input.GetKeyDown(KeyCode.E) && canBeUsed)
        {
            var backpack = Player.instanse.GetComponent<Backpack>();
            ResourcesSaver.AddInVillage(backpack.GetAll());
            backpack.Clear();

            // ��� ���������� ������
            ServiceInfo.CheckpointConditionDone = true;

            CanBeUsed = false;
        }
    }
}
