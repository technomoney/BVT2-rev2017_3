using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Options_BackgroundSelect : SerializedMonoBehaviour
{
    public static Options_BackgroundSelect Instance;
    public Color m_color_unselectedBacking, m_color_selectedBacking;

    public int m_defaultBackgroundIndex;

    private List<Button_ImageButton> m_list_backgroundImageButtons;
    [HideInEditorMode] public List<Sprite> m_list_customBackgrounds;
    public List<Sprite> m_list_defaultBackgrounds;

    [BoxGroup("Video")]
    [InfoBox(
        "Every video needs an accompanying Vector3 for scaling and or rotation.  Even if no scaling or rotation is applied to the video," +
        "Vector3.One should be used in this case")]
    [BoxGroup("Video")]
    public List<VideoClip> m_list_defaultVideos;

    [InfoBox("Since unity can't actually reverse playback we have to separately hold any video we need to play backwards")]
    public VideoClip m_tunnelReverse;

    private List<Button_VideoButton> m_list_videoButtons;
    [BoxGroup("Video")] public List<Vector3> m_list_videoScaling, m_list_videoRotation;

    [InfoBox("Every video must have an accompanying thumbnail since these can't be generated at runtime")]
    [BoxGroup("Video")]
    public List<Sprite> m_list_videoThmbnails;

    public Button_ImageButton m_pfb_button_backgroundImage;
    public Button_VideoButton m_pfb_videoButton;

    public Transform m_scrollRect_normal, m_scrollRect_contrast;
    [HideInEditorMode] public string m_selectedBackgroundName;
    public Transform m_trans_contentParent;

    public Image pfb_backgroundBacking;
    public Button m_button_folder;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Debug.Log(GetType() + " starting");
    }

    /// <summary>
    ///     This should ONLY be called by CustomSprites after all custom images have been loaded
    /// </summary>
    public void Initialize()
    {
        Debug.Log(GetType() + " initializing");

        //build the initial list of buttons with the default images
        m_list_backgroundImageButtons = new List<Button_ImageButton>();

        //we can use this to track every other button so we can move to a new line
        m_list_defaultBackgrounds.ForEach(MakeBgButton);
        m_list_customBackgrounds.ForEach(MakeBgButton);

        //now handle the videos
        //every video needs a thumbnail..
        if (m_list_defaultVideos.Count != m_list_videoThmbnails.Count)
        {
            Debug.Log("Video missing a thumbnail!");
            return;
        }

        //make the video buttons
        m_list_videoButtons = new List<Button_VideoButton>();
        //we have to use index counter here instead of indexof() the clip since we may use multiple 
        //copies of the same video in the list with different rotation and indexof() will only return the 
        //index of the first occurence..
        var index = 0;
        foreach (var clip in m_list_defaultVideos)
        {
            var b = Instantiate(m_pfb_videoButton, m_trans_contentParent);
            //this will assign the matching index to the video button with its thumbnail
            //we also have to apply any special rotation and scaling here...

            b.Initialize(
                m_list_videoThmbnails[index],
                clip,
                m_list_videoRotation[index],
                m_list_videoScaling[index]);

            //make and set the backing
            var i = Instantiate(pfb_backgroundBacking, b.transform);
            b.m_image_backing = i;
            i.transform.SetAsFirstSibling();

            b.event_videoButtonPushed += ButtonPushed_VideoButton;
            m_list_videoButtons.Add(b);

            //add custom videos lol

            //increment the index
            index++;
        }

        //then select default bg
        ButtonPushed_BackgroundButton(m_list_backgroundImageButtons[m_defaultBackgroundIndex]);

        m_button_folder.onClick.AddListener(ShowFolder);
    }

    private void MakeBgButton(Sprite s)
    {
        var b = Instantiate(m_pfb_button_backgroundImage, m_trans_contentParent);
        b.Initialize(s);
        //make and set the backing
        var i = Instantiate(pfb_backgroundBacking, b.transform);
        b.m_image_backing = i;
        i.transform.SetAsFirstSibling();

        b.event_imageButtonPushed += ButtonPushed_BackgroundButton;
        m_list_backgroundImageButtons.Add(b);
    }

    public void AddCustomBg(Sprite s)
    {
        if (m_list_customBackgrounds == null)
            m_list_customBackgrounds = new List<Sprite>();

        m_list_customBackgrounds.Add(s);
    }

    private void ButtonPushed_BackgroundButton(Button_ImageButton imageButton)
    {
        //set all the backings to unselected
        m_list_backgroundImageButtons.ForEach(b => b.m_image_backing.color = m_color_unselectedBacking);
        m_list_videoButtons.ForEach(b => b.m_image_backing.color = m_color_unselectedBacking);
        //then set the selected one
        imageButton.m_image_backing.color = m_color_selectedBacking;
        m_selectedBackgroundName = imageButton.m_image.sprite.name;

        Manager_Background.Instance.ChangeOption_Background(imageButton.m_image);

        //whenever we change the background, set the lastBG for the current test
        if (Manager_Test.Instance.m_selectedTest != null) //is this check necessary?
        {
            Manager_Test.Instance.m_selectedTest.m_options.m_option_background.Change(imageButton.m_image.sprite.name);
            Manager_Test.Instance.m_selectedTest.m_options.m_option_backgroundIsVideo.Change(false);
        }
    }

    private void ButtonPushed_VideoButton(Button_VideoButton videoButton, bool isTestSelection = false)
    {
        //set all the backings to unselected
        m_list_backgroundImageButtons.ForEach(b => b.m_image_backing.color = m_color_unselectedBacking);
        m_list_videoButtons.ForEach(b => b.m_image_backing.color = m_color_unselectedBacking);
        //then set the selected one
        videoButton.m_image_backing.color = m_color_selectedBacking;
        m_selectedBackgroundName = videoButton.m_clip.name;

        Manager_Background.Instance.ChangeOption_Background(videoButton, isTestSelection);

        //whenever we change the background, set the lastBG for the current test
        if (Manager_Test.Instance.m_selectedTest != null) //is this check necessary?
        {
            Manager_Test.Instance.m_selectedTest.m_options.m_option_background.Change(videoButton.m_clip.name);
            Manager_Test.Instance.m_selectedTest.m_options.m_option_backgroundIsVideo.Change(true);
        }
    }

    public void ShowNormalBackgrounds(bool normal = true)
    {
        m_scrollRect_normal.gameObject.SetActive(normal);
        m_scrollRect_contrast.gameObject.SetActive(!normal);
    }

    public void SetBackgroundByName(string backgroundName, bool isVideo, bool isTestSelection = false)
    {
        //find the image button with the sprite of the matching name
        if (!isVideo)
        {
            var img = m_list_backgroundImageButtons.Find(b => b.m_image.sprite.name == backgroundName);
            if (img == null)
            {
                Debug.Log("Problem finding " + backgroundName + " in SetBackgroundByName..");
                return;
            }

            ButtonPushed_BackgroundButton(img);
        }
        else
        {
            var vid = m_list_videoButtons.Find(b => b.m_clip.name == backgroundName);
            if (vid == null)
            {
                Debug.Log("Problem finding " + backgroundName + " in SetBackgroundByName..");
                return;
            }

            ButtonPushed_VideoButton(vid, isTestSelection);
        }
    }

    private void ShowFolder()
    {
        System.Diagnostics.Process.Start(Application.persistentDataPath + "\\Backgrounds");
    }
}