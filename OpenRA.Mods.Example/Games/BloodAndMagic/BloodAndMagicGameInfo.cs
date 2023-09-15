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
using System.Collections.Generic;
using OpenRA.FileSystem;
using OpenRA.Graphics;
using OpenRA.Mods.Common.AudioLoaders;
using OpenRA.Mods.Example.Games.BloodAndMagic.PackageLoaders;
using OpenRA.Mods.Example.Games.BloodAndMagic.SpriteLoaders;
using OpenRA.Video;

namespace OpenRA.Mods.Example.Games.BloodAndMagic
{
	public class BloodAndMagicGameInfo : IGameInfo
	{
		public string Folder => "blood_and_magic";

		public IEnumerable<IPackageLoader> PackageLoaders => new IPackageLoader[] { new StfPackageLoader() };
		public IEnumerable<ISoundLoader> SoundLoaders => new ISoundLoader[] { new WavLoader() };

		public IEnumerable<ISpriteLoader> SpriteLoaders => new ISpriteLoader[]
		{
			new AniSpriteLoader()
		};

		public IEnumerable<IVideoLoader> VideoLoaders => Array.Empty<IVideoLoader>();

		public IEnumerable<string> Packages => new[] { "cines.stf", "main.stf" };

		public IEnumerable<string> AudioExtensions => new[] { ".wav" };
		public IEnumerable<string> SpriteExtensions => new[] { ".spr" };
		public IEnumerable<string> VideoExtensions => Array.Empty<string>();
	}
}
