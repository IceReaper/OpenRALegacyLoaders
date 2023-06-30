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
using OpenRA.Mods.Example.Games.Dominion.FileFormats;

namespace OpenRA.Mods.Example.Games.Dominion.PackageLoaders
{
	public class RdfPackageLoader : IPackageLoader
	{
		const string Line1 = "Resource Data File ? Copyright (c) 1997 Ion Storm, Ltd.\n";
		const string Line2 = "\rDominion Wave Audio Files";
		const string Line3 = "";

		public bool TryParsePackage(Stream s, string filename, FileSystem.FileSystem fs, out IReadOnlyPackage package)
		{
			var start = s.Position;
			var valid = s.Length > 192;

			if (valid)
				valid = Encoding.ASCII.GetString(s.ReadBytes(64)).Split('\0')[0] == Line1;

			if (valid)
				valid = Encoding.ASCII.GetString(s.ReadBytes(64)).Split('\0')[0] == Line2;

			if (valid)
				valid = Encoding.ASCII.GetString(s.ReadBytes(64)).Split('\0')[0] == Line3;

			s.Position = start;

			if (!valid)
			{
				package = null;

				return false;
			}

			package = new Rdf(s, filename);

			return true;
		}
	}
}
