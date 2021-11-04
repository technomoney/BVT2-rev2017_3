using UnityEngine;
using UnityEngine.EventSystems;

public class Testing_TargetTriggering : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void PointerDown(BaseEventData d)
    {
        var hits =
            Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        Debug.Log("Down");
        if (hits.Length > 0)
            foreach (var h in hits)
            {
                if (!h.collider.gameObject.name.Equals("Ring"))
                    continue;
                //this is a valid ehb hit, so manually invoke the target hit event of the target we just hit
                Debug.Log("In Ring");
                break;
            }
    }
}