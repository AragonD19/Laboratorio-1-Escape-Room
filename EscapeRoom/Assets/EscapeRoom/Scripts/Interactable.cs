using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string promptMessage = "Presiona E para interactuar";

    
    public abstract void Interact();
}
