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

namespace OpenRA.Mods.Example.Games.Z.SoundLoaders
{
	public class RawSoundLoader : ISoundLoader
	{
		sealed class RawSoundFormat : ISoundFormat
		{
			public int Channels => 1;
			public int SampleBits => 8;
			public int SampleRate => 11025;
			public float LengthInSeconds => (float)data.Length / SampleRate;

			readonly byte[] data;

			public RawSoundFormat(Stream stream)
			{
				data = stream.ReadAllBytes();
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
			if (stream is not FileStream fileStream ||
			    !fileStream.Name.EndsWith(".raw", StringComparison.OrdinalIgnoreCase))
			{
				sound = null;

				return false;
			}

			sound = new RawSoundFormat(stream);

			return true;
		}
	}
}
