using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// The database context for the application data store
    /// "This is our database representation"
    /// </summary>
    internal sealed class ApplicationDbContext : DbContext
    {
        #region DbSets (Tables)

        /// <summary>
        /// The state data of the application
        /// </summary>
        public DbSet<ApplicationStateDataModel> ApplicationState { get; set; }

        /// <summary>
        /// Data instances made by the user
        /// </summary>
        public DbSet<DataInstanceDataModel> DataInstances { get; set; }

        /// <summary>
        /// Crawler configuration data
        /// </summary>
        public DbSet<CrawlerConfigurationDataModel> CrawlerConfigurations { get; set; }

        /// <summary>
        /// Processing configuration data
        /// </summary>
        public DbSet<IndexProcessingConfigurationDataModel> IndexProcessingConfigurations { get; set; }

        /// <summary>
        /// Indexed document references
        /// </summary>
        public DbSet<IndexedDocumentDataModel> IndexedDocuments { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        #endregion

        #region Model Creating

        /// <summary>
        /// Configures the database structure and relationships
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fluent API

            // Configure App State
            // ------------------------------
            //
            // Set Id as primary key
            modelBuilder.Entity<ApplicationStateDataModel>().HasKey(a => a.Id);

            // Configure Data Instances
            // ------------------------------
            //
            // Set Id as primary key
            modelBuilder.Entity<DataInstanceDataModel>().HasKey(a => a.Id);

            // Configure Crawler Configurations
            // ------------------------------
            //
            // Set Id as primary key
            modelBuilder.Entity<CrawlerConfigurationDataModel>().HasKey(a => a.Id);
            // Set Foreign Key
            modelBuilder.Entity<CrawlerConfigurationDataModel>()
                .HasOne(o => o.DataInstance)
                .WithOne(o => o.CrawlerConfiguration)
                .HasForeignKey<CrawlerConfigurationDataModel>(o => o.DataInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Index Processing Configurations
            // ------------------------------
            //
            // Set Id as primary key
            modelBuilder.Entity<IndexProcessingConfigurationDataModel>().HasKey(a => a.Id);
            // Set Foreign Key
            modelBuilder.Entity<IndexProcessingConfigurationDataModel>()
                .HasOne(o => o.DataInstance)
                .WithOne(o => o.IndexProcessingConfiguration)
                .HasForeignKey<IndexProcessingConfigurationDataModel>(o => o.DataInstanceId)
                .OnDelete(DeleteBehavior.Cascade);
            // Data type conversion management
            modelBuilder.Entity<IndexProcessingConfigurationDataModel>()
                .Property(e => e.CustomStopWords)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, null),
                    v => JsonSerializer.Deserialize<HashSet<string>>(v, null)
                    );
            // Set comparer to CustomStopWords
            var valueComparer = new ValueComparer<HashSet<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => new HashSet<string>(c)
                );
            modelBuilder
                .Entity<IndexProcessingConfigurationDataModel>()
                .Property(e => e.CustomStopWords)
                .Metadata.SetValueComparer(valueComparer);

            // Configure Indexed Documents
            // ------------------------------
            //
            // Set Id as primary key
            modelBuilder.Entity<IndexedDocumentDataModel>().HasKey(a => a.Id);
            // Set Foreign Key
            modelBuilder.Entity<IndexedDocumentDataModel>()
                .HasOne(o => o.DataInstance)
                .WithMany(o => o.IndexedDocuments)
                .HasForeignKey(o => o.DataInstanceId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        #endregion
    }
}
