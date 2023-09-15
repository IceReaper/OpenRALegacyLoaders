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
	public class ExpSprite
	{
		public readonly Pal Palette;
		public readonly ExpSpriteFrame[] Frames;

		public ExpSprite(Stream stream)
		{
			var numSprites = stream.ReadUInt16();

			var offsets = new uint[numSprites];

			for (var i = 0; i < offsets.Length; i++)
				offsets[i] = stream.ReadUInt32();

			var unk1 = new uint[numSprites]; // TODO

			for (var i = 0; i < unk1.Length; i++)
				unk1[i] = stream.ReadUInt8();

			Palette = new Pal(stream);
			Frames = new ExpSpriteFrame[numSprites];

			for (var i = 0; i < numSprites; i++)
			{
				stream.Position = offsets[i];
				Frames[i] = new ExpSpriteFrame(stream, i + 1 == numSprites ? stream.Length : offsets[i + 1]);
			}
		}
	}
}
