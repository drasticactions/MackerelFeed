// <copyright file="FeedTests.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using MackerelFeed.Services;

namespace MackerelFeed.Tests;

/// <summary>
/// Tests the feed parsing.
/// </summary>
[TestClass]
public sealed class FeedTests
{
    private readonly TestAppDispatcher appDispatcher = new();
    private readonly TestErrorHandler errorHandler = new();

    /// <summary>
    /// Test the JsonFeedService.
    /// </summary>
    /// <param name="jsonUri">Json Uri.</param>
    /// <returns>Task.</returns>
    [TestMethod]
    [DataRow("https://daringfireball.net/feeds/json")]
    public async Task JsonFeedServiceTest(string jsonUri)
    {
        using var client = new HttpClient();
        var jsonFeedService = new JsonFeedService(this.errorHandler, client);
        Uri? uri = null;
        if (!Uri.TryCreate(jsonUri, UriKind.Absolute, out uri))
        {
            Assert.Fail($"jsonUri {jsonUri} is not valid");
        }

        var (feedItem, feedItemList) = await jsonFeedService.ReadFeedAsync(uri);
        Assert.IsNotNull(feedItem);
        Assert.IsNotNull(feedItemList);
        Assert.IsTrue(feedItemList.Any());
        Assert.IsTrue(!string.IsNullOrEmpty(feedItem.Name));
    }

    /// <summary>
    /// Test the RssFeedService.
    /// </summary>
    /// <param name="rssUri">Rss Uri.</param>
    /// <returns>Task.</returns>
    [TestMethod]
    [DataRow("https://ascii.jp/rss.xml")]
    public async Task RssFeedServiceTest(string rssUri)
    {
        using var client = new HttpClient();
        var rssFeedService = new RssFeedService(this.errorHandler, client);
        Uri? uri = null;
        if (!Uri.TryCreate(rssUri, UriKind.Absolute, out uri))
        {
            Assert.Fail($"rssUri {rssUri} is not valid");
        }

        var (feedItem, feedItemList) = await rssFeedService.ReadFeedAsync(uri);
        Assert.IsNotNull(feedItem);
        Assert.IsNotNull(feedItemList);
        Assert.IsTrue(feedItemList.Any());
        Assert.IsTrue(!string.IsNullOrEmpty(feedItem.Name));
    }
}