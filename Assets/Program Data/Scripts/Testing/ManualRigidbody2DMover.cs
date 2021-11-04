using UnityEngine;

public class ManualRigidbody2DMover : MonoBehaviour
{
    private Rigidbody2D rb2d;

    // Use this for initialization
    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            rb2d.transform.Translate(0, 10, 0);
    }
}