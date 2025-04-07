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

namespace GiMacroHelper.Model
{
	/// <summary>
	///     The root definition of macros configuration.
	/// </summary>
	internal class MacrosRoot
	{
		public MacrosRoot(
			Dictionary<string, Macro>? macros,
			int? defaultDelay,
			Include[]? include,
			Dictionary<string, string>? shortCuts)
		{
			Macros = macros ?? new Dictionary<string, Macro>();
			ShortCuts = shortCuts ?? new Dictionary<string, string>();
			DefaultDelay = defaultDelay;
			Include = include;
		}

		/// <summary>
		///     The default delay to add between each action.
		/// </summary>
		/// <remarks>This delay will always be performed.</remarks>
		public int? DefaultDelay { get; }

		/// <summary>
		///     The macros defined.
		/// </summary>
		public Dictionary<string, Macro> Macros { get; }

		/// <summary>
		///     Additional macros to be included from other files.
		/// </summary>
		public Include[]? Include { get; }

		/// <summary>
		///     General shortcuts.
		/// </summary>
		public Dictionary<string, string> ShortCuts { get; }
	}
}