using UnityEngine;
using UnityEngine.UI;

public class DamageUI : MonoBehaviour
{
    public Image flashImage;
    public Color flashColor = new Color(1f, 0f, 0f, 0.4f);
    public float flashDuration = 0.5f;

    private float timer;
    private bool flashing = false;

    private void Awake()
    {
        if (flashImage != null)
            flashImage.color = Color.clear;
    }

    public void Flash()
    {
        timer = 0f;
        flashing = true;
        flashImage.color = flashColor;
    }

    private void Update()
    {
        if (!flashing) return;

        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(flashColor.a, 0f, timer / flashDuration);

        flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);

        if (timer >= flashDuration)
        {
            flashing = false;
            flashImage.color = Color.clear;
        }
    }
}
