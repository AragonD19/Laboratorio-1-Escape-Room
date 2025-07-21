using UnityEngine;

public class SequenceButton : Interactable
{
    public int buttonIndex; 
    public ButtonSequenceController controller;

    [Header("Feedback de movimiento")]
    [SerializeField] private float pressDepth = 0.1f;
    [SerializeField] private float pressDuration = 0.1f;

    private Vector3 initialPosition;
    private bool isPressed = false;

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    public override void Interact()
    {
        if (isPressed) return;
        isPressed = true;

        controller.PressButton(buttonIndex);
        StartCoroutine(PressAnimation());
    }

    private System.Collections.IEnumerator PressAnimation()
    {
        Vector3 downPosition = initialPosition + Vector3.down * pressDepth;

        
        float elapsed = 0f;
        while (elapsed < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(initialPosition, downPosition, elapsed / pressDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = downPosition;


        elapsed = 0f;
        while (elapsed < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(downPosition, initialPosition, elapsed / pressDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = initialPosition;

        isPressed = false;
    }
}
