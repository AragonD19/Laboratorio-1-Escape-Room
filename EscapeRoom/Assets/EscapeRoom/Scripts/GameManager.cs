using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool HasBattery { get; private set; } = false;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void WinGame()
    {
        Debug.Log("¡Has ganado el juego!");
        Time.timeScale = 0f;
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
        Debug.Log("Batería recogida.");
    }

    public void UseBattery()
    {
        HasBattery = false;
        Debug.Log("Batería usada.");
    }
}
