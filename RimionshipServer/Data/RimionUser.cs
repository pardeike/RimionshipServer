﻿using Microsoft.AspNetCore.Identity;

namespace RimionshipServer.Data
{
    public class RimionUser : IdentityUser
    {
        public string? AvatarUrl { get; set; }

        public virtual LatestStats? LatestStats { get; set; }
    }
}
