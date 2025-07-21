using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [SerializeField] private Camera playerCamera;
    public TextMeshProUGUI promptTextUI;

    private void Update()
    {
        promptTextUI.text = "";

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableMask))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                promptTextUI.text = interactable.promptMessage;

                if (Input.GetKeyDown(interactKey))
                {
                    interactable.Interact();
                }
            }
        }
    }
}
