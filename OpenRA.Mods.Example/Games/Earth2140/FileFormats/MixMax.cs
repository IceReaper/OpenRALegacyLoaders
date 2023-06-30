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

using System.Collections.Generic;
using System.IO;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Earth2140.FileFormats
{
	public class MixMax
	{
		public class Frame
		{
			public readonly Size Size;
			public readonly byte[] Pixels;

			public Frame(Size size, byte[] pixels)
			{
				Size = size;
				Pixels = pixels;
			}
		}

		public readonly Frame[] Frames;

		public MixMax(Stream stream)
		{
			var frames = new List<Frame>();

			for (var size = 1; size <= 4; size <<= 1)
			for (var i = 0; i < 256; i++)
			{
				var pixels = new byte[size * size * 4];

				for (var j = 0; j < pixels.Length; j += 4)
				{
					var color16 = stream.ReadUInt16();
					pixels[j + 0] = (byte)((color16 & 0xf800) >> 8);
					pixels[j + 1] = (byte)((color16 & 0x07e0) >> 3);
					pixels[j + 2] = (byte)((color16 & 0x001f) << 3);
					pixels[j + 3] = 0xff;
				}

				frames.Add(new Frame(new Size(size, size), pixels));
			}

			Frames = frames.ToArray();
		}
	}
}
