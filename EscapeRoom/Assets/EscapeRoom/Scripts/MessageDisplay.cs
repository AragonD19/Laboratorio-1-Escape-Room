using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MessageDisplay : MonoBehaviour
{
    public static MessageDisplay Instance;

    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float displayTime = 3f;

    private Coroutine currentMessage;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        messageText.gameObject.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        if (currentMessage != null)
            StopCoroutine(currentMessage);

        currentMessage = StartCoroutine(DisplayMessage(message));
    }

    private IEnumerator DisplayMessage(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        messageText.gameObject.SetActive(false);
    }
}
