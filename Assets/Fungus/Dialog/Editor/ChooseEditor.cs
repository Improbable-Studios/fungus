using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;

namespace Fungus
{
	
	[CustomEditor (typeof(Choose))]
	public class ChooseEditor : CommandEditor
	{
		static public bool showTagHelp;
		
		protected SerializedProperty chooseTextProp;
		protected SerializedProperty characterProp;
		protected SerializedProperty chooseDialogProp;
		protected SerializedProperty portraitProp;
		protected SerializedProperty voiceOverClipProp;
		protected SerializedProperty timeoutDurationProp;
		protected SerializedProperty fadeInProp;
		protected SerializedProperty fadeOutProp;
		
		protected virtual void OnEnable()
		{
			chooseTextProp = serializedObject.FindProperty("chooseText");
			characterProp = serializedObject.FindProperty("character");
			portraitProp = serializedObject.FindProperty("portrait");
			chooseDialogProp = serializedObject.FindProperty("chooseDialog");
			voiceOverClipProp = serializedObject.FindProperty("voiceOverClip");
			timeoutDurationProp = serializedObject.FindProperty("timeoutDuration");
			fadeInProp = serializedObject.FindProperty("fadeIn");
			fadeOutProp = serializedObject.FindProperty("fadeOut");
		}
		
		public override void DrawCommandGUI() 
		{
			serializedObject.Update();
			
			Choose t = target as Choose;
			ChooseDialog cd = t.chooseDialog;
			if (t.chooseDialog == null)           // If default box selected
			{
				cd = t.GetFungusScript().defaultChoose; // Try to get character's default choose dialog box
				if (cd == null)                   // If no default specified, Try to get any ChooseDialog in the scene
				{
					cd = GameObject.FindObjectOfType<ChooseDialog>();
				}
			}
			CommandEditor.ObjectField<ChooseDialog>(chooseDialogProp, 
			                                        new GUIContent("Choose Dialog", "Choose Dialog object to use to display the multiple player choices"), 
			                                        new GUIContent("<Default>"),
			                                        ChooseDialog.activeDialogs);
			if (cd != null && cd.nameText != null)
			{
				CommandEditor.ObjectField<Character>(characterProp,
				                                     new GUIContent("Character", "Character to display in dialog"), 
				                                     new GUIContent("<None>"),
				                                     Character.activeCharacters);
			}
			else
			{
				t.character = null;
			}

			bool showPortraits = false;
			// Only show portrait selection if...
			if (t.character != null &&                // Character is selected
			    t.character.portraits != null &&      // Character has a portraits field
			    t.character.portraits.Count > 0 )     // Selected Character has at least 1 portrait
			{
				if (cd != null && cd.characterImage != null) // Check that selected choose dialog has a character image
				{
					showPortraits = true;    
				}
			}			
			if (showPortraits) 
			{
				CommandEditor.ObjectField<Sprite>(portraitProp, 
				                                  new GUIContent("Portrait", "Portrait representing speaking character"), 
				                                  new GUIContent("<None>"),
				                                  t.character.portraits);
			}
			else
			{
				t.portrait = null;
			}
			
			EditorGUILayout.PropertyField(chooseTextProp);
			
			EditorGUILayout.BeginHorizontal();
			
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(new GUIContent("Tag Help", "Show help info for tags"), new GUIStyle(EditorStyles.miniButton)))
			{
				showTagHelp = !showTagHelp;
			}
			EditorGUILayout.EndHorizontal();
			
			if (showTagHelp)
			{
				SayEditor.DrawTagHelpLabel();
			}
			
			EditorGUILayout.Separator();
			
			EditorGUILayout.PropertyField(voiceOverClipProp, new GUIContent("Voice Over Clip", "Voice over audio to play when the choose text is displayed"));
			
			EditorGUILayout.PropertyField(timeoutDurationProp, new GUIContent("Timeout Duration", "Time limit for player to make a choice. Set to 0 for no limit."));

			EditorGUILayout.PropertyField(fadeInProp);

			EditorGUILayout.PropertyField(fadeOutProp);

			if (showPortraits && t.portrait != null)
			{
				Texture2D characterTexture = t.portrait.texture;
				
				float aspect = (float)characterTexture.width / (float)characterTexture.height;
				
				Rect previewRect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.Width(100), GUILayout.ExpandWidth(true));
				
				CharacterEditor characterEditor = Editor.CreateEditor(t.character) as CharacterEditor;
				characterEditor.DrawPreview(previewRect, characterTexture);
				DestroyImmediate(characterEditor);
			}
			
			serializedObject.ApplyModifiedProperties();
		}
	}
	
}