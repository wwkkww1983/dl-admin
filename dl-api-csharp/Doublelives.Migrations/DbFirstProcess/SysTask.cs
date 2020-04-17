﻿using System;
using System.Collections.Generic;

namespace Doublelives.Migrations.DbFirstProcess
{
    public class SysTask : AuditableEntityBase
    {
        public sbyte? Concurrent { get; set; }

        public string Cron { get; set; }

        public string Data { get; set; }

        public sbyte? Disabled { get; set; }

        public DateTime? ExecAt { get; set; }

        public string ExecResult { get; set; }

        public string JobClass { get; set; }

        public string JobGroup { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }
    }
}
