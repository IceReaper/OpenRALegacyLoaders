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
using OpenRA.Mods.Example.Games.JurassicWar.FileFormats;

namespace OpenRA.Mods.Example.Games.JurassicWar.SoundLoaders
{
	public class TrwSoundLoader : ISoundLoader
	{
		sealed class TrwSoundFormat : ISoundFormat
		{
			public int Channels { get; }
			public int SampleBits { get; }
			public int SampleRate { get; }
			public float LengthInSeconds => (float)data.Length / SampleRate;

			readonly byte[] data;

			public TrwSoundFormat(Trw.TrwStream stream)
			{
				Channels = stream.Header.Channels;
				SampleBits = stream.Header.SampleBits;
				SampleRate = (int)stream.Header.SampleRate;
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
			if (stream is not Trw.TrwStream trwStream)
			{
				sound = null;

				return false;
			}

			sound = new TrwSoundFormat(trwStream);

			return true;
		}
	}
}
