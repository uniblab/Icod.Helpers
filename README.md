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
This module contains a static method for building file paths, as 
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

The extension methods bind to `System.String`, `System.IO.Stream`, 
`System.IO.StreamReader`, `System.IO.StringReader`, 
`System.IO.TextReader`, and `System.IO.TextWriter` instances.  
These come in modes for treating files in either line-based 
mode or record-based mode.

#### Line-based
The `ReadLine` extension methods optimize operations of 
treating the file like a source of strings, i.e. an 
`IEnumerable<String>`.

One can specify a line-ending via the `recordSeparator` parameter.

Examples:
``` csharp
System.Func<System.String, System.Collections.Generic.IEnumerable<System.String>> reader;
if ( System.String.IsNullOrEmpty( inputPathName ) ) {
	// no file specified, read from StdIn instead
	reader = ( x ) => System.Console.In.ReadLine( System.Environment.NewLine );
} else {
	// read from specified file
	reader = ( x ) => x.ReadLine();
}

foreach ( var line in reader( inputPathName ) ) {
	// perform work here
}
```

The `WriteLine` extension methods come in two varieties.  
One variety which relies on the host system's idea of a 
line-ending, and one which permits the caller to specify a 
particular line-ending.  This is extremely useful when writing 
files between different hosts, such as from Windows to 
traditional Macintosh.  Either variety is optized for 
accepting a list of lines to write.

Example:
``` csharp
System.Func<System.String, System.Collections.Generic.IEnumerable<System.String>> reader;
if ( System.String.IsNullOrEmpty( inputPathName ) ) {
	// no file specified, read from StdIn instead
	reader = ( x ) => System.Console.In.ReadLine( System.Environment.NewLine );
} else {
	// read from specified file
	reader = ( x ) => x.ReadLine();
}

System.Action<System.String, System.Collections.Generic.IEnumerable<System.String>> writer;
if ( System.String.IsNullOrEmpty( outputPathName ) ) {
	// no file specified, write to StdOut instead
	writer = ( x, y ) => System.Console.Out.WriteLine( lineEnding: System.Environment.NewLine, data: y );
} else {
	// write to specified file
	writer = ( x, y ) => x.WriteLine( y );
}

writer( outputPathName, reader( inputPathName ).Select(
  x => ModifyTheLineSomeHow( x )
) );
```

#### Record-based
Many data file formats are supported by structuring a text file 
such that it can be interpretted as containing records, with 
each record containing columns.  The ubiquitous 
Comma-Separated-Value, CSV, format is the prime example.  These 
extension methods permit one to work with a file as source of 
records, which in turn are a source of columnar values.

The several `ReadRecord` overrides support the specification of 
a record separator, enquoting character, and field separator.

Record-based logic returns an `IEnumerable<IEnumerable<String>>`,
since each record contains one or more columns.

Example:
``` csharp
foreach ( var record in fileName.ReadRecord(
	"\n\r", '\"', ','
) ) {
	foreach ( var column in record ) {
		System.Console.Out.Write( column + "\t" );
	}
	System.Console.Out.Write( "\r\n" );
}
```

Lastly there is an extension method to parse a line of text 
into individual columns.

Example:
``` csharp
foreach ( var record in fileName.ReadLine( "\r\n" ) ) {
	foreach ( var column in record.ReadColumn( '\"', ',' ) ) {
		System.Console.Out.Write( column + "\t" );
	}
	System.Console.Out.Write( "\r\n" );
}
```

### StringHelper
This module several contains `System.String` extension methods.

#### TrimToNull
There is an extension method which trims all whitespace from 
the beginning and end of a `System.String`, and if the result 
is empty will return `null`.

Example:
``` csharp
var ipn = inputPathName.TrimToNull();
System.Func<System.String?, System.Collections.Generic.IEnumerable<System.String>> reader;
if ( ipn is null ) {
	// no file specified, read from StdIn instead
	reader = ( x ) => System.Console.In.ReadLine( System.Environment.NewLine );
} else {
	// read from specified file
	reader = ( x ) => x!.ReadLine();
}
```

#### Compression
There are methods to `Deflate` and `Gzip` a string, returning a `Byte[]` 
representation.  There are corresponding `Inflate` and `Gunzip` methods  
associated with the `Byte[]`.  There is also an extension method to 
properly decode a byte array by code page, `GetString`.

Example:
``` csharp
var input = "Mary had a little lamb";
var codePage = CodePageHelper.GetCodePage( "1252" );
var compressedd = input.Gzip( codePage );
var output = compressedd.Gunzip( codePage );
```

#### Web helper
There is also an extension method for working with web-based retrieval.  
Quite often the content of a web response will be compressed.  Based on 
the "Content-Encoding" header value, the `Byte[]`, the content, will be
decoded as a `String`, or decompressed first and then returned.

If the contentEncoding" parameter value is null, the function assumes 
"identity" encoding.

Example:
``` csharp
var json = webResponse.Body.GetWebString(
	webResponse.Encoding,
	webResponse.ResponseHeaders[ "Content-Encoding" ]
);
```


## Copyright and Licensing
Icod.Helpers is a helper utility containing extension methods 
encapsulating commonly used String, CodePage, and File operations 
into convenient functions.
Copyright (C) 2025 Timothy J. Bruce

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
USA
