using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Manager_Background : MonoBehaviour
{
    public static Manager_Background Instance;
    public Sprite image_defaultBackground;
    public RawImage m_rawImage;
    public VideoPlayer m_videoPlayer;
    public Image image_background { private set; get; }

    // Use this for initialization
    private void Start()
    {
        Debug.Log(GetType() + " starting");

        Instance = this;
        //get the image
        image_background = GetComponentInChildren<Image>();

        //this doesn't actually set anything important, but it will override whatever background
        //is set during development so there isn't a weird flash when the program first loads..
        image_background.sprite = image_defaultBackground;
    }

    /// <summary>
    ///     Change the background to that of the given sprite
    /// </summary>
    public void ChangeOption_Background(Image newBackground)
    {
        image_background.sprite = newBackground.sprite;
        image_background.color = Color.white;
        m_videoPlayer.Stop();
        m_videoPlayer.clip = null;
        m_videoPlayer.enabled = false;
        m_rawImage.enabled = false;
    }

    public void ChangeOption_Background(Image newBackground, Color color)
    {
        image_background.sprite = newBackground.sprite;
        image_background.color = color;
        m_videoPlayer.Stop();
        m_videoPlayer.clip = null;
        m_videoPlayer.enabled = false;
        m_rawImage.enabled = false;
    }

    /// <summary>
    /// change to background to the given video
    /// isTestSelection is a protection against a user clicking the currently selected test when a flow video is selected
    /// the code would normally interpret this as the same video button being pushed (due to the invoke on test selection)
    /// but would give undesired behavior if they clicked the same test that was currently selected
    /// </summary>
    public void ChangeOption_Background(Button_VideoButton videoButton, bool isTestSelection = false)
    {
        var playbackSpeed = 1;
        var rotationOverride = false;
        var clipOverride = false;
        VideoClip overrideClip = null;
        var currentRot = m_rawImage.rectTransform.rotation.eulerAngles;
        var clipName = videoButton.m_clip.name;
        //see if this is an optic flow video or not
        if (clipName == "Horizontal Flow" || clipName == "Vertical Flow" || clipName =="Tunnel_Reduced"|| clipName == "Tunnel_Reduced_Reversed")
        {
            //handle all the garbage we need to here

            //if the optic flow option isn't implemented, set it here, we don't need to do anything else
            if (!Manager_Test.Instance.m_selectedTest.m_options.m_option_opticFlowDirection.isImplemented)
            {
                //set the default direction depending on which video we have pushed
                if (clipName == "Horizontal Flow")
                    Manager_Test.Instance.m_selectedTest.m_options.m_option_opticFlowDirection.Change(Direction.E);
                else if (clipName == "Vertical Flow")
                    Manager_Test.Instance.m_selectedTest.m_options.m_option_opticFlowDirection.Change(Direction.S);
                else //assuming we're tunnel
                    Manager_Test.Instance.m_selectedTest.m_options.m_option_opticFlowDirection.Change(Direction.F);
            }
            //this option is set, meaning we have pushed one of these before, so we need to check if we're pushing the same button,
            //but we still need to ignore it if this is not a test selection
            else if (!isTestSelection)
            {
                if (m_videoPlayer.clip != null && videoButton.m_clip.name == m_videoPlayer.clip.name)
                {
                    //it is the same button as the video currently playing

                    if (m_videoPlayer.clip.name == "Horizontal Flow")
                    {
                        //we want to flip the raw image by -180
                        m_rawImage.rectTransform.rotation =
                            Quaternion.Euler(currentRot.x, currentRot.y - 180, currentRot.z);

                        rotationOverride = true;
                        //set our new direction
                        Manager_Test.Instance.m_selectedTest.m_options.m_option_opticFlowDirection.Change(
                            Manager_Test.Instance.m_selectedTest.m_options.m_option_opticFlowDirection.value ==
                            Direction.E
                                ? Direction.W
                                : Direction.E);
                    }
                    else if (m_videoPlayer.clip.name == "Vertical Flow")
                    {
                        //we want to flip the raw image by -180
                        m_rawImage.rectTransform.rotation =
                            Quaternion.Euler(currentRot.x - 180, currentRot.y, currentRot.z);
                        rotationOverride = true;
                        //set our new direction
                        Manager_Test.Instance.m_selectedTest.m_options.m_option_opticFlowDirection.Change(
                            Manager_Test.Instance.m_selectedTest.m_options.m_option_opticFlowDirection.value ==
                            Direction.N
                                ? Direction.S
                                : Direction.N);
                    }
                    else if(m_videoPlayer.clip.name== "Tunnel_Reduced")
                    {
                        //update our option
                        Manager_Test.Instance.m_selectedTest.m_options.m_option_opticFlowDirection.Change(Direction.R);
                        //load the reverse video
                        clipOverride = true;
                        overrideClip = Options_BackgroundSelect.Instance.m_tunnelReverse;
                    }
                    else if (m_videoPlayer.clip.name == "Tunnel_Reduced_Reversed")
                    {
                        Manager_Test.Instance.m_selectedTest.m_options.m_option_opticFlowDirection.Change(Direction.F);
                        //we don't need to do anything else here since the button by default, has the forward tunnel video
                    }
                }
            }

            //if this is a test selection we need to make sure the video goes back to it's correct rotation
            if (isTestSelection)
            {
                rotationOverride = true;
                var newRot = Quaternion.identity;
                switch ( //set our new direction
                    Manager_Test.Instance.m_selectedTest.m_options.m_option_opticFlowDirection.value)
                {
                    case Direction.N:
                        newRot = Quaternion.Euler(0, currentRot.y, currentRot.z);
                        break;
                    case Direction.S:
                        newRot = Quaternion.Euler(180, currentRot.y, currentRot.z);
                        break;
                    case Direction.E:
                        Quaternion.Euler(currentRot.x, 0, currentRot.z);
                        break;
                    case Direction.W:
                        Quaternion.Euler(currentRot.x, 180, currentRot.z);
                        break;
                    case Direction.F:
                        playbackSpeed = 1;
                        break;
                    case Direction.R:
                        playbackSpeed = -1;
                        break;
                    case Direction.None:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        
        m_videoPlayer.Stop();
        m_videoPlayer.clip = clipOverride ? overrideClip : videoButton.m_clip;
        m_videoPlayer.Play();
        m_rawImage.enabled = true;
        m_videoPlayer.enabled = true;
        m_videoPlayer.playbackSpeed = playbackSpeed;
        m_rawImage.rectTransform.localScale = videoButton.m_vec3_videoScale;
        if (rotationOverride) return;
        m_rawImage.rectTransform.rotation = Quaternion.Euler(videoButton.m_vec3_videoRotation);
    }
}