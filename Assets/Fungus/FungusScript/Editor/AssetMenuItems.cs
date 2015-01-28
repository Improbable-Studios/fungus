using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

namespace Fungus
{

	public class AssetMenuItems
	{
		[MenuItem("Assets/Duplicate", false, 19)]
		static void Duplicate()
		{
			EditorWindow.focusedWindow.SendEvent (EditorGUIUtility.CommandEvent ("Duplicate"));
		}
		[MenuItem("Assets/Rename", false, 19)]
		static void Rename()
		{
			EditorWindow.focusedWindow.SendEvent(Event.KeyboardEvent("f2"));
		}
		
	}

}