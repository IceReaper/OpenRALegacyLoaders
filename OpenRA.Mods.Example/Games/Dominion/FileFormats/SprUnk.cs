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

namespace OpenRA.Mods.Example.Games.Dominion.FileFormats
{
	public class SprUnk
	{
		public readonly uint Unk1;
		public readonly uint Unk2;
		public readonly ushort Unk3;
		public readonly ushort Unk4;

		public SprUnk(Stream stream)
		{
			if (stream.ReadUInt32() != 1 || stream.ReadUInt16() != 0)
				throw new Exception("Unknown value!");

			Unk1 = stream.ReadUInt32();

			if (stream.ReadUInt32() != 0)
				throw new Exception("Unknown value!");

			Unk2 = stream.ReadUInt32();

			if (stream.ReadUInt32() != 0)
				throw new Exception("Unknown value!");

			Unk3 = stream.ReadUInt16();
			Unk4 = stream.ReadUInt16();

			if (stream.ReadBytes((int)(stream.Length - stream.Position)).Any(v => v != 0))
				throw new Exception("Unknown value!");
		}
	}
}
