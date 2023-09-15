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

namespace OpenRA.Mods.Example.Games.Z.FileFormats
{
	public class Blk
	{
		public readonly byte Width = 16;
		public readonly byte Height = 16;
		public readonly byte[][] Frames;

		public Blk(Stream stream)
		{
			Frames = new byte[stream.Length / Width / Height - 32][];

			for (var i = 0; i < Frames.Length; i++)
			{
				// Remove duplicate frame0.
				if (i is 240 or 496)
					stream.Position += 16 * Width * Height;

				Frames[i] = stream.ReadBytes(Width * Height);
			}
		}
	}
}
