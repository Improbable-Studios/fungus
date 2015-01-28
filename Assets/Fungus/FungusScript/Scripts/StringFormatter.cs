using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Text;
using System;

namespace Fungus
{
	public class StringFormatter
	{
		public static string[] formatEnumNames(Enum e, string firstLabel)
		{
			string[] enumLabels = Enum.GetNames(e.GetType());
			enumLabels[0] = firstLabel;
			for (int i=0; i<enumLabels.Length; i++)
			{
				enumLabels[i] = splitCamelCase(enumLabels[i]);
			}
			return enumLabels;
		}
		public static string splitCamelCase(string text)
		{
			if (string.IsNullOrEmpty(text))
				return "";
			StringBuilder newText = new StringBuilder(text.Length * 2);
			newText.Append(text[0]);
			for (int i = 1; i < text.Length; i++)
			{
				if (char.IsUpper(text[i]) && text[i - 1] != ' ')
					newText.Append(' ');
				newText.Append(text[i]);
			}
			return newText.ToString();
		}

	}
	
}