﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Model.HumanAPI
{
    public class PublicTokenRequest
    {
        public string humanId { get; set; } = string.Empty;
        public string clientId { get; set; } = string.Empty;
        public string clientSecret { get; set; }= string.Empty;
    }
}
