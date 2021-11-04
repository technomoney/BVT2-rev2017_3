using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TabThroughFields : MonoBehaviour
{
	public List<TMP_InputField> m_tabbableFields;
	private int m_activeField;
	private bool m_isEnabled;

	void Start()
	{
		if (m_tabbableFields == null || m_tabbableFields.Count <= 0) return;

		m_activeField = 0;
		m_isEnabled = false;
		m_tabbableFields.ForEach(f => f.onSelect.AddListener(delegate { FieldSelected(f); }));
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (!m_isEnabled) return;
			if (m_tabbableFields == null || m_tabbableFields.Count <= 0) return;
			
			m_activeField++;
			//Debug.Log("New Active field: " + m_activeField);

			if (m_activeField >= m_tabbableFields.Count)
				m_activeField = 0;
			
			m_tabbableFields[m_activeField].ActivateInputField();
		}
	}
	
	private void FieldSelected(TMP_InputField field)
	{
		m_activeField = m_tabbableFields.IndexOf(field);
	}

	public void Disable()
	{
		m_isEnabled = false;
	}

	public void ActivateFirstField()
	{
		m_isEnabled = true;
		if (m_tabbableFields == null || m_tabbableFields.Count <= 0)
		{
			Debug.LogError("Trying to activate first field with null fields list...");
			return;
		}
		
		m_tabbableFields[0].ActivateInputField();
	}
}
