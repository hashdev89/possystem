﻿using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Users.Permissions.PermissionItems
{
    public class PermissionItem : ApiEntity
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Token { get; set; }
        public Guid PermissionGroupId { get; set; }
    }
}