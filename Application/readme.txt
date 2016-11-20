.NET Integrated Template Compiler for HTML (Nitch)
v1.00

About
-----

	Nitch is a static site generator that compiles HTML files based on a basic parent-child relationship. "Master" pages are bound to "child" pages via a simple token syntax.

Getting Started
---------------

	1) Run "nitch.exe /create" to get a blank sample site set up. This will create a folder called "new_website_project" and "build.bat".
	2) Execute "build.bat" to compile the "new_website_project" sample site. A new folder called "_nitch" will be created inside the "new_website_project" directory - this is the compiled website.

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

Page Types
----------

	Master Files

		Master files maintain a parent relationship with content files. They should typically contains top-level HTML elements (<html>, <head>, <body>, etc.).

	Content files

		Content files are child pages to the master files.

Tokens
------

	Master
		{{master:/location/of/file.html}}
	
		Defines what master file belongs to the child page.
		Must appear as the first line of the file, before any markup. Must appear only once.
		Must be absolute path from the project root.

	File
		{{file:/path/to/file.png}}

		Defines a file path to a resource (CSS, JS, images, etc.).
		Must be absolute path from the project root.

	Placeholder
		{{placeholder:name}}

		Defines a placeholder for content on a Master page.

	Content
		{{content:name}} {{content:end}}
	
		Defines a region for content to be inserted into a Master page.
		'name' must match a placeholder.
		Cannot have nested {{content:}} tokens; only {{file:}} tokens are valid inside a content block