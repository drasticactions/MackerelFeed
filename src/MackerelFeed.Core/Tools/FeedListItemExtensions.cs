// <copyright file="FeedListItemExtensions.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Web;
using JsonFeedNet;
using MackerelFeed.Models;
using MackerelFeed.Models.OPML;

namespace MackerelFeed.Tools;

/// <summary>
/// FeedListItem Extensions.
/// </summary>
public static class FeedListItemExtensions
{
    /// <summary>
    /// Get Image for Item.
    /// </summary>
    /// <param name="client">The HttpClient.</param>
    /// <param name="item">The FeedListItem.</param>
    /// <param name="items">The list of items.</param>
    /// <returns>Task.</returns>
    public static async Task GetImageForItem(this HttpClient client, FeedListItem item, List<Models.FeedItem> items)
    {
        var imageDownload = Array.Empty<byte>();
        if (item.ImageUri is not null)
        {
            try
            {
                imageDownload = await client.GetByteArrayAsync(item.ImageUri);
            }
            catch (Exception)
            {
                // If it fails to work for whatever reason, ignore it for now, use the placeholder.
            }
        }

        if (!imageDownload.IsValidImage() && item.Uri is not null)
        {
            try
            {
                // If ImageUri is null, try to get the favicon from the site itself.
                imageDownload = await client.GetFaviconFromUriAsync(item.Uri) ?? Array.Empty<byte>();
            }
            catch (Exception)
            {
                // If it fails to work for whatever reason, ignore it for now, use the placeholder.
            }
        }

        if (!imageDownload.IsValidImage() && items.Any() && items.First().Link is string link)
        {
            try
            {
                // If ImageUri is null, try to get the favicon from the site itself.
                imageDownload = await client.GetFaviconFromUriAsync(new Uri(link)) ?? Array.Empty<byte>();
            }
            catch (Exception)
            {
                // If it fails to work for whatever reason, ignore it for now, use the placeholder.
            }
        }

        if (!imageDownload.IsValidImage())
        {
            imageDownload = Utilities.GetPlaceholderIcon();
        }

        item.ImageCache = imageDownload;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeedListItem"/> class.
    /// </summary>
    /// <param name="feed"><see cref="Outline"/>.</param>
    /// <returns><see cref="FeedListItem"/>.</returns>
    public static FeedListItem ToFeedListItem(this Outline feed)
    {
        return new FeedListItem()
        {
            Name = feed.Title,
            Uri = feed.XMLUrl is not null ? new Uri(feed.XMLUrl) : null,
            Link = feed.HTMLUrl,
            ImageUri = null,
            Description = feed.Description,
            Language = feed.Language,
            Folder = feed.Parent is not null ? new FeedFolder() { Name = feed.Parent!.Title } : null,
        };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeedListItem"/> class.
    /// </summary>
    /// <param name="feed"><see cref="Sagara.FeedReader.Feed"/>.</param>
    /// <param name="feedUri">Original Feed Uri.</param>
    /// <returns><see cref="FeedListItem"/>.</returns>
    public static FeedListItem ToFeedListItem(this Sagara.FeedReader.Feed feed, string feedUri)
    {
        return new FeedListItem()
        {
            Name = feed.Title,
            Uri = new Uri(feedUri),
            Link = feed.Link,
            ImageUri = string.IsNullOrEmpty(feed.ImageUrl) ? null : new Uri(feed.ImageUrl),
            Description = feed.Description,
            Language = feed.Language,
            LastUpdatedDate = feed.LastUpdatedDate,
            LastUpdatedDateString = feed.LastUpdatedDateString,
            FeedType = Models.FeedType.Rss,
        };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeedItem"/> class.
    /// </summary>
    /// <param name="item"><see cref="Sagara.FeedReader.FeedReader.FeedItem"/>.</param>
    /// <param name="feedListItem"><see cref="FeedListItem"/>.</param>
    /// <param name="imageUrl">Image Url.</param>
    /// <returns><see cref="Core.FeedItem"/>.</returns>
    public static FeedItem ToFeedItem(this Sagara.FeedReader.FeedItem item, FeedListItem feedListItem, string? imageUrl = "")
    {
        return new FeedItem()
        {
            RssId = item.Id,
            FeedListItemId = feedListItem.Id,
            Title = item.Title,
            Link = item.Link,
            Description = item.Description,
            PublishingDate = item.PublishingDate,
            Author = item.Author,
            Content = item.Content,
            ImageUrl = imageUrl,
        };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeedListItem"/> class.
    /// </summary>
    /// <param name="feed"><see cref="Sagara.FeedReader.Feed"/>.</param>
    /// <param name="oldItem">Original Feed Uri.</param>
    /// <returns><see cref="FeedListItem"/>.</returns>
    public static FeedListItem Update(this Sagara.FeedReader.Feed feed, FeedListItem oldItem)
    {
        oldItem.Name = feed.Title;
        oldItem.Link = feed.Link;
        oldItem.ImageUri = string.IsNullOrEmpty(feed.ImageUrl) ? null : new Uri(feed.ImageUrl);
        oldItem.Description = feed.Description;
        oldItem.Language = feed.Language;
        oldItem.LastUpdatedDate = feed.LastUpdatedDate;
        oldItem.FeedType = FeedType.Rss;
        return oldItem;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeedListItem"/> class.
    /// </summary>
    /// <param name="feed"><see cref="Sagara.FeedReader.Feed"/>.</param>
    /// <param name="oldItem">Old Item.</param>
    /// <returns><see cref="FeedListItem"/>.</returns>
    public static FeedListItem Update(this JsonFeed feed, FeedListItem oldItem)
    {
        oldItem.Name = feed.Title;
        oldItem.Link = feed.HomePageUrl;
        oldItem.ImageUri = string.IsNullOrEmpty(feed.Icon) ? null : new Uri(feed.Icon);
        oldItem.Description = feed.Description;
        oldItem.Language = feed.Language;
        oldItem.FeedType = Models.FeedType.Json;
        return oldItem;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeedItem"/> class.
    /// </summary>
    /// <param name="item"><see cref="FeedItem"/>.</param>
    /// <param name="feedListItem"><see cref="FeedListItem"/>.</param>
    /// <param name="imageUrl">Image Url.</param>
    /// <returns>A <see cref="FeedItem"/>.</returns>
    public static Models.FeedItem ToFeedItem(this JsonFeedItem item, FeedListItem feedListItem, string? imageUrl = "")
    {
        var authors = string.Empty;
        if (item.Authors is not null)
        {
            authors = string.Join(", ", item.Authors.Select(n => n.Name));
        }

        var content = item.ContentHtml ?? item.ContentText;

        return new Models.FeedItem()
        {
            RssId = item.Id,
            FeedListItemId = feedListItem.Id,
            Title = HttpUtility.HtmlDecode(item.Title),
            Link = item.Url,
            Description = string.Empty,
            PublishingDate = item.DatePublished?.DateTime ?? DateTime.MinValue,
            Author = authors,
            Content = HttpUtility.HtmlDecode(content),
            ImageUrl = imageUrl,
        };
    }

    private static async Task<byte[]?> GetFaviconFromUriAsync(this HttpClient client, string uri)
    => await GetByteArrayAsync(client, new Uri(uri));

    private static async Task<byte[]?> GetFaviconFromUriAsync(this HttpClient client, Uri uri)
        => await GetByteArrayAsync(client, new Uri($"{uri.Scheme}://{uri.Host}/favicon.ico"));

    private static async Task<byte[]?> GetByteArrayAsync(this HttpClient client, Uri uri)
    {
        using HttpResponseMessage response = await client.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
        {
            throw new ArgumentException("Could not get image");
        }

        return await response.Content.ReadAsByteArrayAsync();
    }
}