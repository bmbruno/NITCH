.NET Integrated Template Compiler for HTML (Nitch)

About
-----



Using Nitch
-----------

Nitch is a command line tool, and accepts the following parameters at runtime:

	nitch.exe
	Shows help.

	nitch.exe /create
	Creates a default project structure for building websites. This is only a recommended default.

	nitch.exe /create "path-to-folder"
	Creates a default project structure for building websites at the provided location (absolute or relative).

	nitch.exe /build
	Builds all HTML files in and under the current directory.
	
	nitch.exe /build "path-to-folder"
	Builds all HTML files in and under the specified directory.

	nitch.exe /pathing [ rel | abs ]
	Defines how file paths should be rendered:
		'rel' - relative pathing is used; example: "../../images/logo.png"
		'abs' - absolute pathing is used; example: "/sections/core/images/logo.png"