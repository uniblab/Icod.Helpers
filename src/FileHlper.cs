// Icod.Helpers.dll is the Icod.Helpers utility .Net assembly.
// Copyright (C) 2023  Timothy J. Bruce

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

		#region file line
		public static void WriteFileLine( this System.String filePathName, System.Collections.Generic.IEnumerable<System.String> data ) {
			using ( var file = System.IO.File.Open( filePathName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, System.IO.FileShare.None ) ) {
				WriteFileLine( file, data );
			}
		}
		public static void WriteFileLine( this System.IO.Stream stream, System.Collections.Generic.IEnumerable<System.String> data ) {
			_ = stream.Seek( 0, System.IO.SeekOrigin.Begin );
			using ( var writer = new System.IO.StreamWriter( stream, System.Text.Encoding.UTF8, theBufferSize, true ) ) {
				WriteFileLine( writer, data );
			}
			stream.Flush();
			stream.SetLength( stream.Position );
		}
		public static void WriteFileLine( this System.IO.TextWriter writer, System.Collections.Generic.IEnumerable<System.String> data ) {
			foreach ( var datum in data ) {
				writer.WriteLine( datum );
			}
			writer.Flush();
		}

		public static void WriteFileLine( this System.String filePathName, System.String lineEnding, System.Collections.Generic.IEnumerable<System.String> data ) {
			using ( var file = System.IO.File.Open( filePathName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, System.IO.FileShare.None ) ) {
				WriteFileLine( file, lineEnding, data );
			}
		}
		public static void WriteFileLine( this System.IO.Stream stream, System.String lineEnding, System.Collections.Generic.IEnumerable<System.String> data ) {
			_ = stream.Seek( 0, System.IO.SeekOrigin.Begin );
			using ( var writer = new System.IO.StreamWriter( stream, System.Text.Encoding.UTF8, theBufferSize, true ) ) {
				WriteFileLine( writer, lineEnding, data );
			}
			stream.Flush();
			stream.SetLength( stream.Position );
		}
		public static void WriteFileLine( this System.IO.TextWriter writer, System.String lineEnding, System.Collections.Generic.IEnumerable<System.String> data ) {
			var le = lineEnding.TrimToNull();
			foreach ( var datum in data ) {
				writer.Write( datum );
				writer.Write( le );
			}
			writer.Flush();
		}


		public static System.Collections.Generic.IEnumerable<System.String> ReadFileLine( this System.String filePathName ) {
			using ( var file = System.IO.File.Open( filePathName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read ) ) {
				return ReadFileLine( file );
			}
		}
		public static System.Collections.Generic.IEnumerable<System.String> ReadFileLine( this System.IO.Stream stream ) {
			using ( var reader = new System.IO.StreamReader( stream, System.Text.Encoding.UTF8, true, theBufferSize, true ) ) {
				return ReadFileLine( reader );
			}
		}
		public static System.Collections.Generic.IEnumerable<System.String> ReadFileLine( this System.IO.TextReader fileReader ) {
			var line = fileReader.ReadLine();
			while ( null != line ) {
				line = line.TrimToNull();
				if ( null != line ) {
					yield return line;
				}
				line = fileReader.ReadLine();
			}
		}
		#endregion file line

		#region record file
		public static System.String? ReadLine( 
			this System.IO.TextReader file, System.String recordSeparator, System.Char quoteChar
		) {
			if ( EOF == file.Peek() ) {
				return null;
			}
			var rs = recordSeparator.TrimToNull();
			if ( System.String.IsNullOrEmpty( rs ) ) {
				throw new System.ArgumentNullException( nameof( recordSeparator ) );
			} else if ( ( 1 == rs.Length ) && rs.Equals( quoteChar.ToString() ) ) {
				throw new System.InvalidOperationException( "Quote character and record separator cannot be the same." );
			}

			var output = new System.Text.StringBuilder();
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
							break;
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

			return output.ToString();
		}
		public static System.String? ReadLine( this System.IO.TextReader file, System.String recordSeparator ) {
			if ( EOF == file.Peek() ) {
				return null;
			}

			var rs = recordSeparator.ToCharArray();
			System.Int32 i = 0;
			var maxI = rs.Length;
			var line = new System.Text.StringBuilder();
			System.Int32 j = 0;
			System.Int32 c;
			System.Boolean isNull = true;
			do {
				c = file.Read();
				if ( EOL == c ) {
					maxI = 0;
					break;
				}
				isNull = false;
				_ = line.Append( System.Convert.ToChar( c ) );
				if ( line[ j ].Equals( rs[ i ] ) ) {
					i++;
				} else {
					i = 0;
				}
				if ( i == maxI ) {
					break;
				}
				j++;
			} while ( true );
			_ = line.Remove( line.Length - maxI, maxI );
			return isNull ? null : line.ToString();
		}
		public static System.String? ReadLine( 
			this System.IO.StreamReader file, System.String recordSeparator, System.Char quoteChar
		) {
			if ( EOF == file.Peek() ) {
				return null;
			}
			var rs = recordSeparator.TrimToNull();
			if ( System.String.IsNullOrEmpty( rs ) ) {
				throw new System.ArgumentNullException( nameof( recordSeparator ) );
			} else if ( ( 1 == rs.Length ) && rs.Equals( quoteChar.ToString() ) ) {
				throw new System.InvalidOperationException( "Quote character and record separator cannot be the same." );
			}

			var output = new System.Text.StringBuilder();
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
					} else if ( ( leLen <= output.Length ) && ( rs[ i ].Equals( output[ ( output.Length - leLen ) - i ] ) ) ) {
						if ( lenStop <= ++i ) {
							_ = output.Remove( output.Length - leLen, leLen );
							break;
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

			return output.ToString();
		}
		public static System.String? ReadLine( this System.IO.StreamReader file, System.String recordSeparator ) {
			if ( EOF == file.Peek() ) {
				return null;
			}
			var recsep = recordSeparator.TrimToNull();
			if ( System.String.IsNullOrEmpty( recsep ) ) {
				throw new System.ArgumentNullException( nameof( recordSeparator ) );
			}

			var rs = recsep.ToCharArray();
			System.Int32 i = 0;
			var maxI = rs.Length;
			var line = new System.Text.StringBuilder();
			System.Int32 j = 0;
			System.Int32 c;
			System.Boolean isNull = true;
			do {
				c = file.Read();
				if ( EOL == c ) {
					maxI = 0;
					break;
				}
				isNull = false;
				_ = line.Append( System.Convert.ToChar( c ) );
				if ( line[ j ].Equals( rs[ i ] ) ) {
					i++;
				} else {
					i = 0;
				}
				if ( i == maxI ) {
					break;
				}
				j++;
			} while ( true );
			_ = line.Remove( line.Length - maxI, maxI );
			return isNull ? null : line.ToString();
		}

		public static System.Collections.Generic.IEnumerable<System.String?> ReadRecord(
			this System.IO.TextReader file, System.String recordSeparator, System.Char quoteChar, System.Char fieldSeparator
		) {
			var rs = recordSeparator.TrimToNull();
			if ( System.String.IsNullOrEmpty( rs ) ) {
				throw new System.ArgumentNullException( nameof( recordSeparator ) );
			}

			var line = file.ReadLine( recordSeparator, quoteChar );
			if ( null == line ) {
				yield break;
			}
			using ( var reader = new System.IO.StringReader( line ) ) {
				System.Int32 i;
				System.Char c;
				System.String? column;
				var qc = quoteChar;
				do {
					i = reader.Peek();
					if ( EOL == i ) {
						break;
					}
					c = System.Convert.ToChar( i );
					if ( qc.Equals( c ) ) {
						_ = reader.Read();
						column = ReadColumn( reader, quoteChar, true );
						yield return column;
					} else {
						column = ReadColumn( reader, fieldSeparator, false );
						yield return column;
					}
				} while ( true );
			}
		}
		public static System.Collections.Generic.IEnumerable<System.String?> ReadRecord(
			this System.IO.StreamReader file, System.String recordSeparator, System.Char quoteChar, System.Char fieldSeparator
		) {
			if ( file.EndOfStream ) {
				yield break;
			}
			var rs = recordSeparator.TrimToNull();
			if ( System.String.IsNullOrEmpty( rs ) ) {
				throw new System.ArgumentNullException( nameof( recordSeparator ) );
			}

			var line = file.ReadLine( rs, quoteChar );
			if ( null == line ) {
				yield break;
			}
			using ( var reader = new System.IO.StringReader( line ) ) {
				System.Int32 i;
				System.Char c;
				System.String? column;
				var qc = quoteChar;
				do {
					i = reader.Peek();
					if ( EOL == i ) {
						break;
					}
					c = System.Convert.ToChar( i );
					if ( qc.Equals( c ) ) {
						_ = reader.Read();
						column = ReadColumn( reader, quoteChar, true );
						yield return column;
					} else {
						column = ReadColumn( reader, fieldSeparator, false );
						yield return column;
					}
				} while ( true );
			}
		}
		public static System.String? ReadColumn(
			this System.IO.StringReader reader, System.Char @break, System.Boolean readNextOnBreak
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
			return sb.ToString().TrimToNull();
		}
		#endregion record file

		public static System.Nullable<System.Char> ReadChar(
			this System.IO.StringReader reader, System.Char @break, System.Boolean readNextOnBreak
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
		#endregion methods

	}

}