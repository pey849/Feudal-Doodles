using UnityEngine.Networking;
using System;

namespace Doodle.Networking
{
    /// <summary>
    /// An exception thrown when an error is discovered by a network connection or host.
    /// </summary>
    internal class NetworkException : InvalidOperationException
    {
        internal NetworkException( string message, byte error )
            : base( string.Format( "{0}, Network error. Error {1}.", message, (NetworkError) error ) )
        { }
    }
}