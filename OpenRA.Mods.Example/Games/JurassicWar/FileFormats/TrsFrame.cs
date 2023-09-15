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

namespace OpenRA.Mods.Example.Games.JurassicWar.FileFormats
{
	public class TrsFrame
	{
		public readonly int Width;
		public readonly int Height;
		public readonly int X1;
		public readonly int Y1;
		public readonly int X2;
		public readonly int Y2;
		public readonly byte[] Pixels;

		public TrsFrame(Stream stream, uint dataOffset)
		{
			var offset = stream.ReadUInt32();
			Width = stream.ReadUInt16();
			Height = stream.ReadUInt16();
			X1 = stream.ReadInt16();
			Y1 = stream.ReadInt16();
			X2 = stream.ReadInt16();
			Y2 = stream.ReadInt16();

			stream.Position = dataOffset + offset;
			Pixels = stream.ReadBytes(Width * Height);
		}
	}
}
