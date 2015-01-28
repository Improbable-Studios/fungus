using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Else", 
	             "Marks the start of a command block to be executed when the preceding If statement is False.")]
	[AddComponentMenu("")]
	public class Else : Command
	{
		public override void OnEnter()
		{
			if (parentSequence == null)
			{
				return;
			}

			// Stop if this is the last command in the list
			if (commandIndex >= parentSequence.commandList.Count - 1)
			{
				Stop();
				return;
			}

			// Find the next End command at the same indent level as this Else command
			int indent = indentLevel;
			for (int i = commandIndex + 1; i < parentSequence.commandList.Count; ++i)
			{
				Command command = parentSequence.commandList[i];
				
				if (command.indentLevel == indent)
				{
					System.Type type = command.GetType();
					if (type == typeof(EndIf) || // Legacy support for old EndIf command
					    type == typeof(End))
					{
						// Execute command immediately after the EndIf command
						Continue(command.commandIndex + 1);
						return;
					}
				}
			}
			
			// No End command found
			Stop();
		}

		public override bool OpenBlock()
		{
			return true;
		}

		public override bool CloseBlock()
		{
			return true;
		}

		public override Color GetButtonColor()
		{
			return new Color32(253, 253, 150, 255);
		}
	}

}