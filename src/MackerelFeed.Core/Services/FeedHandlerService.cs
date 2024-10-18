// <copyright file="FeedHandlerService.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using DA.UI.Services;
using MackerelFeed.Events;
using MackerelFeed.Exceptions;
using MackerelFeed.Models;
using SQLite;

namespace MackerelFeed.Services;

public class FeedHandlerService
{
    private readonly IErrorHandler errorHandler;
    private readonly DatabaseService database;
    private readonly JsonFeedService jsonFeedService;
    private readonly RssFeedService rssFeedService;

    public FeedHandlerService(DatabaseService database, JsonFeedService jsonFeedService, RssFeedService rssFeedService, IErrorHandler errorHandler)
    {
        this.database = database;
        this.jsonFeedService = jsonFeedService;
        this.rssFeedService = rssFeedService;
        this.errorHandler = errorHandler;
    }

    public async Task<(FeedListItem? FeedList, IList<Models.FeedItem>? FeedItemList)> FetchRssFeed(Uri feedUri, CancellationToken? token = default)
    {
        var (feedListItem, feedItem) = await this.rssFeedService.ReadFeedAsync(feedUri, token: token);

        return (feedListItem, feedItem);
    }

    /// <summary>
    /// Refresh Feeds Async.
    /// </summary>
    /// <param name="progress">Optional Progress Marker.</param>
    /// <returns>Task.</returns>
    public async Task RefreshFeedsAsync(IProgress<RssCacheFeedUpdate>? progress = default)
    {
        var feeds = await this.database.GetFeedListItemsAsync();
        await this.RefreshFeedsAsync(feeds, progress);
    }

    /// <summary>
    /// Refresh Feeds Async.
    /// </summary>
    /// <param name="feeds">Feeds.</param>
    /// <param name="progress">Optional Progress Marker.</param>
    /// <returns>Task.</returns>
    public async Task RefreshFeedsAsync(IEnumerable<FeedListItem> feeds, IProgress<RssCacheFeedUpdate>? progress = default)
    {
        
    }
}