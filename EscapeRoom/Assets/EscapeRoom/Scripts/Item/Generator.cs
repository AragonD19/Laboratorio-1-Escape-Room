using UnityEngine;

public class Generator : MonoBehaviour
{
    private bool hasBattery = false;

    public bool HasBattery()
    {
        return hasBattery;
    }

    public bool PlaceBattery()
    {
        if (hasBattery) return false;

        hasBattery = true;
        return true;
    }
}
