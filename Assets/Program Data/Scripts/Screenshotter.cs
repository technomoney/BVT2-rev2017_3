using System.Collections;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;

public class Screenshotter : SerializedMonoBehaviour
{
    //public Canvas m_canvas;

    private string imagePath;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
            StartCoroutine(TakeScreenShot());
    }

    private IEnumerator TakeScreenShot()
    {
        Debug.Log("Starting Screenshot");

        // We should only read the screen buffer after rendering is complete
        yield return new WaitForEndOfFrame();

        var width = Screen.width;
        var height = Screen.height;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        var bytes = tex.EncodeToPNG();
        Destroy(tex);

        // For testing purposes, also write to a file in the project folder
        imagePath = Application.dataPath + "/SavedScreen.png";
        File.WriteAllBytes(imagePath, bytes);

        Debug.Log("Screenshot saved to: " + imagePath);

        yield return new WaitForSeconds(1);

        StartCoroutine(CropScreenShot());
    }

    private IEnumerator CropScreenShot()
    {
        var request = new WWW("file:///" + imagePath);
        yield return request;

        int imageX = 500, imageY = 500;

        var tex = request.texture;
        var cropped = new Texture2D(imageX, imageY);

        var colors = tex.GetPixels(
            Screen.width / 2 - imageX / 2, Screen.height / 2 - imageY / 2, imageX, imageY);
        cropped.SetPixels(colors);

        var bytes = cropped.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/croppedImage.png", bytes);

        Debug.Log("Finished Cropping");
    }
}