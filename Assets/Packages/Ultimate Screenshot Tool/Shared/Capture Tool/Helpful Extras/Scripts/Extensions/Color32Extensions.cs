using UnityEngine;

namespace TRS.CaptureTool.Extras
{
    public static class Color32Extensions
    {
        public static Color32[] AlphaBlend(this Color32[] background, Color32[] foreground, bool replace = false)
        {
            if (background.Length != foreground.Length)
                throw new System.InvalidOperationException("AlphaBlend only works with two equal sized images");

            Color32[] resultPixels = background;
            if (!replace)
                resultPixels = new Color32[background.Length];
            for (int i = 0; i < background.Length; i++)
                resultPixels[i] = background[i].AlphaBlend(foreground[i]);
            return resultPixels;
        }

        public static Color32 AlphaBlend(this Color32 backgroundColor, Color32 foregroundColor)
        {
            return (Color32)((Color)backgroundColor).AlphaBlend((Color)foregroundColor);
        }

        public static Color AlphaBlend(this Color backgroundColor, Color foregroundColor)
        {
            float sourceAlpha = foregroundColor.a;
            float destAlpha = 1f - foregroundColor.a;
            float resultAlpha = sourceAlpha + destAlpha * backgroundColor.a;
            Color resultColor = (foregroundColor * sourceAlpha + backgroundColor * backgroundColor.a * destAlpha) / resultAlpha;
            resultColor.a = resultAlpha;
            return resultColor;
        }
    }
}