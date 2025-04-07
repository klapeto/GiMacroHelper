// =========================================================================
// 
// GiMacroHelper
// 
// Copyright (C) 2025 Ioannis Panagiotopoulos
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>
// =========================================================================

using System.Diagnostics;

namespace GiMacroHelper
{
	internal static class DelayHelper
	{
		public static void Sleep(double milliseconds)
		{
			if (milliseconds <= 0.0)
			{
				return;
			}

			// Avoid Thread.Sleep(). Sleeps on windows has significant overhead
			// so try to spin as much as possible. Linux does not seem to have this problem.
			SpinForMs(milliseconds);
		}

		private static void SpinForMs(double ms)
		{
			SpinWait spin = default;
			var sp = Stopwatch.StartNew();
			while (sp.ElapsedMilliseconds < ms)
			{
				spin.SpinOnce();
			}
		}
	}
}