using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cyrus.ZConsole.Core.Entities
{
    [Table(Name = "antennas_pattern")]
    public class AntennasPattern
    {
        private DateTime _logDate = DateTime.Now;
        [Column(Name = "DEF_NAME", IsPrimary = true)]
        public string DefName { get; set; }
        [Column(Name = "NAME")]
        public string Name { get; set; }
        [Column(Name = "MAKE")]
        public string Make { get; set; }
        [Column(Name = "FREQUENCY")]
        public double? Frequency { get; set; }
        [Column(Name = "H_WIDTH")]
        public double? HWidth { get; set; }
        [Column(Name = "V_WIDTH")]
        public double? VWidth { get; set; }
        [Column(Name = "FRONT_TO_BACK")]
        public double? FrontToBack { get; set; }
        [Column(Name = "GAIN")]
        public double? Gain { get; set; }
        [Column(Name = "TILT")]
        public string Tilt { get; set; }
        [Column(Name = "HORIZONTAL")]
        public string Horizontal { get; set; }
        [Column(Name = "VERTICAL")]
        public string Vertical { get; set; }
        [Column(Name = "VERTICAL_FIX")]
        public string VerticalFix { get; set; }
        [Column(Name = "COMMENT_DATE")]
        public string CommentDate { get; set; }
        [Column(Name = "COMMENT")]
        public string Comment { get; set; }
        [Column(Name = "LOG_DATE")]
        public DateTime LogDate { get { return _logDate; } set { _logDate = value; } }
    }
}
