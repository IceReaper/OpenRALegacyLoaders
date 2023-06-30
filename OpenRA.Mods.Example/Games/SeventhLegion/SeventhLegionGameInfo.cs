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
using OpenRA.Mods.Example.Games.SeventhLegion.SoundLoaders;
using OpenRA.Mods.Example.Games.SeventhLegion.SpriteLoaders;
using OpenRA.Video;

namespace OpenRA.Mods.Example.Games.SeventhLegion
{
	public class SeventhLegionGameInfo : IGameInfo
	{
		public string Folder => "7th_legion";

		public IEnumerable<IPackageLoader> PackageLoaders => Array.Empty<IPackageLoader>();
		public IEnumerable<ISoundLoader> SoundLoaders => new ISoundLoader[] { new SmpSoundLoader(), new WavLoader() };
		public IEnumerable<ISpriteLoader> SpriteLoaders => new[] { new BimSpriteLoader() };
		public IEnumerable<IVideoLoader> VideoLoaders => Array.Empty<IVideoLoader>();

		public IEnumerable<string> Packages => new[] { "gfx", "sfx" };

		public IEnumerable<string> AudioExtensions => new[] { ".smp", ".wav" };
		public IEnumerable<string> SpriteExtensions => new[] { ".bim" };
		public IEnumerable<string> VideoExtensions => Array.Empty<string>();
	}
}
