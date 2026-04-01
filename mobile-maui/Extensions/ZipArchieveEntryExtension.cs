using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BioBrain.Extensions
{
	public static class ZipArchiveEntryExtension
	{
		public static bool IsDirectory(this ZipArchiveEntry entry)
		{
			int nameLength = entry.FullName.Length;
			bool result =
				((nameLength > 0) &&
				 ((entry.FullName[nameLength - 1] == '/') || (entry.FullName[nameLength - 1] == '\\')));
			return result;
		}
		public static void ExportToFolder(this ZipArchiveEntry entry, string dstPath)
		{
			if (entry.IsDirectory())
			{
				Directory.CreateDirectory(Path.Combine(dstPath, entry.FullName));
			}
			else
			{
				entry.ExtractToFile(dstPath);
			}
		}
	}
}
