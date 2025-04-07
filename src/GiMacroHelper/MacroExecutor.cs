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

using System.Collections.Concurrent;
using System.Diagnostics;
using Desktop.Robot.Clicks;
using Desktop.Robot.Linux;
using GiMacroHelper.Model;

namespace GiMacroHelper
{
	/// <summary>
	///     Executes macros.
	/// </summary>
	internal class MacroExecutor : IDisposable
	{
		private readonly IClick _leftClick = Mouse.LeftButton();
		private readonly BlockingCollection<LogInstance> _logQueue = new();
		private readonly Thread _logThread;
		private readonly ModelLoader _modelLoader;
		private readonly Dictionary<string, PreDefinedAction> _predefinedActions;
		private readonly IClick _rightClick = Mouse.RightButton();
		private readonly Robot _robot = new();

		public MacroExecutor(ModelLoader modelLoader)
		{
			_modelLoader = modelLoader;
			_predefinedActions = new Dictionary<string, PreDefinedAction>(StringComparer.OrdinalIgnoreCase)
			{
				["delay"] = OnAdditionalDelay,
				["left mouse down"] = OnLeftMouseDown,
				["left mouse up"] = OnLeftMouseUp,
				["right mouse down"] = OnRightMouseDown,
				["right mouse up"] = OnRightMouseUp,
				["key down"] = OnKeyDown,
				["key up"] = OnKeyUp,
			};

			_logThread = new Thread(LogProcedure)
			{
				Name = "[Log Procedure]",
				IsBackground = true,
			};
			_logThread.Start();
		}

		public void Dispose()
		{
			_logQueue.CompleteAdding();
			_logThread.Join();
			_logQueue.Dispose();
		}

		/// <summary>
		///     Executes the specified macro.
		/// </summary>
		/// <param name="macro">The macro to execute.</param>
		public void Execute(Macro macro)
		{
			var sp = Stopwatch.StartNew();
			var indent = 0;
			DoExecute(sp, ref indent, macro);
			sp.Stop();
		}

		private void Log(double timeStamp, string message)
		{
			_logQueue.Add(new LogInstance(timeStamp, message));
		}

		private void LogIntended(double timeStamp, int intend, string message)
		{
			_logQueue.Add(new LogInstance(timeStamp, intend, message));
		}

		private void LogProcedure()
		{
			while (!_logQueue.IsCompleted)
			{
				try
				{
					var log = _logQueue.Take();
					Console.WriteLine(
						log.Intend is > 0
							? $"[{log.TimeStamp,6:F2}]{new string('|', log.Intend ?? 0)}{log.Message}"
							: $"[{log.TimeStamp,6:F2}]{log.Message}");
				}
				catch (InvalidOperationException)
				{
					return;
				}
			}
		}

		private void DoExecute(Stopwatch macroStopWatch, ref int intend, Macro macro)
		{
			var thisBatchSp = Stopwatch.StartNew();
			LogIntended(macroStopWatch.Elapsed.TotalMilliseconds, intend++, $"[-]Executing: {macro.Name}");
			foreach (var action in macro.Actions)
			{
				try
				{
					var name = action.Name;
					if (_predefinedActions.TryGetValue(name, out var actionToExecute))
					{
						actionToExecute(macroStopWatch, intend, action);
					}
					else
					{
						if (_modelLoader.Model.Macros.TryGetValue(name, out var secondaryMacroAction))
						{
							DoExecute(macroStopWatch, ref intend, secondaryMacroAction);
						}
						else
						{
							Console.WriteLine(
								$"Error: '{action.Name}' action macro was referenced by '{macro.Name}' but was not found");
							return;
						}
					}
				}
				finally
				{
					if (_modelLoader.Model.DefaultDelay is > 0)
					{
						DelayHelper.Sleep(_modelLoader.Model.DefaultDelay.Value);
					}
				}
			}

			thisBatchSp.Stop();
			LogIntended(
				macroStopWatch.Elapsed.TotalMilliseconds,
				--intend,
				$"[+]Completed: {macro.Name} ({thisBatchSp.Elapsed.TotalMilliseconds:F2}ms)");
		}

		private void OnKeyUp(Stopwatch macroStopwatch, int indent, MacroAction a)
		{
			if (string.IsNullOrEmpty(a.Char))
			{
				Console.WriteLine("Error: 'Key Up' action without 'Char' field");
				return;
			}

			_robot.KeyUp(a.Char.First());
		}

		private void OnKeyDown(Stopwatch macroStopwatch, int indent, MacroAction a)
		{
			if (string.IsNullOrEmpty(a.Char))
			{
				Console.WriteLine("Error: 'Key Down' action without 'Char' field");
				return;
			}

			_robot.KeyDown(a.Char.First());
		}

		private void OnRightMouseUp(Stopwatch macroStopwatch, int indent, MacroAction a)
		{
			_robot.MouseUp(_rightClick);
		}

		private void OnRightMouseDown(Stopwatch macroStopwatch, int indent, MacroAction a)
		{
			_robot.MouseDown(_rightClick);
		}

		private void OnLeftMouseUp(Stopwatch macroStopwatch, int indent, MacroAction a)
		{
			_robot.MouseUp(_leftClick);
		}

		private void OnLeftMouseDown(Stopwatch macroStopwatch, int indent, MacroAction a)
		{
			_robot.MouseDown(_leftClick);
		}

		private void OnAdditionalDelay(Stopwatch macroStopwatch, int indent, MacroAction a)
		{
			if (a.Ms == null)
			{
				Console.WriteLine("Error: 'Additional delay' action without 'Ms' field with the time to delay");
				return;
			}

			var sp = Stopwatch.StartNew();
			DelayHelper.Sleep((a.Ms ?? 0) + (_modelLoader.Model.DefaultDelay ?? 0));
			sp.Stop();
			LogIntended(
				macroStopwatch.Elapsed.TotalMilliseconds,
				indent,
				$"[+]Slept: {a.Ms:F2}ms (actual: {sp.Elapsed.TotalMilliseconds:F2}ms)");
		}

		private delegate void PreDefinedAction(Stopwatch sp, int intend, MacroAction action);

		private class LogInstance
		{
			public LogInstance(double timeStamp, int intend, string message)
			{
				TimeStamp = timeStamp;
				Intend = intend;
				Message = message;
			}

			public LogInstance(double timeStamp, string message)
			{
				TimeStamp = timeStamp;
				Message = message;
			}

			public double TimeStamp { get; }

			public int? Intend { get; }

			public string Message { get; }
		}
	}
}