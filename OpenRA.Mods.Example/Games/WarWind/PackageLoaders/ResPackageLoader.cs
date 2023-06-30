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
using System.Text.RegularExpressions;
using OpenRA.FileSystem;
using OpenRA.Mods.Example.Games.WarWind.FileFormats;

namespace OpenRA.Mods.Example.Games.WarWind.PackageLoaders
{
	public class ResPackageLoader : IPackageLoader
	{
		public bool TryParsePackage(Stream s, string filename, FileSystem.FileSystem fs, out IReadOnlyPackage package)
		{
			if (!Regex.IsMatch(Path.GetFileName(filename), "^res.\\d{3}$", RegexOptions.IgnoreCase))
			{
				package = null;

				return false;
			}

			package = new Res(s, filename);

			return true;
		}
	}
}
