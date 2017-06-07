/**
 * ScriptTemplateTool
 * Auto create script by script template.
 * 
 * Create by onelei one 6/7/2017 11:20:46 AM.
 * Copyright (c) 6/7/2017 11:20:46 AM. All rights reserved.
**/
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using UnityEditor.ProjectWindowCallback;
using System.Text.RegularExpressions;

public class ScriptTemplateTool
{
	[MenuItem("Assets/Create/C# ScriptTemplate", false, 80)]
	public static void CreateScriptTemplate()
	{
		ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
			ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
			GetSelectedPathOrFallback() + "/NewScriptTemplate.cs",
			null,
			"Assets/Editor/ScriptTemplate/ScriptTemplate.cs");
	}

	public static string GetSelectedPathOrFallback()
	{
		string path = "Assets";
		foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
		{
			path = AssetDatabase.GetAssetPath(obj);
			if (!string.IsNullOrEmpty(path) && File.Exists(path))
			{
				path = Path.GetDirectoryName(path);
				break;
			}
		}
		return path;
	}
}    

class MyDoCreateScriptAsset : EndNameEditAction
{
	public override void Action(int instanceId, string pathName, string resourceFile)
	{
		UnityEngine.Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
		ProjectWindowUtil.ShowCreatedAsset(o);
	}

	internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
	{
		string fullPath = Path.GetFullPath(pathName);
		StreamReader streamReader = new StreamReader(resourceFile);
		string text = streamReader.ReadToEnd();
		streamReader.Close();
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
		// replace space
		fileNameWithoutExtension = fileNameWithoutExtension.Replace(" ","");
		// class name
		text = Regex.Replace(text, "ScriptTemplate", fileNameWithoutExtension);
		// file name
		text = Regex.Replace(text, "FileName", fileNameWithoutExtension);
		// create time
		text = Regex.Replace(text, "CreateTime", DateTime.Now.ToString());
		bool encoderShouldEmitUTF8Identifier = true;
		bool throwOnInvalidBytes = false;
		UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
		bool append = false;
		StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
		streamWriter.Write(text);
		streamWriter.Close();
		AssetDatabase.ImportAsset(pathName);
		return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
	}

}     
