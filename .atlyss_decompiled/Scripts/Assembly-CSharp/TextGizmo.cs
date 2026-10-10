using System.Collections.Generic;
using UnityEngine;

public class TextGizmo
{
	public enum TextAlignment
	{
		LeftTop = 36,
		CenterTop = 20,
		RightTop = 12,
		LeftMiddle = 34,
		CenterMiddle = 18,
		RightMiddle = 10,
		LeftBottom = 33,
		CenterBottom = 17,
		RightBottom = 9
	}

	private static TextGizmo textGizmo = null;

	private Dictionary<char, string> texturePathLookup;

	private Camera editorCamera;

	private static readonly string availableCharacters = " !#%'()+,-.0123456789;=abcdefghijklmnopqrstuvwxyz_{}~\\?\":/*";

	private static readonly string[] newLineCharacterCombinations = new string[2] { "\n", "\r\n" };

	private static TextAlignment DefaultTextAligment => TextAlignment.LeftMiddle;

	private static Color DefaultColor => default;

	private static int CharTextureWidth => 8;

	private static int CharTextureHeight => 11;

	private static string AvailableCharacters => availableCharacters;

	private static string[] NewLineCharacterCombinations => newLineCharacterCombinations;

	private TextGizmo()
	{
		editorCamera = Camera.current;
		texturePathLookup = new Dictionary<char, string>();
		for (int i = 0; i < AvailableCharacters.Length; i++)
		{
			texturePathLookup.Add(AvailableCharacters[i], "TextGizmo/CharacterImages/text_" + AvailableCharacters[i] + ".png");
		}
	}

	private static void Initialize()
	{
		textGizmo = new TextGizmo();
	}

	public static void DrawExtended(Vector3 position, string text)
	{
		DrawExtended(position, text, DefaultTextAligment, DefaultColor);
	}

	public static void DrawExtended(Vector3 position, string text, Color color)
	{
		DrawExtended(position, text, DefaultTextAligment, color);
	}

	public static void DrawExtended(Vector3 position, string text, TextAlignment alignment)
	{
		DrawExtended(position, text, alignment, DefaultColor);
	}

	public static void DrawExtended(Vector3 position, string text, TextAlignment alignment, Color color)
	{
		if (textGizmo == null)
		{
			Initialize();
		}
		Vector3 vector = textGizmo.editorCamera.WorldToScreenPoint(position);
		List<List<string>> list = GetAvailableSymbolIconPaths(text.ToLower());
		int count = list.Count;
		GetLineMaxLength(list);
		float num = GetTextVerticalOffset(count, alignment);
		for (int i = 0; i < list.Count; i++)
		{
			float num2 = GetLineHorizontalOffset(list[i].Count, alignment);
			for (int j = 0; j < list[i].Count; j++)
			{
				float num3 = num2 + (float)(j * CharTextureWidth);
				float num4 = num - (float)(i * CharTextureHeight);
				Gizmos.DrawIcon(textGizmo.editorCamera.ScreenToWorldPoint(new Vector3(vector.x + num3, vector.y + num4, vector.z)), list[i][j], allowScaling: false, color);
			}
		}
		static List<List<string>> GetAvailableSymbolIconPaths(string text2)
		{
			List<List<string>> list2 = new List<List<string>>
			{
				new List<string>()
			};
			for (int k = 0; k < text2.Length; k++)
			{
				bool flag = false;
				string[] array = NewLineCharacterCombinations;
				foreach (string text3 in array)
				{
					if (text2.Length - (k + 1) >= text3.Length)
					{
						int num5 = k;
						if (text2.Substring(num5, k + 2 - num5) == text3)
						{
							list2.Add(new List<string>());
							k += text3.Length - 1;
							flag = true;
							break;
						}
					}
				}
				if (!flag && textGizmo.texturePathLookup.ContainsKey(text2[k]))
				{
					list2[list2.Count - 1].Add(textGizmo.texturePathLookup[text2[k]]);
				}
			}
			return list2;
		}
		static float GetLineHorizontalOffset(int lineLength, TextAlignment textAlignment)
		{
			if ((textAlignment & (TextAlignment)0x20) != 0)
			{
				return (float)CharTextureWidth / 2f;
			}
			if ((textAlignment & (TextAlignment)0x10) != 0)
			{
				return (float)(-CharTextureWidth * lineLength) / 2f;
			}
			if ((textAlignment & (TextAlignment)8) != 0)
			{
				return (float)(-CharTextureWidth) * ((float)lineLength - 0.5f);
			}
			return 0f;
		}
		static int GetLineMaxLength(List<List<string>> availableSymbolIconPaths)
		{
			int num5 = 0;
			foreach (List<string> availableSymbolIconPath in availableSymbolIconPaths)
			{
				if (availableSymbolIconPath.Count > num5)
				{
					num5 = availableSymbolIconPath.Count;
				}
			}
			return num5;
		}
		static float GetTextVerticalOffset(int numberOfLines, TextAlignment textAlignment)
		{
			if ((textAlignment & (TextAlignment)4) != 0)
			{
				return (float)(-CharTextureHeight) / 2f;
			}
			if ((textAlignment & (TextAlignment)2) != 0)
			{
				return (float)(CharTextureHeight * (numberOfLines - 1)) / 2f;
			}
			if ((textAlignment & (TextAlignment)1) != 0)
			{
				return (float)CharTextureHeight * ((float)numberOfLines - 0.5f);
			}
			return 0f;
		}
	}

	public static void DrawBasic(Vector3 position, string text)
	{
		DrawBasic(position, text, DefaultColor);
	}

	public static void DrawBasic(Vector3 position, string text, Color color)
	{
		if (textGizmo == null)
		{
			Initialize();
		}
		string text2 = text.ToLower();
		Vector3 vector = textGizmo.editorCamera.WorldToScreenPoint(position);
		for (int i = 0; i < text2.Length; i++)
		{
			if (textGizmo.texturePathLookup.ContainsKey(text2[i]))
			{
				float num = (float)CharTextureWidth * (0.5f + (float)i);
				Gizmos.DrawIcon(textGizmo.editorCamera.ScreenToWorldPoint(new Vector3(vector.x + num, vector.y, vector.z)), textGizmo.texturePathLookup[text2[i]], allowScaling: false, color);
			}
		}
	}
}
