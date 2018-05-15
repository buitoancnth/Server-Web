using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ServerBTS2.Models
{
    public class LichSuQuanLy
    {
        [Key]
        public int IDLichSu { get; set; }
        public int IDTram { get; set; }
        public string IDQuanLy { get; set; }
        public DateTime ThoiGianLamViec { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }

    }
}