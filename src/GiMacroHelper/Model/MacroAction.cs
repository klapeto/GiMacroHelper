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
	///     Defines a macro action.
	/// </summary>
	public class MacroAction
	{
		/// <summary>
		///     The name of the macro/action to perform.
		/// </summary>
		/// <remarks>It can be an existing macro name or a pre-defined action name</remarks>
		/// <example>"Normal Attack"</example>
		public string Name { get; init; } = string.Empty;

		/// <summary>
		///     The amount of milliseconds to delay in case the action is about delaying.
		/// </summary>
		public int? Ms { get; init; }

		/// <summary>
		///     The character to press in case the action is related to Keyboard key press.
		/// </summary>
		public string? Char { get; init; }
	}
}