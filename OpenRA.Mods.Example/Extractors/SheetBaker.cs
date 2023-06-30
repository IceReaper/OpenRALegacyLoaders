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
using System.Linq;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Extractors
{
	public class SheetBaker
	{
		sealed class Entry
		{
			public readonly Rectangle Rectangle;
			public readonly byte[] Pixels;

			public Entry(Rectangle rectangle, byte[] pixels)
			{
				Rectangle = rectangle;
				Pixels = pixels;
			}
		}

		readonly List<Entry> frames = new();
		readonly int channels;

		public SheetBaker(int channels)
		{
			this.channels = channels;
		}

		public void Add(Rectangle rectangle, byte[] frame)
		{
			frames.Add(new Entry(rectangle, frame));
		}

		public byte[] Bake(out int width, out int height, out int offsetX, out int offsetY)
		{
			if (!frames.Any())
			{
				width = 1;
				height = 1;
				offsetX = 0;
				offsetY = 0;

				return new byte[channels];
			}

			var frameOffset = new int2(frames.Min(f => f.Rectangle.Left), frames.Min(f => f.Rectangle.Top));
			var frameWidth = frames.Max(f => f.Rectangle.Right - frameOffset.X);
			var frameHeight = frames.Max(f => f.Rectangle.Bottom - frameOffset.Y);
			var frameSize = new Size(frameWidth, frameHeight);
			var frameRectangle = new Rectangle(frameOffset, frameSize);
			var framesX = (int)Math.Ceiling(Math.Sqrt(frames.Count));
			var framesY = (int)Math.Ceiling(frames.Count / (float)framesX);

			width = framesX * frameRectangle.Width;
			height = framesY * frameRectangle.Height;
			offsetX = frameRectangle.X;
			offsetY = frameRectangle.Y;

			var data = new byte[width * height * channels];

			for (var i = 0; i < frames.Count; i++)
			{
				var frame = frames[i];
				var frameX = i % framesX * frameRectangle.Width - frameRectangle.X + frame.Rectangle.X;

				for (var y = 0; y < frame.Rectangle.Height; y++)
				{
					var frameY = i / framesX * frameRectangle.Height + y - frameRectangle.Y + frame.Rectangle.Y;
					var sourceIndex = y * frame.Rectangle.Width * channels;
					var targetIndex = (frameY * width + frameX) * channels;
					var length = frame.Rectangle.Width * channels;

					Array.Copy(frame.Pixels, sourceIndex, data, targetIndex, length);
				}
			}

			return data;
		}
	}
}
