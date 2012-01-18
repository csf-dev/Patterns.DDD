//  
//  DatabaseRepositoryFactory.cs
//  
//  Author:
//       Craig Fowler <craig@craigfowler.me.uk>
// 
//  Copyright (c) 2012 CSF Software Limited
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Configuration;

namespace CSF.Patterns.DDD.Data.Database
{
  /// <summary>
  /// <para>Base class for repository factories that work from a database.</para>
  /// </summary>
  public abstract class DatabaseRepositoryFactory : RepositoryFactoryBase
  {
    #region fields
    
    private string _connectionStringName;
    
    #endregion
    
    #region properties
    
    /// <summary>
    /// <para>Gets and sets the name of the connection string (in the application configuration) to use.</para>
    /// </summary>
    public virtual string ConnectionStringName
    {
      get {
        return _connectionStringName;
      }
      set {
        _connectionStringName = value;
        
        if(!String.IsNullOrEmpty(value)
           && String.IsNullOrEmpty(this.ConnectionString)
           && String.IsNullOrEmpty(this.ProviderNamespace))
        {
          try
          {
            this.PopulateFromConfiguredConnectionString();
          }
          // If there is no connection string by this name then this exception might occur, in which case do nothing
          catch(NullReferenceException) {}
        }
      }
    }
    
    /// <summary>
    /// <para>Read-only.  Gets the connection string that will be used to connect to the database provider.</para>
    /// </summary>
    public virtual string ConnectionString
    {
      get;
      set;
    }
    
    /// <summary>
    /// <para>
    /// Read-only.  Gets the namespace where a <see cref="Type"/> implementing <see cref="System.Data.IDbConnection"/>
    /// (that will act as the database backend) is located.
    /// </para>
    /// </summary>
    public virtual string ProviderNamespace
    {
      get;
      set;
    }
    
    #endregion
    
    #region methods
    
    /// <summary>
    /// Populates (and overwrites) <see cref="ConnectionString"/> and <see cref="ProviderNamespace"/> with data found
    /// within the application configuration's connectionstrings property, using <see cref="ConnectionStringName"/>.
    /// </summary>
    protected void PopulateFromConfiguredConnectionString()
    {
      if(String.IsNullOrEmpty(this.ConnectionStringName))
      {
        throw new InvalidOperationException("No connection string name has been provided.");
      }
      
      ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[this.ConnectionStringName];
      this.ConnectionString = settings.ConnectionString;
      this.ProviderNamespace = settings.ProviderName;
    }
    
    #endregion
    
    #region IRepositoryFactory implementation
    
    /// <summary>
    /// <para>Creates a new repository connection.</para>
    /// </summary>
    /// <returns>
    /// A <see cref="IRepositoryConnection"/>
    /// </returns>
    public override IRepositoryConnection GetConnection ()
    {
      if(String.IsNullOrEmpty(this.ConnectionString) || String.IsNullOrEmpty(this.ProviderNamespace))
      {
        this.PopulateFromConfiguredConnectionString();
      }
      
      return new DatabaseRepositoryConnection(this.ConnectionString, this.ProviderNamespace, this);
    }
    
    #endregion
    
    #region constructor
    
    /// <summary>
    /// <para>Initialises this instance.</para>
    /// </summary>
    public DatabaseRepositoryFactory () : base() {}
    
    #endregion
  }
}

