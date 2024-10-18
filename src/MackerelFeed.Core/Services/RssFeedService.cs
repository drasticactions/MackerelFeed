// <copyright file="RssFeedService.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using DA.UI.Services;
using MackerelFeed.Models;
using MackerelFeed.Tools;
using Sagara.FeedReader;

namespace MackerelFeed.Services;

/// <summary>
/// Rss Feed Service.
/// </summary>
public class RssFeedService : IFeedService
{
    private HttpClient client;
    private IErrorHandler errorHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="RssFeedService"/> class.
    /// </summary>
    /// <param name="errorHandler"><see cref="IErrorHandler"/>.</param>
    /// <param name="client">Optional HttpClient for reading the RSS and image data.</param>
    public RssFeedService(IErrorHandler errorHandler, HttpClient? client = default)
    {
        this.client = client ?? new HttpClient();
        if (string.IsNullOrEmpty(this.client.DefaultRequestHeaders.UserAgent.ToString()))
        {
            this.client.DefaultRequestHeaders.UserAgent.ParseAdd("MackerelFeed/1.0");
        }
        this.errorHandler = errorHandler;
    }

    /// <inheritdoc/>
    public MackerelFeed.Models.FeedType ServiceType => MackerelFeed.Models.FeedType.Rss;

    /// <inheritdoc/>
    public async Task<(FeedListItem? FeedList, IList<Models.FeedItem>? FeedItemList)> ReadFeedAsync(string stringResponse, FeedListItem feedItem, CancellationToken? token = default)
    {
        var feed = FeedReader.ReadFromString(stringResponse);
        feedItem = feed.Update(feedItem);
        var items = feed.Items.Select(n => n.ToFeedItem(feedItem)).ToList();

        if (!feedItem.ImageCache?.IsValidImage() ?? true)
        {
            await this.client.GetImageForItem(feedItem, items);
        }

        return (feedItem, items);
    }

    /// <inheritdoc/>
    public async Task<(FeedListItem? FeedList, IList<Models.FeedItem>? FeedItemList)> ReadFeedAsync(Uri feedUri, FeedListItem? feedItem = null, CancellationToken? token = null)
    {
        var stringResponse = await this.client.GetStringAsync(feedUri);
        return await this.ReadFeedAsync(stringResponse, feedItem ?? new FeedListItem() { Uri = feedUri }, token);
    }
}