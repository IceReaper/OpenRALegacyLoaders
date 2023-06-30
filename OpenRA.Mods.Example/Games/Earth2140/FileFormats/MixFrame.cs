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
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Earth2140.FileFormats
{
	public class MixFrame
	{
		public readonly ushort Width;
		public readonly ushort Height;
		public readonly byte Palette;
		public readonly byte[] Pixels;
		public readonly bool Is32Bpp;

		public MixFrame(Stream stream)
		{
			Width = stream.ReadUInt16();
			Height = stream.ReadUInt16();
			var type = stream.ReadUInt8();
			Palette = stream.ReadUInt8();

			switch (type)
			{
				case 1:
					Pixels = stream.ReadBytes(Width * Height);

					break;

				case 2:
					Pixels = new byte[Width * Height * 4];
					Is32Bpp = true;

					for (var i = 0; i < Pixels.Length; i += 4)
					{
						var color16 = stream.ReadUInt16();
						Pixels[i + 0] = (byte)((color16 & 0xf800) >> 8);
						Pixels[i + 1] = (byte)((color16 & 0x07e0) >> 3);
						Pixels[i + 2] = (byte)((color16 & 0x001f) << 3);
						Pixels[i + 3] = 0xff;
					}

					break;

				case 9:
					Pixels = new byte[Width * Height];

					var widthCopy = stream.ReadUInt32();
					var heightCopy = stream.ReadUInt32();

					if (Width != widthCopy || Height != heightCopy)
						throw new Exception("Broken mix frame!");

					var dataSize = stream.ReadUInt32();
					var numScanLines = stream.ReadUInt32();
					var numPatterns = stream.ReadUInt32();
					var scanLinesOffset = stream.ReadUInt32();
					var dataOffsetsOffset = stream.ReadUInt32();
					var patternsOffset = stream.ReadUInt32();
					var compressedImageDataOffset = stream.ReadUInt32();

					if (scanLinesOffset != stream.Position - 6)
						throw new Exception("Broken mix frame!");

					var scanLines = new int[numScanLines];

					for (var i = 0; i < numScanLines; i++)
						scanLines[i] = stream.ReadUInt16();

					if (dataOffsetsOffset != stream.Position - 6)
						throw new Exception("Broken mix frame!");

					var dataOffsets = new int[numScanLines];

					for (var i = 0; i < numScanLines; i++)
						dataOffsets[i] = stream.ReadUInt16();

					if (patternsOffset != stream.Position - 6)
						throw new Exception("Broken mix file!");

					var patterns = stream.ReadBytes((int)numPatterns);
					var data = new SegmentStream(stream, compressedImageDataOffset + 6, dataSize);

					var writePosition = 0;

					for (var i = 0; i < Height; i++)
					{
						data.Position = dataOffsets[i];

						if (scanLines[i] == scanLines[i + 1])
							writePosition += Width;
						else
						{
							for (var j = scanLines[i]; j < scanLines[i + 1]; j += 2)
							{
								writePosition += patterns[j];
								var pixels = patterns[j + 1];
								Array.Copy(data.ReadBytes(pixels), 0, Pixels, writePosition, pixels);
								writePosition += pixels;
							}

							if (writePosition % Width != 0)
								writePosition += Width - writePosition % Width;
						}
					}

					break;

				default:
					throw new Exception("Unknown MixSprite type " + type);
			}
		}
	}
}
