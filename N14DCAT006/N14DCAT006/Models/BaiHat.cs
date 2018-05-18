namespace N14DCAT006.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BaiHat")]
    public partial class BaiHat
    {
        [Key]
        public int MaBaiHat { get; set; }

        public string TenBaiHat { get; set; }

        public int? NamPhatHanh { get; set; }

        public string SangTac { get; set; }

        public string Abum { get; set; }

        public string LoiBaiHat { get; set; }

        public int? LuotXem { get; set; }

        public int? LuotTai { get; set; }

        public string AnhBiaBaiHat { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgayUp { get; set; }

        public string link { get; set; }

        public string TheLoai { get; set; }

        public string CaSi { get; set; }
    }
}
