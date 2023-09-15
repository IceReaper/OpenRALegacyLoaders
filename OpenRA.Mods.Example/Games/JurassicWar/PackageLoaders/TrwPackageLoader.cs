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
using System.Text;
using OpenRA.FileSystem;
using OpenRA.Mods.Example.Games.JurassicWar.FileFormats;

namespace OpenRA.Mods.Example.Games.JurassicWar.PackageLoaders
{
	public class TrwPackageLoader : IPackageLoader
	{
		public bool TryParsePackage(Stream s, string filename, FileSystem.FileSystem fs, out IReadOnlyPackage package)
		{
			var start = s.Position;
			var valid = s.Length > 4;

			if (valid)
				valid = Encoding.ASCII.GetString(s.ReadBytes(4)) == "TRW\u001a";

			s.Position = start;

			if (!valid)
			{
				package = null;

				return false;
			}

			package = new Trw(s, filename);

			return true;
		}
	}
}
