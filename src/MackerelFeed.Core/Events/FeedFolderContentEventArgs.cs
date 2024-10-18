// <copyright file="FeedFolderContentEventArgs.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using MackerelFeed.Models;

namespace MackerelFeed.Events;

/// <summary>
/// Feed Folder Content Event Args.
/// </summary>
public class FeedFolderContentEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FeedFolderContentEventArgs"/> class.
    /// </summary>
    /// <param name="feed">Feed Folder.</param>
    public FeedFolderContentEventArgs(FeedFolder feed)
    {
        this.Folder = feed;
    }

    /// <summary>
    /// Gets the Feed Folder.
    /// </summary>
    public FeedFolder Folder { get; }
}
