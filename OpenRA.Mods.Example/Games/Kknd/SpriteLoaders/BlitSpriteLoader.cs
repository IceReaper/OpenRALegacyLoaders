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
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Example.Games.Kknd.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Kknd.SpriteLoaders
{
	public class BlitSpriteLoader : ISpriteLoader
	{
		sealed class BlitSpriteFrame : ISpriteFrame
		{
			public SpriteFrameType Type => SpriteFrameType.Rgba32;
			public Size Size { get; }
			public Size FrameSize { get; }
			public float2 Offset { get; }
			public byte[] Data { get; }

			public bool DisableExportPadding => true;

			public BlitSpriteFrame(Blit blit, BlitFrame blitFrame)
			{
				FrameSize = Size = new Size((int)blitFrame.Width, (int)blitFrame.Height);
				Offset = new int2(
					(int)(blitFrame.Width / 2 - blitFrame.OffsetX),
					(int)(blitFrame.Height / 2 - blitFrame.OffsetY));

				Data = blitFrame.Pixels.SelectMany(pixel =>
				{
					return new[]
					{
						blit.Palette[pixel].R,
						blit.Palette[pixel].G,
						blit.Palette[pixel].B,
						blit.Palette[pixel].A
					};
				}).ToArray();
			}
		}

		public bool TryParseSprite(Stream s, string filename, out ISpriteFrame[] frames, out TypeDictionary metadata)
		{
			metadata = null;

			if (!filename.EndsWith($".{Blit.Type}") || s is not Lvl.LvlStream lvlStream)
			{
				frames = null;

				return false;
			}

			// This is damn ugly, but BLIT uses offsets from LVL start.
			lvlStream.BaseStream.Position = lvlStream.BaseOffset;
			var blit = new Blit(lvlStream);
			frames = blit.Images.Select(blitFrame => new BlitSpriteFrame(blit, blitFrame) as ISpriteFrame).ToArray();

			return true;
		}
	}
}
