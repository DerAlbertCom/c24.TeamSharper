# TeamSharper

TeamSharper will help you managing and deploying team settings for ReSharper (v7+). It creates or modifies  the *.DotSettings files for every solution beneath a common root folder hierarchy. To learn more about ReSharper option layers, see [this entry on the JetBrains .NET Tools Blog](http://blogs.jetbrains.com/dotnet/2012/09/save-or-save-to-in-resharper-options/).

## Usage

### Settings file

All actions of TeamSharper are controlled using a central settings file containing the layers that will be included in the "Team Settings" section of your ReSharper options. This file is JSON formatted and might look similar to this:

	{
	    "layers": [{
	        "id": "F0B47A2A84C84BA38AC884675483C3DE",
	        "relativePath": "Subfolder1/Basic-Coding-Guidelines-All-Languages.DotSettings",
	        "relativePriority": 1
	    }, {
	        "id": "B3EEAD2BD91042EC8E45199EA4B8BFFB",
	        "relativePath": "Subfolder2/Advanced-CSharp-Coding-Guidelines.DotSettings",
	        "relativePriority": 2
	    }, {
	        "id": "436A1EC43C0F47229CE331DBA473D0C9",
	        "relativePath": "Subfolder3/Special-JavaScript-Coding-Guidelines.DotSettings",
	        "relativePriority": 3
	    }]
	}

The `id` is just a simple GUID (must be unique for each layer), `relativePath` is relative to the location of the settings file, `relativePriority` controlls the precedence of option imports.

### Command line options

Once you have a valid settings file at hand, you can execute TeamSharper using the following command line options:

* *-s, --setting* Path to the settings file
* *-d, --directory* Path to the root directory containing your Visual Studio solution files
* *-t, --test* Use this flag to execute as a "dry run" (no files will be harmed) 

Example:

	TeamSharper.exe -s "..\my-settings.json" -d "..\..\src\trunk\"

This will go up one directory to the `my-settings.json` file, read the configuration, and apply it to all solutions  beneath the `trunk` directory.

### How it works

TeamSharper creates or modifies a `\*.sln.DotSettings` file for every Solution (`\*.sln`) file containing links to the referenced `*.DotSettings` files. These links are saved as relative Paths. There is also an absolute path which ReSharper will happily ignore if it's invalid (and TeamSharper will provide an invalid one). This is because, if ReSharper only follows the relative path, you can check in all the settings into your source control system, and it will work even if every team member checks out the code to a different location. That's certainly not the most solid solution but the only one we could think of that allows us to automatically **deploy** and **update** ReSharper settings for the entire team (without resorting to DrobBox or similar services).

-----

License: MIT - Copyright (c) 2013 CHECK24


