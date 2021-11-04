﻿using System.Collections.Generic;
using UnityEngine;

namespace TRS.CaptureTool
{
    public class ScreenshotDisplayScript : MonoBehaviour
    {
        const int STACK_SIZE = 3;
        const float MAX_ROTATION = 20f;
        const float MAX_X_POSITION_DIFF = 5f;
        const float MAX_Y_POSITION_DIFF = 5f;

        public GameObject framedScreenshotPrefab;

        Queue<GameObject> framedScreenshots = new Queue<GameObject>();

        void OnEnable()
        {
            ScreenshotScript.ScreenshotTaken += AddNewScreenshot;
        }

        void OnDisable()
        {
            ScreenshotScript.ScreenshotTaken -= AddNewScreenshot;
        }

        void AddNewScreenshot(Texture2D texture)
        {
            if (framedScreenshots.Count >= STACK_SIZE)
                Destroy(framedScreenshots.Dequeue());

            GameObject framedScreenshot = Instantiate(framedScreenshotPrefab, transform);
            RectTransform frameScreenshotRectTransform = (RectTransform)framedScreenshot.transform;
            frameScreenshotRectTransform.localScale = Vector3.one;

            float xPosition = Random.Range(-MAX_X_POSITION_DIFF, MAX_X_POSITION_DIFF);
            float yPosition = Random.Range(-MAX_Y_POSITION_DIFF, MAX_Y_POSITION_DIFF);
            frameScreenshotRectTransform.anchoredPosition3D = new Vector3(xPosition, yPosition, 0);

            float rotation = Random.Range(-MAX_ROTATION, MAX_ROTATION);
            framedScreenshot.transform.Rotate(new Vector3(0, 0, rotation));

            framedScreenshot.GetComponent<FramedScreenshotScript>().SetTexture(texture);

            Vector2 frameSize = frameScreenshotRectTransform.sizeDelta;
            Vector2 maxSize = ((RectTransform)transform).rect.size;
            float newScale = Mathf.Min(maxSize.x / frameSize.x, maxSize.y / frameSize.y);
            frameScreenshotRectTransform.sizeDelta *= newScale;


            framedScreenshots.Enqueue(framedScreenshot);
        }
    }
}