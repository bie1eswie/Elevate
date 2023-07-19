using Elevate.Model.HumanAPI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Data.HumanAPI
{
    public class HumanAPIContext: DbContext
    {
        public DbSet<HumanAPIUser> HumanAPIUsers { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public HumanAPIContext(DbContextOptions<HumanAPIContext> options) : base(options)
        {

        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
