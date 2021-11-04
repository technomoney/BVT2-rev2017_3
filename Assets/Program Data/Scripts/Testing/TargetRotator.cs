using System.Collections;
using UnityEngine;

public class TargetRotator : MonoBehaviour
{
    public Vector2 origin;
    public float speed, magnitude;
    public Target t1, t2;
    public float t1_angle, t2_angle, angle;

    // Use this for initialization
    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(Rotate());

        if (Input.GetKeyDown(KeyCode.D))
        {
            var newPos = t1.rectTransform.anchoredPosition;
            newPos.x = Mathf.Cos(angle) * magnitude;
            newPos.y = Mathf.Sin(angle) * magnitude;

            t1.rectTransform.anchoredPosition = newPos;
        }
    }

    private IEnumerator Rotate()
    {
        while (true)
        {
            t1_angle += Time.deltaTime * speed;
            t2_angle += Time.deltaTime * speed;

            var newPos = t1.rectTransform.anchoredPosition;
            newPos.x = Mathf.Cos(t1_angle) * magnitude;
            newPos.y = Mathf.Sin(t1_angle) * magnitude;

            t1.rectTransform.anchoredPosition = newPos;

            newPos = t2.rectTransform.anchoredPosition;
            newPos.x = Mathf.Cos(t2_angle) * magnitude;
            newPos.y = Mathf.Sin(t2_angle) * magnitude;

            t2.rectTransform.anchoredPosition = newPos;

            yield return null;
        }
    }
}