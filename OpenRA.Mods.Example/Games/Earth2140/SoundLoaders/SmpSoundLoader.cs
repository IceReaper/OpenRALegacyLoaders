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
using OpenRA.Mods.Example.Games.Earth2140.FileFormats;

namespace OpenRA.Mods.Example.Games.Earth2140.SoundLoaders
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
			if (stream is not Wd.WdStream wdStream ||
			    !wdStream.Name.EndsWith(".smp", StringComparison.OrdinalIgnoreCase))
			{
				sound = null;

				return false;
			}

			sound = new SmpSoundFormat(stream.ReadAllBytes());

			return true;
		}
	}
}
