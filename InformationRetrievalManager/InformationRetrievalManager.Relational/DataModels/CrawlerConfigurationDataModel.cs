using InformationRetrievalManager.Core;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// The data model representing crawler configuration.
    /// </summary>
    [ValidableModel(typeof(CrawlerConfigurationDataModel))]
    public class CrawlerConfigurationDataModel
    {
        #region Limit Constants

        public static readonly bool SiteAddress_IsRequired = true;
        public static readonly ushort SiteAddress_MaxLength = 350;
        public static readonly string SiteAddress_CanContainRegex = @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)";
        public static readonly string SiteAddress_DefaultValue = "";

        public static readonly bool SiteSuffix_IsRequired = true;
        public static readonly ushort SiteSuffix_MaxLength = 999;
        public static readonly string SiteSuffix_CanContainRegex = @"(\{0\})+";
        public static readonly string SiteSuffix_DefaultValue = "";

        public static readonly int StartPageNo_MinValue = 0;
        public static readonly int StartPageNo_MaxValue = int.MaxValue;
        public static readonly int StartPageNo_DefaultValue = 1;

        public static readonly int MaxPageNo_MinValue = 1;
        public static readonly int MaxPageNo_MaxValue = int.MaxValue;
        public static readonly int MaxPageNo_DefaultValue = 10;

        public static readonly int PageNoModifier_MinValue = 1;
        public static readonly int PageNoModifier_MaxValue = int.MaxValue;
        public static readonly int PageNoModifier_DefaultValue = PageNoModifier_MinValue;

        public static readonly int SearchInterval_MinValue = 500;
        public static readonly int SearchInterval_MaxValue = 3_600_000; // 1 hour
        public static readonly int SearchInterval_DefaultValue = 1000;

        public static readonly bool SiteUrlArticlesXPath_IsRequired = true;
        public static readonly ushort SiteUrlArticlesXPath_MaxLength = 255;
        public static readonly string SiteUrlArticlesXPath_DefaultValue = "";

        public static readonly bool SiteArticleContentAreaXPath_IsRequired = true;
        public static readonly ushort SiteArticleContentAreaXPath_MaxLength = 255;
        public static readonly string SiteArticleContentAreaXPath_DefaultValue = "";

        public static readonly bool SiteArticleTitleXPath_IsRequired = true;
        public static readonly ushort SiteArticleTitleXPath_MaxLength = 999;
        public static readonly string SiteArticleTitleXPath_DefaultValue = "";

        public static readonly bool SiteArticleCategoryXPath_IsRequired = false;
        public static readonly ushort SiteArticleCategoryXPath_MaxLength = 255;
        public static readonly string SiteArticleCategoryXPath_DefaultValue = "";

        public static readonly bool SiteArticleDateTimeXPath_IsRequired = false;
        public static readonly ushort SiteArticleDateTimeXPath_MaxLength = 255;
        public static readonly string SiteArticleDateTimeXPath_DefaultValue = "";

        public static readonly bool SiteArticleDateTimeFormat_IsRequired = false;
        public static readonly ushort SiteArticleDateTimeFormat_MaxLength = 50;
        public static readonly string SiteArticleDateTimeFormat_DefaultValue = "yyyy-MM-dd HH:mm";

        public static readonly bool SiteArticleDateTimeCultureInfo_IsRequired = false;
        public static readonly ushort SiteArticleDateTimeCultureInfo_MaxLength = 10;
        public static readonly string SiteArticleDateTimeCultureInfo_DefaultValue = "en-US";

        #endregion

        #region Properties (Keys / Relations)

        /// <summary>
        /// Primary Key
        /// </summary>
        [ValidateIgnore]
        public long Id { get; set; }

        /// <summary>
        /// Foreign Key for <see cref="DataInstance"/>
        /// </summary>
        [ValidateIgnore]
        public long DataInstanceId { get; set; }

        /// <summary>
        /// Reference to ONE data instance // Fluent API
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [ValidateIgnore]
        public DataInstanceDataModel DataInstance { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// The base size address
        /// </summary>
        [ValidateString(nameof(SiteAddress), typeof(CrawlerConfigurationDataModel),
            pIsRequired: nameof(SiteAddress_IsRequired),
            pMaxLength: nameof(SiteAddress_MaxLength),
            pCanContainRegex: nameof(SiteAddress_CanContainRegex))]
        public string SiteAddress { get; set; }

        /// <summary>
        /// The suffix of <see cref="SiteAddress"/> address for more specific selection of page articles
        /// </summary>
        [ValidateString(nameof(SiteSuffix), typeof(CrawlerConfigurationDataModel),
            pIsRequired: nameof(SiteSuffix_IsRequired),
            pMaxLength: nameof(SiteSuffix_MaxLength),
            pCanContainRegex: nameof(SiteSuffix_CanContainRegex))]
        public string SiteSuffix { get; set; }

        /// <summary>
        /// Start page number for crawling
        /// </summary>
        [ValidateInteger(nameof(StartPageNo), typeof(CrawlerConfigurationDataModel),
            pMinValue: nameof(StartPageNo_MinValue),
            pMaxValue: nameof(StartPageNo_MaxValue))]
        public int StartPageNo { get; set; }

        /// <summary>
        /// Maximal page number for crawling
        /// </summary>
        [ValidateInteger(nameof(MaxPageNo), typeof(CrawlerConfigurationDataModel),
            pMinValue: nameof(MaxPageNo_MinValue),
            pMaxValue: nameof(MaxPageNo_MaxValue))]
        public int MaxPageNo { get; set; }

        /// <summary>
        /// Page number modifier value
        /// </summary>
        [ValidateInteger(nameof(PageNoModifier), typeof(CrawlerConfigurationDataModel),
            pMinValue: nameof(PageNoModifier_MinValue),
            pMaxValue: nameof(PageNoModifier_MaxValue))]
        public int PageNoModifier { get; set; }

        /// <summary>
        /// Delay between search tasks (ms)
        /// </summary>
        [ValidateInteger(nameof(SearchInterval), typeof(CrawlerConfigurationDataModel),
            pMinValue: nameof(SearchInterval_MinValue),
            pMaxValue: nameof(SearchInterval_MaxValue))]
        public int SearchInterval { get; set; }

        /// <summary>
        /// XPath to select URLs of articles
        /// </summary>
        [ValidateString(nameof(SiteUrlArticlesXPath), typeof(CrawlerConfigurationDataModel),
            pIsRequired: nameof(SiteUrlArticlesXPath_IsRequired),
            pMaxLength: nameof(SiteUrlArticlesXPath_MaxLength))]
        public string SiteUrlArticlesXPath { get; set; }

        /// <summary>
        /// XPath to content of the article
        /// </summary>
        [ValidateString(nameof(SiteArticleContentAreaXPath), typeof(CrawlerConfigurationDataModel),
            pIsRequired: nameof(SiteArticleContentAreaXPath_IsRequired),
            pMaxLength: nameof(SiteArticleContentAreaXPath_MaxLength))]
        public string SiteArticleContentAreaXPath { get; set; }

        /// <summary>
        /// XPath to title of the article
        /// </summary>
        [ValidateString(nameof(SiteArticleTitleXPath), typeof(CrawlerConfigurationDataModel),
            pIsRequired: nameof(SiteArticleTitleXPath_IsRequired),
            pMaxLength: nameof(SiteArticleTitleXPath_MaxLength))]
        public string SiteArticleTitleXPath { get; set; }

        /// <summary>
        /// XPath to category of the article
        /// </summary>
        [ValidateString(nameof(SiteArticleCategoryXPath), typeof(CrawlerConfigurationDataModel),
            pIsRequired: nameof(SiteArticleCategoryXPath_IsRequired),
            pMaxLength: nameof(SiteArticleCategoryXPath_MaxLength))]
        public string SiteArticleCategoryXPath { get; set; }

        /// <summary>
        /// XPath to date-time of the article
        /// </summary>
        [ValidateString(nameof(SiteArticleDateTimeXPath), typeof(CrawlerConfigurationDataModel),
            pIsRequired: nameof(SiteArticleDateTimeXPath_IsRequired),
            pMaxLength: nameof(SiteArticleDateTimeXPath_MaxLength))]
        public string SiteArticleDateTimeXPath { get; set; }

        /// <summary>
        /// Date-time related format (<see cref="SiteArticleDateTimeXPath"/>)
        /// </summary>
        [ValidateString(nameof(SiteArticleDateTimeFormat), typeof(CrawlerConfigurationDataModel),
            pIsRequired: nameof(SiteArticleDateTimeFormat_IsRequired),
            pMaxLength: nameof(SiteArticleDateTimeFormat_MaxLength))]
        public string SiteArticleDateTimeFormat { get; set; }

        /// <summary>
        /// Date-time related culture info (<see cref="SiteArticleDateTimeXPath"/>)
        /// </summary>
        [ValidateString(nameof(SiteArticleDateTimeCultureInfo), typeof(CrawlerConfigurationDataModel),
            pIsRequired: nameof(SiteArticleDateTimeCultureInfo_IsRequired),
            pMaxLength: nameof(SiteArticleDateTimeCultureInfo_MaxLength))]
        public string SiteArticleDateTimeCultureInfo { get; set; }

        #endregion
    }
}
