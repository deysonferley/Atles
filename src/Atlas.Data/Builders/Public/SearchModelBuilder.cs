﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Models;
using Atlas.Models.Public.Search;
using Markdig;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Builders.Public
{
    public class SearchModelBuilder : ISearchModelBuilder
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IGravatarService _gravatarService;

        public SearchModelBuilder(AtlasDbContext dbContext,
            ICacheManager cacheManager,
            IGravatarService gravatarService)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _gravatarService = gravatarService;
        }

        public async Task<SearchPageModel> BuildSearchPageModelAsync(Guid siteId, IList<Guid> forumIds, QueryOptions options)
        {
            var result = new SearchPageModel
            {
                Posts = await SearchPostModels(forumIds, options)
            };

            return result;
        }

        public async Task<PaginatedData<SearchPostModel>> SearchPostModels(IList<Guid> forumIds, QueryOptions options, Guid? memberId = null)
        {
            var postsQuery = _dbContext.Posts
                .Where(x =>
                    forumIds.Contains(x.ForumId) &&
                    x.Status == StatusType.Published &&
                    (x.Topic == null || x.Topic.Status == StatusType.Published));

            if (!string.IsNullOrWhiteSpace(options.Search))
            {
                postsQuery = postsQuery
                    .Where(x => x.Title.Contains(options.Search) || x.Content.Contains(options.Search));
            }

            if (memberId != null)
            {
                postsQuery = postsQuery.Where(x => x.MemberId == memberId);
            }

            var posts = await postsQuery
                .OrderByDescending(x => x.TimeStamp)
                .Skip(options.Skip)
                .Take(options.PageSize)
                .Select(p => new
                {
                    p.Id,
                    TopicId = p.TopicId ?? p.Id,
                    IsTopic = p.TopicId == null,
                    Title = p.Title ?? p.Topic.Title,
                    p.Content,
                    p.TimeStamp,
                    p.MemberId,
                    MemberDisplayName = p.Member.DisplayName,
                    p.ForumId,
                    ForumName = p.Forum.Name
                })
                .ToListAsync();

            var items = posts.Select(post => new SearchPostModel
            {
                Id = post.Id,
                TopicId = post.TopicId,
                IsTopic = post.IsTopic,
                Title = post.Title,
                Content = Markdown.ToHtml(post.Content),
                TimeStamp = post.TimeStamp,
                MemberId = post.MemberId,
                MemberDisplayName = post.MemberDisplayName,
                ForumId = post.ForumId,
                ForumName = post.ForumName
            }).ToList();

            var totalRecords = await postsQuery.CountAsync();

            return new PaginatedData<SearchPostModel>(items, totalRecords, options.PageSize);
        }
    }
}