﻿using System;
using System.Collections.Generic;

namespace Doublelives.Migrations.DbFirstProcess
{
    public class SysRole : AuditableEntityBase
    {
        public int? Deptid { get; set; }

        public string Name { get; set; }

        public int? Num { get; set; }

        public int? Pid { get; set; }

        public string Tips { get; set; }

        public int? Version { get; set; }
    }
}
