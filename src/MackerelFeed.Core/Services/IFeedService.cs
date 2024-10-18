// <copyright file="IFeedService.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using MackerelFeed.Models;

namespace MackerelFeed.Services;

/// <summary>
/// Feed Service.
/// The base of handling RSS feeds and other lists.
/// </summary>
public interface IFeedService
{
    /// <summary>
    /// Gets the service type.
    /// </summary>
    FeedType ServiceType { get; }

    /// <summary>
    /// Read a given feed from a URI.
    /// </summary>
    /// <param name="feedUri">The feedUri.</param>
    /// <param name="feedItem">The Feed Item.</param>
    /// <param name="token">Optional Token.</param>
    /// <returns><see cref="FeedListItem"/> and <see cref="FeedItem"/>.</returns>
    Task<(FeedListItem? FeedList, IList<FeedItem>? FeedItemList)> ReadFeedAsync(Uri feedUri, FeedListItem feedItem, CancellationToken? token = default);

    /// <summary>
    /// Read a given feed from the string response.
    /// </summary>
    /// <param name="stringResponse">The string.</param>
    /// <param name="feedItem">The Feed Item.</param>
    /// <param name="token">Optional Token.</param>
    /// <returns><see cref="FeedListItem"/> and <see cref="FeedItem"/>.</returns>
    Task<(FeedListItem? FeedList, IList<FeedItem>? FeedItemList)> ReadFeedAsync(string stringResponse, FeedListItem? feedItem = null, CancellationToken? token = default);
}