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

namespace OpenRA.Mods.Example.Games.SeventhLegion.SoundLoaders
{
	public class SmpSoundLoader : ISoundLoader
	{
		sealed class SmpSoundFormat : ISoundFormat
		{
			public int Channels => 1;
			public int SampleBits => 8;
			public int SampleRate => 16000;
			public float LengthInSeconds => (float)data.Length / SampleRate;

			readonly byte[] data;

			public SmpSoundFormat(byte[] data)
			{
				this.data = data;
			}

			public Stream GetPCMInputStream()
			{
				return new MemoryStream(data);
			}

			public void Dispose()
			{
			}
		}

		public bool TryParseSound(Stream stream, out ISoundFormat sound)
		{
			if (stream is not FileStream fileStream || !fileStream.Name.EndsWith(".smp", StringComparison.OrdinalIgnoreCase))
			{
				sound = null;

				return false;
			}

			if (stream.ReadUInt16() == 0x2e01)
				stream.Position += 78;
			else
				stream.Position -= 2;

			// TODO there may be 80 bytes header crap, if it starts with 0x 01 2e 5c.
			sound = new SmpSoundFormat(stream.ReadAllBytes());

			return true;
		}
	}
}
