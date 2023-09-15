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
using System.Linq;
using System.Text;

namespace OpenRA.Mods.Example.Games.JurassicWar.FileFormats
{
	public class Trs
	{
		public readonly TrsFrame[] Frames;

		public Trs(Stream stream)
		{
			var magic = Encoding.ASCII.GetString(stream.ReadBytes(4));

			if (magic != "TRS\u001a")
				throw new Exception();

			var entries = stream.ReadUInt32();
			var entriesCopy = stream.ReadUInt32();
			var dataOffset = stream.ReadUInt32();

			if (stream.ReadBytes(16).Any(e => e != 0x00))
				throw new Exception();

			if (entriesCopy != entries)
				throw new Exception();

			Frames = new TrsFrame[entries];

			for (var i = 0; i < entries; i++)
			{
				stream.Position = 32 + i * 16;
				Frames[i] = new TrsFrame(stream, dataOffset);
			}
		}
	}
}
