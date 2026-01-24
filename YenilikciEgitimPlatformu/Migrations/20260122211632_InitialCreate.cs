using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YenilikciEgitimPlatformu.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DuyuruKategorileri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ikon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Renk = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuyuruKategorileri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Iller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlakaKodu = table.Column<int>(type: "int", nullable: false),
                    Bolge = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Iller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roller",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rozetler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    GorselUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Ikon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Seviye = table.Column<int>(type: "int", nullable: false),
                    Renk = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    XPOdulu = table.Column<int>(type: "int", nullable: false),
                    Kategori = table.Column<int>(type: "int", nullable: false),
                    KosulEventAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    KosulDegeri = table.Column<int>(type: "int", nullable: true),
                    NadirMi = table.Column<bool>(type: "bit", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    Sira = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rozetler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ilceler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IlId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ilceler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ilceler_Iller_IlId",
                        column: x => x.IlId,
                        principalTable: "Iller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolClaim_Roller_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IslemTuru = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityTuru = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EskiDeger = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YeniDeger = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAdresi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Begeniler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BegeniTuru = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IliskiliEntityId = table.Column<int>(type: "int", nullable: false),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GonderiId = table.Column<int>(type: "int", nullable: true),
                    YorumId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Begeniler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bildirimler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AliciKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    GonderenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Baslik = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Mesaj = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Turu = table.Column<int>(type: "int", nullable: false),
                    HedefUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IliskiliEntityTuru = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IliskiliEntityId = table.Column<int>(type: "int", nullable: true),
                    OkunduMu = table.Column<bool>(type: "bit", nullable: false),
                    OkunmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ikon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Renk = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bildirimler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CagriBilgileri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    KisaAciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DetayliAciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KapakGorseliUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CagriTuru = table.Column<int>(type: "int", nullable: false),
                    Kategori = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    KurumAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ResmiLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IletisimEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IletisimTelefon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    YayinlanmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CagriBaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CagriBitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BasvuruBaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BasvuruBitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KatilimSartlari = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HedefKitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EtkinlikBaslangicSaati = table.Column<TimeSpan>(type: "time", nullable: true),
                    EtkinlikBitisSaati = table.Column<TimeSpan>(type: "time", nullable: true),
                    EtkinlikYeri = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    OnlineLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaksimumKatilimci = table.Column<int>(type: "int", nullable: true),
                    SartnameDosyaUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OdulBilgisi = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Butce = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    YayindaMi = table.Column<bool>(type: "bit", nullable: false),
                    OneCikanMi = table.Column<bool>(type: "bit", nullable: false),
                    HedefIlId = table.Column<int>(type: "int", nullable: true),
                    HedefIlceId = table.Column<int>(type: "int", nullable: true),
                    HedefOkulId = table.Column<int>(type: "int", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturanKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GuncelleyenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CagriBilgileri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CagriBilgileri_Ilceler_HedefIlceId",
                        column: x => x.HedefIlceId,
                        principalTable: "Ilceler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CagriBilgileri_Iller_HedefIlId",
                        column: x => x.HedefIlId,
                        principalTable: "Iller",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CagriEkDosyalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CagriBilgisiId = table.Column<int>(type: "int", nullable: false),
                    DosyaUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DosyaTuru = table.Column<int>(type: "int", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DosyaBoyutu = table.Column<long>(type: "bigint", nullable: true),
                    Sira = table.Column<int>(type: "int", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturanKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GuncelleyenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CagriEkDosyalari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CagriEkDosyalari_CagriBilgileri_CagriBilgisiId",
                        column: x => x.CagriBilgisiId,
                        principalTable: "CagriBilgileri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CagriTakipleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CagriBilgisiId = table.Column<int>(type: "int", nullable: false),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    TakipTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmailBildirimiAlsin = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CagriTakipleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CagriTakipleri_CagriBilgileri_CagriBilgisiId",
                        column: x => x.CagriBilgisiId,
                        principalTable: "CagriBilgileri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dosyalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DosyaAdi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OrijinalDosyaAdi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DosyaYolu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DosyaUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DosyaBoyutu = table.Column<long>(type: "bigint", nullable: false),
                    Turu = table.Column<int>(type: "int", nullable: false),
                    IliskiliEntityTuru = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IliskiliEntityId = table.Column<int>(type: "int", nullable: true),
                    Genislik = table.Column<int>(type: "int", nullable: true),
                    Yukseklik = table.Column<int>(type: "int", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IndiriliSayisi = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DuyuruId = table.Column<int>(type: "int", nullable: true),
                    EtkinlikId = table.Column<int>(type: "int", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturanKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GuncelleyenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dosyalar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Duyurular",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Icerik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ozet = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    KapakGorseliUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KategoriId = table.Column<int>(type: "int", nullable: false),
                    Oncelik = table.Column<int>(type: "int", nullable: false),
                    HedefIlId = table.Column<int>(type: "int", nullable: true),
                    HedefIlceId = table.Column<int>(type: "int", nullable: true),
                    HedefOkulId = table.Column<int>(type: "int", nullable: true),
                    YayindaMi = table.Column<bool>(type: "bit", nullable: false),
                    OneCikanMi = table.Column<bool>(type: "bit", nullable: false),
                    OnaylandiMi = table.Column<bool>(type: "bit", nullable: false),
                    DisLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PdfUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GoruntulenmeSayisi = table.Column<int>(type: "int", nullable: false),
                    TakipEdenSayisi = table.Column<int>(type: "int", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturanKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GuncelleyenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Duyurular", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Duyurular_DuyuruKategorileri_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "DuyuruKategorileri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Duyurular_Ilceler_HedefIlceId",
                        column: x => x.HedefIlceId,
                        principalTable: "Ilceler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Duyurular_Iller_HedefIlId",
                        column: x => x.HedefIlId,
                        principalTable: "Iller",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DuyuruTakipleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DuyuruId = table.Column<int>(type: "int", nullable: false),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    TakipTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuyuruTakipleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DuyuruTakipleri_Duyurular_DuyuruId",
                        column: x => x.DuyuruId,
                        principalTable: "Duyurular",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EtkinlikKatilimcilari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EtkinlikId = table.Column<int>(type: "int", nullable: false),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KatildiMi = table.Column<bool>(type: "bit", nullable: false),
                    KatilimTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IptalEdildiMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtkinlikKatilimcilari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Etkinlikler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ozet = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    KapakGorseliUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Turu = table.Column<int>(type: "int", nullable: false),
                    Konum = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OnlineLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaksimumKatilimci = table.Column<int>(type: "int", nullable: true),
                    MevcutKatilimci = table.Column<int>(type: "int", nullable: false),
                    KayitAcik = table.Column<bool>(type: "bit", nullable: false),
                    YayindaMi = table.Column<bool>(type: "bit", nullable: false),
                    OneCikanMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturanKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GuncelleyenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etkinlikler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gonderiler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Icerik = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Turu = table.Column<int>(type: "int", nullable: false),
                    MedyaUrlleri = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Hashtagler = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProjeYonetimId = table.Column<int>(type: "int", nullable: true),
                    BegeniSayisi = table.Column<int>(type: "int", nullable: false),
                    YorumSayisi = table.Column<int>(type: "int", nullable: false),
                    PaylasimSayisi = table.Column<int>(type: "int", nullable: false),
                    GoruntulenmeSayisi = table.Column<int>(type: "int", nullable: false),
                    YayindaMi = table.Column<bool>(type: "bit", nullable: false),
                    SabitlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturanKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GuncelleyenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gonderiler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciClaim", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProfilFotografiUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    KapakFotografiUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    KullaniciTipi = table.Column<int>(type: "int", nullable: false),
                    OkulId = table.Column<int>(type: "int", nullable: true),
                    Brans = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Sinif = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OgrenciNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IlId = table.Column<int>(type: "int", nullable: true),
                    IlceId = table.Column<int>(type: "int", nullable: true),
                    Adres = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TwitterHandle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LinkedInUrl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeneyimPuani = table.Column<int>(type: "int", nullable: false),
                    Seviye = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    EmailBildirimleri = table.Column<bool>(type: "bit", nullable: false),
                    PushBildirimleri = table.Column<bool>(type: "bit", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SonGirisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SonAktiviteTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kullanicilar_Ilceler_IlceId",
                        column: x => x.IlceId,
                        principalTable: "Ilceler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Kullanicilar_Iller_IlId",
                        column: x => x.IlId,
                        principalTable: "Iller",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "KullaniciLogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_KullaniciLogin_Kullanicilar_UserId",
                        column: x => x.UserId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciRolleri",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciRolleri", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_KullaniciRolleri_Kullanicilar_UserId",
                        column: x => x.UserId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KullaniciRolleri_Roller_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciRozetleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RozetId = table.Column<int>(type: "int", nullable: false),
                    KazanilmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KazanimAciklamasi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GorunurMu = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturanKullaniciId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciRozetleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciRozetleri_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciRozetleri_Kullanicilar_OlusturanKullaniciId1",
                        column: x => x.OlusturanKullaniciId1,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KullaniciRozetleri_Rozetler_RozetId",
                        column: x => x.RozetId,
                        principalTable: "Rozetler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciToken",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_KullaniciToken_Kullanicilar_UserId",
                        column: x => x.UserId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModerasyonKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IcerikTuru = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IcerikId = table.Column<int>(type: "int", nullable: false),
                    IcerikSahibiId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Aksiyon = table.Column<int>(type: "int", nullable: false),
                    Gerekce = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    OrijinalIcerik = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModerasyonTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IhlalTuru = table.Column<int>(type: "int", nullable: true),
                    CiddiyetSeviyesi = table.Column<int>(type: "int", nullable: false),
                    KullaniciModerasyonSayisi = table.Column<int>(type: "int", nullable: false),
                    ModeratorNotu = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturanKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModerasyonKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModerasyonKayitlari_Kullanicilar_IcerikSahibiId",
                        column: x => x.IcerikSahibiId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModerasyonKayitlari_Kullanicilar_OlusturanKullaniciId",
                        column: x => x.OlusturanKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Okullar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Turu = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Adres = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    MEBOkulKodu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IlId = table.Column<int>(type: "int", nullable: false),
                    IlceId = table.Column<int>(type: "int", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturanKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GuncelleyenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Okullar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Okullar_Ilceler_IlceId",
                        column: x => x.IlceId,
                        principalTable: "Ilceler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Okullar_Iller_IlId",
                        column: x => x.IlId,
                        principalTable: "Iller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Okullar_Kullanicilar_GuncelleyenKullaniciId",
                        column: x => x.GuncelleyenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Okullar_Kullanicilar_OlusturanKullaniciId",
                        column: x => x.OlusturanKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Okullar_Kullanicilar_SilenKullaniciId",
                        column: x => x.SilenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjeKategorileri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Ikon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Renk = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sira = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturanKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GuncelleyenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjeKategorileri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjeKategorileri_Kullanicilar_GuncelleyenKullaniciId",
                        column: x => x.GuncelleyenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjeKategorileri_Kullanicilar_OlusturanKullaniciId",
                        column: x => x.OlusturanKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjeKategorileri_Kullanicilar_SilenKullaniciId",
                        column: x => x.SilenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Yorumlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Icerik = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    YorumTuru = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IliskiliEntityId = table.Column<int>(type: "int", nullable: false),
                    UstYorumId = table.Column<int>(type: "int", nullable: true),
                    BegeniSayisi = table.Column<int>(type: "int", nullable: false),
                    OnaylandiMi = table.Column<bool>(type: "bit", nullable: false),
                    IsaretliMi = table.Column<bool>(type: "bit", nullable: false),
                    GonderiId = table.Column<int>(type: "int", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturanKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GuncelleyenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yorumlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Yorumlar_Gonderiler_GonderiId",
                        column: x => x.GonderiId,
                        principalTable: "Gonderiler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Yorumlar_Kullanicilar_GuncelleyenKullaniciId",
                        column: x => x.GuncelleyenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Yorumlar_Kullanicilar_OlusturanKullaniciId",
                        column: x => x.OlusturanKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Yorumlar_Kullanicilar_SilenKullaniciId",
                        column: x => x.SilenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Yorumlar_Yorumlar_UstYorumId",
                        column: x => x.UstYorumId,
                        principalTable: "Yorumlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjeYonetimleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjeAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    KisaAciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ProjeAciklamasi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KapakGorseliUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    KurucuKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Amac = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Hedefler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Yontem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BeklenenCikti = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KategoriId = table.Column<int>(type: "int", nullable: true),
                    Etiketler = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaxKatilimciSayisi = table.Column<int>(type: "int", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HedefBitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TamamlanmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IlerlemeYuzdesi = table.Column<int>(type: "int", nullable: false),
                    HerkeseAcikMi = table.Column<bool>(type: "bit", nullable: false),
                    YeniUyeKabuluAcikMi = table.Column<bool>(type: "bit", nullable: false),
                    YayindaMi = table.Column<bool>(type: "bit", nullable: false),
                    KaynakCagriBilgisiId = table.Column<int>(type: "int", nullable: true),
                    OkulId = table.Column<int>(type: "int", nullable: true),
                    GoruntulenmeSayisi = table.Column<int>(type: "int", nullable: false),
                    BegeniSayisi = table.Column<int>(type: "int", nullable: false),
                    YorumSayisi = table.Column<int>(type: "int", nullable: false),
                    PaylasimSayisi = table.Column<int>(type: "int", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturanKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GuncelleyenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjeYonetimleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjeYonetimleri_CagriBilgileri_KaynakCagriBilgisiId",
                        column: x => x.KaynakCagriBilgisiId,
                        principalTable: "CagriBilgileri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjeYonetimleri_Kullanicilar_GuncelleyenKullaniciId",
                        column: x => x.GuncelleyenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjeYonetimleri_Kullanicilar_KurucuKullaniciId",
                        column: x => x.KurucuKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjeYonetimleri_Kullanicilar_OlusturanKullaniciId",
                        column: x => x.OlusturanKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjeYonetimleri_Kullanicilar_SilenKullaniciId",
                        column: x => x.SilenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjeYonetimleri_Okullar_OkulId",
                        column: x => x.OkulId,
                        principalTable: "Okullar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjeYonetimleri_ProjeKategorileri_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "ProjeKategorileri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjeAktiviteleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjeYonetimId = table.Column<int>(type: "int", nullable: false),
                    Tur = table.Column<int>(type: "int", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AktiviteTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Yer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OnlineLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    KatilimciIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturanKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GuncelleyenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjeAktiviteleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjeAktiviteleri_Kullanicilar_GuncelleyenKullaniciId",
                        column: x => x.GuncelleyenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjeAktiviteleri_Kullanicilar_OlusturanKullaniciId",
                        column: x => x.OlusturanKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjeAktiviteleri_Kullanicilar_SilenKullaniciId",
                        column: x => x.SilenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjeAktiviteleri_ProjeYonetimleri_ProjeYonetimId",
                        column: x => x.ProjeYonetimId,
                        principalTable: "ProjeYonetimleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjeDosyalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjeYonetimId = table.Column<int>(type: "int", nullable: false),
                    DosyaAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DosyaYolu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DosyaTuru = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DosyaBoyutu = table.Column<long>(type: "bigint", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Kategori = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Versiyon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturanKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GuncelleyenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjeDosyalari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjeDosyalari_Kullanicilar_GuncelleyenKullaniciId",
                        column: x => x.GuncelleyenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjeDosyalari_Kullanicilar_OlusturanKullaniciId",
                        column: x => x.OlusturanKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjeDosyalari_Kullanicilar_SilenKullaniciId",
                        column: x => x.SilenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjeDosyalari_ProjeYonetimleri_ProjeYonetimId",
                        column: x => x.ProjeYonetimId,
                        principalTable: "ProjeYonetimleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjeEkipUyeleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjeYonetimId = table.Column<int>(type: "int", nullable: false),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Rol = table.Column<int>(type: "int", nullable: false),
                    OzelRol = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    KatilmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AyrilmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    KurucuMu = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjeEkipUyeleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjeEkipUyeleri_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjeEkipUyeleri_ProjeYonetimleri_ProjeYonetimId",
                        column: x => x.ProjeYonetimId,
                        principalTable: "ProjeYonetimleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjeGorevleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjeYonetimId = table.Column<int>(type: "int", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AtananKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    Oncelik = table.Column<int>(type: "int", nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SonTeslimTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IlerlemeYuzdesi = table.Column<int>(type: "int", nullable: false),
                    TahminiSure = table.Column<int>(type: "int", nullable: true),
                    GerceklesenSure = table.Column<int>(type: "int", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturanKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GuncelleyenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjeGorevleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjeGorevleri_Kullanicilar_AtananKullaniciId",
                        column: x => x.AtananKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjeGorevleri_Kullanicilar_GuncelleyenKullaniciId",
                        column: x => x.GuncelleyenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjeGorevleri_Kullanicilar_OlusturanKullaniciId",
                        column: x => x.OlusturanKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjeGorevleri_Kullanicilar_SilenKullaniciId",
                        column: x => x.SilenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjeGorevleri_ProjeYonetimleri_ProjeYonetimId",
                        column: x => x.ProjeYonetimId,
                        principalTable: "ProjeYonetimleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityTuru_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityTuru", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_IslemTarihi",
                table: "AuditLogs",
                column: "IslemTarihi");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_IslemTuru",
                table: "AuditLogs",
                column: "IslemTuru");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_KullaniciId",
                table: "AuditLogs",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Begeniler_BegeniTuru_IliskiliEntityId_KullaniciId",
                table: "Begeniler",
                columns: new[] { "BegeniTuru", "IliskiliEntityId", "KullaniciId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Begeniler_GonderiId",
                table: "Begeniler",
                column: "GonderiId");

            migrationBuilder.CreateIndex(
                name: "IX_Begeniler_KullaniciId",
                table: "Begeniler",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Begeniler_YorumId",
                table: "Begeniler",
                column: "YorumId");

            migrationBuilder.CreateIndex(
                name: "IX_Bildirimler_AliciKullaniciId",
                table: "Bildirimler",
                column: "AliciKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Bildirimler_GonderenKullaniciId",
                table: "Bildirimler",
                column: "GonderenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Bildirimler_IliskiliEntityTuru_IliskiliEntityId",
                table: "Bildirimler",
                columns: new[] { "IliskiliEntityTuru", "IliskiliEntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Bildirimler_OkunduMu",
                table: "Bildirimler",
                column: "OkunduMu");

            migrationBuilder.CreateIndex(
                name: "IX_Bildirimler_Turu",
                table: "Bildirimler",
                column: "Turu");

            migrationBuilder.CreateIndex(
                name: "IX_CagriBilgileri_CagriTuru",
                table: "CagriBilgileri",
                column: "CagriTuru");

            migrationBuilder.CreateIndex(
                name: "IX_CagriBilgileri_GuncelleyenKullaniciId",
                table: "CagriBilgileri",
                column: "GuncelleyenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_CagriBilgileri_HedefIlceId",
                table: "CagriBilgileri",
                column: "HedefIlceId");

            migrationBuilder.CreateIndex(
                name: "IX_CagriBilgileri_HedefIlId",
                table: "CagriBilgileri",
                column: "HedefIlId");

            migrationBuilder.CreateIndex(
                name: "IX_CagriBilgileri_HedefOkulId",
                table: "CagriBilgileri",
                column: "HedefOkulId");

            migrationBuilder.CreateIndex(
                name: "IX_CagriBilgileri_OlusturanKullaniciId",
                table: "CagriBilgileri",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_CagriBilgileri_SilenKullaniciId",
                table: "CagriBilgileri",
                column: "SilenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_CagriBilgileri_Slug",
                table: "CagriBilgileri",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CagriBilgileri_YayindaMi",
                table: "CagriBilgileri",
                column: "YayindaMi");

            migrationBuilder.CreateIndex(
                name: "IX_CagriEkDosyalari_CagriBilgisiId",
                table: "CagriEkDosyalari",
                column: "CagriBilgisiId");

            migrationBuilder.CreateIndex(
                name: "IX_CagriEkDosyalari_GuncelleyenKullaniciId",
                table: "CagriEkDosyalari",
                column: "GuncelleyenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_CagriEkDosyalari_OlusturanKullaniciId",
                table: "CagriEkDosyalari",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_CagriEkDosyalari_SilenKullaniciId",
                table: "CagriEkDosyalari",
                column: "SilenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_CagriTakipleri_CagriBilgisiId_KullaniciId",
                table: "CagriTakipleri",
                columns: new[] { "CagriBilgisiId", "KullaniciId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CagriTakipleri_KullaniciId",
                table: "CagriTakipleri",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Dosyalar_ApplicationUserId",
                table: "Dosyalar",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Dosyalar_DuyuruId",
                table: "Dosyalar",
                column: "DuyuruId");

            migrationBuilder.CreateIndex(
                name: "IX_Dosyalar_EtkinlikId",
                table: "Dosyalar",
                column: "EtkinlikId");

            migrationBuilder.CreateIndex(
                name: "IX_Dosyalar_GuncelleyenKullaniciId",
                table: "Dosyalar",
                column: "GuncelleyenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Dosyalar_IliskiliEntityTuru_IliskiliEntityId",
                table: "Dosyalar",
                columns: new[] { "IliskiliEntityTuru", "IliskiliEntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Dosyalar_OlusturanKullaniciId",
                table: "Dosyalar",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Dosyalar_SilenKullaniciId",
                table: "Dosyalar",
                column: "SilenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Dosyalar_Turu",
                table: "Dosyalar",
                column: "Turu");

            migrationBuilder.CreateIndex(
                name: "IX_Duyurular_BaslangicTarihi_BitisTarihi",
                table: "Duyurular",
                columns: new[] { "BaslangicTarihi", "BitisTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_Duyurular_GuncelleyenKullaniciId",
                table: "Duyurular",
                column: "GuncelleyenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Duyurular_HedefIlceId",
                table: "Duyurular",
                column: "HedefIlceId");

            migrationBuilder.CreateIndex(
                name: "IX_Duyurular_HedefIlId",
                table: "Duyurular",
                column: "HedefIlId");

            migrationBuilder.CreateIndex(
                name: "IX_Duyurular_HedefOkulId",
                table: "Duyurular",
                column: "HedefOkulId");

            migrationBuilder.CreateIndex(
                name: "IX_Duyurular_KategoriId",
                table: "Duyurular",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Duyurular_OlusturanKullaniciId",
                table: "Duyurular",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Duyurular_OneCikanMi",
                table: "Duyurular",
                column: "OneCikanMi");

            migrationBuilder.CreateIndex(
                name: "IX_Duyurular_SilenKullaniciId",
                table: "Duyurular",
                column: "SilenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Duyurular_Slug",
                table: "Duyurular",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Duyurular_YayindaMi",
                table: "Duyurular",
                column: "YayindaMi");

            migrationBuilder.CreateIndex(
                name: "IX_DuyuruTakipleri_DuyuruId_KullaniciId",
                table: "DuyuruTakipleri",
                columns: new[] { "DuyuruId", "KullaniciId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DuyuruTakipleri_KullaniciId",
                table: "DuyuruTakipleri",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_EtkinlikKatilimcilari_EtkinlikId_KullaniciId",
                table: "EtkinlikKatilimcilari",
                columns: new[] { "EtkinlikId", "KullaniciId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EtkinlikKatilimcilari_KullaniciId",
                table: "EtkinlikKatilimcilari",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Etkinlikler_BaslangicTarihi_BitisTarihi",
                table: "Etkinlikler",
                columns: new[] { "BaslangicTarihi", "BitisTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_Etkinlikler_GuncelleyenKullaniciId",
                table: "Etkinlikler",
                column: "GuncelleyenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Etkinlikler_KayitAcik",
                table: "Etkinlikler",
                column: "KayitAcik");

            migrationBuilder.CreateIndex(
                name: "IX_Etkinlikler_OlusturanKullaniciId",
                table: "Etkinlikler",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Etkinlikler_SilenKullaniciId",
                table: "Etkinlikler",
                column: "SilenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Etkinlikler_Turu",
                table: "Etkinlikler",
                column: "Turu");

            migrationBuilder.CreateIndex(
                name: "IX_Etkinlikler_YayindaMi",
                table: "Etkinlikler",
                column: "YayindaMi");

            migrationBuilder.CreateIndex(
                name: "IX_Gonderiler_GuncelleyenKullaniciId",
                table: "Gonderiler",
                column: "GuncelleyenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Gonderiler_OlusturanKullaniciId",
                table: "Gonderiler",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Gonderiler_OlusturulmaTarihi",
                table: "Gonderiler",
                column: "OlusturulmaTarihi");

            migrationBuilder.CreateIndex(
                name: "IX_Gonderiler_ProjeYonetimId",
                table: "Gonderiler",
                column: "ProjeYonetimId");

            migrationBuilder.CreateIndex(
                name: "IX_Gonderiler_SabitlenmeTarihi",
                table: "Gonderiler",
                column: "SabitlenmeTarihi");

            migrationBuilder.CreateIndex(
                name: "IX_Gonderiler_SilenKullaniciId",
                table: "Gonderiler",
                column: "SilenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Gonderiler_YayindaMi",
                table: "Gonderiler",
                column: "YayindaMi");

            migrationBuilder.CreateIndex(
                name: "IX_Ilceler_IlId_Ad",
                table: "Ilceler",
                columns: new[] { "IlId", "Ad" });

            migrationBuilder.CreateIndex(
                name: "IX_Iller_PlakaKodu",
                table: "Iller",
                column: "PlakaKodu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciClaim_UserId",
                table: "KullaniciClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Kullanicilar",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_IlceId",
                table: "Kullanicilar",
                column: "IlceId");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_IlId",
                table: "Kullanicilar",
                column: "IlId");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_OkulId",
                table: "Kullanicilar",
                column: "OkulId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Kullanicilar",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciLogin_UserId",
                table: "KullaniciLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciRolleri_RoleId",
                table: "KullaniciRolleri",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciRozetleri_KullaniciId_RozetId",
                table: "KullaniciRozetleri",
                columns: new[] { "KullaniciId", "RozetId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciRozetleri_OlusturanKullaniciId1",
                table: "KullaniciRozetleri",
                column: "OlusturanKullaniciId1");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciRozetleri_RozetId",
                table: "KullaniciRozetleri",
                column: "RozetId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerasyonKayitlari_Aksiyon",
                table: "ModerasyonKayitlari",
                column: "Aksiyon");

            migrationBuilder.CreateIndex(
                name: "IX_ModerasyonKayitlari_IcerikSahibiId",
                table: "ModerasyonKayitlari",
                column: "IcerikSahibiId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerasyonKayitlari_IcerikTuru_IcerikId",
                table: "ModerasyonKayitlari",
                columns: new[] { "IcerikTuru", "IcerikId" });

            migrationBuilder.CreateIndex(
                name: "IX_ModerasyonKayitlari_ModerasyonTarihi",
                table: "ModerasyonKayitlari",
                column: "ModerasyonTarihi");

            migrationBuilder.CreateIndex(
                name: "IX_ModerasyonKayitlari_OlusturanKullaniciId",
                table: "ModerasyonKayitlari",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Okullar_AktifMi",
                table: "Okullar",
                column: "AktifMi");

            migrationBuilder.CreateIndex(
                name: "IX_Okullar_GuncelleyenKullaniciId",
                table: "Okullar",
                column: "GuncelleyenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Okullar_IlceId",
                table: "Okullar",
                column: "IlceId");

            migrationBuilder.CreateIndex(
                name: "IX_Okullar_IlId",
                table: "Okullar",
                column: "IlId");

            migrationBuilder.CreateIndex(
                name: "IX_Okullar_MEBOkulKodu",
                table: "Okullar",
                column: "MEBOkulKodu");

            migrationBuilder.CreateIndex(
                name: "IX_Okullar_OlusturanKullaniciId",
                table: "Okullar",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Okullar_SilenKullaniciId",
                table: "Okullar",
                column: "SilenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeAktiviteleri_GuncelleyenKullaniciId",
                table: "ProjeAktiviteleri",
                column: "GuncelleyenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeAktiviteleri_OlusturanKullaniciId",
                table: "ProjeAktiviteleri",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeAktiviteleri_ProjeYonetimId",
                table: "ProjeAktiviteleri",
                column: "ProjeYonetimId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeAktiviteleri_SilenKullaniciId",
                table: "ProjeAktiviteleri",
                column: "SilenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeDosyalari_GuncelleyenKullaniciId",
                table: "ProjeDosyalari",
                column: "GuncelleyenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeDosyalari_OlusturanKullaniciId",
                table: "ProjeDosyalari",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeDosyalari_ProjeYonetimId",
                table: "ProjeDosyalari",
                column: "ProjeYonetimId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeDosyalari_SilenKullaniciId",
                table: "ProjeDosyalari",
                column: "SilenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeEkipUyeleri_AktifMi",
                table: "ProjeEkipUyeleri",
                column: "AktifMi");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeEkipUyeleri_KullaniciId",
                table: "ProjeEkipUyeleri",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeEkipUyeleri_ProjeYonetimId_KullaniciId",
                table: "ProjeEkipUyeleri",
                columns: new[] { "ProjeYonetimId", "KullaniciId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjeGorevleri_AtananKullaniciId",
                table: "ProjeGorevleri",
                column: "AtananKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeGorevleri_Durum",
                table: "ProjeGorevleri",
                column: "Durum");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeGorevleri_GuncelleyenKullaniciId",
                table: "ProjeGorevleri",
                column: "GuncelleyenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeGorevleri_OlusturanKullaniciId",
                table: "ProjeGorevleri",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeGorevleri_Oncelik",
                table: "ProjeGorevleri",
                column: "Oncelik");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeGorevleri_ProjeYonetimId",
                table: "ProjeGorevleri",
                column: "ProjeYonetimId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeGorevleri_SilenKullaniciId",
                table: "ProjeGorevleri",
                column: "SilenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeKategorileri_GuncelleyenKullaniciId",
                table: "ProjeKategorileri",
                column: "GuncelleyenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeKategorileri_OlusturanKullaniciId",
                table: "ProjeKategorileri",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeKategorileri_SilenKullaniciId",
                table: "ProjeKategorileri",
                column: "SilenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeYonetimleri_Durum",
                table: "ProjeYonetimleri",
                column: "Durum");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeYonetimleri_GuncelleyenKullaniciId",
                table: "ProjeYonetimleri",
                column: "GuncelleyenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeYonetimleri_KategoriId",
                table: "ProjeYonetimleri",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeYonetimleri_KaynakCagriBilgisiId",
                table: "ProjeYonetimleri",
                column: "KaynakCagriBilgisiId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeYonetimleri_KurucuKullaniciId",
                table: "ProjeYonetimleri",
                column: "KurucuKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeYonetimleri_OkulId",
                table: "ProjeYonetimleri",
                column: "OkulId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeYonetimleri_OlusturanKullaniciId",
                table: "ProjeYonetimleri",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeYonetimleri_SilenKullaniciId",
                table: "ProjeYonetimleri",
                column: "SilenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeYonetimleri_Slug",
                table: "ProjeYonetimleri",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjeYonetimleri_YayindaMi",
                table: "ProjeYonetimleri",
                column: "YayindaMi");

            migrationBuilder.CreateIndex(
                name: "IX_RolClaim_RoleId",
                table: "RolClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roller",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Rozetler_AktifMi",
                table: "Rozetler",
                column: "AktifMi");

            migrationBuilder.CreateIndex(
                name: "IX_Rozetler_Kategori",
                table: "Rozetler",
                column: "Kategori");

            migrationBuilder.CreateIndex(
                name: "IX_Rozetler_KosulEventAdi",
                table: "Rozetler",
                column: "KosulEventAdi");

            migrationBuilder.CreateIndex(
                name: "IX_Yorumlar_GonderiId",
                table: "Yorumlar",
                column: "GonderiId");

            migrationBuilder.CreateIndex(
                name: "IX_Yorumlar_GuncelleyenKullaniciId",
                table: "Yorumlar",
                column: "GuncelleyenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Yorumlar_OlusturanKullaniciId",
                table: "Yorumlar",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Yorumlar_OnaylandiMi",
                table: "Yorumlar",
                column: "OnaylandiMi");

            migrationBuilder.CreateIndex(
                name: "IX_Yorumlar_SilenKullaniciId",
                table: "Yorumlar",
                column: "SilenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Yorumlar_UstYorumId",
                table: "Yorumlar",
                column: "UstYorumId");

            migrationBuilder.CreateIndex(
                name: "IX_Yorumlar_YorumTuru_IliskiliEntityId",
                table: "Yorumlar",
                columns: new[] { "YorumTuru", "IliskiliEntityId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Kullanicilar_KullaniciId",
                table: "AuditLogs",
                column: "KullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Begeniler_Gonderiler_GonderiId",
                table: "Begeniler",
                column: "GonderiId",
                principalTable: "Gonderiler",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Begeniler_Kullanicilar_KullaniciId",
                table: "Begeniler",
                column: "KullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Begeniler_Yorumlar_YorumId",
                table: "Begeniler",
                column: "YorumId",
                principalTable: "Yorumlar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bildirimler_Kullanicilar_AliciKullaniciId",
                table: "Bildirimler",
                column: "AliciKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bildirimler_Kullanicilar_GonderenKullaniciId",
                table: "Bildirimler",
                column: "GonderenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CagriBilgileri_Kullanicilar_GuncelleyenKullaniciId",
                table: "CagriBilgileri",
                column: "GuncelleyenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CagriBilgileri_Kullanicilar_OlusturanKullaniciId",
                table: "CagriBilgileri",
                column: "OlusturanKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CagriBilgileri_Kullanicilar_SilenKullaniciId",
                table: "CagriBilgileri",
                column: "SilenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CagriBilgileri_Okullar_HedefOkulId",
                table: "CagriBilgileri",
                column: "HedefOkulId",
                principalTable: "Okullar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CagriEkDosyalari_Kullanicilar_GuncelleyenKullaniciId",
                table: "CagriEkDosyalari",
                column: "GuncelleyenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CagriEkDosyalari_Kullanicilar_OlusturanKullaniciId",
                table: "CagriEkDosyalari",
                column: "OlusturanKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CagriEkDosyalari_Kullanicilar_SilenKullaniciId",
                table: "CagriEkDosyalari",
                column: "SilenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CagriTakipleri_Kullanicilar_KullaniciId",
                table: "CagriTakipleri",
                column: "KullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Dosyalar_Duyurular_DuyuruId",
                table: "Dosyalar",
                column: "DuyuruId",
                principalTable: "Duyurular",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dosyalar_Etkinlikler_EtkinlikId",
                table: "Dosyalar",
                column: "EtkinlikId",
                principalTable: "Etkinlikler",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dosyalar_Kullanicilar_ApplicationUserId",
                table: "Dosyalar",
                column: "ApplicationUserId",
                principalTable: "Kullanicilar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dosyalar_Kullanicilar_GuncelleyenKullaniciId",
                table: "Dosyalar",
                column: "GuncelleyenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Dosyalar_Kullanicilar_OlusturanKullaniciId",
                table: "Dosyalar",
                column: "OlusturanKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Dosyalar_Kullanicilar_SilenKullaniciId",
                table: "Dosyalar",
                column: "SilenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Duyurular_Kullanicilar_GuncelleyenKullaniciId",
                table: "Duyurular",
                column: "GuncelleyenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Duyurular_Kullanicilar_OlusturanKullaniciId",
                table: "Duyurular",
                column: "OlusturanKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Duyurular_Kullanicilar_SilenKullaniciId",
                table: "Duyurular",
                column: "SilenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Duyurular_Okullar_HedefOkulId",
                table: "Duyurular",
                column: "HedefOkulId",
                principalTable: "Okullar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DuyuruTakipleri_Kullanicilar_KullaniciId",
                table: "DuyuruTakipleri",
                column: "KullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EtkinlikKatilimcilari_Etkinlikler_EtkinlikId",
                table: "EtkinlikKatilimcilari",
                column: "EtkinlikId",
                principalTable: "Etkinlikler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EtkinlikKatilimcilari_Kullanicilar_KullaniciId",
                table: "EtkinlikKatilimcilari",
                column: "KullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Etkinlikler_Kullanicilar_GuncelleyenKullaniciId",
                table: "Etkinlikler",
                column: "GuncelleyenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Etkinlikler_Kullanicilar_OlusturanKullaniciId",
                table: "Etkinlikler",
                column: "OlusturanKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Etkinlikler_Kullanicilar_SilenKullaniciId",
                table: "Etkinlikler",
                column: "SilenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Gonderiler_Kullanicilar_GuncelleyenKullaniciId",
                table: "Gonderiler",
                column: "GuncelleyenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Gonderiler_Kullanicilar_OlusturanKullaniciId",
                table: "Gonderiler",
                column: "OlusturanKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Gonderiler_Kullanicilar_SilenKullaniciId",
                table: "Gonderiler",
                column: "SilenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Gonderiler_ProjeYonetimleri_ProjeYonetimId",
                table: "Gonderiler",
                column: "ProjeYonetimId",
                principalTable: "ProjeYonetimleri",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_KullaniciClaim_Kullanicilar_UserId",
                table: "KullaniciClaim",
                column: "UserId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanicilar_Okullar_OkulId",
                table: "Kullanicilar",
                column: "OkulId",
                principalTable: "Okullar",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Okullar_Kullanicilar_GuncelleyenKullaniciId",
                table: "Okullar");

            migrationBuilder.DropForeignKey(
                name: "FK_Okullar_Kullanicilar_OlusturanKullaniciId",
                table: "Okullar");

            migrationBuilder.DropForeignKey(
                name: "FK_Okullar_Kullanicilar_SilenKullaniciId",
                table: "Okullar");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Begeniler");

            migrationBuilder.DropTable(
                name: "Bildirimler");

            migrationBuilder.DropTable(
                name: "CagriEkDosyalari");

            migrationBuilder.DropTable(
                name: "CagriTakipleri");

            migrationBuilder.DropTable(
                name: "Dosyalar");

            migrationBuilder.DropTable(
                name: "DuyuruTakipleri");

            migrationBuilder.DropTable(
                name: "EtkinlikKatilimcilari");

            migrationBuilder.DropTable(
                name: "KullaniciClaim");

            migrationBuilder.DropTable(
                name: "KullaniciLogin");

            migrationBuilder.DropTable(
                name: "KullaniciRolleri");

            migrationBuilder.DropTable(
                name: "KullaniciRozetleri");

            migrationBuilder.DropTable(
                name: "KullaniciToken");

            migrationBuilder.DropTable(
                name: "ModerasyonKayitlari");

            migrationBuilder.DropTable(
                name: "ProjeAktiviteleri");

            migrationBuilder.DropTable(
                name: "ProjeDosyalari");

            migrationBuilder.DropTable(
                name: "ProjeEkipUyeleri");

            migrationBuilder.DropTable(
                name: "ProjeGorevleri");

            migrationBuilder.DropTable(
                name: "RolClaim");

            migrationBuilder.DropTable(
                name: "Yorumlar");

            migrationBuilder.DropTable(
                name: "Duyurular");

            migrationBuilder.DropTable(
                name: "Etkinlikler");

            migrationBuilder.DropTable(
                name: "Rozetler");

            migrationBuilder.DropTable(
                name: "Roller");

            migrationBuilder.DropTable(
                name: "Gonderiler");

            migrationBuilder.DropTable(
                name: "DuyuruKategorileri");

            migrationBuilder.DropTable(
                name: "ProjeYonetimleri");

            migrationBuilder.DropTable(
                name: "CagriBilgileri");

            migrationBuilder.DropTable(
                name: "ProjeKategorileri");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "Okullar");

            migrationBuilder.DropTable(
                name: "Ilceler");

            migrationBuilder.DropTable(
                name: "Iller");
        }
    }
}
