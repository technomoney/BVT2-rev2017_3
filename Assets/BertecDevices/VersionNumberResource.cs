using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class VersionNumberResource
{
   public string FileVersion;
   public string ProductVersion;
   public string BuildDate;
   public string ScmCommitID;

   public static VersionNumberResource Load()
   {
      TextAsset jsonFile = Resources.Load<TextAsset>("BUILDNUM");

      VersionNumberResource d = JsonUtility.FromJson<VersionNumberResource>(jsonFile.text);

      return d;
   }
}