using HeathenEngineering.UIX;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Window_AddPatient : BVT_Window
{
    public static Window_AddPatient Instance;

    public TextMeshProUGUI m_txt_title;
    public Button m_button_add, m_button_cancel;
    [BoxGroup("On Screen Keyboard")] public Button m_button_keyboardToggle, m_button_keyboard_hide;
    public Button m_button_randomizeId;
    [BoxGroup("Date of Birth")] public TMP_Dropdown m_dob_month, m_dob_day, m_dob_year;
    [BoxGroup("Height")] public TMP_Dropdown m_height_feet, m_height_inches;

    public TMP_InputField m_input_id;
    private Vector2 m_keyboard_hidePosition;

    private bool m_keyboard_isSowing;
    [BoxGroup("On Screen Keyboard")] public Vector2 m_keyboard_showPosition;

    [BoxGroup("Name")] public TMP_InputField m_name_first, m_name_last;
    [BoxGroup("Notes")] public TMP_InputField m_notes;

    [BoxGroup("On Screen Keyboard")] public KeyboardOutputManager m_onScreenKeyboard;
    [BoxGroup("Gender")] public Toggle m_toggle_male, m_toggle_female;
    [BoxGroup("Weight")] public TMP_InputField m_weight;

    private bool m_editingPatient;

    private void Awake()
    {
        Instance = this;
        m_button_cancel.onClick.AddListener(Hide);
        m_button_add.onClick.AddListener(ButtonPushed_Add);

        m_name_first.onSelect.AddListener(delegate { SetKeyboardFocus(m_name_first); });
        m_name_last.onSelect.AddListener(delegate { SetKeyboardFocus(m_name_last); });
        m_weight.onSelect.AddListener(delegate { SetKeyboardFocus(m_weight); });
        m_notes.onSelect.AddListener(delegate { SetKeyboardFocus(m_notes); });

        m_button_keyboardToggle.onClick.AddListener(KeyboardToggle);
        m_button_keyboard_hide.onClick.AddListener(KeyboardHide);

        m_keyboard_hidePosition = m_onScreenKeyboard.GetComponent<RectTransform>().anchoredPosition;
        m_keyboard_isSowing = false;

        m_button_randomizeId.onClick.AddListener(RandomizeId);
        m_editingPatient = false;
    }

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        //populate the years
        for (var x = 2020; x >= 1900; x--)
        {
            var d = new TMP_Dropdown.OptionData(x.ToString());
            m_dob_year.options.Add(d);
        }

        //then set the preview label since we're populating this at runtime
        m_dob_year.transform.Find("Label").GetComponent<TextMeshProUGUI>().text =
            m_dob_year.options[m_dob_year.value].text;
    }


    private void ButtonPushed_Add()
    {
        //the only thing that is required is a patient ID
        if (m_input_id.text == string.Empty || m_name_first.text == string.Empty || m_name_last.text == string.Empty)
        {
            //todo should be a popup or text on the window itself that tells if it is valid
            Debug.Log("Must enter patient ID");
            return;
        }

        var p = new Patient();
        p.SetName(m_name_first.text, m_name_last.text);
        p.SetGender(m_toggle_male.isOn ? "Male" : "Female");
        p.SetDoB(m_dob_month.options[m_dob_month.value].text,
            m_dob_day.options[m_dob_day.value].text,
            m_dob_year.options[m_dob_year.value].text);
        p.SetHeight(m_height_feet.options[m_height_feet.value].text,
            m_height_inches.options[m_height_inches.value].text);
        p.SetWeight(m_weight.text);
        p.SetNotes(m_notes.text);
        p.SetId(m_input_id.text);

        Manager_Patient.Instance.AddNewPatient(p);

        KeyboardHide();

        //when a patient is added we can automatically select it and close the patient manager
        Manager_Patient.Instance.SetSelectedPatient(p);
        Hide();
        Manager_Patient.Instance.Hide();
        //if we're editing a patient we want to refresh the patient detail screen
        if (m_editingPatient) Window_PatientDetail.Instance.Show(p, true, false);
    }

    private void ClearFields()
    {
        m_name_first.text = string.Empty;
        m_name_last.text = string.Empty;
        m_weight.text = string.Empty;
        m_notes.text = string.Empty;
        m_height_feet.value = 4;
        m_height_inches.value = 9;
        m_dob_month.value = 0;
        m_dob_day.value = 0;
        m_dob_year.value = 38;
        m_toggle_male.isOn = true;
    }

    public override void Show()
    {
        ClearFields();

        base.Show();
        
        //change the labels in case we edited something
        m_txt_title.text = "Add New Client";
        m_button_add.GetComponentInChildren<TextMeshProUGUI>().text = "Add";
        m_editingPatient = false;
        m_button_randomizeId.gameObject.SetActive(true);
        m_input_id.interactable = true;

        //when we show this we need to randomize the patient id automatically
        RandomizeId();
        
        GetComponent<TabThroughFields>().ActivateFirstField();
    }

    /// <summary>
    /// Open the add client window for editing an existing patient
    /// </summary>
    public void ShowForEdit(Patient p)
    {
        //change the titles
        m_txt_title.text = "Edit Client";
        m_button_add.GetComponentInChildren<TextMeshProUGUI>().text = "Edit";
        m_editingPatient = true;
        m_button_randomizeId.gameObject.SetActive(false);
        m_input_id.interactable = false;

        //fill the fields
        m_name_first.text = p.m_name_first;
        m_name_last.text = p.m_name_last;
        m_input_id.text = p.m_id;
        if (p.m_gender.Equals("Male"))
            m_toggle_male.isOn = true;
        else m_toggle_female.isOn = true;

        //get our month int index
        var monthIndex = -1;
        switch (p.m_dob_month)
        {
            case "January":
                monthIndex = 0;
                break;
            case "February":
                monthIndex = 1;
                break;
            case "March":
                monthIndex = 2;
                break;
            case "April":
                monthIndex = 3;
                break;
            case "May":
                monthIndex = 4;
                break;
            case "June":
                monthIndex = 5;
                break;
            case "July":
                monthIndex = 6;
                break;
            case "August":
                monthIndex = 7;
                break;
            case "September":
                monthIndex = 8;
                break;
            case "October":
                monthIndex = 9;
                break;
            case "November":
                monthIndex = 10;
                break;
            case "December":
                monthIndex = 11;
                break;
            default:
                Debug.LogError("Problem deriving month index");
                break;
        }

        m_dob_month.value = monthIndex;
        m_dob_day.value = int.Parse(p.m_dob_day) - 1;
        
        //the year is weird, the drop down goes from [0]2020 - [220]1900, so we have to go backwards from 2020 to get 
        //at the index we want
        m_dob_year.value = 2020 - int.Parse(p.m_dob_year);
        m_height_feet.value = int.Parse(p.m_height_feet) - 1;
        m_height_inches.value = int.Parse(p.m_height_inches) - 1;
        m_weight.text = p.m_weight;
        m_notes.text = p.m_notes;

        base.Show();
    }

    public override void Hide()
    {
        ClearFields();
        GetComponent<TabThroughFields>().Disable();
        base.Hide();
    }

    private void SetKeyboardFocus(TMP_InputField field)
    {
        m_onScreenKeyboard.linkedGameObject = field.gameObject;
        m_onScreenKeyboard.linkedBehaviour = field.GetComponent<TMP_InputField>();
    }

    private void KeyboardToggle()
    {
        m_keyboard_isSowing = !m_keyboard_isSowing;
        if (m_keyboard_isSowing)
            m_onScreenKeyboard.GetComponent<RectTransform>().anchoredPosition = m_keyboard_showPosition;
        else KeyboardHide();
    }

    private void KeyboardHide()
    {
        m_keyboard_isSowing = false;
        m_onScreenKeyboard.GetComponent<RectTransform>().anchoredPosition = m_keyboard_hidePosition;
    }

    private void RandomizeId()
    {
        char[] alphabet =
            {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 'o', 'p', 'r', 's', 't', 'w', 'x', 'y', 'x'};

        var id = string.Empty;
        id += alphabet[Random.Range(0, alphabet.Length)];
        id += alphabet[Random.Range(0, alphabet.Length)];
        id += Random.Range(0, 999999);

        m_input_id.text = id;
    }
}