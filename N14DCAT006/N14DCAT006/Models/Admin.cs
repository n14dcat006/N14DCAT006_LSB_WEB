namespace N14DCAT006.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Admin")]
    public partial class Admin
    {
        [Key]
        public int MaAdmin { get; set; }

        [StringLength(50)]
        public string TaiKhoanAdmin { get; set; }

        [StringLength(50)]
        public string HoTenAdmin { get; set; }

        [StringLength(50)]
        public string MatKhauAdmin { get; set; }
    }
}
