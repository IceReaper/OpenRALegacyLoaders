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
using OpenRA.Video;

namespace OpenRA.Mods.Example.Games.Generic.FileFormats
{
	public class Flc : IVideo
	{
		public ushort FrameCount { get; }
		public byte Framerate { get; }
		public ushort Width { get; }
		public ushort Height { get; }
		public byte[] CurrentFrameData { get; }

		public int CurrentFrameIndex { get; private set; }

		public bool HasAudio => false;
		public byte[] AudioData => Array.Empty<byte>();
		public int AudioChannels => 0;
		public int SampleBits => 0;
		public int SampleRate => 0;

		readonly byte[][] frames;
		readonly byte[] palette = new byte[1024];

		public Flc(Stream stream)
		{
			var size = stream.ReadUInt32();

			if (stream.ReadUInt16() != 0xaf12)
				throw new Exception("Broken flc file!");

			FrameCount = stream.ReadUInt16();
			Width = stream.ReadUInt16();
			Height = stream.ReadUInt16();

			frames = new byte[FrameCount + 1][];
			CurrentFrameData = new byte[Exts.NextPowerOf2(Height) * Exts.NextPowerOf2(Width) * 4];

			var depth = stream.ReadUInt16();

			if (depth != 8)
				throw new Exception("Broken flc file!");

			var flags = stream.ReadUInt16();

			if (flags != 3)
				throw new Exception("Broken flc file!");

			Framerate = (byte)(stream.ReadUInt32() / 2);

			if (stream.ReadBytes(2).Any(b => b != 0x00))
				throw new Exception("Broken flc file!");

			stream.ReadUInt32(); // created
			stream.ReadUInt32(); // creator
			stream.ReadUInt32(); // updated
			stream.ReadUInt32(); // updater
			var aspectX = stream.ReadUInt16();
			var aspectY = stream.ReadUInt16();

			if (aspectX > 1 || aspectY > 1)
				throw new Exception("Broken flc file!");

			if (stream.ReadBytes(38).Any(b => b != 0x00))
				throw new Exception("Broken flc file!");

			stream.ReadUInt32(); // frame1
			stream.ReadUInt32(); // frame2

			if (stream.ReadBytes(40).Any(b => b != 0x00))
				throw new Exception("Broken flc file!");

			for (var i = 0; i < frames.Length; i++)
				frames[i] = stream.ReadBytes((int)(stream.ReadUInt32() - 4));

			if (stream.Position != size)
				throw new Exception("Broken flc file!");

			ApplyFrame();
		}

		public void AdvanceFrame()
		{
			CurrentFrameIndex++;
			ApplyFrame();
		}

		public void Reset()
		{
			CurrentFrameIndex = 0;
			ApplyFrame();
		}

		void ApplyFrame()
		{
			var stream = new MemoryStream(frames[CurrentFrameIndex]);

			if (stream.ReadUInt16() != 0xf1fa)
				throw new Exception("Broken flc file!");

			var chunks = stream.ReadUInt16();
			var delay = stream.ReadUInt16(); // TODO use this somehow

			if (stream.ReadBytes(6).Any(b => b != 0x00))
				throw new Exception("Broken flc frame!");

			for (var i = 0; i < chunks; i++)
			{
				var chunkStart = stream.Position;
				var chunkSize = stream.ReadUInt32();
				var chunkType = stream.ReadUInt16();

				switch (chunkType)
				{
					case 4:
					{
						var numChunks = stream.ReadUInt16();

						for (var chunk = 0; chunk < numChunks; chunk++)
						{
							var skipColors = stream.ReadUInt8();
							var numColors = (int)stream.ReadUInt8();

							if (numColors == 0)
								numColors = 256;

							for (var color = 0; color < numColors; color++)
							{
								Array.Copy(stream.ReadBytes(3).Reverse().ToArray(), 0, palette,
									(skipColors + color) * 4, 3);
								palette[(skipColors + color) * 4 + 3] = 0xff;
							}
						}

						break;
					}

					case 7:
					{
						var numLines = stream.ReadUInt16();
						var y = 0;

						while (numLines > 0)
						{
							var numChunks = stream.ReadInt16();

							if (numChunks > 0)
							{
								numLines--;
								var x = 0;

								for (var chunk = 0; chunk < numChunks; chunk++)
								{
									x += stream.ReadUInt8();
									var count = (sbyte)stream.ReadUInt8();

									if (count > 0)
									{
										for (var j = 0; j < count; j++)
										{
											Draw(x++, y, stream.ReadUInt8());
											Draw(x++, y, stream.ReadUInt8());
										}
									}
									else
									{
										var index1 = stream.ReadUInt8();
										var index2 = stream.ReadUInt8();

										for (var j = 0; j < -count; j++)
										{
											Draw(x++, y, index1);
											Draw(x++, y, index2);
										}
									}
								}

								y++;
							}
							else
								y += -numChunks;
						}

						break;
					}

					case 12:
					{
						var firstLine = stream.ReadUInt16();
						var numLines = stream.ReadUInt16();

						for (var y = firstLine; y < firstLine + numLines; y++)
						{
							var numChunks = stream.ReadUInt8();
							var x = 0;

							for (var chunk = 0; chunk < numChunks; chunk++)
							{
								x += stream.ReadUInt8();
								var count = (sbyte)stream.ReadUInt8();

								if (count < 0)
								{
									var index = stream.ReadUInt8();

									for (var j = 0; j < -count; j++)
										Draw(x++, y, index);
								}
								else
								{
									for (var j = 0; j < count; j++)
										Draw(x++, y, stream.ReadUInt8());
								}
							}
						}

						break;
					}

					case 15:
					{
						for (var y = 0; y < Height; y++)
						{
							var numChunks = stream.ReadUInt8();
							var x = 0;

							for (var chunk = 0; chunk < numChunks; chunk++)
							{
								var count = (sbyte)stream.ReadUInt8();

								if (count > 0)
								{
									var index = stream.ReadUInt8();

									for (var j = 0; j < count; j++)
										Draw(x++, y, index);
								}
								else
								{
									for (var j = 0; j < -count; j++)
										Draw(x++, y, stream.ReadUInt8());
								}
							}
						}

						break;
					}

					case 16:
						for (var y = 0; y < Height; y++)
						for (var x = 0; x < Width; x++)
							Draw(x, y, stream.ReadUInt8());

						break;

					case 18:
						// TODO this is a thumbnail image.
						stream.Position += chunkSize - 6;

						break;

					default:
						throw new Exception("Broken flc frame!");
				}

				if (stream.Position - chunkStart != chunkSize)
					stream.Position += chunkStart + chunkSize - stream.Position;
			}
		}

		void Draw(int x, int y, int index)
		{
			Array.Copy(palette, index * 4, CurrentFrameData, (y * Exts.NextPowerOf2(Width) + x) * 4, 4);
		}
	}
}
