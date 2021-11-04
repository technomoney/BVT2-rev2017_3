using UnityEngine;

namespace TRS.CaptureTool.Extras
{
    public static class PathExtensions
    {
        public static string MimeTypeForFilePath(string filePath)
        {
            string extension = System.IO.Path.GetExtension(filePath);
            switch (extension)
            {
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".mp4":
                    return "video/mp4";
                default:
                    throw new UnityException("Unhandled Extension");
            }
        }
    }
}