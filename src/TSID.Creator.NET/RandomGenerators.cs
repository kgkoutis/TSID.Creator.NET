﻿using System.Security.Cryptography;

namespace TSID.Creator.NET;

/// <summary>
/// A helper class that provides a cryptographically secure random number generator or a simple random number generator.
///
/// <remarks>
/// The class allows the user to supply his own implementation of a cryptographically secure RandomNumberGenerator
/// This is done like so:
/// <br/>
/// <code>
/// var random = RandomGenerators.OfCryptographicallySecureRandom(() => new MyRandomNumberGenerator());
/// </code>
/// <br/>
///
/// This can be used in conjunction with the <see cref="TsidFactory.Builder"/> class in method <see cref="TsidFactory.Builder.WithRandom"/>
/// </remarks>
/// </summary>
public class RandomGenerators
{
    private readonly RandomNumberGenerator _cryptographicallySecureRandomNumberGenerator;
    private readonly Random _pseudoRandomNumberGenerator;
    protected RandomGenerators(RandomNumberGenerator rng) => _cryptographicallySecureRandomNumberGenerator = rng;
    protected RandomGenerators() => _pseudoRandomNumberGenerator = Random.Shared;
    
    public RandomNumberGenerator GetCryptographicallySecureRandomNumberGeneratorOrThrow()
    {
        if (_cryptographicallySecureRandomNumberGenerator == null)
            throw new InvalidOperationException("Cannot get secure random number generator when using insecure random");
        return _cryptographicallySecureRandomNumberGenerator;
    }
    
    public Random GetSimpleRandomNumberGeneratorOrThrow()
    {
        if (_pseudoRandomNumberGenerator == null)
            throw new InvalidOperationException("Cannot get insecure random number generator when using secure random number generator");
        return _pseudoRandomNumberGenerator;
    }

    public static RandomGenerators OfCryptographicallySecureRandom() => new RandomGenerators(RandomNumberGenerator.Create());
    public static RandomGenerators OfCryptographicallySecureRandom(Func<RandomNumberGenerator> factory) => new RandomGenerators(factory());
    public static RandomGenerators OfSimpleRandomNumberGenerator() => new RandomGenerators();
    
    public bool IsCryptographicallySecure() => _cryptographicallySecureRandomNumberGenerator != null;

    public int NextInt(int maxValue = int.MaxValue)
    {
        if (IsCryptographicallySecure())
        {
            // This method is static so it's common for all overriders of RandomNumberGenerator
            return RandomNumberGenerator.GetInt32(maxValue); 
        }
        
        return GetSimpleRandomNumberGeneratorOrThrow().Next(maxValue);
    }
    
    public byte[] NextBytes(int length)
    {
        if (IsCryptographicallySecure())
        {
            var randomNumber = new byte[length];
            var rng = GetCryptographicallySecureRandomNumberGeneratorOrThrow();
            rng.GetBytes(randomNumber);
            return randomNumber;
        }
        
        var bytes = new byte[length];
        GetSimpleRandomNumberGeneratorOrThrow().NextBytes(bytes);
        return bytes;
    }
}