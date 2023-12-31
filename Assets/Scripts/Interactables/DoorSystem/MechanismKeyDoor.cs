using Enums;
using UnityEngine;

public class MechanismKeyDoor : DoorBase,IInteractable
{
    [SerializeField] private Animator keyAnimator;
    public void OnInteraction()
    {
        if (!doorOpen)
        {
            // Check if player has a key here!
            KeyCheck();
            return;
        }
    }

    public void KeyCheck()
    {
        // If player has a key, open the door.
        if (PlayerStatsManager.Instance.GetCurrentKeys() > 0)
        {
            keyAnimator.Play("Unlock");
            PlayerStatsManager.Instance.UseKey(KeyType.none);
            ToggleMechanism(true);
        }
    }
}
