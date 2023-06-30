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
using OpenRA.Video;

namespace OpenRA.Mods.Example.Games.Kknd.FileFormats
{
	public class Vbc : IVideo
	{
		public ushort FrameCount => (ushort)frames.Length;
		public byte Framerate { get; }
		public ushort Width { get; }
		public ushort Height { get; }
		public byte[] CurrentFrameData { get; private set; }
		public string TextData { get; private set; }
		public int CurrentFrameIndex { get; private set; }
		public bool HasAudio => true;
		public byte[] AudioData { get; }
		public int AudioChannels => 1;
		public int SampleBits { get; }
		public int SampleRate { get; }

		readonly VbcFrame[] frames;
		uint[,] currentFrame;
		uint[] palette;
		readonly int stride;

		public Vbc(Stream stream)
		{
			if (Encoding.ASCII.GetString(stream.ReadBytes(4)) != "SIFF")
				throw new Exception("VBC: Invalid vbc (invalid SIFF section)");

			var fileSize = BitConverter.ToUInt32(stream.ReadBytes(4).Reverse().ToArray());

			if (fileSize + 8 != stream.Length)
				throw new Exception("VBC: Broken file size");

			if (Encoding.ASCII.GetString(stream.ReadBytes(4)) != "VBV1")
				throw new Exception("VBC: Invalid vbc (not VBV1)");

			if (Encoding.ASCII.GetString(stream.ReadBytes(4)) != "VBHD")
				throw new Exception("VBC: Invalid vbc (not VBHD)");

			var headerSize = BitConverter.ToUInt32(stream.ReadBytes(4).Reverse().ToArray());
			var version = stream.ReadUInt16();

			if (version != 1)
				throw new Exception("VBC: Unknown version");

			Width = stream.ReadUInt16();
			Height = stream.ReadUInt16();
			var unk1 = stream.ReadUInt32();

			if (unk1 != 0)
				throw new Exception("VBC: Unknown value");

			var frames = stream.ReadUInt16();
			SampleBits = stream.ReadUInt16();
			SampleRate = stream.ReadUInt16();

			if (stream.ReadBytes((int)(headerSize - 16)).Any(b => b != 0x00))
				throw new Exception("VBC: Unknown value");

			if (Width == 640 && Height == 240)
			{
				Height = 480;
				stride = 2;
			}
			else
				stride = 1;

			CurrentFrameData = new byte[Exts.NextPowerOf2(Width) * Exts.NextPowerOf2(Height) * 4];
			currentFrame = new uint[Height / stride, Width];
			palette = new uint[256];

			if (Encoding.ASCII.GetString(stream.ReadBytes(4)) != "BODY")
				throw new Exception("VBC: Invalid chunk");

			var bodySize = BitConverter.ToUInt32(stream.ReadBytes(4).Reverse().ToArray());
			var start = stream.Position;

			this.frames = new VbcFrame[frames];

			for (var i = 0; i < frames; i++)
				this.frames[i] = new VbcFrame(stream);

			Framerate = (byte)(1000 / (this.frames.Sum(frame => frame.Duration ?? 1) / this.frames.Length));

			var audio = new MemoryStream();

			foreach (var frame in this.frames.Where(f => f.Audio != null))
				audio.Write(frame.Audio);

			AudioData = audio.ToArray();
			TextData = string.Join("", this.frames.Select(e => e.Text).Where(e => e != null).ToArray());

			if (bodySize != stream.Position - start)
				throw new Exception("VBC: Broken size");

			Reset();
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
			if (CurrentFrameIndex == 0)
			{
				currentFrame = new uint[Height / stride, Width];
				Array.Fill(palette, uint.MinValue);
			}

			currentFrame = frames[CurrentFrameIndex].ApplyFrame(currentFrame, ref palette);

			for (var y = 0; y < Height / stride; y++)
			for (var i = 0; i < stride; i++)
			{
				Buffer.BlockCopy(
					currentFrame,
					y * Width * 4,
					CurrentFrameData,
					(y * stride + i) * Exts.NextPowerOf2(Width) * 4,
					Width * 4);
			}
		}
	}
}
