using UnityEngine;

public class LeverAction : Interactable
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject laverUP;
    [SerializeField] private GameObject laverDOWN;
    [SerializeField] private Generator generator;

    private bool isUsed = false;

    public override void Interact()
    {
        if (isUsed) return;

        if (generator == null || !generator.HasBattery())
        {
            MessageDisplay.Instance.ShowMessage("Esta palanca no tiene energía.");
            return;
        }

        if (door != null)
        {
            door.SetActive(false);
            laverUP.SetActive(false);
            laverDOWN.SetActive(true);
            MessageDisplay.Instance.ShowMessage("Palanca accionada: " + door.name + " desactivado.");
        }

        isUsed = true;
    }
}
