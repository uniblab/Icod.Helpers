﻿// Icod.Helpers.dll is the Icod.Helpers utility .Net assembly.
// Copyright (C) 2025  Timothy J. Bruce

/*
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
*/

namespace Icod.Helpers {

	/// <include file='..\doc\Icod.Helpers.xml' path='types/type[@name="StringHelper"]/member[@name=""]/*'/>
	[System.Xml.Serialization.XmlType( IncludeInSchema = false )]
	[Icod.LgplLicense]
	[Icod.Author( "Timothy J. Bruce" )]
	[Icod.ReportBugsTo( "uniblab@hotmail.com" )]
	public static class StringHelper {

		/// <include file='..\doc\Icod.Helpers.xml' path='types/type[@name="StringHelper"]/member[@name="TrimToNull(System.String)"]/*'/>
#if NET5_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
		public static System.String? TrimToNull( this System.String? @string ) {
#else
		public static System.String TrimToNull( this System.String @string ) {
#endif
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			@string = @string.Trim();
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			return @string;
		}

		/// <include file='..\doc\Icod.Helpers.xml' path='types/type[@name="StringHelper"]/member[@name="Compress(System.String,System.Text.Encoding,System.Func{System.IO.Stream,System.IO.Compression.CompressionMode,System.Boolean,System.IO.Stream})"]/*'/>
		public static System.Byte[] Compress(
			this System.String @string,
			System.Text.Encoding encoding,
			System.Func<System.IO.Stream, System.IO.Compression.CompressionMode, System.Boolean, System.IO.Stream> compressor
		) {
			using ( var input = new System.IO.MemoryStream( encoding.GetBytes( @string ), false ) ) {
				using ( var worker = compressor( input, System.IO.Compression.CompressionMode.Compress, true ) ) {
					using ( var output = new System.IO.MemoryStream() ) {
						worker.CopyTo( output );
						output.Flush();
						_ = output.Seek( 0, System.IO.SeekOrigin.Begin );
						return output.ToArray();
					}
				}
			}
		}
		/// <include file='..\doc\Icod.Helpers.xml' path='types/type[@name="StringHelper"]/member[@name="Gzip(System.String,System.Text.Encoding)"]/*'/>
		public static System.Byte[] Gzip( this System.String @string, System.Text.Encoding encoding ) {
			return @string.Compress(
				encoding,
				( stream, compressionMode, leaveOpen ) => new System.IO.Compression.GZipStream( stream, compressionMode, leaveOpen )
			);
		}
		/// <include file='..\doc\Icod.Helpers.xml' path='types/type[@name="StringHelper"]/member[@name="Deflate(System.String,System.Text.Encoding)"]/*'/>
		public static System.Byte[] Deflate( this System.String @string, System.Text.Encoding encoding ) {
			return @string.Compress(
				encoding,
				( stream, compressionMode, leaveOpen ) => new System.IO.Compression.DeflateStream( stream, compressionMode, leaveOpen )
			);
		}

		/// <include file='..\doc\Icod.Helpers.xml' path='types/type[@name="StringHelper"]/member[@name="GetString(System.String,System.Text.Encoding)"]/*'/>
		public static System.String GetString( this System.Byte[] response, System.Text.Encoding encoding ) {
			return encoding.GetString( response );
		}

		/// <include file='..\doc\Icod.Helpers.xml' path='types/type[@name="StringHelper"]/member[@name="Decompress(System.Byte[],System.Text.Encoding,System.Func{System.IO.Stream,System.IO.Compression.CompressionMode,System.Boolean,System.IO.Stream})"]/*'/>
		public static System.String Decompress(
			this System.Byte[] response,
			System.Text.Encoding encoding,
			System.Func<System.IO.Stream, System.IO.Compression.CompressionMode, System.Boolean, System.IO.Stream> decompressor
		) {
			using ( var input = new System.IO.MemoryStream( response, false ) ) {
				using ( var worker = decompressor( input, System.IO.Compression.CompressionMode.Decompress, true ) ) {
					using ( var output = new System.IO.MemoryStream() ) {
						worker.CopyTo( output );
						output.Flush();
						_ = output.Seek( 0, System.IO.SeekOrigin.Begin );
						return output.ToArray().GetString( encoding );
					}
				}
			}
		}
		/// <include file='..\doc\Icod.Helpers.xml' path='types/type[@name="StringHelper"]/member[@name="Gunzip(System.Byte[],System.Text.Encoding)"]/*'/>
		public static System.String Gunzip( this System.Byte[] response, System.Text.Encoding encoding ) {
			return response.Decompress(
				encoding,
				( stream, compressionMode, leaveOpen ) => new System.IO.Compression.GZipStream( stream, compressionMode, leaveOpen )
			);
		}
		/// <include file='..\doc\Icod.Helpers.xml' path='types/type[@name="StringHelper"]/member[@name="Inflate(System.Byte[],System.Text.Encoding)"]/*'/>
		public static System.String Inflate( this System.Byte[] response, System.Text.Encoding encoding ) {
			return response.Decompress(
				encoding,
				( stream, compressionMode, leaveOpen ) => new System.IO.Compression.DeflateStream( stream, compressionMode, leaveOpen )
			);
		}

		/// <include file='..\doc\Icod.Helpers.xml' path='types/type[@name="StringHelper"]/member[@name="GetWebString(System.Byte[],System.Text.Encoding,System.String)"]/*'/>
		public static System.String GetWebString(
#if NET5_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
			this System.Byte[] response, System.Text.Encoding encoding, System.String? contentEncoding
#else
			this System.Byte[] response, System.Text.Encoding encoding, System.String contentEncoding
#endif
		) {
			System.String ce = ( contentEncoding.TrimToNull() ?? "identity" ).ToLower();
			return ( ce ).Equals( "identity", System.StringComparison.Ordinal )
				? response.GetString( encoding )
				: ce.Equals( "gzip", System.StringComparison.Ordinal )
					? response.Gunzip( encoding )
					: ce.Equals( "deflate", System.StringComparison.Ordinal )
						? response.Inflate( encoding )
						: throw new System.InvalidOperationException( System.String.Format(
							"Unknown Content-Encoding value received from server: {0}",
							contentEncoding
						) )
			;
		}

	}

}