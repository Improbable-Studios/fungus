using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Label", 
	             "Marks a position in the command list for execution to jump to.")]
	[AddComponentMenu("")]
	public class Label : Command
	{
		public string key = "";

		public override void OnEnter()
		{
			Continue();
		}

		public override string GetSummary()
		{
			return key + ":";
		}

		public override Color GetButtonColor()
		{
			return new Color32(200, 200, 253, 255);
		}
	}

}