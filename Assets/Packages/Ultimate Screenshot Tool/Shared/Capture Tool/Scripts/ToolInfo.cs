namespace TRS.CaptureTool
{
    public static class ToolInfo
    {
        public static bool isScreenshotTool;
        public static bool isGifTool;

        static ToolInfo()
        {
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (System.Reflection.Assembly assembly in assemblies)
            {
                if (assembly.GetName().Name != "Assembly-CSharp")
                    continue;

                System.Type[] types = assembly.GetTypes();
                foreach (System.Type type in types)
                {
                    if (type.ToString().Contains("TRS.CaptureTool.ScreenshotScript"))
                        isScreenshotTool = true;
                    if (type.ToString().Contains("TRS.CaptureTool.GifScript"))
                        isGifTool = true;
                }
            }

        }
    }
}