using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Brandix.DCAP.API.Models
{
    public partial class DCAPDbContext : DbContext
    {
        public DCAPDbContext()
        {
        }

        public DCAPDbContext(DbContextOptions<DCAPDbContext> options)
            : base(options)
        {
            // this.SetCommandTimeOut(Infrastructure.DatabaseFacade);
        }

        

        public virtual DbSet<Buyer> Buyer { get; set; }
        public virtual DbSet<Buyerdiv> Buyerdiv { get; set; }
        public virtual DbSet<Clientconfig> Clientconfig { get; set; }
        public virtual DbSet<Dcl> Dcl { get; set; }
        public virtual DbSet<Dclh> Dclh { get; set; }
        public virtual DbSet<Dcm> Dcm { get; set; }
        public virtual DbSet<Dedep> Dedep { get; set; }
        public virtual DbSet<Dedepinst> Dedepinst { get; set; }                
        public virtual DbSet<Defectops> Defectops { get; set; }
        public virtual DbSet<Dep> Dep { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Detxn> Detxn { get; set; }
        public virtual DbSet<Group_Barcode_Detail> Group_Barcode_Detail { get; set; }
        public virtual DbSet<TTOpearation> TTOpearation { get; set; }
        public virtual DbSet<TravelStatus> TravelStatus { get; set; }
        public virtual DbSet<GoodControl> GoodControl { get; set; }
        public virtual DbSet<GoodControlDetails> GoodControlDetails { get; set; }
        public virtual DbSet<GroupBarcodeMapping> GroupBarcodeMapping { get; set; }
        public virtual DbSet<Factory> Factory { get; set; }
        public virtual DbSet<Favorite> Favorite { get; set; }
        public virtual DbSet<Group> Group { get; set; }
        public virtual DbSet<L1> L1 { get; set; }
        public virtual DbSet<L2> L2 { get; set; }
        public virtual DbSet<L3> L3 { get; set; }
        public virtual DbSet<L4> L4 { get; set; }
        public virtual DbSet<L5> L5 { get; set; }
        public virtual DbSet<L5bc> L5bc { get; set; }
        public virtual DbSet<L5bcPrint> L5bcPrint { get; set; }
        public virtual DbSet<L5mo> L5mo { get; set; }
        public virtual DbSet<L5moops> L5moops { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<WarehouseLocation> WarehouseLocation { get; set; }
        public virtual DbSet<Manualdetxn> Manualdetxn { get; set; }
        public virtual DbSet<Prodhour> Prodhour { get; set; }
        public virtual DbSet<Prodtarget> Prodtarget { get; set; }
        public virtual DbSet<Rejectreason> Rejectreason { get; set; }
        public virtual DbSet<Rrcat> Rrcat { get; set; }
        public virtual DbSet<Sbu> Sbu { get; set; }
        public virtual DbSet<Secfunction> Secfunction { get; set; }
        public virtual DbSet<Secparm> Secparm { get; set; }
        public virtual DbSet<Secuser> Secuser { get; set; }
        public virtual DbSet<Secuserright> Secuserright { get; set; }
        public virtual DbSet<Secuserrightdep> Secuserrightdep { get; set; }
        public virtual DbSet<TClientconfig> TClientconfig { get; set; }
        public virtual DbSet<TDepList> TDepList { get; set; }
        public virtual DbSet<Team> Team { get; set; }
        public virtual DbSet<Teambundle> Teambundle { get; set; }        
        public virtual DbSet<TeamCounter> TeamCounter { get; set; }
        public virtual DbSet<BuudyTagCounter> BuudyTagCounter { get; set; }
        public virtual DbSet<GroupBarcode> GroupBarcode { get; set; }
        public virtual DbSet<DeletedGroupBarcode> DeletedGroupBarcode { get; set; }
        public virtual DbSet<TravelBarcodeDetails> TravelBarcodeDetails { get; set; }
        public virtual DbSet<BuddyBagBarcode> BuddyBagBarcode { get; set; }
        public virtual DbSet<Teamopp> Teamopp { get; set; }
        public virtual DbSet<TRelatednodes> TRelatednodes { get; set; }
        public virtual DbSet<Wf> Wf { get; set; }
        public virtual DbSet<Wfdep> Wfdep { get; set; }
        public virtual DbSet<Wfdeplink> Wfdeplink { get; set; }
        public virtual DbSet<Wfdepmulqty> Wfdepmulqty { get; set; }
        public virtual DbSet<ZWfdepmatrix> ZWfdepmatrix { get; set; }
        public virtual DbSet<ZWfteam> ZWfteam { get; set; }
        public virtual DbSet<Washes> Washes { get; set; }
        public virtual DbSet<SFprocess> SFprocess { get; set; }
        public virtual DbSet<InvoiceParameter> InvoiceParameter { get; set; }
        public virtual DbSet<InvoiceDetails> InvoiceDetails { get; set; }
        public virtual DbSet<InvoiceHeaderInformation> InvoiceHeaderInformation { get; set; }

        // Unable to generate entity type for table 'ortxn'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("Server=10.227.240.116;Database=dcap_test;User=dcap_dev;Password=dcap@123;Connection Timeout=360;");//10.227.240.116 dcap dcap_bfl 10.20.154.119
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Buyer>(entity =>
            {
                entity.ToTable("buyer");

                entity.Property(e => e.BuyerCode)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<Buyerdiv>(entity =>
            {
                entity.HasKey(e => new { e.BuyerId, e.BuyerDivId });

                entity.ToTable("buyerdiv");

                entity.Property(e => e.BuyerDivCode)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.HasOne(d => d.Buyer)
                    .WithMany(p => p.Buyerdiv)
                    .HasForeignKey(d => d.BuyerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_buyerdiv_BuyerId");
            });

            modelBuilder.Entity<Clientconfig>(entity =>
            {
                entity.HasKey(e => e.ClientId);

                entity.ToTable("clientconfig");

                entity.HasIndex(e => e.ClientIp)
                    .HasName("ClientIP_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.TeamId)
                    .HasName("FK_clientconfig_team_idx");

                entity.Property(e => e.ClientId).HasColumnType("varchar(50)");

                entity.Property(e => e.ClientIp)
                    .HasColumnName("ClientIP")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.DataCaptureMode)
                    .HasColumnType("int(2)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.LoginMode)
                    .HasColumnType("int(2)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.OpCode1).HasColumnType("int(10)");

                entity.Property(e => e.OpCode2).HasColumnType("int(10)");

                entity.Property(e => e.OperationName).HasColumnType("varchar(50)");

                entity.Property(e => e.RecStatus).HasColumnType("int(1)");

                entity.Property(e => e.SelectMode)
                    .HasColumnType("int(2)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.TxnMode).HasColumnType("int(1)");

                entity.Property(e => e.UserId).HasColumnType("char(20)");

                entity.Property(e => e.WfdepinstId)
                    .HasColumnName("WFDEPInstId")
                    .HasColumnType("int(10)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.WfId)
                    .HasColumnName("WFId")
                    .HasColumnType("int(10)")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<Dcl>(entity =>
            {
                entity.ToTable("dcl");

                entity.HasIndex(e => e.StructureNo)
                    .HasName("Index_Struct");

                entity.Property(e => e.Dclid).HasColumnName("DCLId");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.Dclname)
                    .IsRequired()
                    .HasColumnName("DCLName")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.IsHide).HasColumnType("int(1)");

                entity.Property(e => e.LevelHierarchy).HasColumnType("int(11)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.HasOne(d => d.StructureNoNavigation)
                    .WithMany(p => p.Dcl)
                    .HasForeignKey(d => d.StructureNo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dcl_1");
            });

            modelBuilder.Entity<Dclh>(entity =>
            {
                entity.HasKey(e => e.StructureNo);

                entity.ToTable("dclh");

                entity.HasIndex(e => e.StructureNo)
                    .HasName("Index_Struct");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.StructName)
                    .IsRequired()
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<Dcm>(entity =>
            {
                entity.ToTable("dcm");

                entity.Property(e => e.Dcmid).HasColumnName("DCMId");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.Dcmname)
                    .IsRequired()
                    .HasColumnName("DCMName")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
            });

            modelBuilder.Entity<Dedep>(entity =>
            {
                entity.HasKey(e => new { e.Wfid, e.Depid, e.Seq });

                entity.ToTable("dedep");
 
                entity.HasIndex(e => new { e.Depid, e.Wfid, e.L1id, e.L2id, e.L3id, e.L4id, e.L5id })
                    .HasName("Index_6");
               
                entity.Property(e => e.Wfid).HasColumnName("WFId");

                entity.Property(e => e.Depid).HasColumnName("DEPId");

                entity.Property(e => e.Seq).HasColumnType("int(10)");

                entity.Property(e => e.CreatedBy).HasColumnType("varchar(45)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine).HasColumnType("varchar(45)");

                entity.Property(e => e.Dclid).HasColumnName("DCLId");

                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.L2id).HasColumnName("L2Id");

                entity.Property(e => e.L3id).HasColumnName("L3Id");

                entity.Property(e => e.L4id).HasColumnName("L4Id");

                entity.Property(e => e.L5id).HasColumnName("L5Id");

                entity.Property(e => e.L5moid)
                    .HasColumnName("L5MOId")
                    .HasColumnType("int(10)");

                entity.Property(e => e.ModifiedBy).HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine).HasColumnType("varchar(25)");

                entity.Property(e => e.OperationCode).HasColumnType("int(5)");

                entity.Property(e => e.Qty01).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty01Ns)
                    .HasColumnName("Qty01NS")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02Ns)
                    .HasColumnName("Qty02NS")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03Ns)
                    .HasColumnName("Qty03NS")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.RecStatus).HasColumnType("int(11)");

                entity.Property(e => e.TxnMode).HasColumnType("int(1)");

                entity.Property(e => e.WorkCenter).HasColumnType("varchar(30)");
            });

            modelBuilder.Entity<Dedepinst>(entity =>
            {
                entity.HasKey(e => new { e.dedepinstKey });

                entity.ToTable("dedepinst");

                entity.HasIndex(e => new { e.WfdepinstId, e.Wfid, e.L1id, e.L2id, e.L3id, e.L4id, e.L5id })
                    .HasName("Index_7");

                //entity.HasIndex(e => new { e.WfdepinstId, e.Seq })
                    //.HasName("Index_WfDepInst_Seq");

                entity.Property(e => e.WfdepinstId).HasColumnName("WFDEPInstId");

                entity.Property(e => e.CreatedBy).HasColumnType("varchar(45)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine).HasColumnType("varchar(45)");

                entity.Property(e => e.Dclid).HasColumnName("DCLId");

                entity.Property(e => e.Depid).HasColumnName("DEPId");

                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.L2id).HasColumnName("L2Id");

                entity.Property(e => e.L3id).HasColumnName("L3Id");

                entity.Property(e => e.L4id).HasColumnName("L4Id");

                entity.Property(e => e.L5id).HasColumnName("L5Id");

                entity.Property(e => e.L5moid)
                    .HasColumnName("L5MOId")
                    .HasColumnType("int(10)");

                entity.Property(e => e.ModifiedBy).HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine).HasColumnType("varchar(25)");

                entity.Property(e => e.OperationCode).HasColumnType("int(5)");

                entity.Property(e => e.PlussMinus).HasColumnType("int(1)");

                entity.Property(e => e.Qty01).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty01Ns)
                    .HasColumnName("Qty01NS")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02Ns)
                    .HasColumnName("Qty02NS")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03Ns)
                    .HasColumnName("Qty03NS")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.RecStatus).HasColumnType("int(11)");

                entity.Property(e => e.TxnMode).HasColumnType("int(1)");

                entity.Property(e => e.Wfid).HasColumnName("WFId");

                entity.Property(e => e.WorkCenter).HasColumnType("varchar(30)");

                entity.Property(e => e.dedepinstKey).HasColumnName("dedepinstKey");

                entity.HasOne(d => d.Wfdepinst)
                    .WithMany(p => p.Dedepinst)
                    .HasForeignKey(d => d.WfdepinstId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dedepinst_WFDEP");
            });

            modelBuilder.Entity<Defectops>(entity =>
            {
                entity.HasKey(e => e.DopsId);

                entity.ToTable("defectops");

                entity.HasIndex(e => e.DopsScode)
                    .HasName("Index_SCode")
                    .IsUnique();

                entity.Property(e => e.DopsId)
                    .HasColumnName("DOpsId")
                    .HasColumnType("int(4)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.DopsCatId)
                    .IsRequired()
                    .HasColumnName("DOpsCatId")
                    .HasColumnType("varchar(10)")
                    .HasDefaultValueSql("'130'");

                entity.Property(e => e.DopsDesc)
                    .IsRequired()
                    .HasColumnName("DOpsDesc")
                    .HasColumnType("varchar(60)");

                entity.Property(e => e.DopsDescS)
                    .HasColumnName("DOpsDescS")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.DopsDescT)
                    .HasColumnName("DOpsDescT")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.DopsName)
                    .IsRequired()
                    .HasColumnName("DOpsName")
                    .HasColumnType("varchar(40)");

                entity.Property(e => e.DopsNameS)
                    .HasColumnName("DOpsNameS")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.DopsNameT)
                    .HasColumnName("DOpsNameT")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.DopsScode)
                    .IsRequired()
                    .HasColumnName("DOpsSCode")
                    .HasColumnType("varchar(15)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
            });

            modelBuilder.Entity<Dep>(entity =>
            {
                entity.ToTable("dep");

                entity.HasIndex(e => e.OperationCode)
                    .HasName("OperationCode")
                    .IsUnique();

                entity.Property(e => e.Depid).HasColumnName("DEPId");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.Depdesc)
                    .IsRequired()
                    .HasColumnName("DEPDesc")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Depimage)
                    .IsRequired()
                    .HasColumnName("DEPImage")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.OperationCode).HasColumnType("int(10)");

                entity.Property(e => e.ScanOpMode)
                    .HasColumnType("int(2)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.StructureNo).HasColumnType("int(3)");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => new { e.GroupCode, e.Sbucode, e.FacCode, e.LocCode, e.DeptCode });

                entity.ToTable("department");

                entity.Property(e => e.GroupCode).HasColumnType("varchar(10)");

                entity.Property(e => e.Sbucode)
                    .HasColumnName("SBUCode")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.FacCode).HasColumnType("varchar(20)");

                entity.Property(e => e.LocCode).HasColumnType("varchar(20)");

                entity.Property(e => e.DeptCode).HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.DeptName)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Department)
                    .HasForeignKey(d => new { d.GroupCode, d.Sbucode, d.FacCode, d.LocCode })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_department_Location");
            });

            modelBuilder.Entity<Detxn>(entity =>
            {
                entity.HasKey(e => e.DetxnKey);

                entity.ToTable("detxn");

                entity.HasIndex(e => new { e.WfdepinstId, e.L5mono })
                    .HasName("Index_WFDEPInstId_L5MONo");

                entity.HasIndex(e => new { e.BarCodeNo, e.Depid })
                    .HasName("Index_BC_DEPId");

                entity.HasIndex(e => new { e.TeamId, e.TxnDateTime, e.OperationCode, e.HourNo })
                    .HasName("FK_TeamId_TxnDate_OpCoe_Hour");

                entity.HasIndex(e => new { e.L1id, e.L2id, e.L3id, e.L4id, e.L5id })
                    .HasName("Index_L1L2L3");

                entity.HasIndex(e => new { e.OperationCode })
                    .HasName("Index_OperationCode"); 

                entity.HasIndex(e => new { e.TxnMode })
                    .HasName("Index_TxnMode"); 

                entity.HasIndex(e => new { e.L1id, e.JobNo })
                    .HasName("JonNoIndex");

                entity.HasIndex(e => new { e.L1id, e.L2id, e.L4id, e.BagBarCodeNo, e.TravelBarCodeNo })
                    .HasName("BagBarcodeIndex"); 

                entity.HasIndex(e => new { e.TxnDateTime })
                    .HasName("Index_DeTxn"); 

                entity.HasIndex(e => e.Depid)
                    .HasName("FK_detxn_DEPID");

                entity.HasIndex(e => new { e.UploadStatus })
                    .HasName("Index_UploadStatus");

                entity.Property(e => e.AppBy).HasColumnType("varchar(20)");

                entity.Property(e => e.AppStatus).HasColumnType("int(11)");

                entity.Property(e => e.AppTime).HasColumnType("datetime");

                entity.Property(e => e.BarCodeNo)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.BagBarCodeNo)
                    .HasColumnName("BagBarCodeNo")
                    .HasColumnType("varchar(25)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.TravelBarCodeNo)
                    .HasColumnName("TravelBarCodeNo")
                    .HasColumnType("varchar(25)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.CreatedBy).HasColumnType("varchar(45)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine).HasColumnType("varchar(45)");

                entity.Property(e => e.Dclid).HasColumnName("DCLId");

                entity.Property(e => e.Dcmid).HasColumnName("DCMId");

                entity.Property(e => e.Depid).HasColumnName("DEPId");

                entity.Property(e => e.Detxncol)
                    .HasColumnName("detxncol")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.DopsId).HasColumnName("DOpsId");

                entity.Property(e => e.EnteredBy).HasColumnType("varchar(20)");

                entity.Property(e => e.ErrorCode).HasColumnType("varchar(100)");

                entity.Property(e => e.ErrorDescription).HasColumnType("varchar(1000)");

                entity.Property(e => e.HasError).HasColumnType("int(1)");

                entity.Property(e => e.HourNo).HasColumnType("int(11)");

                entity.Property(e => e.InstanceNo).HasColumnType("int(11)");

                entity.Property(e => e.IsSucess).HasColumnType("int(1)");

                entity.Property(e => e.JobNo).HasColumnType("varchar(30)");

                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.L2id).HasColumnName("L2Id");

                entity.Property(e => e.L3id).HasColumnName("L3Id");

                entity.Property(e => e.L4id).HasColumnName("L4Id");

                entity.Property(e => e.L5id).HasColumnName("L5Id");

                entity.Property(e => e.L5moid)
                    .HasColumnName("L5MOId")
                    .HasColumnType("int(10)");

                entity.Property(e => e.L5mono)
                    .HasColumnName("L5MONo")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.ModifiedBy).HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine).HasColumnType("varchar(25)");

                entity.Property(e => e.OperationCode).HasColumnType("int(5)");

                entity.Property(e => e.Qty01).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty01Ns)
                    .HasColumnName("Qty01NS")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02Ns)
                    .HasColumnName("Qty02NS")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03Ns)
                    .HasColumnName("Qty03NS")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.RecStatus).HasColumnType("int(11)");

                entity.Property(e => e.Rrid).HasColumnName("RRId");

                entity.Property(e => e.TxnDateTime).HasColumnType("datetime");

                entity.Property(e => e.TxnMode).HasColumnType("int(1)");

                entity.Property(e => e.UploadBy).HasColumnType("varchar(20)");

                entity.Property(e => e.UploadStatus).HasColumnType("int(1)");

                entity.Property(e => e.UploadTime).HasColumnType("datetime");

                entity.Property(e => e.WfdepinstId).HasColumnName("WFDEPInstId");

                entity.Property(e => e.Wfid).HasColumnName("WFId");

                entity.Property(e => e.WorkCenter).HasColumnType("varchar(30)");

                entity.HasOne(d => d.Dep)
                    .WithMany(p => p.Detxn)
                    .HasForeignKey(d => d.Depid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_detxn_DEPID");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.Detxn)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK_detxn_Team");
            });

            modelBuilder.Entity<Group_Barcode_Detail>(entity =>
            {
                entity.HasKey(e => e.DetxnKey);

                entity.ToTable("group_barcode_detail");

                entity.HasIndex(e => new {e.L1id, e.L2id, e.L3id, e.L4id, e.BarCodeNo, e.TxnMode})
                    .HasName("Index_L1L2L3L4_Barcode_TxnMode");

                entity.HasIndex(e => new {e.L1id, e.L2id, e.L3id, e.L4id, e.L5id, e.L5moid, e.BarCodeNo, e.TxnMode})
                    .HasName("Index_L1L2L3L4L5L5MO_Barcode_TxnMode");

                entity.Property(e => e.WfdepinstId).HasColumnName("WFDEPInstId");

                entity.Property(e => e.Seq).HasColumnName("Seq");

                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.L2id).HasColumnName("L2Id");

                entity.Property(e => e.L3id).HasColumnName("L3Id");

                entity.Property(e => e.L4id).HasColumnName("L4Id");

                entity.Property(e => e.L5id).HasColumnName("L5Id");

                entity.Property(e => e.L5moid)
                    .HasColumnName("L5MOId")
                    .HasColumnType("int(10)");

                entity.Property(e => e.L5mono)
                    .HasColumnName("L5MONo")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.BarCodeNo)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.TxnMode).HasColumnType("int(1)");

                entity.Property(e => e.AlterBarcode)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.AlterTxnMode).HasColumnType("int(1)");

                entity.Property(e => e.Wfid).HasColumnName("WFId");

                entity.Property(e => e.Depid).HasColumnName("DEPId");

                entity.Property(e => e.Dclid).HasColumnName("DCLId");

                entity.Property(e => e.Dcmid).HasColumnName("DCMId");

                entity.Property(e => e.TxnDateTime).HasColumnType("datetime");
                
                entity.Property(e => e.Qty01).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty01Ns)
                    .HasColumnName("Qty01NS")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02Ns)
                    .HasColumnName("Qty02NS")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03Ns)
                    .HasColumnName("Qty03NS")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.EnteredBy).HasColumnType("varchar(20)");

                entity.Property(e => e.RecStatus).HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy).HasColumnType("varchar(45)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine).HasColumnType("varchar(45)");

                entity.Property(e => e.ModifiedBy).HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine).HasColumnType("varchar(25)");

                entity.Property(e => e.DetxnKey).HasColumnType("int(10)");
            });

            modelBuilder.Entity<TravelStatus>(entity =>
            {
                entity.HasKey(e => e.DetxnKey);

                entity.ToTable("travel_status");

                entity.HasIndex(e => e.Depid)
                    .HasName("FK_detxn_DEPID");

                entity.HasIndex(e => new { e.BarCodeNo, e.Depid })
                    .HasName("Index_BC_DEPId");

                entity.HasIndex(e => new { e.WfdepinstId, e.Seq })
                    .HasName("Index_WfdepInst_Seq")
                    .IsUnique();

                entity.HasIndex(e => new { e.TeamId, e.TxnDateTime, e.OperationCode, e.HourNo })
                    .HasName("FK_TeamId_TxnDate_OpCoe_Hour");

                entity.HasIndex(e => new { e.L1id, e.L2id, e.L3id, e.L4id, e.L5id })
                    .HasName("Index_L1L2L3");

                entity.Property(e => e.AppBy).HasColumnType("varchar(20)");

                entity.Property(e => e.AppStatus).HasColumnType("int(11)");

                entity.Property(e => e.AppTime).HasColumnType("datetime");

                entity.Property(e => e.BarCodeNo)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.CreatedBy).HasColumnType("varchar(45)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine).HasColumnType("varchar(45)");

                entity.Property(e => e.Dclid).HasColumnName("DCLId");

                entity.Property(e => e.Dcmid).HasColumnName("DCMId");

                entity.Property(e => e.Depid).HasColumnName("DEPId");

                entity.Property(e => e.Detxncol)
                    .HasColumnName("detxncol")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.DopsId).HasColumnName("DOpsId");

                entity.Property(e => e.EnteredBy).HasColumnType("varchar(20)");

                entity.Property(e => e.ErrorCode).HasColumnType("varchar(100)");

                entity.Property(e => e.ErrorDescription).HasColumnType("varchar(1000)");

                entity.Property(e => e.HasError).HasColumnType("int(1)");

                entity.Property(e => e.HourNo).HasColumnType("int(11)");

                entity.Property(e => e.InstanceNo).HasColumnType("int(11)");

                entity.Property(e => e.IsSucess).HasColumnType("int(1)");

                entity.Property(e => e.JobNo).HasColumnType("varchar(30)");

                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.L2id).HasColumnName("L2Id");

                entity.Property(e => e.L3id).HasColumnName("L3Id");

                entity.Property(e => e.L4id).HasColumnName("L4Id");

                entity.Property(e => e.L5id).HasColumnName("L5Id");

                entity.Property(e => e.ModifiedBy).HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine).HasColumnType("varchar(25)");

                entity.Property(e => e.OperationCode).HasColumnType("int(5)");

                entity.Property(e => e.Qty01).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty01Ns)
                    .HasColumnName("Qty01NS")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02Ns)
                    .HasColumnName("Qty02NS")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03Ns)
                    .HasColumnName("Qty03NS")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.RecStatus).HasColumnType("int(11)");

                entity.Property(e => e.Rrid).HasColumnName("RRId");

                entity.Property(e => e.TxnDateTime).HasColumnType("datetime");

                entity.Property(e => e.TxnMode).HasColumnType("int(1)");

                entity.Property(e => e.UploadBy).HasColumnType("varchar(20)");

                entity.Property(e => e.UploadStatus).HasColumnType("int(1)");

                entity.Property(e => e.UploadTime).HasColumnType("datetime");

                entity.Property(e => e.WfdepinstId).HasColumnName("WFDEPInstId");

                entity.Property(e => e.Wfid).HasColumnName("WFId");

                entity.Property(e => e.WorkCenter).HasColumnType("varchar(30)");
            });

            modelBuilder.Entity<GoodControl>(entity =>
            {
                entity.HasKey(e => new {e.Seq, e.ControlId, e.L1id, e.L2id, e.L3id, e.L4id, e.L5id, e.BarCodeNo, e.TxnMode});

                entity.ToTable("good_control");

                entity.HasIndex(e => new { e.BarCodeNo })
                    .HasName("Index_BC_DEPId");

                entity.HasIndex(e => new { e.ControlId })
                    .HasName("Index_WfdepInst_Seq")
                    .IsUnique();

                entity.HasIndex(e => new { e.TxnDateTime })
                    .HasName("FK_TeamId_TxnDate_OpCoe_Hour");

                entity.Property(e => e.Seq).HasColumnType("int(10)");

                entity.Property(e => e.ControlId).HasColumnType("varchar(20)");

                entity.Property(e => e.ControlType).HasColumnType("int(10)");

                entity.Property(e => e.BarCodeNo)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''");
                
                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.L2id).HasColumnName("L2Id");

                entity.Property(e => e.L3id).HasColumnName("L3Id");

                entity.Property(e => e.L4id).HasColumnName("L4Id");

                entity.Property(e => e.L5id).HasColumnName("L5Id");

                entity.Property(e => e.TxnMode).HasColumnType("int(1)");

                entity.Property(e => e.TxnDateTime).HasColumnType("datetime");

                entity.Property(e => e.Qty01).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03).HasColumnType("decimal(11,2)");

                entity.Property(e => e.TxnStatus).HasColumnType("int(1)");;

                entity.Property(e => e.Remark).HasColumnType("varchar(1000)");

                entity.Property(e => e.IsSucess).HasColumnType("int(1)");

                entity.Property(e => e.WarLocCode).HasColumnType("varchar(20)");

                entity.Property(e => e.RecStatus).HasColumnType("int(11)");

                entity.Property(e => e.Return).HasColumnType("int(1)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasColumnType("varchar(45)");

                entity.Property(e => e.CreatedMachine).HasColumnType("varchar(45)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy).HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedMachine).HasColumnType("varchar(25)");

                entity.Property(e => e.RecivedDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<GoodControlDetails>(entity =>
            {
                entity.HasKey(e => new { e.Seq, e.ControlId });

                entity.ToTable("good_control_details");

                entity.HasIndex(e => new { e.Depid })
                    .HasName("Index_BC_DEPId");

                entity.HasIndex(e => new { e.ControlId })
                    .HasName("Index_WfdepInst_Seq")
                    .IsUnique();

                entity.HasIndex(e => new { e.TxnDateTime, e.OperationCode })
                    .HasName("FK_TeamId_TxnDate_OpCoe_Hour");

                entity.Property(e => e.Seq).HasColumnType("int(10)");

                entity.Property(e => e.ControlId).HasColumnType("varchar(20)");

                entity.Property(e => e.ControlType).HasColumnType("int(10)");

                entity.Property(e => e.Return).HasColumnType("int(1)");

                entity.Property(e => e.WFId)
                    .HasColumnType("int(10)")
                    .HasColumnName("WFId");

                entity.Property(e => e.Depid)
                    .HasColumnType("int(10)")
                    .HasColumnName("DEPId");

                entity.Property(e => e.TxnDateTime).HasColumnType("datetime");

                entity.Property(e => e.Qty01).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03).HasColumnType("decimal(11,2)");

                entity.Property(e => e.JobNo).HasColumnType("varchar(30)");

                entity.Property(e => e.OperationCode).HasColumnType("int(5)");

                entity.Property(e => e.EnteredBy).HasColumnType("varchar(20)");

                entity.Property(e => e.TxnStatus).HasColumnType("int(1)");

                entity.Property(e => e.ErrorCode).HasColumnType("varchar(100)");

                entity.Property(e => e.Remark).HasColumnType("varchar(1000)");

                entity.Property(e => e.Approver).HasColumnType("varchar(20)");

                entity.Property(e => e.LocCodeFrom).HasColumnType("varchar(20)");

                entity.Property(e => e.LocCode).HasColumnType("varchar(20)");

                entity.Property(e => e.ReceiverName).HasColumnType("varchar(45)");

                entity.Property(e => e.ReceiverEmail).HasColumnType("varchar(45)");

                entity.Property(e => e.WatcherEmail).HasColumnType("varchar(45)");

                entity.Property(e => e.VehicleNo).HasColumnType("varchar(20)");

                entity.Property(e => e.ApprovalStatus).HasColumnType("int(11)");

                entity.Property(e => e.ApprovedDateTime).HasColumnType("datetime");

                entity.Property(e => e.SecurityPassedBy).HasColumnType("varchar(20)");

                entity.Property(e => e.SecurityPassedDateTime).HasColumnType("datetime");

                entity.Property(e => e.SecurityReceivedBy).HasColumnType("varchar(20)");

                entity.Property(e => e.SecurityPassedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ClosedBy).HasColumnType("varchar(20)");

                entity.Property(e => e.ClosedDateTime).HasColumnType("datetime");

                entity.Property(e => e.InvoiceStatus).HasColumnType("int(1)");

                entity.Property(e => e.InvoiceNumber).HasColumnType("varchar(200)");


                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasColumnType("varchar(45)");

                entity.Property(e => e.CreatedMachine).HasColumnType("varchar(45)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy).HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedMachine).HasColumnType("varchar(25)");
            });

            modelBuilder.Entity<Factory>(entity =>
            {
                entity.HasKey(e => new { e.GroupCode, e.Sbucode, e.FacCode });

                entity.ToTable("factory");

                entity.HasIndex(e => e.Sbucode)
                    .HasName("FK_sbu_FacCode_idx_idx");

                entity.Property(e => e.GroupCode).HasColumnType("varchar(10)");

                entity.Property(e => e.Sbucode)
                    .HasColumnName("SBUCode")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.FacCode).HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.FacName)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
            });

            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasKey(e => new { e.ClientId, e.LevelId, e.Fvalue });

                entity.ToTable("favorite");

                entity.Property(e => e.ClientId).HasColumnType("varchar(50)");

                entity.Property(e => e.LevelId).HasColumnType("int(10)");

                entity.Property(e => e.Fvalue)
                    .HasColumnName("FValue")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.Fvalue02)
                    .HasColumnName("FValue02")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.GroupCode);

                entity.ToTable("group");

                entity.Property(e => e.GroupCode).HasColumnType("varchar(10)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.GroupName)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
            });

            modelBuilder.Entity<L1>(entity =>
            {
                entity.ToTable("l1");

                entity.HasIndex(e => e.L1no)
                    .HasName("Index_L1No");

                entity.HasIndex(e => e.Ref01)
                    .HasName("FK_style_1");

                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.L1colVijitha)
                    .HasColumnName("l1colVijitha")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.L1desc)
                    .IsRequired()
                    .HasColumnName("L1Desc")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.L1no)
                    .IsRequired()
                    .HasColumnName("L1No")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.L1status)
                    .HasColumnName("L1Status")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.QtyMax)
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValueSql("'0.00'");

                entity.Property(e => e.Ref01).HasColumnType("varchar(30)");

                entity.Property(e => e.Ref02).HasColumnType("varchar(30)");

                entity.Property(e => e.Ref03).HasColumnType("varchar(30)");

                entity.Property(e => e.Ref04).HasColumnType("varchar(30)");

                entity.Property(e => e.Ref05).HasColumnType("varchar(30)");

                entity.Property(e => e.StructureNo).HasColumnType("int(3)");

                entity.Property(e => e.BuyerDivId).HasColumnType("int(10)");

                entity.Property(e => e.BuyerId).HasColumnType("int(10)");

                entity.Property(e => e.AchivedComments).HasColumnType("varchar(200)");

                //entity.Property(e => e.Wfid).HasColumnName("WFId");
            });

            modelBuilder.Entity<L2>(entity =>
            {
                entity.HasKey(e => new { e.L1id, e.L2id });

                entity.ToTable("l2");

                entity.HasIndex(e => e.L2no)
                    .HasName("Index_L2No");

                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.L2id).HasColumnName("L2Id");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
                
                entity.Property(e => e.DeliveryDate).HasColumnType("datetime");

                entity.Property(e => e.L2desc)
                    .IsRequired()
                    .HasColumnName("L2Desc")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.L2no)
                    .IsRequired()
                    .HasColumnName("L2No")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.L2status)
                    .HasColumnName("L2Status")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.NextBcNo)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.QtyMax)
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValueSql("'0.00'");

                entity.Property(e => e.Ref01).HasColumnType("varchar(50)");

                entity.Property(e => e.Ref02).HasColumnType("varchar(50)");

                entity.Property(e => e.Wfid).HasColumnName("WFId");

                entity.HasOne(d => d.L1)
                    .WithMany(p => p.L2)
                    .HasForeignKey(d => d.L1id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_l2_L1");
            });

            modelBuilder.Entity<L3>(entity =>
            {
                entity.HasKey(e => new { e.L1id, e.L2id, e.L3id });

                entity.ToTable("l3");

                entity.HasIndex(e => e.L3no)
                    .HasName("Index_L3No");

                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.L2id).HasColumnName("L2Id");

                entity.Property(e => e.L3id).HasColumnName("L3Id");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.L3desc)
                    .IsRequired()
                    .HasColumnName("L3Desc")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.L3no)
                    .IsRequired()
                    .HasColumnName("L3No")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.L3status)
                    .HasColumnName("L3Status")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.QtyMax)
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValueSql("'0.00'");

                entity.HasOne(d => d.L)
                    .WithMany(p => p.L3)
                    .HasForeignKey(d => new { d.L1id, d.L2id })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_l3_L2");
            });

            modelBuilder.Entity<L4>(entity =>
            {
                entity.HasKey(e => new { e.L1id, e.L2id, e.L3id, e.L4id });

                entity.ToTable("l4");

                entity.HasIndex(e => e.L4no)
                    .HasName("Index_L4No");

                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.L2id).HasColumnName("L2Id");

                entity.Property(e => e.L3id).HasColumnName("L3Id");

                entity.Property(e => e.L4id).HasColumnName("L4Id");

                entity.Property(e => e.WashCatId).HasColumnType("int(10)");

                entity.Property(e => e.WashDescription).HasColumnType("varchar(500)");

                entity.Property(e => e.WashType).HasColumnType("varchar(50)");

                entity.Property(e => e.SFCatId).HasColumnType("int(10)");

                entity.Property(e => e.SubinPO).HasColumnType("varchar(45)");

                entity.Property(e => e.GarmentWeight).HasColumnType("decimal(11,2)");

                entity.Property(e => e.WashDuration).HasColumnType("decimal(11,2)");

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Category).HasColumnType("varchar(45)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.L4desc)
                    .IsRequired()
                    .HasColumnName("L4Desc")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.L4no)
                    .IsRequired()
                    .HasColumnName("L4No")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.L4status)
                    .HasColumnName("L4Status")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.QtyMax)
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValueSql("'0.00'");

                entity.HasOne(d => d.L)
                    .WithMany(p => p.L4)
                    .HasForeignKey(d => new { d.L1id, d.L2id, d.L3id })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_l4_L3");
            });

            modelBuilder.Entity<L5>(entity =>
            {
                entity.HasKey(e => new { e.L1id, e.L2id, e.L3id, e.L4id, e.L5id });

                entity.ToTable("l5");

                entity.HasIndex(e => e.L5no)
                    .HasName("Index_L5No");

                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.L2id).HasColumnName("L2Id");

                entity.Property(e => e.L3id).HasColumnName("L3Id");

                entity.Property(e => e.L4id).HasColumnName("L4Id");

                entity.Property(e => e.L5id).HasColumnName("L5Id");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.L5desc)
                    .IsRequired()
                    .HasColumnName("L5Desc")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.L5no)
                    .IsRequired()
                    .HasColumnName("L5No")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.L5status)
                    .HasColumnName("L5Status")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.QtyMax)
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValueSql("'0.00'");

                entity.HasOne(d => d.L)
                    .WithMany(p => p.L5)
                    .HasForeignKey(d => new { d.L1id, d.L2id, d.L3id, d.L4id })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_l5_L4");
            });

            modelBuilder.Entity<L5bc>(entity =>
            {
                entity.HasKey(e => e.BarCodeNo);

                entity.ToTable("l5bc");

                entity.HasIndex(e => new { e.L1id, e.L2id, e.L3id, e.L4id, e.L5id })
                    .HasName("L1toL5Index");

                entity.HasIndex(e => new { e.L1id, e.L2id, e.L5id, e.IsPrinted, e.IsDuplicateBc })
                    .HasName("GroupByIndex");

                entity.HasIndex(e => e.L1id)
                    .HasName("L1Id");

                entity.HasIndex(e => e.L2id)
                    .HasName("L2Id");

                entity.HasIndex(e => e.L5id)
                    .HasName("FK_L5_idx");

                entity.Property(e => e.BarCodeNo).HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedBy).HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine).HasColumnType("varchar(20)");

                entity.Property(e => e.IsDuplicateBc)
                    .HasColumnName("IsDuplicateBC")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.IsPrinted)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.L2id).HasColumnName("L2Id");

                entity.Property(e => e.L3id).HasColumnName("L3Id");

                entity.Property(e => e.L4id).HasColumnName("L4Id");

                entity.Property(e => e.L5bcisUsed)
                    .HasColumnName("L5BCIsUsed")
                    .HasColumnType("int(11)");

                entity.Property(e => e.L5bcstatus)
                    .HasColumnName("L5BCStatus")
                    .HasColumnType("int(11)");

                entity.Property(e => e.L5desc)
                    .HasColumnName("L5Desc")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.L5id).HasColumnName("L5Id");

                entity.Property(e => e.LotNo).HasColumnType("varchar(10)");

                entity.Property(e => e.ModifiedBy).HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine).HasColumnType("varchar(20)");

                entity.Property(e => e.Pattern).HasColumnType("varchar(10)");

                entity.Property(e => e.QtyMax).HasColumnType("int(11)");

                entity.Property(e => e.RecStatus).HasColumnType("int(11)");

                entity.Property(e => e.TeamId).HasColumnType("int(11)");
            });

            modelBuilder.Entity<L5bcPrint>(entity =>
            {
                entity.ToTable("l5bc_print");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BcCount)
                    .HasColumnName("BC_Count")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BcStart)
                    .HasColumnName("BC_Start")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.IsDuplicateBc)
                    .HasColumnName("IsDuplicateBC")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IsPrinted).HasColumnType("int(11)");

                entity.Property(e => e.L1desc01)
                    .HasColumnName("L1Desc_01")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.L1id)
                    .HasColumnName("L1Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.L2desc)
                    .IsRequired()
                    .HasColumnName("L2Desc")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.L2id)
                    .HasColumnName("L2Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.L3id)
                    .HasColumnName("L3Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.L3no)
                    .HasColumnName("L3No")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.L4id)
                    .HasColumnName("L4Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.L4no)
                    .HasColumnName("L4No")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.L5desc)
                    .IsRequired()
                    .HasColumnName("L5Desc")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.L5id)
                    .HasColumnName("L5Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LotNo).HasColumnType("varchar(10)");

                entity.Property(e => e.ModifiedBy).HasColumnType("varchar(45)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine).HasColumnType("varchar(45)");

                entity.Property(e => e.Pattern).HasColumnType("varchar(10)");

                entity.Property(e => e.RecStatus).HasColumnType("int(11)");

                entity.Property(e => e.TeamId).HasColumnType("int(11)");
            });

            modelBuilder.Entity<L5mo>(entity =>
            {
                entity.HasKey(e => new { e.L1id, e.L2id, e.L3id, e.L4id, e.L5id, e.L5moid });

                entity.ToTable("l5mo");

                entity.HasIndex(e => new { e.L1id, e.L2id, e.L3id, e.L4id, e.L5id, e.L5mostatus, e.RecStatus })
                    .HasName("L5mo");

                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.L2id).HasColumnName("L2Id");

                entity.Property(e => e.L3id).HasColumnName("L3Id");

                entity.Property(e => e.L4id).HasColumnName("L4Id");

                entity.Property(e => e.L5id).HasColumnName("L5Id");

                entity.Property(e => e.L5moid).HasColumnName("L5MOId");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.L5mono)
                    .HasColumnName("L5MONo")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.L5mostatus)
                    .HasColumnName("L5MOStatus")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.QtyMax)
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValueSql("'0.00'");

                entity.HasOne(d => d.L)
                    .WithMany(p => p.L5mo)
                    .HasForeignKey(d => new { d.L1id, d.L2id, d.L3id, d.L4id, d.L5id })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_l5mo_L5");
            });

            modelBuilder.Entity<L5moops>(entity =>
            {
                entity.HasKey(e => new { e.GroupCode, e.Sbucode, e.FacCode, e.L1id, e.L2id, e.L3id, e.L4id, e.L5id, e.L5moid, e.OperationCode });

                entity.ToTable("l5moops");

                entity.HasIndex(e => new { e.L1id, e.L2id, e.L3id, e.L4id, e.L5id, e.L5moid })
                    .HasName("FK_l5motxn_L5mo");

                entity.HasIndex(e => new { e.L1id, e.L2id, e.L3id, e.L4id, e.OperationCode })
                    .HasName("Index_L1L2L3L4_OperationCode");

                entity.HasIndex(e => new { e.OperationCode })
                    .HasName("OppCodeIndex");
                    
                entity.Property(e => e.GroupCode).HasColumnType("varchar(10)");

                entity.Property(e => e.Sbucode)
                    .HasColumnName("SBUCode")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.FacCode).HasColumnType("varchar(20)");

                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.L2id).HasColumnName("L2Id");

                entity.Property(e => e.L3id).HasColumnName("L3Id");

                entity.Property(e => e.L4id).HasColumnName("L4Id");

                entity.Property(e => e.L5id).HasColumnName("L5Id");

                entity.Property(e => e.L5moid).HasColumnName("L5MOId");

                entity.Property(e => e.OperationCode).HasColumnType("int(10)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.L5mono)
                    .HasColumnName("L5MONo")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.L5moopsStatus)
                    .HasColumnName("L5MOOpsStatus")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.OrderQty)
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValueSql("'0.00'");

                entity.Property(e => e.ReportedQty)
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValueSql("'0.00'");

                entity.Property(e => e.ScrappedQty)
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValueSql("'0.00'");

                entity.Property(e => e.TxnDateTimeMax).HasColumnType("datetime");

                entity.Property(e => e.WorkCenter).HasColumnType("varchar(30)");

                entity.HasOne(d => d.L)
                    .WithMany(p => p.L5moops)
                    .HasForeignKey(d => new { d.L1id, d.L2id, d.L3id, d.L4id, d.L5id, d.L5moid })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_l5motxn_L5mo");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(e => new { e.GroupCode, e.Sbucode, e.FacCode, e.LocCode });

                entity.ToTable("location");

                entity.Property(e => e.GroupCode).HasColumnType("varchar(10)");

                entity.Property(e => e.Sbucode)
                    .HasColumnName("SBUCode")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.FacCode).HasColumnType("varchar(20)");

                entity.Property(e => e.LocCode).HasColumnType("varchar(20)");

                entity.Property(e => e.LocAddress).HasColumnType("varchar(145)");

                entity.Property(e => e.LocDescription).HasColumnType("varchar(45)");

                entity.Property(e => e.VATNo).HasColumnType("varchar(45)");

                entity.Property(e => e.SVATNo).HasColumnType("varchar(45)");

                entity.Property(e => e.Atten).HasColumnType("varchar(45)");

                entity.Property(e => e.TelNo).HasColumnType("varchar(45)");

                entity.Property(e => e.CustomerNo).HasColumnType("varchar(20)");
                
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.LocName)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.HasOne(d => d.Factory)
                    .WithMany(p => p.Location)
                    .HasForeignKey(d => new { d.GroupCode, d.Sbucode, d.FacCode })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_location_Factory");
            });

            modelBuilder.Entity<WarehouseLocation>(entity =>
            {
                entity.HasKey(e => new { e.FacCode, e.WarCode, e.WarLocCode });

                entity.ToTable("warehouse_location");

                entity.Property(e => e.FacCode).HasColumnType("varchar(20)");

                entity.Property(e => e.WarCode).HasColumnType("varchar(20)");

                entity.Property(e => e.WarName).HasColumnType("varchar(50)");

                entity.Property(e => e.WarLocCode).HasColumnType("varchar(20)");

                entity.Property(e => e.WarLocName).HasColumnType("varchar(145)");

                entity.Property(e => e.WarLocAddress).HasColumnType("varchar(45)");

                entity.Property(e => e.RecStatus).HasColumnType("int(1)");
                
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
            });

            modelBuilder.Entity<Manualdetxn>(entity =>
            {
                entity.ToTable("manualdetxn");

                entity.Property(e => e.AppBy).HasColumnType("varchar(20)");

                entity.Property(e => e.AppStatus).HasColumnType("int(11)");

                entity.Property(e => e.AppTime).HasColumnType("datetime");

                entity.Property(e => e.BarCodeNo)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.CreatedBy).HasColumnType("varchar(45)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine).HasColumnType("varchar(45)");

                entity.Property(e => e.Dclid).HasColumnName("DCLId");

                entity.Property(e => e.Dcmid).HasColumnName("DCMId");

                entity.Property(e => e.Depid).HasColumnName("DEPId");

                entity.Property(e => e.Detxncol)
                    .HasColumnName("detxncol")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.EnteredBy).HasColumnType("varchar(20)");

                entity.Property(e => e.ErrorCode).HasColumnType("int(10)");

                entity.Property(e => e.ErrorDescription).HasColumnType("varchar(1000)");

                entity.Property(e => e.HasError).HasColumnType("int(1)");

                entity.Property(e => e.HourNo).HasColumnType("int(11)");

                entity.Property(e => e.InstanceNo).HasColumnType("int(11)");

                entity.Property(e => e.IsSucess).HasColumnType("int(1)");

                entity.Property(e => e.JobNo).HasColumnType("varchar(30)");

                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.L2id).HasColumnName("L2Id");

                entity.Property(e => e.L3id).HasColumnName("L3Id");

                entity.Property(e => e.L4id).HasColumnName("L4Id");

                entity.Property(e => e.L5id).HasColumnName("L5Id");

                entity.Property(e => e.L5moid)
                    .HasColumnName("L5MOId")
                    .HasColumnType("int(10)");

                entity.Property(e => e.L5mono)
                    .HasColumnName("L5MONo")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.ModifiedBy).HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine).HasColumnType("varchar(25)");

                entity.Property(e => e.OperationCode).HasColumnType("int(5)");

                entity.Property(e => e.PlussMinus).HasColumnType("int(1)");

                entity.Property(e => e.Qty01).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty01Ns)
                    .HasColumnName("Qty01NS")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02Ns)
                    .HasColumnName("Qty02NS")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03Ns)
                    .HasColumnName("Qty03NS")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.RecStatus).HasColumnType("int(11)");

                entity.Property(e => e.Rrid)
                    .HasColumnName("RRId")
                    .HasColumnType("varchar(3)");

                entity.Property(e => e.SubmittedBy).HasColumnType("varchar(45)");

                entity.Property(e => e.SubmittedOn).HasColumnType("datetime");

                entity.Property(e => e.TxnDateTime).HasColumnType("datetime");

                entity.Property(e => e.TxnMode).HasColumnType("int(1)");

                entity.Property(e => e.UploadBy).HasColumnType("varchar(20)");

                entity.Property(e => e.UploadStatus).HasColumnType("int(1)");

                entity.Property(e => e.UploadTime).HasColumnType("datetime");

                entity.Property(e => e.WfdepinstId).HasColumnName("WFDEPInstId");

                entity.Property(e => e.Wfid).HasColumnName("WFId");

                entity.Property(e => e.WorkCenter).HasColumnType("varchar(30)");
            });

            modelBuilder.Entity<Prodhour>(entity =>
            {
                entity.HasKey(e => e.HourNo);

                entity.ToTable("prodhour");

                entity.HasIndex(e => e.HourNo)
                    .HasName("HourNo01_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.Etime)
                    .IsRequired()
                    .HasColumnName("ETime")
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.HourName)
                    .IsRequired()
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.Stime)
                    .IsRequired()
                    .HasColumnName("STime")
                    .HasColumnType("varchar(4)");
            });

            modelBuilder.Entity<Prodtarget>(entity =>
            {
                entity.HasKey(e => new { e.TeamId, e.TxnDate, e.L1id, e.OperationCode });

                entity.ToTable("prodtarget");

                entity.Property(e => e.GroupCode)
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.SBUCode)
                    .HasColumnType("varchar(20)");
                
                entity.Property(e => e.FacCode)
                    .HasColumnType("varchar(20)");
                
                entity.Property(e => e.TeamId)
                    .HasColumnType("int");

                entity.Property(e => e.TxnDate).HasColumnType("date");

                entity.Property(e => e.L1id)
                    .HasColumnType("int");

                entity.Property(e => e.OperationCode)
                    .HasColumnType("int");

                entity.Property(e => e.Depid)
                    .HasColumnType("int");

                entity.Property(e => e.Qty01)
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.RecStatus)
                    .HasColumnType("int(1)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
            });

            modelBuilder.Entity<Rejectreason>(entity =>
            {
                entity.HasKey(e => e.Rrid);

                entity.ToTable("rejectreason");

                entity.HasIndex(e => e.Scode)
                    .HasName("Index_SCode")
                    .IsUnique();

                entity.Property(e => e.Rrid)
                    .HasColumnName("RRId")
                    .HasColumnType("int(4)");

                entity.Property(e => e.DopsId)
                    .HasColumnName("DopsId")
                    .HasColumnType("int(4)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.RrcatId)
                    .HasColumnName("RRCatId")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.Rrdesc)
                    .IsRequired()
                    .HasColumnName("RRDesc")
                    .HasColumnType("varchar(60)");

                entity.Property(e => e.RrdescSin)
                    .HasColumnName("RRDescSin")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.RrdescTem)
                    .HasColumnName("RRDescTem")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.Rrname)
                    .IsRequired()
                    .HasColumnName("RRName")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.RrnameSin)
                    .HasColumnName("RRNameSin")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.RrnameTem)
                    .HasColumnName("RRNameTem")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.RejectType)
                    .HasColumnName("RejectType")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Rrtype)
                    .HasColumnName("RRType")
                    .HasColumnType("int(2)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Scode)
                    .HasColumnName("SCode")
                    .HasColumnType("varchar(6)");
            });

            modelBuilder.Entity<Rrcat>(entity =>
            {
                entity.ToTable("rrcat");

                entity.Property(e => e.RrcatId)
                    .HasColumnName("RRCatId")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.RrcatName)
                    .IsRequired()
                    .HasColumnName("RRCatName")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.RrcatNameSin)
                    .HasColumnName("RRCatNameSin")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.RrcatNameTem)
                    .HasColumnName("RRCatNameTem")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.Rrtype)
                    .HasColumnName("RRType")
                    .HasColumnType("int(2)");
            });

            modelBuilder.Entity<Sbu>(entity =>
            {
                entity.HasKey(e => new { e.GroupCode, e.Sbucode });

                entity.ToTable("sbu");

                entity.Property(e => e.GroupCode).HasColumnType("varchar(10)");

                entity.Property(e => e.Sbucode)
                    .HasColumnName("SBUCode")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.Sbuname)
                    .IsRequired()
                    .HasColumnName("SBUName")
                    .HasColumnType("varchar(50)");

                entity.HasOne(d => d.GroupCodeNavigation)
                    .WithMany(p => p.Sbu)
                    .HasForeignKey(d => d.GroupCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_sbu_GroupCode");
            });

            modelBuilder.Entity<Secfunction>(entity =>
            {
                entity.HasKey(e => e.FunctionId);

                entity.ToTable("secfunction");

                entity.Property(e => e.FunctionId).HasColumnType("char(10)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.CreatedUser)
                    .IsRequired()
                    .HasColumnType("varchar(15)");

                entity.Property(e => e.FuncName)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.LocWiseAccess)
                    .IsRequired()
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.ModifiedUser)
                    .IsRequired()
                    .HasColumnType("varchar(15)");
            });

            modelBuilder.Entity<Secparm>(entity =>
            {
                entity.HasKey(e => e.TagNo);

                entity.ToTable("secparm");

                entity.Property(e => e.TagNo).HasColumnType("varchar(20)");

                entity.Property(e => e.StrValue)
                    .IsRequired()
                    .HasColumnType("varchar(45)");
            });

            modelBuilder.Entity<Secuser>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("secuser");

                entity.Property(e => e.UserId).HasColumnType("char(20)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(15)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.DatePasswordExpiry).HasColumnType("date");

                entity.Property(e => e.DatePwdChanged).HasColumnType("datetime");

                entity.Property(e => e.DateUserActiveFrom).HasColumnType("date");

                entity.Property(e => e.DateUserExpire).HasColumnType("date");

                entity.Property(e => e.LastAccessedDateTime).HasColumnType("datetime");

                entity.Property(e => e.LastLogonDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.ModifiedUser)
                    .IsRequired()
                    .HasColumnType("varchar(15)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.UnsuccessAttemptDateTime).HasColumnType("datetime");

                entity.Property(e => e.UserIdN)
                    .IsRequired()
                    .HasColumnType("char(20)");
            });

            modelBuilder.Entity<Secuserright>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.FunctionId });

                entity.ToTable("secuserright");

                entity.HasIndex(e => e.FunctionId)
                    .HasName("FK_secuserright_Func");

                entity.Property(e => e.UserId).HasColumnType("char(15)");

                entity.Property(e => e.FunctionId).HasColumnType("char(10)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.CreatedUser)
                    .IsRequired()
                    .HasColumnType("varchar(15)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.ModifiedUser)
                    .IsRequired()
                    .HasColumnType("char(15)");

                entity.HasOne(d => d.Function)
                    .WithMany(p => p.Secuserright)
                    .HasForeignKey(d => d.FunctionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_secuserright_Func");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Secuserright)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_secuserright_SecUser");
            });

            modelBuilder.Entity<Secuserrightdep>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.FunctionId, e.GroupCode, e.Sbucode, e.FacCode, e.Depid, e.StructureNo });

                entity.ToTable("secuserrightdep");

                entity.HasIndex(e => e.Depid)
                    .HasName("FK_secuserrightdep_DEP");

                entity.Property(e => e.UserId).HasColumnType("char(15)");

                entity.Property(e => e.FunctionId).HasColumnType("varchar(25)");

                entity.Property(e => e.GroupCode).HasColumnType("varchar(10)");

                entity.Property(e => e.Sbucode)
                    .HasColumnName("SBUCode")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.FacCode).HasColumnType("varchar(20)");

                entity.Property(e => e.Depid).HasColumnName("DEPId");

                entity.Property(e => e.StructureNo).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.CreatedUser)
                    .IsRequired()
                    .HasColumnType("varchar(15)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.ModifiedUser)
                    .IsRequired()
                    .HasColumnType("char(15)");
            });

            modelBuilder.Entity<TClientconfig>(entity =>
            {
                entity.HasKey(e => e.ClientId);

                entity.ToTable("t_clientconfig");

                entity.HasIndex(e => e.WfdepinstId)
                    .HasName("WFDEPInstId_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ClientId).HasColumnType("varchar(50)");

                entity.Property(e => e.DataCaptureMode).HasColumnType("int(2)");

                entity.Property(e => e.FacName).HasColumnType("varchar(50)");

                entity.Property(e => e.LoginMode).HasColumnType("int(2)");

                entity.Property(e => e.OpCode1).HasColumnType("int(10)");

                entity.Property(e => e.OpCode2).HasColumnType("int(10)");

                entity.Property(e => e.OperationName).HasColumnType("varchar(50)");

                entity.Property(e => e.RecStatus).HasColumnType("int(1)");

                entity.Property(e => e.SelectMode).HasColumnType("int(2)");

                entity.Property(e => e.TeamId).HasColumnType("int(11)");

                entity.Property(e => e.TeamName).HasColumnType("varchar(50)");

                entity.Property(e => e.WfdepinstId)
                    .HasColumnName("WFDEPInstId")
                    .HasColumnType("int(10)")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<TDepList>(entity =>
            {
                entity.HasKey(e => e.WfdepinstId);

                entity.ToTable("t_dep_list");

                entity.Property(e => e.WfdepinstId)
                    .HasColumnName("WFDEPInstId")
                    .HasColumnType("int(10)");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("team");

                entity.HasIndex(e => e.FacCode)
                    .HasName("FK_team_factor");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.DeptCode)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Erpcode01)
                    .HasColumnName("ERPCode01")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Erpcode02)
                    .HasColumnName("ERPCode02")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.FacCode)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.GroupCode)
                    .IsRequired()
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.LocCode)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.Sbucode)
                    .IsRequired()
                    .HasColumnName("SBUCode")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.TeamCode)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.TeamName)
                    .IsRequired()
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<Teambundle>(entity =>
            {
                entity.HasKey(e => e.TeamId);

                entity.ToTable("teambundle");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.Qty).HasColumnType("decimal(11,2)");
            });

            modelBuilder.Entity<TeamCounter>(entity =>
            {
                entity.HasKey(e => new { e.CounterId, e.WfdepinstId });

                //entity.HasIndex(e => new { e.WfdepinstId })
                   // .HasName("Index_WFDEPInstId");

                //entity.HasIndex(e => new { e.BagBarCodeNo, e.WfdepinstId })
                    //.HasName("Index_BC_DEPId");

                //entity.HasIndex(e => new { e.L1id, e.L2id, e.L4id })
                    //.HasName("Index_L1L2L4");

                //entity.HasIndex(e => new { e.CounterType })
                    //.HasName("Index_CounterType");

                //entity.HasIndex(e => new { e.CounterId })
                    //.HasName("Index_CounterID");

                //entity.HasIndex(e => new { e.CounterId, e.BagBarCodeNo })
                    //.HasName("Index_CounterID_BagBarcode");
                    
                entity.HasIndex(e => new { e.WfdepinstId, e.L1id, e.L2id, e.L4id, e.BagBarCodeNo })
                    .HasName("Index_WFDEPInstID_L1L2L4_BagBarcode");

                //entity.HasIndex(e => new { e.WfdepinstId, e.L1id, e.L2id, e.L4id })
                    //.HasName("Index_WFDEPInstId_L1L2L4");

                entity.ToTable("team_counter");

                entity.Property(e => e.CounterId)
                    .HasColumnName("CounterID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.WfdepinstId).HasColumnName("WFDEPInstId");

                entity.Property(e => e.CounterType).HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.L2id).HasColumnName("L2Id");

                entity.Property(e => e.L3id).HasColumnName("L3Id");

                entity.Property(e => e.L4id).HasColumnName("L4Id");

                entity.Property(e => e.L5id).HasColumnName("L5Id");

                entity.Property(e => e.BagBarCodeNo)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.BagSize).HasColumnName("BagSize");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.Qty01).HasColumnType("int(11)");

                entity.Property(e => e.Qty02).HasColumnType("int(11)");

                entity.Property(e => e.Qty03).HasColumnType("int(11)");

                entity.Property(e => e.CutQty).HasColumnType("int(11)");

                entity.Property(e => e.IntCutQty).HasColumnType("int(11)");

                entity.Property(e => e.CounterNumber).HasColumnType("int(11)");

                entity.Property(e => e.BagStatus)
                    .HasColumnType("int(11)")
                    .HasColumnName("BagStatus");

                entity.Property(e => e.RecStatus).HasColumnType("int(11)");
            });

            modelBuilder.Entity<BuudyTagCounter>(entity =>
            {
                entity.HasKey(e => new { e.CounterId, e.WfdepinstId });

                //entity.HasIndex(e => new { e.WfdepinstId })
                    //.HasName("Index_WFDEPInstId");

                //entity.HasIndex(e => new { e.BagBarCodeNo, e.WfdepinstId })
                   // .HasName("Index_BC_DEPId");

                //entity.HasIndex(e => new { e.L1id, e.L2id, e.L4id })
                  //  .HasName("Index_L1L2L4");

                //entity.HasIndex(e => new { e.CounterType })
                  //  .HasName("Index_CounterType");

                //entity.HasIndex(e => new { e.CounterId })
                  //  .HasName("Index_CounterID");

                //entity.HasIndex(e => new { e.CounterId, e.BagBarCodeNo })
                    //.HasName("Index_CounterID_BagBarcode");
                    
                //entity.HasIndex(e => new { e.WfdepinstId, e.L1id, e.L2id, e.L4id, e.BagBarCodeNo })
                    //.HasName("Index_WFDEPInstID_L1L2L4_BagBarcode");

                entity.HasIndex(e => new { e.WfdepinstId, e.L1id, e.L2id, e.L4id, e.TravelBarCodeNo, e.RRType, e.RRId })
                    .HasName("Index_WFDEPInstId_L1L2L4");

                //entity.HasIndex(e => new { e.RRId })
                    //.HasName("Index_RRId");

                entity.Property(e => e.RRName).HasColumnType("varchar(20)");

                entity.ToTable("buudy_tag_counter");

                entity.Property(e => e.CounterId)
                    .HasColumnName("CounterID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.WfdepinstId).HasColumnName("WFDEPInstId");

                entity.Property(e => e.CounterType).HasColumnType("int(11)");

                entity.Property(e => e.TravelBarCodeNo).HasColumnType("varchar(20)");

                entity.Property(e => e.RRType).HasColumnType("int(2)");

                entity.Property(e => e.RRId).HasColumnType("int(4)");
                
                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.L2id).HasColumnName("L2Id");

                entity.Property(e => e.L3id).HasColumnName("L3Id");

                entity.Property(e => e.L4id).HasColumnName("L4Id");

                entity.Property(e => e.L5id).HasColumnName("L5Id");

                entity.Property(e => e.BagBarCodeNo)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.BagSize).HasColumnName("BagSize");

                entity.Property(e => e.Qty01).HasColumnType("int(11)");

                entity.Property(e => e.Qty02).HasColumnType("int(11)");

                entity.Property(e => e.Qty03).HasColumnType("int(11)");

                entity.Property(e => e.CutQty).HasColumnType("int(11)");

                entity.Property(e => e.CounterNumber).HasColumnType("int(11)");

                entity.Property(e => e.BagStatus)
                    .HasColumnType("int(11)")
                    .HasColumnName("BagStatus");

                entity.Property(e => e.JobQty).HasColumnType("int(11)");

                entity.Property(e => e.Weight).HasColumnType("decimal(11,2)");

                entity.Property(e => e.TrollyNo).HasColumnType("varchar(20)");

                entity.Property(e => e.Remarks).HasColumnType("varchar(1000)");

                entity.Property(e => e.RecStatus).HasColumnType("int(11)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
            });

            modelBuilder.Entity<GroupBarcode>(entity =>
            {
                entity.HasKey(e => new { e.Seq, e.WFId, e.L1id, e.L2id, e.L3id, e.L4id, e.BagBarCodeNo, e.TxnMode }); //{ e.Seq, e.L1id, e.L2id, e.L3id, e.L4id, e.L5id, e.BagBarCodeNo, e.WFId, e.DEPId });

                entity.HasIndex(e => new { e.DEPId, e.WFId })
                    .HasName("Index_WFId_DEPId");

                //entity.HasIndex(e => new { e.WFDEPInstId, e.DEPId })
                    //.HasName("Index_WFDEPInstId_DEPId");

                //entity.HasIndex(e => new { e.BagBarCodeNo, e.DEPId })
                    //.HasName("Index_BC_DEPId");

                //entity.HasIndex(e => new { e.L1id, e.L2id, e.L3id, e.L4id, e.L5id })
                    //.HasName("Index_L1L2L3L4L5");
                
                //entity.HasIndex(e => new { e.TxnMode })
                    //.HasName("Index_TxnMode");

                //entity.HasIndex(e => new { e.TxnMode, e.BagBarCodeNo })
                    //.HasName("Index_TxnMode_Barcode");

                entity.HasIndex(e => new { e.L1id, e.L2id, e.L3id, e.L4id, e.L5id, e.BagBarCodeNo, e.TxnMode })
                    .HasName("Index_L1L2L3L4L5_Barcode");

                //entity.HasIndex(e => new { e.L1id, e.L2id, e.L3id, e.L4id, e.BagBarCodeNo, e.TxnMode })
                    //.HasName("Index_L1L2L4_BagBarCodeNo");

                //entity.HasIndex(e => new { e.BagBarCodeNo })
                    //.HasName("IndexBagBarCodeNo");

                //entity.HasIndex(e => new { e.L1id, e.L2id, e.L4id })
                    //.HasName("Index_L1L2L4");

                entity.HasIndex(e => new { e.CreatedDateTime})
                    .HasName("Index_TxnDateTime");

                entity.ToTable("group_barcode");

                entity.Property(e => e.Seq).HasColumnName("Seq");

                entity.Property(e => e.WFId).HasColumnName("WFId");

                entity.Property(e => e.WFDEPInstId).HasColumnName("WFDEPInstId");

                entity.Property(e => e.DEPId).HasColumnName("DEPId");

                entity.Property(e => e.BagBarCodeNo)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''");
                
                entity.Property(e => e.TxnMode).HasColumnType("int(1)");

                entity.Property(e => e.Qty01).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty01NS).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02NS).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03NS).HasColumnType("decimal(11,2)");

                entity.Property(e => e.DispatchReadyQty).HasColumnType("decimal(11,2)");

                entity.Property(e => e.CutQty).HasColumnType("int(11)");

                entity.Property(e => e.OperationCode).HasColumnType("int(5)");

                entity.Property(e => e.WorkCenter).HasColumnType("varchar(30)");

                entity.Property(e => e.RecStatus).HasColumnType("int(11)");

                entity.Property(e => e.TxnStatus).HasColumnType("int(1)");

                entity.Property(e => e.DispatchMode).HasColumnType("int(1)");

                entity.Property(e => e.SplitStatus).HasColumnType("int(1)");
                
                entity.Property(e => e.SMode).HasColumnType("int(1)");

                entity.Property(e => e.InvoiceStatus).HasColumnType("int(1)");

                entity.Property(e => e.InvoiceNumber).HasColumnType("varchar(20)");
                
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
            });

            modelBuilder.Entity<DeletedGroupBarcode>(entity =>
            {
                entity.HasKey(e => new { e.Seq, e.L1id, e.L2id, e.L3id, e.L4id, e.L5id, e.BagBarCodeNo, e.WFId, e.DEPId });

                entity.HasIndex(e => new { e.DEPId, e.WFId })
                    .HasName("Index_WFId_DEPId");

                entity.HasIndex(e => new { e.WFDEPInstId, e.DEPId })
                    .HasName("Index_WFDEPInstId_DEPId");

                entity.HasIndex(e => new { e.BagBarCodeNo, e.DEPId })
                    .HasName("Index_BC_DEPId");

                entity.HasIndex(e => new { e.L1id, e.L2id, e.L3id, e.L4id, e.L5id })
                    .HasName("Index_L1L2L3L4L5");
                
                entity.HasIndex(e => new { e.TxnMode })
                    .HasName("Index_TxnMode");

                entity.HasIndex(e => new { e.TxnMode, e.BagBarCodeNo })
                    .HasName("Index_TxnMode_Barcode");

                entity.HasIndex(e => new { e.L1id, e.L2id, e.L3id, e.L4id, e.L5id, e.BagBarCodeNo })
                    .HasName("Index_L1L2L3L4L5_Barcode");

                entity.HasIndex(e => new { e.L1id, e.L2id, e.L3id, e.L4id, e.BagBarCodeNo, e.TxnMode })
                    .HasName("Index_L1L2L4_BagBarCodeNo");

                entity.HasIndex(e => new { e.BagBarCodeNo })
                    .HasName("IndexBagBarCodeNo");

                entity.HasIndex(e => new { e.L1id, e.L2id, e.L4id })
                    .HasName("Index_L1L2L4");

                entity.HasIndex(e => new { e.CreatedDateTime})
                    .HasName("Index_TxnDateTime");

                entity.ToTable("deleted_group_barcodes");

                entity.Property(e => e.Seq).HasColumnName("Seq");

                entity.Property(e => e.WFId).HasColumnName("WFId");

                entity.Property(e => e.WFDEPInstId).HasColumnName("WFDEPInstId");

                entity.Property(e => e.DEPId).HasColumnName("DEPId");

                entity.Property(e => e.BagBarCodeNo)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''");
                
                entity.Property(e => e.TxnMode).HasColumnType("int(1)");

                entity.Property(e => e.Qty01).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty01NS).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02NS).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03NS).HasColumnType("decimal(11,2)");

                entity.Property(e => e.DispatchReadyQty).HasColumnType("decimal(11,2)");

                entity.Property(e => e.CutQty).HasColumnType("int(11)");

                entity.Property(e => e.OperationCode).HasColumnType("int(5)");

                entity.Property(e => e.WorkCenter).HasColumnType("varchar(30)");

                entity.Property(e => e.RecStatus).HasColumnType("int(11)");

                entity.Property(e => e.TxnStatus).HasColumnType("int(1)");

                entity.Property(e => e.DispatchMode).HasColumnType("int(1)");

                entity.Property(e => e.SplitStatus).HasColumnType("int(1)");

                entity.Property(e => e.InvoiceStatus).HasColumnType("int(1)");

                entity.Property(e => e.InvoiceNumber).HasColumnType("varchar(20)");
                
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
            });

            modelBuilder.Entity<TTOpearation>(entity =>
            {
                entity.HasKey(e => new { e.Seq, e.BarCodeNo, e.TxnMode, e.WFDEPInstId, e.TxnDateTime, e.OperationCode, e.PlussMinus });

                entity.HasIndex(e => new { e.TxnDateTime })
                    .HasName("Index_DeTxn");

                entity.HasIndex(e => new { e.WFDEPInstId })
                    .HasName("Index_WFDEPInstId");

                entity.HasIndex(e => new { e.CreatedDateTime})
                    .HasName("Index_TxnDateTime");

                entity.ToTable("tt_operation");

                entity.Property(e => e.Seq).HasColumnName("Seq");

                entity.Property(e => e.BarCodeNo)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.WFDEPInstId).HasColumnName("WFDEPInstId");

                entity.Property(e => e.WFId).HasColumnName("WFId");
                
                entity.Property(e => e.TxnMode).HasColumnType("int(1)");

                entity.Property(e => e.TeamId).HasColumnName("TeamId");

                entity.Property(e => e.TxnDateTime).HasColumnName("TxnDateTime");

                entity.Property(e => e.Qty01).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty01NS).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02NS).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03NS).HasColumnType("decimal(11,2)");

                entity.Property(e => e.RecStatus).HasColumnType("int(11)");

                entity.Property(e => e.OperationCode).HasColumnName("OperationCode");

                entity.Property(e => e.PlussMinus).HasColumnName("PlussMinus");
                
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
            });

            modelBuilder.Entity<BuddyBagBarcode>(entity =>
            {
                entity.HasKey(e => new { e.Seq,  e.WFId, e.DEPId, e.BuddyBagBarcodeNo });

                entity.HasIndex(e => new { e.BuddyBagBarcodeNo })
                    .HasName("Index_BuddyBagBarcode");

                entity.ToTable("buddy_bag_barcode");

                entity.Property(e => e.WFId).HasColumnName("WFId");

                entity.Property(e => e.DEPId).HasColumnName("DEPId");

                entity.Property(e => e.Seq).HasColumnName("Seq");

                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.L2id).HasColumnName("L2Id");

                entity.Property(e => e.L3id).HasColumnName("L3Id");

                entity.Property(e => e.L4id).HasColumnName("L4Id");

                entity.Property(e => e.L5id).HasColumnName("L5Id");

                entity.Property(e => e.RefBarcode)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''");
                
                entity.Property(e => e.BuddyBagBarcodeNo)
                    .HasColumnName("BuddyBagBarcode")
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''");
                
                entity.Property(e => e.TxnMode).HasColumnType("int(1)");

                entity.Property(e => e.Qty01).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty01NS).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02NS).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03NS).HasColumnType("decimal(11,2)");

                entity.Property(e => e.OperationCode).HasColumnType("int(5)");

                entity.Property(e => e.WorkCenter).HasColumnType("varchar(30)");

                entity.Property(e => e.RecStatus).HasColumnType("int(11)");

                entity.Property(e => e.TxnStatus).HasColumnType("int(1)");

                entity.Property(e => e.JobQty).HasColumnType("int(11)");

                entity.Property(e => e.Weight).HasColumnType("decimal(11,2)");

                entity.Property(e => e.TrollyNo).HasColumnType("varchar(20)");

                entity.Property(e => e.AllocationDate).HasColumnType("datetime");

                entity.Property(e => e.TravelStatus).HasColumnType("int(1)");

                entity.Property(e => e.EPF).HasColumnType("varchar(20)");

                entity.Property(e => e.FacCode).HasColumnType("varchar(20)");
                
                entity.Property(e => e.Remarks).HasColumnType("varchar(45)");
                
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
            });

            modelBuilder.Entity<TravelBarcodeDetails>(entity =>
            {
                entity.HasKey(e => new { e.WFId,  e.DEPId, e.Seq, e.TravelBarCodeNo });

                entity.ToTable("travel_barcode_details");
                
                entity.Property(e => e.Seq).HasColumnName("Seq");

                entity.Property(e => e.WFId).HasColumnName("WFId");

                entity.Property(e => e.DEPId).HasColumnName("DEPId");

                entity.Property(e => e.Color).HasColumnType("varchar(40)");

                entity.Property(e => e.TravelBarCodeNo)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''");
                
                entity.Property(e => e.TxnMode).HasColumnType("int(1)");

                entity.Property(e => e.SplitStatus).HasColumnType("int(1)");

                entity.Property(e => e.SMode).HasColumnType("int(1)");

                entity.Property(e => e.Qty01).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty01NS).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02NS).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03NS).HasColumnType("decimal(11,2)");

                entity.Property(e => e.OperationCode).HasColumnType("int(5)");

                entity.Property(e => e.WorkCenter).HasColumnType("varchar(30)");

                entity.Property(e => e.RecStatus).HasColumnType("int(11)");

                entity.Property(e => e.JobQty).HasColumnType("int(11)");

                entity.Property(e => e.Weight).HasColumnType("decimal(11,2)");

                entity.Property(e => e.TrollyNo).HasColumnType("varchar(20)");

                entity.Property(e => e.AllocationDate).HasColumnType("datetime");

                entity.Property(e => e.TravelStatus).HasColumnType("int(1)");

                entity.Property(e => e.EPF).HasColumnType("varchar(20)");

                entity.Property(e => e.FacCode).HasColumnType("varchar(20)");

                entity.Property(e => e.Remarks).HasColumnType("varchar(1000)");

                entity.Property(e => e.PlannedMachine).HasColumnType("varchar(25)");

                entity.Property(e => e.PlanedDateTime).HasColumnType("datetime");
                
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
            });

            modelBuilder.Entity<GroupBarcodeMapping>(entity =>
            {
                entity.HasKey(e => new { e.Seq, e.ChildBarcode, e.ChildTxnMode });

                entity.HasIndex(e => new { e.MotherBarcode, e.MotherTxnMode })
                    .HasName("Index_MBMTCBCT");

                entity.HasIndex(e => new { e.WFDEPInstId, e.MotherBarcode, e.MotherTxnMode })
                    .HasName("Index_WFinstMBMT");

                entity.HasIndex(e => new { e.WFDEPInstId, e.ChildBarcode, e.ChildTxnMode })
                    .HasName("Index_WFinstCBCT");

                entity.HasIndex(e => new { e.ChildBarcode, e.ChildTxnMode })
                    .HasName("Index_CBCT");
                
                entity.HasIndex(e => new { e.MotherBarcode, e.MotherTxnMode })
                    .HasName("Index_MBMT");

                entity.ToTable("group_barcode_mapping");

                entity.Property(e => e.Seq).HasColumnName("Seq");

                entity.Property(e => e.WFDEPInstId).HasColumnName("WFDEPInstId");

                entity.Property(e => e.MotherBarcode)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.MotherTxnMode).HasColumnName("MotherTxnMode");

                entity.Property(e => e.ChildBarcode)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.ChildTxnMode).HasColumnName("ChildTxnMode");

                entity.Property(e => e.Qty01).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty01NS).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02NS).HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03NS).HasColumnType("decimal(11,2)");
                
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
            });
            
            modelBuilder.Entity<Teamopp>(entity =>
            {
                entity.HasKey(e => new { e.TeamId, e.OperationCode });

                entity.ToTable("teamopp");

                entity.Property(e => e.CreatedBy).HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine).HasColumnType("varchar(25)");

                entity.Property(e => e.DeviationWorkCenter).HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy).HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine).HasColumnType("varchar(25)");

                entity.Property(e => e.RecStatus).HasColumnType("int(11)");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.Teamopp)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK_teamopp_1");
            });

            modelBuilder.Entity<TRelatednodes>(entity =>
            {
                entity.HasKey(e => e.WfdepidLink);

                entity.ToTable("t_relatednodes");

                entity.HasIndex(e => e.Depid)
                    .HasName("FK_DEPId_idx");

                entity.HasIndex(e => e.TeamId)
                    .HasName("FK_TeamID_idx");

                entity.Property(e => e.WfdepidLink).HasColumnName("WFDEPIdLink");

                entity.Property(e => e.Depdesc)
                    .IsRequired()
                    .HasColumnName("DEPDesc")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Depid).HasColumnName("DEPId");

                entity.Property(e => e.TeamName).HasColumnType("varchar(45)");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.TRelatednodes)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamID");
            });

            modelBuilder.Entity<Wf>(entity =>
            {
                entity.ToTable("wf");

                entity.HasIndex(e => e.WfidRef)
                    .HasName("Index_RefId");

                entity.HasIndex(e => new { e.GroupCode, e.Sbucode, e.FacCode })
                    .HasName("FK_wf_Fac");

                entity.Property(e => e.Wfid).HasColumnName("WFId");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.DclstructNo)
                    .HasColumnName("DCLStructNo")
                    .HasColumnType("int(3)");

                entity.Property(e => e.FacCode)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.GroupCode)
                    .IsRequired()
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.L1id)
                    .HasColumnName("L1Id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.Sbucode)
                    .IsRequired()
                    .HasColumnName("SBUCode")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Wfdesc)
                    .IsRequired()
                    .HasColumnName("WFDesc")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.WfidRef).HasColumnName("WFIdRef");

                entity.Property(e => e.Wsstatus).HasColumnName("WSStatus");

                entity.HasOne(d => d.Factory)
                    .WithMany(p => p.Wf)
                    .HasForeignKey(d => new { d.GroupCode, d.Sbucode, d.FacCode })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_wf_Fac");
            });

            modelBuilder.Entity<Wfdep>(entity =>
            {
                entity.HasKey(e => e.WfdepinstId);

                entity.ToTable("wfdep");

                entity.HasIndex(e => e.Depid)
                    .HasName("FK_wfdep_DEP");

                entity.HasIndex(e => e.TeamId)
                    .HasName("FK_Team_idx");

                entity.HasIndex(e => e.Wfid)
                    .HasName("Index_WFId");

                entity.Property(e => e.WfdepinstId).HasColumnName("WFDEPInstId");

                entity.Property(e => e.Bccheck)
                    .HasColumnName("BCCheck")
                    .HasColumnType("int(2)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Bqsplit).HasColumnName("BQSplit");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.Dclid)
                    .HasColumnName("DCLId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Dcmid)
                    .HasColumnName("DCMId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Depid).HasColumnName("DEPId");

                entity.Property(e => e.ExOpCode).HasColumnType("int(5)");

                entity.Property(e => e.InheritDepid)
                    .HasColumnName("InheritDEPId")
                    .HasColumnType("int(5)");

                entity.Property(e => e.L1id).HasColumnName("L1Id");

                entity.Property(e => e.LimitDclid)
                    .HasColumnName("LimitDCLId")
                    .HasColumnType("int(10)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.LimitWithLevel)
                    .HasColumnType("int(10)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.LimitWithPredDclid)
                    .HasColumnName("LimitWithPredDCLId")
                    .HasColumnType("int(10)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.LimitWithPredScrapDclid)
                    .HasColumnName("LimitWithPredScrapDCLId")
                    .HasColumnType("int(10)");

                entity.Property(e => e.LimitWithWf)
                    .HasColumnName("LimitWithWF")
                    .HasColumnType("int(10)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.OperationCode).HasColumnType("int(5)");

                entity.Property(e => e.PredDepid)
                    .HasColumnName("PredDEPId")
                    .HasColumnType("int(10)");

                entity.Property(e => e.ScanCounter).HasColumnType("int(11)");

                entity.Property(e => e.POCounterEnable).HasColumnType("int(11)");

                entity.Property(e => e.ValidateSheduleChange).HasColumnType("int(11)");

                entity.Property(e => e.DataCaptureMode).HasColumnType("int(11)");    

                entity.Property(e => e.POCounterNumber).HasColumnType("int(11)"); 

                entity.Property(e => e.OppValidationQty).HasColumnType("int(11)"); 

                entity.Property(e => e.RejectReasonSelectMode).HasColumnType("int(1)");
                
                entity.Property(e => e.BagDepId).HasColumnName("BagDepId");

                entity.Property(e => e.NextSeqNo).HasColumnType("int(11)");

                entity.Property(e => e.ScanOpMode)
                    .HasColumnType("int(2)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Sname)
                    .HasColumnName("SName")
                    .HasColumnType("varchar(10)")
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.SplitDclid)
                    .HasColumnName("SplitDCLId")
                    .HasColumnType("int(10)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TeamId).HasDefaultValueSql("'0'");

                entity.Property(e => e.Wfdepstatus).HasColumnName("WFDEPStatus");

                entity.Property(e => e.Wfid)
                    .HasColumnName("WFId")
                    .HasColumnType("int(10)");

                entity.Property(e => e.WorkCenter).HasColumnType("varchar(30)");

                entity.HasOne(d => d.Dep)
                    .WithMany(p => p.Wfdep)
                    .HasForeignKey(d => d.Depid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DEP");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.Wfdep)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK_Team");
 
            });

            modelBuilder.Entity<Wfdeplink>(entity =>
            {
                entity.HasKey(e => new { e.WfdepinstId, e.WfdepidLink });

                entity.ToTable("wfdeplink");

                entity.HasIndex(e => e.WfdepidLink)
                    .HasName("Index_2");

                entity.Property(e => e.WfdepinstId).HasColumnName("WFDEPInstId");

                entity.Property(e => e.WfdepidLink).HasColumnName("WFDEPIdLink");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.LinkType).HasDefaultValueSql("'1'");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.HasOne(d => d.Wfdepinst)
                    .WithMany(p => p.Wfdeplink)
                    .HasForeignKey(d => d.WfdepinstId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_wfdeplink_WFDEP");
            });

            modelBuilder.Entity<Wfdepmulqty>(entity =>
            {
                entity.HasKey(e => new { e.WfdepinstId, e.Seq });

                entity.ToTable("wfdepmulqty");

                entity.Property(e => e.WfdepinstId).HasColumnName("WFDEPInstId");

                entity.Property(e => e.Cf).HasColumnName("CF");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.LabelName)
                    .IsRequired()
                    .HasColumnType("varchar(15)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.HasOne(d => d.Wfdepinst)
                    .WithMany(p => p.Wfdepmulqty)
                    .HasForeignKey(d => d.WfdepinstId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_wfdepmulqty_WFDEP");
            });

            modelBuilder.Entity<ZWfdepmatrix>(entity =>
            {
                entity.ToTable("z_wfdepmatrix");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ColNo).HasColumnType("int(11)");

                entity.Property(e => e.ColType)
                    .IsRequired()
                    .HasColumnType("varchar(5)");

                entity.Property(e => e.Depid)
                    .HasColumnName("DEPId")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InsStatus)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.RowNo).HasColumnType("int(11)");

                entity.Property(e => e.TeamId).HasColumnType("int(11)");

                entity.Property(e => e.WfdepinstId)
                    .HasColumnName("WFDEPInstId")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.WfdepinstIdDisplay)
                    .HasColumnName("WFDEPInstId_Display")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Wfid)
                    .HasColumnName("WFId")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<ZWfteam>(entity =>
            {
                entity.HasKey(e => new { e.TeamId, e.Wfid });

                entity.ToTable("z_wfteam");

                entity.Property(e => e.TeamId).ValueGeneratedOnAdd();

                entity.Property(e => e.Wfid)
                    .HasColumnName("WFId")
                    .HasColumnType("int(10)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
            });
        
            modelBuilder.Entity<Washes>(entity =>
            {
                 entity.HasKey(e => new { e.WashCatId });

                entity.ToTable("washes");

                entity.Property(e => e.WashCatId)
                    .HasColumnName("WashCatId")
                    .HasColumnType("int(10)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.WashName)
                    .IsRequired()
                    .HasColumnName("WashName")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.WashType)
                    .HasColumnName("WashType")
                    .HasColumnType("int(2)");
            });
        
            modelBuilder.Entity<SFprocess>(entity =>
            {
                entity.HasKey(e => new { e.SFCatId });

                entity.ToTable("sfprocess");

                entity.Property(e => e.SFCatId)
                    .HasColumnName("SFCatId")
                    .HasColumnType("int(10)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.SFName)
                    .IsRequired()
                    .HasColumnName("SFName")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.SFType)
                    .HasColumnName("SFType")
                    .HasColumnType("int(2)");
            });

            modelBuilder.Entity<InvoiceParameter>(entity =>
            {
                entity.HasKey(e => new { e.InvoiceKey });

                entity.ToTable("invoice_parameter");

                entity.HasIndex(e => new { e.InvoiceKey, e.NextInvoiceNo })
                    .HasName("INDEX_InvoiceKey_NextInvoiceNo");

                entity.HasIndex(e => new { e.RecStatus })
                    .HasName("INDEX_RecStatus");

                entity.Property(e => e.InvoiceKey)
                    .HasColumnType("int(11)");

                entity.Property(e => e.NextInvoiceNo)
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.VAT)
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.NBT)
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.ExchangeRate)
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.RecStatus).HasColumnType("int(1)");

                entity.Property(e => e.EffectiveDateFrom).HasColumnType("datetime");

                entity.Property(e => e.EffectiveDateTo).HasColumnType("datetime");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
            });

            modelBuilder.Entity<InvoiceDetails>(entity =>
            {
                entity.HasKey(e => new { e.InvoiceDetailKey });

                entity.ToTable("invoice_details");

                entity.HasIndex(e => new { e.InvoiceNo, e.ControlId, e.ControlType })
                    .HasName("INDEX_InvoiceNo_ControlId");

                entity.HasIndex(e => new { e.InvoiceDetailKey, e.Seq })
                    .HasName("INDEX_Seq");

                entity.HasIndex(e => new { e.InvoiceSeq })
                    .HasName("INDEX_InvoiceSeq");

                entity.Property(e => e.InvoiceDetailKey)
                    .HasColumnType("int(11)");

                entity.Property(e => e.InvoiceSeq)
                    .HasColumnType("int(10)");

                entity.Property(e => e.InvoiceNo)
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ControlId)
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ControlType)
                    .HasColumnType("int(10)");
                
                entity.Property(e => e.Seq)
                    .HasColumnType("int(10)");

                entity.Property(e => e.WFId)
                    .HasColumnType("int(11)");

                entity.Property(e => e.L1Id)
                    .HasColumnType("int(10)");

                entity.Property(e => e.L2Id)
                    .HasColumnType("int(10)");

                entity.Property(e => e.L3Id)
                    .HasColumnType("int(10)");

                entity.Property(e => e.L4Id)
                    .HasColumnType("int(10)");

                entity.Property(e => e.L5Id)
                    .HasColumnType("int(10)");

                entity.Property(e => e.BarcodeNo)
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.TxnDateTime).HasColumnType("datetime");

                entity.Property(e => e.Qty01)
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty02)
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Qty03)
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Remark).HasColumnType("varchar(45)");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
            });

            modelBuilder.Entity<InvoiceHeaderInformation>(entity =>
            {
                entity.HasKey(e => new { e.InvoiceNo });

                entity.ToTable("invoice_header_information");

                entity.Property(e => e.InvoiceNo)
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.TxnDateTime)
                    .HasColumnType("datetime");

                entity.Property(e => e.VAT)
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.NBT)
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.ExchangeRate)
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.TotalQty)
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.TotalPrice)
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.TotalPriceIncludingVAT)
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.CreatedDateTime)
                    .HasColumnType("datetime");

                entity.Property(e => e.CreatedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedMachine)
                    .IsRequired()
                    .HasColumnType("varchar(25)");
            });
        }
    }
}
