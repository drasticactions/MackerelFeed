// <copyright file="DatabaseException.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace MackerelFeed.Exceptions;

/// <summary>
/// Database Exception.
/// </summary>
public class DatabaseException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseException"/> class.
    /// </summary>
    /// <param name="message">Message.</param>
    public DatabaseException(string message)
        : base(message)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseException"/> class.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="ex">Original exception.</param>
    public DatabaseException(string message, Exception ex)
        : base(message, ex)
    {
    }
}