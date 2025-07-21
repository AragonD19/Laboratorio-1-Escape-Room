using UnityEngine;

public class BatteryPickup : Interactable
{
    public override void Interact()
    {
        MessageDisplay.Instance.ShowMessage("Batería recogida");
        
        GameManager.Instance.PickUpBattery();
        Destroy(gameObject);
    }
}
