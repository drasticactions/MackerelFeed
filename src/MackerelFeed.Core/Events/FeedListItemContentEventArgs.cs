// <copyright file="FeedListItemContentEventArgs.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using MackerelFeed.Models;

namespace MackerelFeed.Events;

/// <summary>
/// Feed List Item Content Event Args.
/// </summary>
public class FeedListItemContentEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FeedListItemContentEventArgs"/> class.
    /// </summary>
    /// <param name="feed">Feed List Item.</param>
    public FeedListItemContentEventArgs(FeedListItem? feed)
    {
        this.Feed = feed;
    }

    /// <summary>
    /// Gets the Feed List Item.
    /// </summary>
    public FeedListItem? Feed { get; }
}
