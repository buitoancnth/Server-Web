namespace ServerBTS2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BaoCaos",
                c => new
                    {
                        IDBaoCao = c.Int(nullable: false, identity: true),
                        IDQuanLy = c.String(),
                        ThoiGian = c.DateTime(nullable: false),
                        VanDe = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IDBaoCao);
            
            CreateTable(
                "dbo.HinhAnhs",
                c => new
                    {
                        IDHinhAnh = c.Int(nullable: false, identity: true),
                        IDTheLoaiAnh = c.Int(nullable: false),
                        IDChung = c.Int(nullable: false),
                        TenAnh = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IDHinhAnh);
            
            CreateTable(
                "dbo.HinhAnhTrams",
                c => new
                    {
                        IDHinhAnh = c.Int(nullable: false, identity: true),
                        IDTram = c.Int(nullable: false),
                        Ten = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IDHinhAnh);
            
            CreateTable(
                "dbo.LichSuQuanLies",
                c => new
                    {
                        IDLichSu = c.Int(nullable: false, identity: true),
                        IDTram = c.Int(nullable: false),
                        IDQuanLy = c.String(),
                        ThoiGianLamViec = c.DateTime(nullable: false),
                        ThoiGianKetThuc = c.DateTime(),
                    })
                .PrimaryKey(t => t.IDLichSu);
            
            CreateTable(
                "dbo.MatDiens",
                c => new
                    {
                        IDMatDien = c.Int(nullable: false, identity: true),
                        IDNhaTram = c.Int(nullable: false),
                        NgayMatDien = c.DateTime(nullable: false),
                        GioMatDien = c.Time(nullable: false, precision: 7),
                        ThoiGianMayNo = c.Time(nullable: false, precision: 7),
                        ThoiGianNgung = c.Time(nullable: false, precision: 7),
                        TongThoiGianChay = c.Time(precision: 7),
                        QuangDuongDiChuyen = c.Double(nullable: false),
                        TienPhat = c.Double(),
                    })
                .PrimaryKey(t => t.IDMatDien);
            
            CreateTable(
                "dbo.NhaMangs",
                c => new
                    {
                        IDNhaMang = c.Int(nullable: false, identity: true),
                        TenNhaMang = c.String(nullable: false),
                        Image = c.String(),
                    })
                .PrimaryKey(t => t.IDNhaMang);
            
            CreateTable(
                "dbo.NhatKies",
                c => new
                    {
                        IDNhatKy = c.Int(nullable: false, identity: true),
                        IDQuanLy = c.String(),
                        IDTram = c.Int(nullable: false),
                        ThoiGian = c.DateTime(nullable: false),
                        NoiDung = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IDNhatKy);
            
            CreateTable(
                "dbo.NhaTrams",
                c => new
                    {
                        IDNhaTram = c.Int(nullable: false, identity: true),
                        IDNhaMang = c.Int(nullable: false),
                        IDTram = c.Int(nullable: false),
                        CauCap = c.Int(nullable: false),
                        HeThongDien = c.Int(nullable: false),
                        HangRao = c.Int(nullable: false),
                        DieuHoa = c.Int(nullable: false),
                        OnAp = c.Int(nullable: false),
                        CanhBao = c.Int(nullable: false),
                        BinhCuuHoa = c.Int(nullable: false),
                        MayPhatDien = c.Int(nullable: false),
                        ChungMayPhat = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IDNhaTram);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.TheLoaiAnhs",
                c => new
                    {
                        IDTheLoaiAnh = c.Int(nullable: false, identity: true),
                        TenTheLoai = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IDTheLoaiAnh);
            
            CreateTable(
                "dbo.Trams",
                c => new
                    {
                        IDTram = c.Int(nullable: false, identity: true),
                        TenTram = c.String(nullable: false),
                        CotAnten = c.Int(nullable: false),
                        CotTiepDat = c.Int(nullable: false),
                        SanTram = c.Int(nullable: false),
                        Tinh = c.String(nullable: false),
                        ViDo = c.Double(nullable: false),
                        KinhDo = c.Double(nullable: false),
                        IDQuanLy = c.String(nullable: false),
                        BanKinhPhuSong = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.IDTram);
            
            CreateTable(
                "dbo.UserBTS",
                c => new
                    {
                        IDUser = c.String(nullable: false, maxLength: 128),
                        Ten = c.String(nullable: false),
                        DiaChi = c.String(nullable: false),
                        NgaySinh = c.DateTime(nullable: false),
                        GioiTinh = c.String(nullable: false),
                        Image = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        ChucVu = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IDUser);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.UserBTS");
            DropTable("dbo.Trams");
            DropTable("dbo.TheLoaiAnhs");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.NhaTrams");
            DropTable("dbo.NhatKies");
            DropTable("dbo.NhaMangs");
            DropTable("dbo.MatDiens");
            DropTable("dbo.LichSuQuanLies");
            DropTable("dbo.HinhAnhTrams");
            DropTable("dbo.HinhAnhs");
            DropTable("dbo.BaoCaos");
        }
    }
}
