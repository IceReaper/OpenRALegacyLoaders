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
using OpenRA.Mods.Example.Games.Z.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Z.SpriteLoaders
{
	public class LbmSpriteLoader : ISpriteLoader
	{
		sealed class LbmSpriteFrame : ISpriteFrame
		{
			public SpriteFrameType Type => SpriteFrameType.Rgba32;
			public Size Size { get; }
			public Size FrameSize { get; }
			public float2 Offset { get; }
			public byte[] Data { get; }

			public bool DisableExportPadding => true;

			public LbmSpriteFrame(Lbm frame)
			{
				var width = frame.Width;
				var height = frame.Height;

				Size = new Size(width, height);
				FrameSize = new Size(width, height);
				Offset = new float2(width / 2f, height / 2f);

				Data = frame.Pixels.SelectMany(pixel =>
				{
					var color = frame.Palette[pixel];

					return new[] { color.R, color.G, color.B, color.A };
				}).ToArray();
			}
		}

		public bool TryParseSprite(Stream stream, string filename, out ISpriteFrame[] frames,
			out TypeDictionary metadata)
		{
			var start = stream.Position;
			var valid = stream.Length > 12;

			if (valid)
			{
				var magic = Encoding.ASCII.GetString(stream.ReadBytes(4));
				var size = BitConverter.ToUInt32(stream.ReadBytes(4).Reverse().ToArray());
				var type = Encoding.ASCII.GetString(stream.ReadBytes(4));

				if (magic != "FORM" || size != stream.Length - stream.Position + 4 || (type != "PBM " && type != "ILBM"))
					valid = false;

				stream.Position = start;
			}

			if (!valid)
			{
				metadata = null;
				frames = null;

				return false;
			}

			frames = new ISpriteFrame[] { new LbmSpriteFrame(new Lbm(stream)) };
			metadata = null;

			return true;
		}
	}
}
