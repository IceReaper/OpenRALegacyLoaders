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
using System.Text;

namespace OpenRA.Mods.Example.Games.Kknd.FileFormats
{
	public class VbcFrame
	{
		static readonly ushort[] Patterns =
		{
			0x0660, 0xFF00, 0xCCCC, 0xF000, 0x8888, 0x000F, 0x1111, 0xFEC8,
			0x8CEF, 0x137F, 0xF731, 0xC800, 0x008C, 0x0013, 0x3100, 0xCC00,
			0x00CC, 0x0033, 0x3300, 0x0FF0, 0x6666, 0x00F0, 0x0F00, 0x2222,
			0x4444, 0xF600, 0x8CC8, 0x006F, 0x1331, 0x318C, 0xC813, 0x33CC,
			0x6600, 0x0CC0, 0x0066, 0x0330, 0xF900, 0xC88C, 0x009F, 0x3113,
			0x6000, 0x0880, 0x0006, 0x0110, 0xCC88, 0xFC00, 0x00CF, 0x88CC,
			0x003F, 0x1133, 0x3311, 0xF300, 0x6FF6, 0x0603, 0x08C6, 0x8C63,
			0xC631, 0x6310, 0xC060, 0x0136, 0x136C, 0x36C8, 0x6C80, 0x324C
		};

		readonly short globalMotionX;
		readonly short globalMotionY;

		readonly Stream video;
		readonly Stream colors;

		public readonly byte[] Audio;
		public readonly string Text;
		public readonly ushort? Duration;

		public VbcFrame(Stream stream)
		{
			var start = stream.Position;
			var frameSize = stream.ReadUInt32();
			var flags = stream.ReadUInt16();

			if ((flags & 0xff02) != 0)
				throw new Exception("VBC: Unsupported flags");

			if ((flags & 0x0001) != 0)
			{
				globalMotionX = stream.ReadInt16();
				globalMotionY = stream.ReadInt16();
			}

			if ((flags & 0x0004) != 0)
				Audio = stream.ReadBytes((int)(stream.ReadUInt32() - 4));

			if ((flags & 0x0008) != 0)
				video = new MemoryStream(stream.ReadBytes((int)(stream.ReadUInt32() - 4)));

			if ((flags & 0x0010) != 0)
				colors = new MemoryStream(stream.ReadBytes((int)(stream.ReadUInt32() - 4)));

			if ((flags & 0x0020) != 0)
				Duration = stream.ReadUInt16();

			if ((flags & 0x0040) != 0)
				Text = Encoding.ASCII.GetString(stream.ReadBytes((int)(stream.ReadUInt32() - 4)));

			if ((flags & 0x0080) != 0)
			{
				// TODO what is this?
			}

			if (frameSize != stream.Position - start)
				throw new Exception("VBC: Broken size");
		}

		public uint[,] ApplyFrame(uint[,] oldFrame, ref uint[] palette)
		{
			var width = oldFrame.GetLength(1);
			var height = oldFrame.GetLength(0);

			var newFrame = new uint[height, width];
			var shift = (globalMotionX + globalMotionY * width) * 4;

			if (shift >= 0)
				Buffer.BlockCopy(oldFrame, shift, newFrame, 0, oldFrame.Length * 4 - shift);
			else
				Buffer.BlockCopy(oldFrame, 0, newFrame, -shift, oldFrame.Length * 4 + shift);

			if (colors != null)
			{
				colors.Position = 0;

				var firstIndex = colors.ReadUInt8();
				var numColors = (colors.ReadUInt8() - 1) & 0xff;

				for (var i = 0; i <= numColors; i++)
					palette[firstIndex + i] = (uint)((0xff << 24) | (colors.ReadUInt8() << 16) |
					                                 (colors.ReadUInt8() << 8) | (colors.ReadUInt8() << 0));
			}

			if (video == null)
				return newFrame;

			video.Position = 0;

			for (var by = 0; by < height / 4; by++)
			for (var bx = 0; bx < width / 4;)
			{
				var blockTypes = video.ReadUInt8();

				for (var i = 0; i < 4; i++, bx++)
				{
					var blockType = (blockTypes >> (6 - i * 2)) & 0x03;

					switch (blockType)
					{
						case 0:
						{
							break;
						}

						case 1:
						{
							var motion = video.ReadUInt8();

							if (motion == 0)
							{
								for (var y = 0; y < 4; y++)
								for (var x = 0; x < 4; x++)
									newFrame[by * 4 + y, bx * 4 + x] = palette[video.ReadUInt8()];
							}
							else
							{
								var motionX = ((motion & 0xf) ^ 8) - 8;
								var motionY = ((motion >> 4) ^ 8) - 8;

								for (var y = 0; y < 4; y++)
								for (var x = 0; x < 4; x++)
								{
									newFrame[by * 4 + y, bx * 4 + x] = oldFrame[by * 4 + y + globalMotionY + motionY,
										bx * 4 + x + globalMotionX + motionX];
								}
							}

							break;
						}

						case 2:
						{
							var color = palette[video.ReadUInt8()];

							for (var y = 0; y < 4; y++)
							for (var x = 0; x < 4; x++)
								newFrame[by * 4 + y, bx * 4 + x] = color;

							break;
						}

						case 3:
						{
							var patternData = video.ReadUInt8();
							var patternType = patternData >> 6;
							var pattern = Patterns[patternData & 0x3f];

							switch (patternType)
							{
								case 0:
								{
									var pixel0 = palette[video.ReadUInt8()];
									var pixel1 = palette[video.ReadUInt8()];

									for (var y = 0; y < 4; y++)
									for (var x = 0; x < 4; x++)
										newFrame[by * 4 + y, bx * 4 + x] =
											((pattern >> (y * 4 + x)) & 1) == 0 ? pixel0 : pixel1;

									break;
								}

								case 1:
								{
									var pixel = palette[video.ReadUInt8()];

									for (var y = 0; y < 4; y++)
									for (var x = 0; x < 4; x++)
									{
										if (((pattern >> (y * 4 + x)) & 1) == 1)
											newFrame[by * 4 + y, bx * 4 + x] = pixel;
									}

									break;
								}

								case 2:
								{
									var pixel = palette[video.ReadUInt8()];

									for (var y = 0; y < 4; y++)
									for (var x = 0; x < 4; x++)
									{
										if (((pattern >> (y * 4 + x)) & 1) == 0)
											newFrame[by * 4 + y, bx * 4 + x] = pixel;
									}

									break;
								}
							}

							break;
						}
					}
				}
			}

			return newFrame;
		}
	}
}
