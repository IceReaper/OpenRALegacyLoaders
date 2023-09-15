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
using OpenRA.Mods.Example.Games.Z.PackageLoaders;
using OpenRA.Mods.Example.Games.Z.SoundLoaders;
using OpenRA.Mods.Example.Games.Z.SpriteLoaders;
using OpenRA.Video;

namespace OpenRA.Mods.Example.Games.Z
{
	public class ZGameInfo : IGameInfo
	{
		public string Folder => "z";

		public IEnumerable<IPackageLoader> PackageLoaders => new IPackageLoader[] { new ExpPackageLoader() };
		public IEnumerable<ISoundLoader> SoundLoaders => new ISoundLoader[] { new RawSoundLoader() };

		public IEnumerable<ISpriteLoader> SpriteLoaders => new ISpriteLoader[]
		{
			new ExpSpriteLoader(), new BlkSpriteLoader(), new LbmSpriteLoader()
		};

		public IEnumerable<IVideoLoader> VideoLoaders => Array.Empty<IVideoLoader>();

		public IEnumerable<string> Packages => new[]
		{
			"door.exp", "headfx.exp", "main.exp", "sheadfx.exp", "wardata.exp", "audio", "."
		};

		public IEnumerable<string> AudioExtensions => new[] { ".raw" };
		public IEnumerable<string> SpriteExtensions => new[] { ".exp", ".blk", ".lbm" };
		public IEnumerable<string> VideoExtensions => Array.Empty<string>();
	}
}
