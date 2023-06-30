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

namespace OpenRA.Mods.Example.Games.Dominion.FileFormats
{
	// TODO this is no file format. move into SprSpriteLoader !
	public class Spr
	{
		public readonly SprFrame[] Frames;
		public readonly int2[] Offsets;
		public readonly Pal Palette;

		public Spr(ZeroBin.ZeroBinStream stream, uint id)
		{
			if (stream.Container.Animations.TryGetValue(id, out var animation))
			{
				Frames = new SprFrame[animation.Frames.Length];
				Offsets = new int2[animation.Frames.Length];

				for (var i = 0; i < animation.Frames.Length; i++)
				{
					var frameId = animation.Frames[i];

					if (stream.Container.References.TryGetValue(frameId, out var reference))
						frameId = reference.Frame;
					else if (stream.Container.Unks.TryGetValue(frameId, out var unk))
						frameId = animation.Frames[unk.Unk1]; // TODO is this maybe for reversing the animation?

					if (!stream.Container.Frames.TryGetValue(frameId, out var frame))
						throw new Exception("SPR: Unknown file type!");

					Frames[i] = frame;

					Offsets[i] = reference != null
						? new int2(reference.OffsetX, reference.OffsetY)
						: new int2(frame.OffsetX, frame.OffsetY);
				}
			}
			else if (stream.Container.Frames.TryGetValue(id, out var frame))
			{
				Frames = new[] { frame };
				Offsets = new[] { new int2(0, 0) };
			}
			else
				throw new Exception("SPR: Unknown file type!");

			// TODO find mapping!
			Palette = stream.Container.Palettes[54637];
		}
	}
}
