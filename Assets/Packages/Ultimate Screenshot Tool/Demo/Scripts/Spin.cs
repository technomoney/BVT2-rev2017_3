using UnityEngine;

namespace TRS.CaptureTool
{
    public class Spin : MonoBehaviour
    {
        public Vector3 eulers = Vector3.up;
        public float speed = 10f;

        void Update()
        {
            transform.Rotate(eulers, speed * Time.deltaTime);
        }
    }
}