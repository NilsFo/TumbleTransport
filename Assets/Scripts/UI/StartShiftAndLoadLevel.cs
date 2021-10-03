using UnityEngine;
using UnityEngine.SceneManagement;

public class StartShiftAndLoadLevel : MonoBehaviour
{

    public int shift = 0;
    
    public void LoadGame()
    {
        GameState.shift = shift;
        SceneManager.LoadScene("TumbleTransport");
    }
}
