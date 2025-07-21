using System.Collections.Generic;
using UnityEngine;

public class ButtonSequenceController : MonoBehaviour
{
    [Tooltip("Secuencia correcta de botones")]
    public List<int> correctSequence;
    private List<int> currentSequence = new List<int>();

    [Header("Feedback Visual")]
    [SerializeField] private SpriteRenderer feedbackSprite;
    private Color colorNeutral = Color.white;
    private Color colorSuccess = Color.green;
    private Color colorFailure = Color.red;

    [SerializeField] private GameObject objectToActivate;

    public void PressButton(int index)
    {
        currentSequence.Add(index);


        if (currentSequence.Count < correctSequence.Count) return;


        if (IsSequenceCorrect())
        {
            MessageDisplay.Instance.ShowMessage("¡Secuencia correcta!");
            objectToActivate.SetActive(false);
            SetFeedbackColor(colorSuccess);
        }
        else
        {
            MessageDisplay.Instance.ShowMessage("Secuencia incorrecta.");
            SetFeedbackColor(colorFailure);
        }


        currentSequence.Clear();
        Invoke(nameof(ResetSequence), 1f);
    }

    private bool IsSequenceCorrect()
    {
        if (currentSequence.Count != correctSequence.Count) return false;

        for (int i = 0; i < correctSequence.Count; i++)
        {
            if (currentSequence[i] != correctSequence[i])
                return false;
        }
        return true;
    }

    private void ResetSequence()
    {
        currentSequence.Clear();
        SetFeedbackColor(colorNeutral);
    }

    private void SetFeedbackColor(Color color)
    {
        if (feedbackSprite != null)
        {
            feedbackSprite.color = color;
        }
    }
}
