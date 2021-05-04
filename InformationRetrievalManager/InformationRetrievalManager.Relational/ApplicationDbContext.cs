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
        /// Indexed document file references
        /// </summary>
        public DbSet<IndexedFileReferenceDataModel> IndexedFileReferences { get; set; }

        /// <summary>
        /// Indexed documents
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
            // Set up limits
            modelBuilder.Entity<DataInstanceDataModel>().Property(o => o.Name).HasMaxLength(DataInstanceDataModel.Name_MaxLength);

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
            // Set up limits
            modelBuilder.Entity<CrawlerConfigurationDataModel>().Property(o => o.SiteAddress).HasMaxLength(CrawlerConfigurationDataModel.SiteAddress_MaxLength);
            modelBuilder.Entity<CrawlerConfigurationDataModel>().Property(o => o.SiteSuffix).HasMaxLength(CrawlerConfigurationDataModel.SiteSuffix_MaxLength);
            modelBuilder.Entity<CrawlerConfigurationDataModel>().Property(o => o.SiteUrlArticlesXPath).HasMaxLength(CrawlerConfigurationDataModel.SiteUrlArticlesXPath_MaxLength);
            modelBuilder.Entity<CrawlerConfigurationDataModel>().Property(o => o.SiteArticleContentAreaXPath).HasMaxLength(CrawlerConfigurationDataModel.SiteArticleContentAreaXPath_MaxLength);
            modelBuilder.Entity<CrawlerConfigurationDataModel>().Property(o => o.SiteArticleTitleXPath).HasMaxLength(CrawlerConfigurationDataModel.SiteArticleTitleXPath_MaxLength);
            modelBuilder.Entity<CrawlerConfigurationDataModel>().Property(o => o.SiteArticleCategoryXPath).HasMaxLength(CrawlerConfigurationDataModel.SiteArticleCategoryXPath_MaxLength);
            modelBuilder.Entity<CrawlerConfigurationDataModel>().Property(o => o.SiteArticleDateTimeXPath).HasMaxLength(CrawlerConfigurationDataModel.SiteArticleDateTimeXPath_MaxLength);
            modelBuilder.Entity<CrawlerConfigurationDataModel>().Property(o => o.SiteArticleDateTimeFormat).HasMaxLength(CrawlerConfigurationDataModel.SiteArticleDateTimeFormat_MaxLength);
            modelBuilder.Entity<CrawlerConfigurationDataModel>().Property(o => o.SiteArticleDateTimeCultureInfo).HasMaxLength(CrawlerConfigurationDataModel.SiteArticleDateTimeCultureInfo_MaxLength);

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
            // Set up limits
            modelBuilder.Entity<IndexProcessingConfigurationDataModel>().Property(o => o.CustomRegex).HasMaxLength(IndexProcessingConfigurationDataModel.CustomRegex_MaxLength);

            // Configure Indexed File References
            // ------------------------------
            //
            // Set Id as primary key
            modelBuilder.Entity<IndexedFileReferenceDataModel>().HasKey(a => a.Id);
            // Set Foreign Key
            modelBuilder.Entity<IndexedFileReferenceDataModel>()
                .HasOne(o => o.DataInstance)
                .WithMany(o => o.IndexedFileReferences)
                .HasForeignKey(o => o.DataInstanceId)
                .OnDelete(DeleteBehavior.Cascade);
            // Set up constraints
            modelBuilder.Entity<IndexedFileReferenceDataModel>().HasIndex(o => o.Timestamp).IsUnique();

            // Configure Indexed Documents
            // ------------------------------
            //
            // Set Id as primary key
            modelBuilder.Entity<IndexedDocumentDataModel>().HasKey(a => a.Id);
            // Set Foreign Key
            modelBuilder.Entity<IndexedDocumentDataModel>()
                .HasOne(o => o.IndexedFileReference)
                .WithMany(o => o.IndexedDocuments)
                .HasForeignKey(o => o.IndexedFileReferenceId)
                .OnDelete(DeleteBehavior.Cascade);
            // Set up limits
            modelBuilder.Entity<IndexedDocumentDataModel>().Property(o => o.Title).HasMaxLength(IndexedDocumentDataModel.Title_MaxLength);
            modelBuilder.Entity<IndexedDocumentDataModel>().Property(o => o.Category).HasMaxLength(IndexedDocumentDataModel.Category_MaxLength);
            modelBuilder.Entity<IndexedDocumentDataModel>().Property(o => o.SourceUrl).HasMaxLength(IndexedDocumentDataModel.SourceUrl_MaxLength);
            modelBuilder.Entity<IndexedDocumentDataModel>().Property(o => o.Content).HasMaxLength(IndexedDocumentDataModel.Content_MaxLength);
        }

        #endregion
    }
}
