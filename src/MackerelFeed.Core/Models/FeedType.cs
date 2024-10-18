// <copyright file="FeedType.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace MackerelFeed.Models;

/// <summary>
/// Feed Service Type.
/// </summary>
public enum FeedType
{
    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown,

    /// <summary>
    /// Rss.
    /// </summary>
    Rss,

    /// <summary>
    /// Json.
    /// </summary>
    Json,
}