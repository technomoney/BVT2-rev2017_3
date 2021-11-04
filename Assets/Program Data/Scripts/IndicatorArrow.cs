using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class IndicatorArrow : SerializedMonoBehaviour
{
    public float m_moveDistance, m_moveSpeed;
    public RectTransform m_rectTransform;

    private void Start()
    {
        StartCoroutine(Bounce());
    }

    private IEnumerator Bounce()
    {
        var initialPosition = m_rectTransform.anchoredPosition;
        var upTarget = initialPosition;
        var downTarget = new Vector2(initialPosition.x, initialPosition.y - m_moveDistance);
        var targetPosition = downTarget;
        var movingDown = true;
        float i = 0;

        while (true)
        {
            m_rectTransform.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, i);

            i += Time.deltaTime * m_moveSpeed;

            if (i >= 1)
            {
                movingDown = !movingDown;
                i = 0;

                m_rectTransform.anchoredPosition = targetPosition;
                initialPosition = targetPosition;

                targetPosition = movingDown ? downTarget : upTarget;
            }

            yield return null;
        }
    }
}