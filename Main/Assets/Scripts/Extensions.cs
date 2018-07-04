using System;
using System.IO;

/// <summary>
/// A class to put C# extensions in.
/// </summary>
public static class Extensions
{ 
    #region System.IO.Stream

    /// <summary>
    /// Writes an enitre buffer to the given stream.
    /// </summary>
    public static void Write( this Stream @this, byte[] buffer )
    {
        @this.Write( buffer, 0, buffer.Length );
    }

    /// <summary>
    /// Reads the content of this stream and writes it into another.
    /// </summary>=
    public static void CopyTo( this Stream input, Stream output )
    {
        byte[] buffer = new byte[16 * 1024]; // Fairly arbitrary size
        int bytesRead;

        while( ( bytesRead = input.Read( buffer, 0, buffer.Length ) ) > 0 )
        {
            output.Write( buffer, 0, bytesRead );
        }
    }

    #endregion

    #region System.Random

    /// <summary>
    /// Random float value between 0.0 and 1.0.
    /// </summary>
    public static float NextFloat( this Random @this )
    {
        return (float) @this.NextDouble();
    }

    /// <summary>
    /// Random float value between min and max range.
    /// </summary>
    public static float NextFloat( this Random @this, float min, float max )
    {
        return min + @this.NextFloat() * ( max - min );
    }

    #endregion
}
