// Icod.Helpers.dll is the Icod.Helpers utility .Net assembly.
// Copyright (C) 2025  Timothy J. Bruce

/*
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
*/

using System.Linq;

namespace Icod.Helpers {

	[System.Xml.Serialization.XmlType( IncludeInSchema = false )]
	public static class FileHelper {

		#region fields
		private const System.Int32 EOL = -1;
		private const System.Int32 EOF = EOL;

		private const System.Int32 theBufferSize = 16384;
		#endregion fields


		#region methods
		public static System.String PathCombine( System.String path, System.Char pathSeparator, System.String name ) {
			var n = name.TrimToNull();
			if ( System.String.IsNullOrEmpty( n ) ) {
				throw new System.ArgumentNullException( nameof( name ) );
			}
			var p = path.TrimToNull();
			if ( System.String.IsNullOrEmpty( p ) ) {
				throw new System.ArgumentNullException( nameof( path ) );
			}
			var sep = pathSeparator.ToString();
			while ( p.EndsWith( sep ) ) {
				p = p[ ..^1 ];
			}
			while ( n.StartsWith( sep ) ) {
				n = n[ 1.. ];
			}
			return p + sep + name;
		}

		#region line file
		#region write line
		public static void WriteLine( this System.String filePathName, System.Collections.Generic.IEnumerable<System.String> data ) {
			using ( var file = System.IO.File.Open( filePathName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, System.IO.FileShare.None ) ) {
				_ = file.Seek( 0, System.IO.SeekOrigin.Begin );
				WriteLine( file, data );
				file.Flush();
				file.SetLength( file.Position );
			}
		}
		public static void WriteLine( this System.IO.Stream stream, System.Collections.Generic.IEnumerable<System.String> data ) {
			using ( var writer = new System.IO.StreamWriter( stream, System.Text.Encoding.UTF8, theBufferSize, true ) ) {
				WriteLine( writer, data );
			}
		}
		public static void WriteLine( this System.IO.TextWriter writer, System.Collections.Generic.IEnumerable<System.String> data ) {
			foreach ( var datum in data ) {
				writer.WriteLine( datum );
			}
			writer.Flush();
		}

		public static void WriteLine( this System.String filePathName, System.String lineEnding, System.Collections.Generic.IEnumerable<System.String> data ) {
			using ( var file = System.IO.File.Open( filePathName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, System.IO.FileShare.None ) ) {
				_ = file.Seek( 0, System.IO.SeekOrigin.Begin );
				WriteLine( file, lineEnding, data );
				file.Flush();
				file.SetLength( file.Position );
			}
		}
		public static void WriteLine( this System.IO.Stream stream, System.String lineEnding, System.Collections.Generic.IEnumerable<System.String> data ) {
			using ( var writer = new System.IO.StreamWriter( stream, System.Text.Encoding.UTF8, theBufferSize, true ) ) {
				WriteLine( writer, lineEnding, data );
			}
		}
		public static void WriteLine( this System.IO.TextWriter writer, System.String lineEnding, System.Collections.Generic.IEnumerable<System.String> data ) {
			foreach ( var datum in data ) {
				writer.Write( datum );
				writer.Write( lineEnding );
			}
			writer.Flush();
		}
		#endregion write line

		#region read line
		public static System.Collections.Generic.IEnumerable<System.String> ReadLine( this System.String filePathName ) {
			using ( var file = System.IO.File.Open( filePathName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read ) ) {
				return ReadLine( file );
			}
		}
		public static System.Collections.Generic.IEnumerable<System.String> ReadLine( this System.IO.Stream stream ) {
			using ( var reader = new System.IO.StreamReader( stream, System.Text.Encoding.UTF8, true, theBufferSize, true ) ) {
				return ReadLine( reader );
			}
		}
		public static System.Collections.Generic.IEnumerable<System.String> ReadLine( this System.IO.TextReader fileReader ) {
			var line = fileReader.ReadLine();
			while ( null != line ) {
				line = line.TrimToNull();
				if ( null != line ) {
					yield return line;
				}
				line = fileReader.ReadLine();
			}
		}

		public static System.Collections.Generic.IEnumerable<System.String> ReadLine(
			this System.String filePathName, System.String recordSeparator
		) {
			using ( var file = System.IO.File.Open( filePathName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read ) ) {
				return ReadLine( file, recordSeparator );
			}
		}
		public static System.Collections.Generic.IEnumerable<System.String> ReadLine(
			this System.IO.Stream stream, System.String recordSeparator
		) {
			using ( var reader = new System.IO.StreamReader( stream, System.Text.Encoding.UTF8, true, theBufferSize, true ) ) {
				return ReadLine( reader, recordSeparator );
			}
		}
		public static System.Collections.Generic.IEnumerable<System.String> ReadLine(
			this System.IO.TextReader fileReader, System.String recordSeparator
		) {
			if ( EOF == fileReader.Peek() ) {
				yield break;
			}

			var rs = recordSeparator.ToCharArray();
			System.Int32 i = 0;
			var maxI = rs.Length;
			var output = new System.Text.StringBuilder();
			System.Int32 j = 0;
			System.Int32 c;
			do {
				c = fileReader.Read();
				if ( EOL == c ) {
					yield return output.ToString();
					yield break;
				}
				output = output.Append( System.Convert.ToChar( c ) );
				if ( output[ j ].Equals( rs[ i ] ) ) {
					i++;
				} else {
					i = 0;
				}
				j++;
				if ( i == maxI ) {
					output = output.Remove( output.Length - maxI, maxI );
					yield return output.ToString();
					output = output.Clear();
					j = 0;
					i = 0;
				}
			} while ( true );
		}

		public static System.Collections.Generic.IEnumerable<System.String> ReadLine(
			this System.String filePathName, System.String recordSeparator, System.Char quoteChar
		) {
			using ( var file = System.IO.File.Open( filePathName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read ) ) {
				return ReadLine( file, recordSeparator, quoteChar );
			}
		}
		public static System.Collections.Generic.IEnumerable<System.String> ReadLine(
			this System.IO.Stream stream, System.String recordSeparator, System.Char quoteChar
		) {
			using ( var reader = new System.IO.StreamReader( stream, System.Text.Encoding.UTF8, true, theBufferSize, true ) ) {
				return ReadLine( reader, recordSeparator, quoteChar );
			}
		}
		public static System.Collections.Generic.IEnumerable<System.String> ReadLine(
			this System.IO.TextReader file, System.String recordSeparator, System.Char quoteChar
		) {
			if ( EOF == file.Peek() ) {
				yield break;
			}
			var rs = recordSeparator.TrimToNull();
			if ( System.String.IsNullOrEmpty( rs ) ) {
				throw new System.ArgumentNullException( nameof( recordSeparator ) );
			} else if ( ( 1 == rs.Length ) && rs.Equals( quoteChar.ToString() ) ) {
				throw new System.InvalidOperationException( "Quote character and record separator cannot be the same." );
			}

			System.Text.StringBuilder output = new System.Text.StringBuilder();
			var leLen = rs.Length;
			var lenStop = leLen - 1;
			System.Int32 i = 0;
			System.Char c;
			System.Boolean isPlaintext = true;
			System.Int32 p = file.Read();
			while ( EOL != p ) {
				c = System.Convert.ToChar( p );
				output = output.Append( c );
				if ( isPlaintext ) {
					if ( quoteChar.Equals( c ) ) {
						isPlaintext = false;
					} else if (
						( leLen <= output.Length )
						&& ( rs[ i ].Equals( output[ ( output.Length - leLen ) - i ] ) )
					) {
						if ( lenStop <= ++i ) {
							_ = output.Remove( output.Length - leLen, leLen );
							yield return output.ToString();
							output = output.Clear();
						}
					} else {
						i = 0;
					}
				} else {
					if ( quoteChar.Equals( c ) ) {
						p = file.Peek();
						if ( EOL == p ) {
							throw new System.IO.EndOfStreamException();
						}
						c = System.Convert.ToChar( p );
						if ( quoteChar.Equals( c ) ) {
							_ = file.Read();
							output = output.Append( c );
						} else {
							i = 0;
							isPlaintext = true;
						}
					}
				}
				p = file.Read();
			}
		}
		#endregion read line
		#endregion line file

		#region record file
		public static System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<System.String>> ReadRecord(
			this System.String filePathName, System.String recordSeparator, System.Char quoteChar, System.Char fieldSeparator
		) {
			using ( var file = System.IO.File.Open( filePathName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read ) ) {
				return ReadRecord( file, recordSeparator, quoteChar, fieldSeparator );
			}
		}
		public static System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<System.String>> ReadRecord(
			this System.IO.Stream stream, System.String recordSeparator, System.Char quoteChar, System.Char fieldSeparator
		) {
			using ( var reader = new System.IO.StreamReader( stream, System.Text.Encoding.UTF8, true, theBufferSize, true ) ) {
				return ReadRecord( reader, recordSeparator, quoteChar, fieldSeparator );
			}
		}
		public static System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<System.String>> ReadRecord(
			this System.IO.TextReader file, System.String recordSeparator, System.Char quoteChar, System.Char fieldSeparator
		) {
			var rs = recordSeparator.TrimToNull();
			if ( System.String.IsNullOrEmpty( rs ) ) {
				throw new System.ArgumentNullException( nameof( recordSeparator ) );
			}

			System.Collections.Generic.List<System.String> output;
			foreach ( var line in file.ReadLine( recordSeparator, quoteChar ).Where(
				x => null != x
			) ) {
				using ( var reader = new System.IO.StringReader( line ) ) {
					output = new System.Collections.Generic.List<System.String>(
						reader.ReadColumn( quoteChar, fieldSeparator )
					);
					yield return output.AsReadOnly();
				}
			}
		}
		#endregion record file

		#region column
		public static System.Collections.Generic.IEnumerable<System.String> ReadColumn(
			this System.String line, System.Char quoteChar, System.Char fieldSeparator
		) {
			using ( var reader = new System.IO.StringReader( line ) ) {
				return reader.ReadColumn( quoteChar, fieldSeparator );
			}
		}
		public static System.Collections.Generic.IEnumerable<System.String> ReadColumn(
			this System.IO.TextReader reader, System.Char quoteChar, System.Char fieldSeparator
		) {
			System.Int32 i;
			System.Char c;
			System.String column;
			do {
				i = reader.Peek();
				if ( EOL == i ) {
					break;
				}
				c = System.Convert.ToChar( i );
				if ( quoteChar.Equals( c ) ) {
					_ = reader.Read();
					column = reader.ReadColumn( quoteChar, true );
					yield return column;
				} else {
					column = reader.ReadColumn( fieldSeparator, false );
					yield return column;
				}
			} while ( true );
		}
		private static System.String ReadColumn(
			this System.IO.TextReader reader, System.Char @break, System.Boolean readNextOnBreak
		) {
			var sb = new System.Text.StringBuilder( 128 );
			System.Nullable<System.Char> ch;
			do {
				ch = ReadChar( reader, @break, readNextOnBreak );
				if ( ch.HasValue ) {
					sb = sb.Append( ch.Value );
				} else {
					break;
				}
			} while ( true );
			return sb.ToString();
		}
		private static System.Nullable<System.Char> ReadChar(
			this System.IO.TextReader reader, System.Char @break, System.Boolean readNextOnBreak
		) {
			var p = reader.Peek();
			if ( EOL == p ) {
				return null;
			}
			var c = System.Convert.ToChar( reader.Read() );
			if ( @break.Equals( c ) ) {
				if ( readNextOnBreak ) {
					p = reader.Peek();
					if ( EOL == p ) {
						return null;
					} else if ( @break.Equals( System.Convert.ToChar( p ) ) ) {
						return System.Convert.ToChar( reader.Read() );
					} else {
						_ = reader.Read();
					}
				}
				return null;
			}
			return c;
		}
		#endregion column
		#endregion methods

	}

}