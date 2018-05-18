namespace N14DCAT006.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        [Key]
        public int MaUser { get; set; }

        [StringLength(50)]
        public string TaiKhoanUser { get; set; }

        public string HoTenUser { get; set; }

        public string MatKhauUser { get; set; }

        public string Email { get; set; }

        [StringLength(50)]
        public string SoDienThoai { get; set; }

        public string AnhBiaUser { get; set; }

        public bool Status { get; set; }
    }
}
