using UnityEngine;
using UnityEngine.SceneManagement;

public class Hotkeys : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
            //set the screen resolution to full screen 1920x1080
            Screen.SetResolution(1920, 1080, true);
    }
}