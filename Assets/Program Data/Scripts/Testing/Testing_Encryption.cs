using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Sirenix.OdinInspector.Demos;
using UnityEngine;

public class Testing_Encryption : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		
	}

	void Update()
	{
		//make our test file
		if (Input.GetKeyDown(KeyCode.A))
		{
			//make a file and serialize our test object
			var obj = new Testing_EncObj("This is some text", 1234);
			
			var writer = new XmlSerializer(typeof(Testing_EncObj));
			var file = File.Create(Application.dataPath + "EncTestObject");
			writer.Serialize(file, obj);
			file.Close();
			
			Debug.Log("Created: " + obj);
		}
		
		//open and encrypt it
		if (Input.GetKeyDown(KeyCode.B))
		{
			//make a file and serialize our test object
			var reader = new StreamReader(Application.dataPath + "EncTestObject");
			var xmlContent = reader.ReadToEnd();
			reader.Close();
			var encryptedStr = EncryptDecrypt.ProcessString(xmlContent);
			var writer = new StreamWriter(Application.dataPath + "EncTestObject");
			writer.Write(encryptedStr);
			writer.Close();

			Debug.Log("Object Encrypted");
		}
		
		//open, copy test, decrypt, and make a new object
		if (Input.GetKeyDown(KeyCode.C))
		{
			var reader = new StreamReader(Application.dataPath + "EncTestObject");
			
			Testing_EncObj obj = null;
			var serializer = new XmlSerializer(typeof(Testing_EncObj));
			obj = (Testing_EncObj) serializer.Deserialize(reader);
			reader.Close();
			Debug.Log("After Decrypt: " + obj);
		}
		
	}
}
