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

				var fullPath = Path.GetFullPath(assetPath).Replace("\\", "/");
				if (Directory.Exists(fullPath))
				{
					if (isRecursive)
					{
						// 深さ優先とソート付き探索
						Traverse(fullPath, outputPaths, isAbsolutePath);
					}
					else
					{
						outputPaths.Add(isAbsolutePath ? fullPath : assetPath.Replace("\\", "/"));
					}
				}
				else
				{
					outputPaths.Add(isAbsolutePath ? fullPath : assetPath.Replace("\\", "/"));
				}
			}

			return outputPaths;
		}

		// 深さ優先でソートしながらファイル＆ディレクトリを追加
		private static void Traverse(string directory, List<string> result, bool isAbsolutePath)
		{
			string path = directory.Replace("\\", "/");
			string baseAssets = Application.dataPath.Replace("\\", "/");

			// 現在のディレクトリを追加
			result.Add(isAbsolutePath ? path : "Assets" + path.Replace(baseAssets, ""));

			// ファイルをソートして追加(※metaは除外)
			var files = Directory.GetFiles(directory).Where(f => !f.EndsWith(".meta")).OrderBy(f => f, System.StringComparer.OrdinalIgnoreCase);
			foreach (var file in files)
			{
				string normalized = file.Replace("\\", "/");
				result.Add(isAbsolutePath ? normalized : "Assets" + normalized.Replace(baseAssets, ""));
			}

			// サブフォルダもソートして再帰
			var dirs = Directory.GetDirectories(directory).OrderBy(d => d, System.StringComparer.OrdinalIgnoreCase);
			foreach (var dir in dirs)
			{
				Traverse(dir, result, isAbsolutePath);
			}
		}
	}
}
