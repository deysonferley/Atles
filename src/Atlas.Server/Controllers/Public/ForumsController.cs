﻿using System;
using System.Threading.Tasks;
using Atlas.Domain.PermissionSets;
using Atlas.Models;
using Atlas.Models.Public;
using Atlas.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Server.Controllers.Public
{
    [Route("api/public/forums")]
    [ApiController]
    public class ForumsController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IPublicModelBuilder _modelBuilder;
        private readonly ISecurityService _securityService;
        private readonly IPermissionModelBuilder _permissionModelBuilder;

        public ForumsController(IContextService contextService, 
            IPublicModelBuilder modelBuilder,
            ISecurityService securityService,
            IPermissionModelBuilder permissionModelBuilder)
        {
            _contextService = contextService;
            _modelBuilder = modelBuilder;
            _securityService = securityService;
            _permissionModelBuilder = permissionModelBuilder;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ForumPageModel>> Forum(Guid id, [FromQuery] int? page = 1, [FromQuery] string search = null)
        {
            var site = await _contextService.CurrentSiteAsync();

            var permissions = await _permissionModelBuilder.BuildPermissionModelsByForumId(site.Id, id);

            var canViewForum = _securityService.HasPermission(PermissionType.ViewForum, permissions);
            var canViewTopics = _securityService.HasPermission(PermissionType.ViewTopics, permissions);

            if (!canViewForum || !canViewTopics)
            {
                return Unauthorized();
            }

            var model = await _modelBuilder.BuildForumPageModelAsync(site.Id, id, new QueryOptions(search, page));

            if (model == null)
            {
                return NotFound();
            }

            model.CanRead = _securityService.HasPermission(PermissionType.Read, permissions);
            model.CanStart = _securityService.HasPermission(PermissionType.Start, permissions);

            return model;
        }

        [HttpGet("{id}/topics")]
        public async Task<ActionResult<PaginatedData<ForumPageModel.TopicModel>>> Topics(Guid id, [FromQuery] int? page = 1, [FromQuery] string search = null)
        {
            var site = await _contextService.CurrentSiteAsync();

            var permissions = await _permissionModelBuilder.BuildPermissionModelsByForumId(site.Id, id);

            var canViewForum = _securityService.HasPermission(PermissionType.ViewForum, permissions);
            var canViewTopics = _securityService.HasPermission(PermissionType.ViewTopics, permissions);

            if (!canViewForum || !canViewTopics)
            {
                return Unauthorized();
            }

            var result = await _modelBuilder.BuildForumPageModelTopicsAsync(id, new QueryOptions(search, page));

            return result;
        }
    }
}
