using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool HasBattery { get; private set; } = false;

    [SerializeField] private GameObject hasBatteryImage;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void WinGame()
    {
        StartCoroutine(WinSequence());
    }

    private IEnumerator WinSequence()
    {
        MessageDisplay.Instance.ShowMessage("¡Has ganado el juego!");
        yield return new WaitForSeconds(3f);

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void ActivateObject(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(true);
            Debug.Log("Objeto activado: " + obj.name);
        }
    }

    public void PickUpBattery()
    {
        HasBattery = true;
        hasBatteryImage.SetActive(true);
        Debug.Log("Batería recogida.");
    }

    public void UseBattery()
    {
        HasBattery = false;
        hasBatteryImage.SetActive(false);
        Debug.Log("Batería usada.");
    }
}
