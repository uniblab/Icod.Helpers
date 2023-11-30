## Icod.Helpers
Icod.Helpers is a helper utility containing extension methods 
encapsulating commonly used String, CodePage, and File operations 
into convenient functions.

## Usage
The extension methods defined in the Icod.Helpers assembly are 
focused on three key areas:
* CodePage
* File
* String


### CodePageHelper
The `GetCodePage` static method wraps the logic for converting a 
code page specifier.  A common trouble is code page entries are 
often specfied in a configuration file as a string, but values 
might be a name or a string-representation of a number.  This 
static method neatly handles both cases.

Examples: 
``` csharp
var windows = Icod.Helpers.CodePageHelper.GetCodePage( "1252" );
var utf = Icod.Helpers.CodePageHelper.GetCodePage( "utf-8" );
```

### FileHelper
This modile contains a static method for building file paths, as 
well as several extension methods for reading from and writing 
to files.

The static method `BuildPath` functions as per the intrinsic 
`System.IO.Path.PathCombine`, but with logic to avoid doubling 
path characters.

Example:
``` csharp
var extendedPath = Icod.Helpers.FileHelper.BuildPath( @"C:\foo\bar\", @'\', @"\baz" );
assert( extendedPath, "C:\\foo\\bar\\baz" );
```

The extension methods bind to System.String, System.IO.Stream, 
System.IO.StreamReader, System.IO.StringReader, 
System.IO.TextReader, and System.IO.TextWriter instances.  
These come in modes for treating files in either line-based 
mode or record-based mode.

#### Line-based
The `ReadFileLine` extension methods optimize operations of 
treating the file like a source of strings, i.e. an 
`IEnumerable<String>`.

Examples:
``` csharp
System.Func<System.String?, System.Collections.Generic.IEnumerable<System.String>> reader;
if ( System.String.IsNullOrEmpty( inputPathName ) ) {
	// no file specified, read from StdIn instead
	reader = ( x ) => System.Console.In.ReadFileLine();
} else {
	// read from specified file
	reader = ( x ) => x.ReadFileLine();
}

foreach ( var line in reader( inputPathName ) ) {
	// perform work here
}
```

The `WriteFileLine` extension methods come in two varieties.  
One variety which relies on the host system's idea of a 
line-ending, and one which permits the caller to specify a 
particular line-ending.  This is extremely useful when writing 
traditional macintosh files from windows hosts.  Either 
variety is optized for accepting a list of lines to write.

Example:
``` csharp
System.Func<System.String?, System.Collections.Generic.IEnumerable<System.String>> reader;
if ( System.String.IsNullOrEmpty( inputPathName ) ) {
	// no file specified, read from StdIn instead
	reader = ( x ) => System.Console.In.ReadFileLine();
} else {
	// read from specified file
	reader = ( x ) => x.ReadFileLine();
}

System.Action<System.String?, System.Collections.Generic.IEnumerable<System.String>> writer;
if ( System.String.IsNullOrEmpty( outputPathName ) ) {
	// no file specified, write from StdOut instead
	writer = ( x, y ) => WriteFileLine( System.Console.Out, y );
} else {
	// write to specified file
	writer = ( x, y ) => WriteFileLine( x!, y );
}

writer( outputPathName, reader( inputPathName ).Select(
  x => ModifyTheLineSomeHow( x )
) );
```

#### Record-based

### StringHelper

## Copyright and Licensing
Icod.Helpers is a helper utility containing extension methods 
encapsulating commonly used String, CodePage, and File operations 
into convenient functions.
Copyright (C) 2023 Timothy J. Bruce

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
USA
