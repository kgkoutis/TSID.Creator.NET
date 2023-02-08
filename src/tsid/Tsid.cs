using System.Numerics;
using System.Runtime.Serialization;

namespace TSID.Creator.NET;

[Serializable]
public sealed class Tsid : ISerializable, IEquatable<Tsid>, IComparable<Tsid>, IComparable
{
	public int CompareTo(Tsid? other)
	{
		return other == null ? 1 : _number.CompareTo(other._number);
	}

	public int CompareTo(object? obj)
	{
		if (ReferenceEquals(null, obj)) return 1;
		if (ReferenceEquals(this, obj)) return 0;
		return obj is Tsid other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Tsid)}");
	}

	public static bool operator <(Tsid? left, Tsid? right)
	{
		return Comparer<Tsid>.Default.Compare(left, right) < 0;
	}

	public static bool operator >(Tsid? left, Tsid? right)
	{
		return Comparer<Tsid>.Default.Compare(left, right) > 0;
	}

	public static bool operator <=(Tsid? left, Tsid? right)
	{
		return Comparer<Tsid>.Default.Compare(left, right) <= 0;
	}

	public static bool operator >=(Tsid? left, Tsid? right)
	{
		return Comparer<Tsid>.Default.Compare(left, right) >= 0;
	}

	public bool Equals(Tsid? other)
	{
		if (other == null)
			return false;
		if (other.GetType() != typeof(Tsid))
			return false;
		
		return _number == other._number;
	}

	public override bool Equals(object? obj)
	{
		return ReferenceEquals(this, obj) || obj is Tsid other && Equals(other);
	}

	public override int GetHashCode()
	{
		return (int) (_number ^ (_number >>> 32));
	}

	public static bool operator ==(Tsid? left, Tsid? right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(Tsid? left, Tsid? right)
	{
		return !Equals(left, right);
	}

	private static readonly long SerialVersionUid = -5446820982139116297;

	private Tsid(SerializationInfo info, StreamingContext context)
	{
		var succeed = long.TryParse(info.GetString(nameof(_number)), out var number);
		if (!succeed)
			throw new SerializationException($"Cannot deserialize {nameof(Tsid)}");
		
		succeed = long.TryParse(info.GetString(nameof(SerialVersionUid)), out var serialVersionUid);
		if (!succeed)
			throw new SerializationException($"Cannot deserialize {nameof(Tsid)}, missing serial version uid");
		
		if (serialVersionUid != SerialVersionUid)
			throw new SerializationException($"Cannot deserialize {nameof(Tsid)}, wrong serial version uid");
		
		_number = number;
	}
	
	private readonly long _number;

	public const int TsidBytes = 8;
	public const int TsidChars = 13;
	public static readonly long TsidEpoch = (long)(DateTime.Parse("2020-01-01T00:00:00.000Z").ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

	public const int RandomBits = 22;
	public const int RandomMask = 0x003fffff;

	private static readonly char[] AlphabetUppercase = new[]
	{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
			'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K',
			'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Y', 'Z'
	};

	private static readonly char[] AlphabetLowercase = new[]
	{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
			'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k',
			'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z'
	};

	private static readonly long[] AlphabetValues = new long[128];

	static Tsid()
	{
		for (var i = 0; i < AlphabetValues.Length; i++)
		{
			AlphabetValues[i] = -1;
		}

		// Numbers
		AlphabetValues['0'] = 0x00;
		AlphabetValues['1'] = 0x01;
		AlphabetValues['2'] = 0x02;
		AlphabetValues['3'] = 0x03;
		AlphabetValues['4'] = 0x04;
		AlphabetValues['5'] = 0x05;
		AlphabetValues['6'] = 0x06;
		AlphabetValues['7'] = 0x07;
		AlphabetValues['8'] = 0x08;
		AlphabetValues['9'] = 0x09;
		// Lower case
		AlphabetValues['a'] = 0x0a;
		AlphabetValues['b'] = 0x0b;
		AlphabetValues['c'] = 0x0c;
		AlphabetValues['d'] = 0x0d;
		AlphabetValues['e'] = 0x0e;
		AlphabetValues['f'] = 0x0f;
		AlphabetValues['g'] = 0x10;
		AlphabetValues['h'] = 0x11;
		AlphabetValues['j'] = 0x12;
		AlphabetValues['k'] = 0x13;
		AlphabetValues['m'] = 0x14;
		AlphabetValues['n'] = 0x15;
		AlphabetValues['p'] = 0x16;
		AlphabetValues['q'] = 0x17;
		AlphabetValues['r'] = 0x18;
		AlphabetValues['s'] = 0x19;
		AlphabetValues['t'] = 0x1a;
		AlphabetValues['v'] = 0x1b;
		AlphabetValues['w'] = 0x1c;
		AlphabetValues['x'] = 0x1d;
		AlphabetValues['y'] = 0x1e;
		AlphabetValues['z'] = 0x1f;
		// Lower case OIL
		AlphabetValues['o'] = 0x00;
		AlphabetValues['i'] = 0x01;
		AlphabetValues['l'] = 0x01;
		// Upper case
		AlphabetValues['A'] = 0x0a;
		AlphabetValues['B'] = 0x0b;
		AlphabetValues['C'] = 0x0c;
		AlphabetValues['D'] = 0x0d;
		AlphabetValues['E'] = 0x0e;
		AlphabetValues['F'] = 0x0f;
		AlphabetValues['G'] = 0x10;
		AlphabetValues['H'] = 0x11;
		AlphabetValues['J'] = 0x12;
		AlphabetValues['K'] = 0x13;
		AlphabetValues['M'] = 0x14;
		AlphabetValues['N'] = 0x15;
		AlphabetValues['P'] = 0x16;
		AlphabetValues['Q'] = 0x17;
		AlphabetValues['R'] = 0x18;
		AlphabetValues['S'] = 0x19;
		AlphabetValues['T'] = 0x1a;
		AlphabetValues['V'] = 0x1b;
		AlphabetValues['W'] = 0x1c;
		AlphabetValues['X'] = 0x1d;
		AlphabetValues['Y'] = 0x1e;
		AlphabetValues['Z'] = 0x1f;
		// Upper case OIL
		AlphabetValues['O'] = 0x00;
		AlphabetValues['I'] = 0x01;
		AlphabetValues['L'] = 0x01;
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("number", _number);
		
	}

	/// <summary>
	/// <para>
	/// Creates a new TSID.
	/// </para>
	/// <para>
	/// This constructor wraps the input value in an immutable object.
	/// </para>
	/// <param name="number"> a number </param>
	/// </summary>
	public Tsid(long number)
	{
		_number = number;
	}

	/// <summary>
	/// <para>
	/// Converts a number into a TSID.
	/// </para>
	/// <para>
	/// This method wraps the input value in an immutable object.
	/// </para>
	/// <param name="number"> a number </param>
	/// <returns> a <see cref="Tsid"/> </returns>
	/// </summary>
	public static Tsid From(long number)
	{
		return new Tsid(number);
	}

	/// <summary>
	/// <para>
	/// Converts a byte array into a TSID.
	/// </para>
	/// <param name="bytes"> a byte array </param>
	/// <returns> a <see cref="Tsid"/> </returns>
	/// <exception cref="ArgumentException"> throws if bytes are null or its length is not 8 </exception> 
	/// </summary>
	public static Tsid From(byte[] bytes)
	{

		if (bytes == null || bytes.Length != TsidBytes)
		{
			throw new ArgumentException("Invalid TSID bytes"); // null or wrong length!
		}

		long number = 0;

		number |= (bytes[0x0] & 0xffL) << 56;
		number |= (bytes[0x1] & 0xffL) << 48;
		number |= (bytes[0x2] & 0xffL) << 40;
		number |= (bytes[0x3] & 0xffL) << 32;
		number |= (bytes[0x4] & 0xffL) << 24;
		number |= (bytes[0x5] & 0xffL) << 16;
		number |= (bytes[0x6] & 0xffL) << 8;
		number |= (bytes[0x7] & 0xffL);

		return new Tsid(number);
	}

	
	/// <summary>
	/// <para>
	/// Converts a canonical string into a TSID.
	/// </para>
	/// <para>
	/// The input string must be 13 characters long and must contain only characters
	/// from Crockford's base 32 alphabet.
	/// </para>
	/// <para>
	/// The first character of the input string must be between 0 and F.
	/// </para>
	/// <param name="str"> a canonical stringy </param>
	/// <returns> a <see cref="Tsid"/> </returns>
	/// <exception cref="ArgumentException"> throws if the input string is invalid </exception>
	/// <remarks> see also <a href="https://www.crockford.com/base32.html">Crockford's Base 32</a></remarks>
	/// </summary>
	public static Tsid From(string str)
	{

		var chars = ToCharArray(str);

		long number = 0;

		number |= AlphabetValues[chars[0x00]] << 60;
		number |= AlphabetValues[chars[0x01]] << 55;
		number |= AlphabetValues[chars[0x02]] << 50;
		number |= AlphabetValues[chars[0x03]] << 45;
		number |= AlphabetValues[chars[0x04]] << 40;
		number |= AlphabetValues[chars[0x05]] << 35;
		number |= AlphabetValues[chars[0x06]] << 30;
		number |= AlphabetValues[chars[0x07]] << 25;
		number |= AlphabetValues[chars[0x08]] << 20;
		number |= AlphabetValues[chars[0x09]] << 15;
		number |= AlphabetValues[chars[0x0a]] << 10;
		number |= AlphabetValues[chars[0x0b]] << 5;
		number |= AlphabetValues[chars[0x0c]];

		return new Tsid(number);
	}

	/// <summary>
	/// <para>
	/// Converts the TSID into a number.
	/// </para>
	/// <para>
	/// This method simply unwraps the internal value.
	/// </para>
	/// <returns> a number </returns>
	/// </summary>
	public long ToLong()
	{
		return _number;
	}

	/// <summary>
	/// <para>
	/// Converts the TSID into a byte array.
	/// </para>
	/// <para>
	/// This method simply unwraps the internal value.
	/// </para>
	/// <returns> an byte array </returns>
	/// </summary>
	public byte[] ToBytes()
	{

		var bytes = new byte[TsidBytes];

		bytes[0x0] = (byte)(_number >>> 56);
		bytes[0x1] = (byte)(_number >>> 48);
		bytes[0x2] = (byte)(_number >>> 40);
		bytes[0x3] = (byte)(_number >>> 32);
		bytes[0x4] = (byte)(_number >>> 24);
		bytes[0x5] = (byte)(_number >>> 16);
		bytes[0x6] = (byte)(_number >>> 8);
		bytes[0x7] = (byte)(_number);

		return bytes;
	}

	/// <summary>
	/// <para> Returns a fast new TSID. </para>
	/// <para> This static method is a quick alternative to <see cref="TsidCreator.GetTsid"/>.</para>
	/// <para> It employs <see cref="Interlocked"/> to generate up to 2^22 (4,194,304) TSIDs per millisecond. It can be useful, for example, for logging.</para>
	/// <para> Security-sensitive applications that require a cryptographically securepseudo-random generator <b>should</b> use <see cref="TsidCreator.GetTsid"/>.</para>
	/// <para>
	/// System property "tsidcreator.node" and environment variable "TSIDCREATOR_NODE" are ignored by this method.
	/// Therefore, there will be collisions if more than one process is generating TSIDs using this method. In that case,
	/// <see cref="TsidCreator.GetTsid"/> <b>should</b> be used in conjunction with that property or variable.
	/// </para>
	/// <returns> a <see cref="Tsid"/></returns>
	/// </summary>
	public static Tsid Fast()
	{
		var time = ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - TsidEpoch) << RandomBits;
		long tail = LazyHolder.IncrementAndGet() & RandomMask;
		return new Tsid(time | tail);
	}
	
	/// <summary>
	/// <para> Converts the TSID into a canonical string in upper case. </para>
	/// <para> 
	/// The output string is 13 characters long and contains only characters from
	/// from Crockford's base 32 alphabet.
	/// </para>
	/// <para>
	/// For lower case string, use the shorthand <code> Tsid.ToLowerCase() </code> instead
	/// of <code> Tsid.ToString().ToLower() </code>
	/// </para>
	/// <returns> a <see cref="Tsid"/> string </returns>
	/// <remarks> see also <a href="https://www.crockford.com/base32.html">Crockford's Base 32</a></remarks>
	/// </summary>
	public override string ToString()
	{
		return ToString(AlphabetUppercase);
	}
	
	/// <summary>
	/// <para> Converts the TSID into a canonical string in lower case. </para>
	/// <para> 
	/// The output string is 13 characters long and contains only characters from
	/// from Crockford's base 32 alphabet.
	/// </para>
	/// <para>
	/// It is faster shorthand for <code> Tsid.ToString().ToLower() </code>
	/// </para>
	/// <returns> a string </returns>
	/// <remarks> see also <a href="https://www.crockford.com/base32.html">Crockford's Base 32</a></remarks>
	/// </summary>
	public string ToLowerCase()
	{
		return ToString(AlphabetLowercase);
	}

	public DateTime GetDateTime()
	{
		return new DateTime(GetUnixMilliseconds());
	}

	public DateTime GetDateTime(DateTime customEpoch)
	{
		return new DateTime(GetUnixMilliseconds(customEpoch.ToUnixTimeMilliseconds()));
	}

	public long GetUnixMilliseconds()
	{
		return GetTime() + TsidEpoch;
	}

	public long GetUnixMilliseconds(long customEpoch)
	{
		return GetTime() + customEpoch;
	}

	public long GetTime()
	{
		return _number >> RandomBits;
	}

	public long GetRandom()
	{
		return _number & RandomMask;
	}

	public static bool IsValid(string str)
	{
		return !str.Equals(null) && IsValidCharArray(str.ToCharArray());
	}
		
	/// <summary>
	/// <para> Converts the TSID to a base-n encoded string. </para>
	/// <example>
	///	<ul>
	/// <li>TSID: 0AXS751X00W7C</li>
	/// <li>Base: 62</li>
	/// <li>Output: 0T5jFDIkmmy</li>
	/// </ul>
	/// </example>
	/// <para>The output string is left padded with zeros.</para>
	/// <param name="baseInt"> a radix between 2 and 62 </param>
	/// <returns>a base-n encoded string </returns>
	/// </summary>
	public string Encode(int baseInt)
	{
		if (baseInt < 2 || baseInt > 62)
		{
			throw new ArgumentException($"Invalid base: {baseInt}");
		}
		return BaseN.Encode(this, baseInt);
	}

	/// <summary>
	/// <para> Converts a base-n encoded string to a TSID. </para>
	/// <example>
	///	<ul>
	/// <li>string: 05772439BB9F9074</li>
	/// <li>Base: 16</li>
	/// <li>Output: 0AXS476XSZ43M</li>
	/// </ul>
	/// </example>
	/// <para>The output string is left padded with zeros.</para>
	/// <param name="str"> a base-n encoded string </param>
	/// <param name="baseInt"> a radix between 2 and 62 </param>
	/// <returns>a <see cref="Tsid"/> </returns>
	/// </summary>
	public static Tsid Decode(string str, int baseInt)
	{
		if (baseInt < 2 || baseInt > 62)
		{
			throw new ArgumentException($"Invalid base: {baseInt}");
		}
		return BaseN.Decode(str, baseInt);
	}

	/// <summary>
	/// <para> Converts the TSID to a string using a custom format. </para>
	/// <para> The custom format uses a placeholder that will be substituted by the TSID string. Only the first occurrence of a placeholder will replaced. </para>
	/// <para>
	/// Placeholders:
	/// <ul>
	/// <li>%S: canonical string in upper case</li>
	/// <li>%s: canonical string in lower case</li>
	/// <li>%X: hexadecimal in upper case</li>
	/// <li>%x: hexadecimal in lower case</li>
	/// <li>%d: base-10</li>
	/// <li>%z: base-62</li>
	/// </ul>
	/// </para>
	/// <example>
	/// <ul>
	/// <li>An key that starts with a letter:
	/// <ul>
	/// <li>TSID: 0AWE5HZP3SKTK </li>
	/// <li>Format: K%S </li>
	/// <li>Output: K<b>0AWE5HZP3SKTK</b> </li>
	/// </ul>
	/// </li>
	/// <li>A file name in hexadecimal with a prefix and an extension:
	/// <ul>
	/// <li>TSID: 0AXFXR5W7VBX0</li>
	/// <li>Format: DOC-%X.PDF</li>
	/// <li>Output: DOC-<b>0575FDC1786137D6</b>.PDF</li>
	/// </ul>
	/// </li>
	/// </ul>
	/// </example>
	/// <para> The opposite operation can be done by <see cref="Tsid.Unformat"/>. </para>
	/// <param name="format"> a custom format </param>
	/// <returns>a string using a custom format</returns>
	/// </summary>
	public string Format(string format)
	{
		if (format != null)
		{
			var i = format.IndexOf("%");
			if (i < 0 || i == format.Length - 1)
			{
				throw new ArgumentException($"Invalid format string: \"{format}\"");
			}
			var head = format.Substring(0, i);
			var tail = format.Substring(i + 2);
			var placeholder = format[i + 1];
			switch (placeholder)
			{
				case 'S': // canonical string in upper case
					return head + ToString() + tail;
				case 's': // canonical string in lower case
					return head + ToLowerCase() + tail;
				case 'X': // hexadecimal in upper case
					return head + BaseN.Encode(this, 16) + tail;
				case 'x': // hexadecimal in lower case
					return head + BaseN.Encode(this, 16).ToLower() + tail;
				case 'd': // base-10
					return head + BaseN.Encode(this, 10) + tail;
				case 'z': // base-62
					return head + BaseN.Encode(this, 62) + tail;
				default:
					throw new ArgumentException($"Invalid placeholder: \"%%{placeholder}\"");
			}
		}
		throw new ArgumentException($"Invalid format string: \"{format}\"");
	}

	/// <summary>
	/// <para> Converts a string using a custom format to a TSID. </para>
	/// <para> This method does the opposite operation of <see cref="Tsid.Format"/></para>
	/// <example>
	/// <ul>
	/// <li>An key that starts with a letter:
	/// <ul>
	/// <li>string: K<b>0AWE5HZP3SKTK</b></li>
	/// <li>Format: K%S</li>
	/// <li>Output: 0AWE5HZP3SKTK</li>
	/// </ul>
	/// </li>
	/// <li>A file name in hexadecimal with a prefix and an extension:
	/// <ul>
	/// <li>string: DOC-<b>0575FDC1786137D6</b>.PDF</li>
	/// <li>Format: DOC-%X.PDF</li>
	/// <li>Output: 0AXFXR5W7VBX0</li>
	/// </ul>
	/// </li>
	/// </ul>
	/// </example>
	/// <param name="formatted"> a string using a custom format </param>
	/// <param name="format"> a custom format </param>
	/// <returns>a <see cref="Tsid"/></returns>
	/// </summary>
	public static Tsid Unformat(string formatted, string format)
	{
		if (formatted != null && format != null)
		{
			var i = format.IndexOf("%");
			if (i < 0 || i == format.Length - 1)
			{
				throw new ArgumentException($"Invalid format string: \"{format}\"");
			}
			var head = format.Substring(0, i);
			var tail = format.Substring(i + 2);
			var placeholder = format[i + 1];
			var length = formatted.Length - head.Length - tail.Length;
			if (formatted.StartsWith(head) && formatted.EndsWith(tail))
			{
				switch (placeholder)
				{
					case 'S': // canonical string (case insensitive)
						return From(formatted.Substring(i, i + length));
					case 's': // canonical string (case insensitive)
						return From(formatted.Substring(i, i + length));
					case 'X': // hexadecimal (case insensitive)
						return BaseN.Decode(formatted.Substring(i, i + length).ToUpper(), 16);
					case 'x': // hexadecimal (case insensitive)
						return BaseN.Decode(formatted.Substring(i, i + length).ToUpper(), 16);
					case 'd': // base-10
						return BaseN.Decode(formatted.Substring(i, i + length), 10);
					case 'z': // base-62
						return BaseN.Decode(formatted.Substring(i, i + length), 62);
					default:
						throw new ArgumentException($"Invalid placeholder: \"%%{placeholder}\"");
				}
			}
		}
		throw new ArgumentException($"Invalid formatted string: \"{formatted}\"");
	}


	public string ToString(char[] alphabet)
	{
		var chars = new char[TsidChars];

		chars[0x00] = alphabet[(int)((_number >> 60) & 0b11111)];
		chars[0x01] = alphabet[(int)((_number >> 55) & 0b11111)];
		chars[0x02] = alphabet[(int)((_number >> 50) & 0b11111)];
		chars[0x03] = alphabet[(int)((_number >> 45) & 0b11111)];
		chars[0x04] = alphabet[(int)((_number >> 40) & 0b11111)];
		chars[0x05] = alphabet[(int)((_number >> 35) & 0b11111)];
		chars[0x06] = alphabet[(int)((_number >> 30) & 0b11111)];
		chars[0x07] = alphabet[(int)((_number >> 25) & 0b11111)];
		chars[0x08] = alphabet[(int)((_number >> 20) & 0b11111)];
		chars[0x09] = alphabet[(int)((_number >> 15) & 0b11111)];
		chars[0x0a] = alphabet[(int)((_number >> 10) & 0b11111)];
		chars[0x0b] = alphabet[(int)((_number >> 5) & 0b11111)];
		chars[0x0c] = alphabet[(int)(_number & 0b11111)];

		return new string(chars);
	}

	public static char[] ToCharArray(string inputstring)
	{
		var chars = inputstring == null ? null : inputstring.ToCharArray();
		if (!IsValidCharArray(chars))
		{
			throw new ArgumentException($"Invalid TSID string: \"{inputstring}\"");
		}
		return chars;
	}

	public static bool IsValidCharArray(char[] chars)
	{
		if (chars == null || chars.Length != TsidChars)
		{
			return false;
		}

		if ((AlphabetValues[chars[0]] & 0b10000) != 0)
		{
			return false;
		}

		for (var i = 0; i < chars.Length; i++)
		{
			if (AlphabetValues[chars[i]] == -1)
			{
				return false;
			}
		}
		return true;
	}


	private static class BaseN
	{
		private static readonly BigInteger MAX = BigInteger.Parse("18446744073709551616");
		private static readonly string ALPHABET = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"; // base-62

		public static string Encode(Tsid tsid, int baseInt)
		{
			var x = new BigInteger(tsid.ToBytes());
			var radix = BigInteger.Parse(baseInt.ToString());
			var length = (int)Math.Ceiling(64 / (Math.Log(baseInt) / Math.Log(2)));
			var b = length;
			var buffer = new char[length];
			while (x.CompareTo(BigInteger.Zero) > 0)
			{
				var (quotient, remainder) = BigInteger.DivRem(x, radix);
				buffer[--b] = ALPHABET[Convert.ToInt32(remainder)];
				x = quotient;
			}
			while (b > 0)
			{
				buffer[--b] = '0';
			}
			return new string(buffer);
		}

		public static Tsid Decode(string str, int baseInt)
		{
			var x = BigInteger.Zero;
			var radix = BigInteger.Parse(baseInt.ToString());
			var length = (int)Math.Ceiling(64 / (Math.Log(baseInt) / Math.Log(2)));
			if (str == null)
			{
				throw new ArgumentException($"Invalid base-{baseInt} string: null");
			}
			if (str.Length != length)
			{
				throw new ArgumentException($"Invalid base-{baseInt} length: {str.Length}");
			}
			for (var i = 0; i < str.Length; i++)
			{
				long plus = ALPHABET.IndexOf(str[i]);
				if (plus < 0 || plus >= baseInt)
				{
					throw new ArgumentException($"Invalid base-{baseInt} character: {str[i]}");
				}
				x = BigInteger.Add(BigInteger.Multiply(x,radix),BigInteger.Parse(plus.ToString()));
			}
			if (x.CompareTo(MAX) > 0)
			{
				throw new ArgumentException($"Invalid base-{baseInt} value (overflow): {x}");
			}
			return new Tsid(Convert.ToInt32(x));
		}
	}

	private static class LazyHolder
	{
		private static int counter = new Random().Next();

		public static int IncrementAndGet()
		{
			return Interlocked.Increment(ref counter);
		}
	}
}