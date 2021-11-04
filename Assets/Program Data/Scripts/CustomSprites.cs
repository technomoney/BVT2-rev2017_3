using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CustomSprites : MonoBehaviour
{
    public static string PROGRAMDATAPATH;
    private List<string> m_list_bgPaths, m_list_targetPaths;

    public void Initialize()
    {
        Debug.Log(GetType() + " starting");

        //get any custom backgrounds present in the application path
        var path = Application.persistentDataPath;

        PROGRAMDATAPATH = path;

        //see if the folders for backgrounds and targets exists, if not, make them
        if (!Directory.Exists(path + "/Backgrounds"))
            Directory.CreateDirectory(path + "/Backgrounds");
        if (!Directory.Exists(path + "/Targets"))
            Directory.CreateDirectory(path + "/Targets");

        Debug.Log("Application Path: " + path);

        //backgrounds
        var dir = new DirectoryInfo(path + "/Backgrounds");

        var fileInfo = dir.GetFiles("*.png"); //get all pngs
        m_list_bgPaths = new List<string>();
        foreach (var f in fileInfo)
            m_list_bgPaths.Add(f.FullName);
        fileInfo = dir.GetFiles("*.jpg"); //get all jpgs
        foreach (var f in fileInfo)
            m_list_bgPaths.Add(f.FullName);

        Debug.Log("Found " + m_list_bgPaths.Count + " customs bg paths");


        //load each custom background
        //if we don't have any custom background, we can ignore this and just initialize the defaults
        //if (m_list_bgPaths.Count <= 0) Options_BackgroundSelect.Instance.Initialize();
        //else
        m_list_bgPaths.ForEach(s => StartCoroutine(LoadCustomSprite(s, m_list_bgPaths.IndexOf(s))));

        //do the same for targets
        dir = new DirectoryInfo(path + "/Targets");
        fileInfo = dir.GetFiles("*.png");
        m_list_targetPaths = new List<string>();
        foreach (var f in fileInfo)
            m_list_targetPaths.Add(f.FullName);

        //load each custom target
        //if (m_list_targetPaths.Count <= 0) Options_TargetSelect.Instance.Initialize();
        //else
        m_list_targetPaths.ForEach(t => StartCoroutine(LoadCustomSprite(t, m_list_targetPaths.IndexOf(t), false)));
    }

    /// <summary>
    ///     When loading the sprites we have to pull them from the folder and convert them from textures to sprites
    ///     This take a certain amount of time and we don't want the target and bg menus to initialize until all the
    ///     loading is finished, which is why we have to pass the index and manually call initialize for each
    ///     image manager
    /// </summary>
    private IEnumerator LoadCustomSprite(string path, int spriteIndex, bool bg = true)
    {
        var request = new WWW("file:///" + path);
        yield return request;

        //we can't use a www request to get a sprite, so we get a texture2d and then convert
        var tex = request.texture;
        var s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        s.name = "Custom";
        if (bg)
            //now add this sprite to the custom list in the bg manager
            Options_BackgroundSelect.Instance.AddCustomBg(s);
        else
            Options_TargetSelect.Instance.AddCustomTarget(s);
    }
}