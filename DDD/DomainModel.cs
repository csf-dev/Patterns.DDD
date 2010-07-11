
using System;
using System.Reflection;
using System.Windows.Forms;

namespace CraigFowler.Patterns.DDD
{
  public abstract class DomainModel : IDisposable
  {
    #region static fields
    
    private static IConfigurationRoot configurationCache;
    private static DateTime cacheTimestamp;
    private static TimeSpan cacheTTL;
    
    #endregion
    
    #region fields
    
    private RepositoryContainer repository;
    private IConfigurationRoot configuration;
    
    #endregion
    
    #region properties
    
    protected RepositoryContainer Repository
    {
      get {
        return repository;
      }
      private set {
        if(value == null)
        {
          throw new ArgumentNullException("value");
        }
        
        repository = value;
      }
    }
    
    protected IConfigurationRoot Configuration
    {
      get {
        return configuration;
      }
      private set {
        if(value == null)
        {
          throw new ArgumentNullException("value");
        }
        
        configuration = value;
      }
    }
    
    public bool Disposed
    {
      get {
        return this.Repository.Disposed;
      }
    }
    
    #endregion
    
    #region methods
    
    public void Dispose()
    {
      if(this.Disposed)
      {
        throw new ObjectDisposedException("Repository");
      }
      
      this.Repository.Dispose();
    }
    
    protected IConfigurationRoot GetConfiguration()
    {
      DateTime expiry;
      
      if(ConfigurationCacheTTL == TimeSpan.Zero)
      {
        expiry = DateTime.MaxValue;
      }
      else
      {
        expiry = ConfigurationCacheTimestamp + ConfigurationCacheTTL;
      }
      
      if(ConfigurationCache == null || expiry < DateTime.Now)
      {
        ConfigurationCache = ReadConfiguration();
        ConfigurationCacheTimestamp = DateTime.Now;
      }
      
      return ConfigurationCache;
    }
    
    protected abstract RepositoryContainer InitialiseRepository(IRepositoryConfiguration config);
    
    protected abstract IConfigurationRoot ReadConfiguration();
    
    #endregion
    
    #region constructors and destructor
    
    public DomainModel()
    {
      this.Configuration = GetConfiguration();
      this.Repository = InitialiseRepository(this.Configuration.Repository);
    }
    
    ~DomainModel()
    {
      if(!this.Disposed)
      {
        this.Dispose();
      }
    }
    
    static DomainModel()
    {
      ConfigurationCache = null;
      ConfigurationCacheTimestamp = DateTime.MinValue;
      ConfigurationCacheTTL = TimeSpan.Zero;
    }
    
    #endregion
    
    #region static properties
    
    public static TimeSpan ConfigurationCacheTTL
    {
      get {
        return cacheTTL;
      }
      set {
        cacheTTL = value;
      }
    }
    
    protected static IConfigurationRoot ConfigurationCache
    {
      get {
        return configurationCache;
      }
      private set {
        configurationCache = value;
      }
    }
    
    protected static DateTime ConfigurationCacheTimestamp
    {
      get {
        return cacheTimestamp;
      }
      set {
        cacheTimestamp = value;
      }
    }
    
    public static Version Version
    {
      get {
        return Assembly.GetExecutingAssembly().GetName().Version;
      }
    }
    
    public static string VersionName
    {
      get {
        string number, info;
        
        number = Version.ToString();
        info = Application.ProductVersion;
        
        return String.Format("{0}: {1}", info, number);
      }
    }
    
    #endregion
  }
}
