# GI Macro Helper

A helper program to help perform Mouse/Keyboard macros. Helpful for certain games.

 - JSON based configuration
 - Predefined macros
 - Macro calls from other macros
 - Shortcuts
 - Auto config reload on config file save
 - High precision timing for small delays (<50ms) (Works around Windows timing inconsistencies)

## How to use

### Editing the configuration

1. (Optional) Configure the `Config.json` file to meet your needs. You can check how to change this file [here](#how-to-configure).
2. Execute the `GiMacroHelper` program.
3. You can modify the configuration files while the program is running without having to restarting the program. It will automatically be reloaded on save.
4. Perform the shortcuts you defined to run the macros.

## How to configure

The configuration is defined in json files. There you can define macros and shortcuts to trigger them. There are pre-defined macros that you can use without re-defining them.

### The file structure
```
{
  "DefaultDelay": 50,
  "ShortCuts": {
    "=": "N3"
  },
  "Include": [
    {
      "Path": "./Include/StandardMacros.json",
      "Override": true
    }
  ],
  "Macros": {
    "N2": {
      "ShortCut": "Ctrl+/",
      "Actions": [
        {
          "Name": "Normal"
        },
        {
          "Name": "Delay",
          "Ms": 100
        },
        {
          "Name": "Key Down",
          "Char": "e"
        },
        {
          "Name": "Key Up",
          "Char": "e"
        }
      ]
    }
  }
}
```
- **"DefaultDelay"** (Optional) It sets the delay in milliseconds that will be placed between each action on every macro.
- **"ShortCuts"** (Optional) If you want to use a few shortcuts and swap the target macro easily.
- **"Include"** (Optional) It defines a list of additional files to be included. The macros in these files will be merged to this one. 
If these files have additional **"Include"** inside, these will be merged too. The included files have the same format as this.
  - **"Path"** The relative of absolute filepath to the file to include
  - **"Override"** (Optional) If this is `true` then if a macro is defined in this file with the same name as a macro in a previously
defined file, this one will override it.
- **"Macros"** It defines the macros for this file. Each name should be unique in this file.
  - **"ShortCut"** (Optional) The shortcut that will trigger this macro. It is defined with modifiers+character. E.g. "Ctrl+Alt+=" or "Ctrl+]" or "p".
  - **"Actions"** The collection of actions to be executed in the order needed.
    - **Name** The name of the macro/predefined action to perform at this step
    - **"Ms"** (Optional) Only if the action name is "Additional Delay", you define the amount of milliseconds to delay.
    - **"Char"** (Optional) Only if the action name is "Key Up" or "Key Down", you define the key char to press/release.

In the example above, the "Normal" action is defined inside the "./Include/StandardMacros.json". There it is defined as
Mouse Down/Mouse Up. In this way, instead of copying 2 actions when needing to perform a "Normal", you just use the name
"Normal" and it will perform the actions that defined in the "Normal" macro at this step (Mouse Down/Up).

### Pre-defined actions
 - **"Delay"** Wait for certain milliseconds before next action. If you use this, you should also define the **"Ms"** value.
 - **"Left Mouse Down"** Presses the left mouse button. It will remain pressed until a **"Left Mouse Up"** action is performed.
 - **"Left Mouse Up"** It releases a previously pressed left mouse button that was done via **"Left Mouse Down"**.
 - **"Right Mouse Down"** Presses the right mouse button. It will remain pressed until a **"Right Mouse Up"** action is performed.
 - **"Right Mouse Up"** It releases a previously pressed right mouse button that was done via **"Right Mouse Down"**.
 - **"Key Down"** Presses the specified key. It will remain pressed until a **"Key Up"** action is performed with the same **"Char"**.
   If you use this, you should also define the **"Char"** value.
 - **"Key Up"** It releases a previously pressed key that was done via **"Key Down"** with the same **"Char"**.
  If you use this, you should also define the **"Char"** value.

## License

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.


This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.


You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>