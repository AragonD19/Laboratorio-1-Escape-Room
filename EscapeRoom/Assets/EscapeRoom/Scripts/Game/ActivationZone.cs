using UnityEngine;

public class ActivationZone : MonoBehaviour
{
    public GameObject objectToActivate;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.ActivateObject(objectToActivate);
        }
    }
}
