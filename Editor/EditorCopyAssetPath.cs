using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Isshi777
{
	public static class EditorCopyAssetPath
	{
		[MenuItem("Assets/Copy Asset Path/Absolute Path")]
		public static void CopyAbsolutePath()
		{
			var paths = GetSelectedPaths(true, false);
			CopyToClipboard(paths);
		}

		[MenuItem("Assets/Copy Asset Path/Absolute Path (Recursive)")]
		public static void CopyAbsolutePathRecursive()
		{
			var paths = GetSelectedPaths(true, true);
			CopyToClipboard(paths);
		}

		[MenuItem("Assets/Copy Asset Path/Assets Path")]
		public static void CopyAssetsPath()
		{
			var paths = GetSelectedPaths(false, false);
			CopyToClipboard(paths);
		}

		[MenuItem("Assets/Copy Asset Path/Assets Path (Recursive)")]
		public static void CopyAssetsPathRecursive()
		{
			var paths = GetSelectedPaths(false, true);
			CopyToClipboard(paths);
		}

		private static void CopyToClipboard(List<string> paths)
		{
			if (paths.Count > 0)
			{
				GUIUtility.systemCopyBuffer = string.Join("\n", paths);
			}
		}

		private static List<string> GetSelectedPaths(bool isAbsolutePath, bool isRecursive)
		{
			var outputPaths = new List<string>();
			foreach (var obj in Selection.objects)
			{
				var assetPath = AssetDatabase.GetAssetPath(obj);
				if (string.IsNullOrEmpty(assetPath))
				{
					continue;
				}

				var fullPath = Path.GetFullPath(assetPath);
				if (isAbsolutePath)
				{
					outputPaths.Add(fullPath.Replace("\\", "/"));
				}
				else
				{
					outputPaths.Add(assetPath.Replace("\\", "/"));
				}

				if (Directory.Exists(fullPath) && isRecursive)
				{
					// metaファイルは除く
					var allFiles = Directory.GetFileSystemEntries(fullPath, "*", SearchOption.AllDirectories).Where(x => !x.EndsWith(".meta"));
					foreach (var file in allFiles)
					{
						if (isAbsolutePath)
						{
							outputPaths.Add(file.Replace("\\", "/"));
						}
						else
						{
							var relativePath = "Assets" + file.Replace("\\", "/").Replace(Application.dataPath, "");
							Debug.Log(file);
							Debug.Log(Application.dataPath);
							outputPaths.Add(relativePath);
						}
					}
				}
			}
			return outputPaths;
		}
	}
}
