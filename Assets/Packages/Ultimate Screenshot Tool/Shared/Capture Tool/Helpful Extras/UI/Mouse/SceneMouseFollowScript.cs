using UnityEngine;

namespace TRS.CaptureTool.Extras
{
    public class SceneMouseFollowScript : MouseFollowScript
    {
        void Update()
        {
            Vector3 screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + transform.lossyScale.z);
            Vector3 position = Camera.main.ScreenToWorldPoint(screenPoint);
            transform.position = position;
        }
    }
}