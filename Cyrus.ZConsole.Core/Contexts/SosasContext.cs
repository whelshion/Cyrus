using Cyrus.ZConsole.Core.Entities;
using FreeSql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cyrus.ZConsole.Core.Contexts
{
    public class SosasContext : DbContext
    {
        public DbSet<AntennasPattern> AntennasPatterns { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            IFreeSql fsql = new FreeSqlBuilder()
                .UseConnectionString(DataType.MySql,
                    @"Server=192.168.5.222;database=ROSAS;uid=root;pwd=000000;ConvertZeroDateTime=True;Charset=utf8;SslMode=None;AllowUserVariables=True")
                //.UseAutoSyncStructure(true) //自动同步实体结构到数据库
                .Build();
            builder.UseFreeSql(fsql);
        }
    }
}
