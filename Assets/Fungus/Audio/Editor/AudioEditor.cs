using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;


namespace Fungus
{
	
	[CustomEditor (typeof(Audio))]
	public class AudioEditor : Editor
	{
		protected SerializedProperty audioSourceProp;
		
		protected virtual void OnEnable()
		{
			audioSourceProp = serializedObject.FindProperty("audioSource");
		}
		
		public override void OnInspectorGUI() 
		{
			serializedObject.Update();

			Audio t = target as Audio;
			GUIContent[] toolbarIcons = {new GUIContent("Play Once"),new GUIContent("Play Loop"),new GUIContent("Pause"),new GUIContent("Stop")};
			int toolbarInt = -1;
			toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarIcons, GUILayout.MinHeight(20));
			if (toolbarInt == 0)
			{
				if (!t.audioSource.isPlaying)
				{
					t.audioSource.loop = false;
					t.audioSource.PlayOneShot(t.audioSource.audio.clip);
				}
			}
			if (toolbarInt == 1)
			{
				if (!t.audioSource.isPlaying)
				{
					t.audioSource.loop = true;
					t.audioSource.Play();
				}
			}
			if (toolbarInt == 2)
			{
				t.audioSource.Pause();
			}
			if (toolbarInt == 3)
			{
				t.audioSource.Stop();
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}