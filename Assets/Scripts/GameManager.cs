using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public InputManager InputManager { get; private set; }

    private void Awake()
    {
        Instance = this;
        InputManager = new();
    }
}
