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

namespace OpenRA.Mods.Example.Games.BloodAndMagic.FileFormats
{
	public class AniFrame
	{
		public readonly int Width;
		public readonly int Height;
		public readonly int X;
		public readonly int Y;
		public readonly byte[] Pixels;

		public AniFrame(Stf.StfStream stream)
		{
			X = stream.Header.ReadInt32();
			Y = stream.Header.ReadInt32();

			Width = (ushort)(stream.Header.ReadUInt16() * 2);
			Height = stream.Header.ReadUInt16();
			stream.Header.ReadUInt16(); // priority
			var frameOffset = stream.Header.ReadUInt32();

			stream.Position = frameOffset;

			Pixels = new byte[Width * Height];

			for (var i = 0; i < Width * Height; i += 2)
				Pixels[i] = Pixels[i + 1] = stream.ReadUInt8();
		}
	}
}
