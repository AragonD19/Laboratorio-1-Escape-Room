using UnityEngine;

public class BatteryPickup : Interactable
{
    public override void Interact()
    {
        Debug.Log("Batería recogida");
        
        GameManager.Instance.PickUpBattery();
        Destroy(gameObject);
    }
}
