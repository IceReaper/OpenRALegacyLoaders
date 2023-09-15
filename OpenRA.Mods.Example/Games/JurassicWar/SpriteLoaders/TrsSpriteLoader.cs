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
using OpenRA.Graphics;
using OpenRA.Mods.Example.Games.JurassicWar.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.JurassicWar.SpriteLoaders
{
	public class TrsSpriteLoader : ISpriteLoader
	{
		sealed class TrsSpriteFrame : ISpriteFrame
		{
			public SpriteFrameType Type => SpriteFrameType.Rgba32;
			public Size Size { get; }
			public Size FrameSize { get; }
			public float2 Offset { get; }
			public byte[] Data { get; }

			public bool DisableExportPadding => true;

			public TrsSpriteFrame(TrsFrame frame, Pal palette)
			{
				var width = frame.Width;
				var height = frame.Height;

				Size = new Size(width, height);
				FrameSize = new Size(width, height);
				Offset = new float2(width / 2f + frame.X1, height / 2f + frame.Y1);

				Data = frame.Pixels.SelectMany(pixel =>
				{
					var color = palette.Colors[pixel];

					return new[] { color.R, color.G, color.B, color.A };
				}).ToArray();
			}
		}

		public bool TryParseSprite(Stream stream, string filename, out ISpriteFrame[] frames,
			out TypeDictionary metadata)
		{
			var start = stream.Position;
			var valid = stream.Length > 4;

			if (valid)
				valid = Encoding.ASCII.GetString(stream.ReadBytes(4)) == "TRS\u001a";

			stream.Position = start;

			if (!valid)
			{
				metadata = null;
				frames = null;

				return false;
			}

			var paletteContainer = Game.ModData.ModFiles.MountedPackages.First(package =>
				package.Name.EndsWith("_PWAR01.TRC", StringComparison.OrdinalIgnoreCase));

			var palette = new Pal(paletteContainer.GetStream("MAIN.PAL"));

			frames = new Trs(stream).Frames.Select(frame => new TrsSpriteFrame(frame, palette)).ToArray<ISpriteFrame>();
			metadata = null;

			return true;
		}
	}
}
