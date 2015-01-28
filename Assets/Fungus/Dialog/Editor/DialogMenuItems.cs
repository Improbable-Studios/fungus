using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fungus
{

	public class DialogMenuItems 
	{

		[MenuItem("GameObject/Fungus/Dialog/Character")]
		static void CreateCharacter()
		{
			FungusScriptMenuItems.SpawnPrefab("Assets/Fungus/Dialog/Prefabs/Character.prefab");
		}

		[MenuItem("GameObject/Fungus/Dialog/NarratorDialog")]
		static void CreateNarratorDialog()
		{
			FungusScriptMenuItems.SpawnPrefab("Assets/Fungus/Dialog/Prefabs/NarratorDialog.prefab");
		}

		[MenuItem("GameObject/Fungus/Dialog/SayDialog")]
		static void CreateSayDialog()
		{
			FungusScriptMenuItems.SpawnPrefab("Assets/Fungus/Dialog/Prefabs/SayDialog.prefab");
		}

		[MenuItem("GameObject/Fungus/Dialog/ChooseDialog")]
		static void CreateChooseDialog()
		{
			FungusScriptMenuItems.SpawnPrefab("Assets/Fungus/Dialog/Prefabs/ChooseDialog.prefab");
		}

		[MenuItem("GameObject/Fungus/Portrait/PortraitStage")]
		static void CreatePortraitStage()
		{
			FungusScriptMenuItems.SpawnPrefab("Assets/Fungus/Portrait/Prefabs/PortraitStage.prefab");
		}
		
		[MenuItem("GameObject/Fungus/Portrait/PortraitPosition")]
		static void CreatePortraitPosition()
		{
			FungusScriptMenuItems.SpawnPrefab("Assets/Fungus/Portrait/Prefabs/PortraitPosition.prefab");
		}

		[MenuItem("GameObject/Fungus/Audio/AudioSource")]
		static void CreateAudioSource()
		{
			FungusScriptMenuItems.SpawnPrefab("Assets/Fungus/Audio/Prefabs/AudioSource.prefab");
		}
	}
}