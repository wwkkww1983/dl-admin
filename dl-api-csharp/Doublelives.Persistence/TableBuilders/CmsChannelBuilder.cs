﻿using Doublelives.Domain.Cms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doublelives.Persistence.TableBuilders
{
    public class CmsChannelBuilder : IEntityTypeConfiguration<CmsChannel>
    {
        public void Configure(EntityTypeBuilder<CmsChannel> builder)
        {
            builder.ToTable("cms_channel");

            builder.HasComment("文章栏目");

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("INTEGER");

            builder.Property(e => e.Code)
                .HasColumnName("code")
                .HasColumnType("varchar(64)")
                .HasComment("编码")
                .HasCharSet("utf8")
                .HasCollation("utf8_general_ci");

            builder.Property(e => e.CreateBy)
                .HasColumnName("create_by")
                .HasColumnType("INTEGER")
                .HasComment("创建人");

            builder.Property(e => e.CreateTime)
                .HasColumnName("create_time")
                .HasColumnType("datetime")
                .HasComment("创建时间/注册时间");

            builder.Property(e => e.ModifyBy)
                .HasColumnName("modify_by")
                .HasColumnType("INTEGER")
                .HasComment("最后更新人");

            builder.Property(e => e.ModifyTime)
                .HasColumnName("modify_time")
                .HasColumnType("datetime")
                .HasComment("最后更新时间");

            builder.Property(e => e.Name)
                .HasColumnName("name")
                .HasColumnType("varchar(64)")
                .HasComment("名称")
                .HasCharSet("utf8")
                .HasCollation("utf8_general_ci");
        }
    }

}
