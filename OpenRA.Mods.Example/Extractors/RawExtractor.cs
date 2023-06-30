#region Copyright & License Information

/*
 * Copyright (c) The OpenRA Developers and Contributors
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */

#endregion

using System.IO;
using OpenRA.FileSystem;

namespace OpenRA.Mods.Example.Extractors
{
	public static class RawExtractor
	{
		public static void Extract(IReadOnlyPackage package, string name)
		{
			var outputFile = Path.Combine("Extracted", name);

			Directory.CreateDirectory(Path.GetDirectoryName(outputFile) ?? string.Empty);

			using var stream = package.GetStream(name);
			File.WriteAllBytes(outputFile, stream.ReadAllBytes());
		}
	}
}
