namespace N14DCAT006.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class N14DCAT006DbContext : DbContext
    {
        public N14DCAT006DbContext()
            : base("name=N14DCAT006DbContext1")
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<BaiHat> BaiHats { get; set; }
        public virtual DbSet<DownloadBaiHat> DownloadBaiHats { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
