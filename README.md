# README

## About

NITCH is a static site generator that compiles HTML files based on a basic parent-child relationship. "Master" pages are bound to "child" pages via a simple token syntax.

Current version: 1.00

## Requirements

Windows 7, 8, or 10
.NET 4.5.2 runtime

## Getting Started

1) Run `nitch.exe /create` to get a blank sample site set up. This will create a folder called "new_website_project" and "build.bat"

2) Execute "build.bat" to compile the "new_website_project" sample site. A new folder called "_nitch" will be created inside the "new_website_project" directory. This is the compiled website.

### Tokens

These are the tokens you can use in your source files:

**Master**

Example usage: `{{master:/location/of/file.html}}`

* Defines what master file belongs to the child page.
* Must appear as the first line of the file, before any markup. Must appear only once.
* Must be absolute path from the project root.

**File**

Example usage: `{{file:/path/to/file.png}}`

* Defines a file path to a resource (CSS, JS, images, etc.).
* Must be absolute path from the project root.

**Placeholder**

Exmaple usage: `{{placeholder:name}}`

* Defines a placeholder for content on a Master page.

**Content**

Example Usage: `{{content:name}} <!-- CONTENT HERE --> {{content:end}}`

* Defines a region for content to be inserted into a Master page.
* `name` must match a placeholder.
* Cannot have nested {{content:}} tokens; only {{file:}} tokens are valid inside a content block

## Command Line Arguments

NITCH is a command line tool:

`nitch.exe`

It accepts a few arguments at runtime:

### Create

`nitch.exe /create`
Creates a default project structure for building websites. This is a recommended default, but not required.

`nitch.exe /create "path-to-folder"`
Creates a default project structure for building websites at the provided location (absolute or relative).

### Build

`nitch.exe /build`
Builds all HTML files in and under the current directory.

`nitch.exe /build "path-to-folder"`
Builds all HTML files in and under the specified directory.

### Pathing

`nitch.exe /pathing [ rel | abs ]`
Defines how file paths should be generated:

* `rel` - relative pathing is used; example output: `../../images/logo.png`
* `abs` - absolute pathing is used; example output: `/sections/core/images/logo.png`

In general, you can use relative pathing ('rel') to preview your website from the local filesystem, and use absolute pathing ('abs') for viewing/hosting on a web server.
	
## Contact the Author

For questions and comments, contact Brandon Bruno via email or Twitter:

[@BrandonMBruno](https://www.twitter.com/BrandonMBruno)
bmbruno (at) gmail (dot) com

## License

See the accompanying LICENSE.TXT file for more information.
