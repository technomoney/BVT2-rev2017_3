using System.Text;
using UnityEngine;

public class EncryptDecrypt : MonoBehaviour
{
    private const int key = 129;

    public static string ProcessString(string textToEncrypt)
    {
        var inSb = new StringBuilder(textToEncrypt);
        var outSb = new StringBuilder(textToEncrypt.Length);
        for (var i = 0; i < textToEncrypt.Length; i++)
        {
            var c = inSb[i];
            c = (char) (c ^ key);
            outSb.Append(c);
        }

        //testing
        //Debug.Log(outSb.ToString());

        return outSb.ToString();
    }
}