﻿using Doublelives.Domain.Infrastructure;

namespace Doublelives.Domain.Messages
{
    public class MessageTemplate : AuditableEntityBase
    {
        public string Code { get; set; }

        public string Cond { get; set; }

        public string Content { get; set; }

        public int IdMessageSender { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public virtual MessageSender IdMessageSenderNavigation { get; set; }
    }
}