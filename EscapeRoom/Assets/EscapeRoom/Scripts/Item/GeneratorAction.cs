using UnityEngine;

public class GeneratorAction : Interactable
{
    [SerializeField] private GameObject Battery;
    [SerializeField] private Generator generator;

    public override void Interact()
    {
        if (GameManager.Instance.HasBattery && generator.PlaceBattery())
        {
            GameManager.Instance.UseBattery();
            Battery.SetActive(true);
            MessageDisplay.Instance.ShowMessage("Batería colocada en el generador.");
        }
    }
}
