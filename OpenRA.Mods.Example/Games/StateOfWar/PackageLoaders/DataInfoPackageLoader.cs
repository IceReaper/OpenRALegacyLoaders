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

using System;
using System.IO;
using OpenRA.FileSystem;
using OpenRA.Mods.Example.Games.StateOfWar.FileFormats;

namespace OpenRA.Mods.Example.Games.StateOfWar.PackageLoaders
{
	public class DataInfoPackageLoader : IPackageLoader
	{
		public bool TryParsePackage(Stream s, string filename, FileSystem.FileSystem fs, out IReadOnlyPackage package)
		{
			if (!filename.EndsWith(".data", StringComparison.OrdinalIgnoreCase))
			{
				package = null;

				return false;
			}

			if (!fs.TryOpen($"{filename[..^5]}.info", out var infoStream))
			{
				package = null;

				return false;
			}

			package = new DataInfo(infoStream, s, filename);

			return true;
		}
	}
}
