namespace DAL.DBModels
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class SvarbotDbSys : DbContext
    {
        public SvarbotDbSys()
            : base("name=SvarbotDbSys")
        {
        }

        public virtual DbSet<Accounts> Accounts { get; set; }
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<CategoryType> CategoryType { get; set; }
        public virtual DbSet<ClickCount> ClickCount { get; set; }
        public virtual DbSet<Favorites> Favorites { get; set; }
        public virtual DbSet<Form> Form { get; set; }
        public virtual DbSet<Instruks_Veiledning> Instruks_Veiledning { get; set; }
        public virtual DbSet<Langinstruks> Langinstruks { get; set; }
        public virtual DbSet<Superuser> Superuser { get; set; }
        public virtual DbSet<Undercategory> Undercategory { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accounts>()
                .Property(e => e.navn)
                .IsUnicode(false);

            modelBuilder.Entity<Accounts>()
                .HasMany(e => e.Favorites)
                .WithRequired(e => e.Accounts)
                .HasForeignKey(e => e.Brukernavn)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Accounts>()
                .HasMany(e => e.Form)
                .WithRequired(e => e.Accounts)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Categories>()
                .Property(e => e.Category_name)
                .IsUnicode(false);

            modelBuilder.Entity<Categories>()
                .HasMany(e => e.Undercategory)
                .WithRequired(e => e.Categories)
                .HasForeignKey(e => e.Category_Id);

            modelBuilder.Entity<Categories>()
                .HasMany(e => e.Form)
                .WithRequired(e => e.Categories)
                .HasForeignKey(e => e.CategoryId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CategoryType>()
                .Property(e => e.TypeName)
                .IsUnicode(false);

            modelBuilder.Entity<CategoryType>()
                .HasMany(e => e.Categories)
                .WithRequired(e => e.CategoryType)
                .HasForeignKey(e => e.Category_type_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ClickCount>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<Favorites>()
                .Property(e => e.Brukernavn)
                .IsUnicode(false);

            modelBuilder.Entity<Form>()
                .Property(e => e.Navn)
                .IsUnicode(false);

            modelBuilder.Entity<Form>()
                .Property(e => e.SBNummer)
                .IsFixedLength();

            modelBuilder.Entity<Form>()
                .Property(e => e.AntallBerort)
                .IsFixedLength();

            modelBuilder.Entity<Instruks_Veiledning>()
                .Property(e => e.Inskruks_beskrivelse)
                .IsUnicode(false);

            modelBuilder.Entity<Instruks_Veiledning>()
                .Property(e => e.Instruks_URL)
                .IsUnicode(false);

            modelBuilder.Entity<Instruks_Veiledning>()
                .HasMany(e => e.Undercategory)
                .WithRequired(e => e.Instruks_Veiledning)
                .HasForeignKey(e => e.Instruks_Veiledning_Id);

            modelBuilder.Entity<Langinstruks>()
                .Property(e => e.tekst)
                .IsUnicode(false);

            modelBuilder.Entity<Langinstruks>()
                .HasMany(e => e.Undercategory)
                .WithOptional(e => e.Langinstruks)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Superuser>()
                .Property(e => e.Username)
                .IsFixedLength();

            modelBuilder.Entity<Undercategory>()
                .Property(e => e.Undercategory_name)
                .IsUnicode(false);
        }
    }
}
