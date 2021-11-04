using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Options_TargetSelect : MonoBehaviour
{
    public static Options_TargetSelect Instance;

    private int col;
    public int m_defaultTargetIndex;
    [HideInEditorMode] public List<Sprite> m_list_customTargets;

    public List<Sprite> m_list_defaultTargets;
    private List<Button_ImageButton> m_list_targetButtons;
    public Button_ImageButton m_pfb_button_targetButton;

    public Transform m_scrollRect_normal, m_scrollRect_contrast;
    public Transform m_trans_contentParent;
    public Image pfb_image_targetBacking;
    public Button m_button_folder;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Debug.Log(GetType() + " starting");
    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }

    /// <summary>
    ///     This should ONLY be called by CustomSprites after all custom images have been loaded
    /// </summary>
    public void Initialize()
    {
        Debug.Log(GetType() + " initializing");

        //build the initial list of buttons with the default images
        m_list_targetButtons = new List<Button_ImageButton>();

        m_list_defaultTargets.ForEach(MakeTargetButton);
        m_list_customTargets.ForEach(MakeTargetButton);

        //select the default target
        ButtonPushed_TargetButton(m_list_targetButtons[m_defaultTargetIndex]);

        m_button_folder.onClick.AddListener(ShowFolder);
    }

    public void ShowNormalTargets(bool normal = true)
    {
        m_scrollRect_normal.gameObject.SetActive(normal);
        m_scrollRect_contrast.gameObject.SetActive(!normal);
    }

    public void AddCustomTarget(Sprite s)
    {
        if (m_list_customTargets == null)
            m_list_customTargets = new List<Sprite>();

        m_list_customTargets.Add(s);
    }

    private void MakeTargetButton(Sprite s)
    {
        var b = Instantiate(m_pfb_button_targetButton, m_trans_contentParent);
        b.Initialize(s);
        var i = Instantiate(pfb_image_targetBacking, b.transform);
        i.transform.SetAsFirstSibling();
        b.m_image_backing = i;
        b.event_imageButtonPushed += ButtonPushed_TargetButton;
        m_list_targetButtons.Add(b);
    }

    private void ButtonPushed_TargetButton(Button_ImageButton imageButton)
    {
        //set all the backings to unselected
        m_list_targetButtons.ForEach(b =>
            b.m_image_backing.color = Options_BackgroundSelect.Instance.m_color_unselectedBacking);
        //then set the selected one
        imageButton.m_image_backing.color = Options_BackgroundSelect.Instance.m_color_selectedBacking;

        Manager_Targets.Instance.ChangeOption_TargetGraphic(imageButton.m_image);

        //whenever we set a target, set the current test
        if (Manager_Test.Instance.m_selectedTest != null) //necessary?
            Manager_Test.Instance.m_selectedTest.m_options.m_option_target.Change(imageButton.m_image.sprite.name);
    }

    public void SetTargetByName(string targetName)
    {
        var b = m_list_targetButtons.Find(t => t.m_image.sprite.name == targetName);

        if (b == null)
        {
            Debug.Log("Problem finding target " + targetName + " in SetTargetByName()..");
            return;
        }

        ButtonPushed_TargetButton(b);
    }
    
    private void ShowFolder()
    {
        System.Diagnostics.Process.Start(Application.persistentDataPath + "\\Targets");
    }
}