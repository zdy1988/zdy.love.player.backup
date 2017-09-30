﻿using System.Runtime.InteropServices;
using Saar.FFmpeg.CSharp;

namespace Saar.FFmpeg.Structs {
	[StructLayout(LayoutKind.Sequential)]
	unsafe public struct AVPicture {
		public BytePtrArray8 Data;
		public fixed int Linesize[8];
	}
}
