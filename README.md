TSID Creator .NET
======================================================
A .NET library for generating Time-Sorted Unique Identifiers (TSID).
Compatible with .NET Standard 2.0/.NET Standard 2.1.

It brings together ideas from [Twitter's Snowflake](https://github.com/twitter-archive/snowflake/tree/snowflake-2010) and [ULID Spec](https://github.com/ulid/spec).

In summary:

*   Sorted by generation time;
*   Can be stored as an integer of 64 bits;
*   Can be stored as a string of 13 chars;
*   String format is encoded to [Crockford's base32](https://www.crockford.com/base32.html);
*   String format is URL safe, is case insensitive, and has no hyphens;
*   Shorter than UUID, ULID and KSUID.


This library is a port of the Java code found [here](https://github.com/f4b6a3/tsid-creator).

It is **highly recommended** to consult first the original Java code before using this library, 
in terms of usage and performance.

## Usage

Create a TSID:

```csharp
Tsid tsid = TsidCreator.GetTsid();
```

Create a TSID as `long`:

```csharp
long number = TsidCreator.GetTsid().ToLong(); // 38352658567418872
```

Create a TSID as `String`:

```csharp
string string = TsidCreator.GetTsid().ToString(); // 01226N0640J7Q
```

The TSID generator is [thread-safe](https://en.wikipedia.org/wiki/Thread_safety).