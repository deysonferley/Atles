﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Atlas.Domain.Posts;
using Docs.Attributes;

namespace Atlas.Domain.Users
{
    [DocTarget(Consts.DocsContextForum)]
    public class User
    {
        public Guid Id { get; private set; }
        [Column("IdentityUserId")]
        public string IdentityUserId { get; private set; }
        public string Email { get; private set; }
        public string DisplayName { get; private set; }
        public int TopicsCount { get; private set; }
        public int RepliesCount { get; private set; }
        public StatusType Status { get; private set; }
        public DateTime TimeStamp { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Event> Events { get; set; }

        public User()
        {
            
        }

        public User(Guid id, string identityUserId, string email, string displayName)
        {
            New(id, identityUserId, email, displayName);
        }

        public User(string identityUserId, string email, string displayName)
        {
            New(Guid.NewGuid(), identityUserId, email, displayName);
        }

        private void New(Guid id, string identityUserId, string email, string displayName)
        {
            Id = id;
            IdentityUserId = identityUserId;
            Email = email;
            DisplayName = displayName;
            Status = StatusType.Pending;
            TimeStamp = DateTime.UtcNow;
        }

        public void Confirm()
        {
            Status = StatusType.Active;
        }

        public void UpdateDetails(string displayName)
        {
            DisplayName = displayName;
        }

        public void IncreaseTopicsCount()
        {
            TopicsCount += 1;
        }

        public void IncreaseRepliesCount()
        {
            RepliesCount += 1;
        }

        public void DecreaseTopicsCount()
        {
            TopicsCount -= 1;

            if (TopicsCount < 0)
            {
                TopicsCount = 0;
            }
        }

        public void DecreaseRepliesCount()
        {
            RepliesCount -= 1;

            if (RepliesCount < 0)
            {
                RepliesCount = 0;
            }
        }

        public void Suspend()
        {
            Status = StatusType.Suspended;
        }

        public void Reinstate()
        {
            Status = StatusType.Active;
        }

        public void Delete()
        {
            Status = StatusType.Deleted;
        }
    }
}