//  
//  DatabaseRepositoryConnection.cs
//  
//  Author:
//       Craig Fowler <craig@craigfowler.me.uk>
// 
//  Copyright (c) 2011 Craig Fowler
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
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace CraigFowler.Patterns.DDD.Data.Database
{
  /// <summary>
  /// <para>
  /// A <see cref="IRepositoryConnection"/> that makes use of a connection string to connect to a database.
  /// </para>
  /// </summary>
  public class DatabaseRepositoryConnection : RepositoryConnectionBase
  {
    #region fields
    
    private static Dictionary<string, Type> _connectionProviders;
    
    #endregion
    
    #region properties
    
    /// <summary>
    /// <para>Read-only.  Gets the connection string that is used to create the database connection.</para>
    /// </summary>
    public virtual string ConnectionString
    {
      get;
      set;
    }
    
    /// <summary>
    /// <para>
    /// Read-only.  Gets the fully-qualified namespace that contains exactly one <see cref="Type"/> that implements
    /// <see cref="IDbConnection"/>.
    /// </para>
    /// </summary>
    public virtual string ProviderNamespace
    {
      get;
      set;
    }
    
    /// <summary>
    /// <para>Read-only.  Gets the underlying connection to the database backend.</para>
    /// </summary>
    public virtual IDbConnection Connection
    {
      get;
      private set;
    }
    
    #endregion
    
    #region methods
    
    /// <summary>
    /// <para>
    /// Overloaded.  Gets the appropriate <see cref="Type"/> that implements <see cref="IDbConnection"/> corresponding
    /// to the <see cref="ProviderNamespace"/> for this instance.
    /// </para>
    /// </summary>
    /// <returns>
    /// A <see cref="Type"/>
    /// </returns>
    protected virtual Type GetConnectionBackendType()
    {
      Type output;
      
      if(_connectionProviders.ContainsKey(this.ProviderNamespace))
      {
        output = _connectionProviders[this.ProviderNamespace];
        
        if(output == null)
        {
          string message = String.Format("Database provider namespace '{0}' has a null provider type registered.",
                                         this.ProviderNamespace);
          throw new InvalidOperationException(message);
        }
      }
      else
      {
        output = GetConnectionBackendType(this.ProviderNamespace);
        
        if(output == null)
        {
          string message = String.Format("Unsupported or unrecognised database provider namespace: '{0}'.  Is a " +
                                         "required assembly missing or not loaded?",
                                         this.ProviderNamespace);
          throw new NotSupportedException(message);
        }
        
        _connectionProviders[this.ProviderNamespace] = output;
      }
      
      return output;
    }
    
    /// <summary>
    /// <para>Establishes the database connection for this instance.</para>
    /// </summary>
    /// <returns>
    /// A <see cref="IDbConnection"/>
    /// </returns>
    protected virtual IDbConnection EstablishConnection()
    {
      Type provider = this.GetConnectionBackendType();
      ConstructorInfo constructor = provider.GetConstructor(new Type[] { typeof(string) });
      
      if(constructor == null)
      {
        string message = String.Format("The provider type '{0}' does not contain an appropriate constructor for a " +
                                       "connection string.",
                                       provider.FullName);
        throw new NotSupportedException(message);
      }
      
      return (IDbConnection) constructor.Invoke(new object[] { this.ConnectionString });
    }
    
    #endregion
    
    #region IRepositoryConnection implementation
    
    /// <summary>
    /// <para>Read-only.  Gets whether or not this repository connection type supports transactions.</para>
    /// </summary>
    public override bool HasTransactionSupport
    {
      get {
        return true;
      }
    }

    /// <summary>
    /// <para>Creates and returns a new <see cref="IRepositoryTransaction"/>.</para>
    /// </summary>
    /// <returns>
    /// An <see cref="IRepositoryTransaction"/>
    /// </returns>
    public override IRepositoryTransaction CreateTransaction()
    {
      if(!this.HasTransactionSupport)
      {
        throw new NotSupportedException("This connection does not support transactions.");
      }
      
      return new DatabaseRepositoryTransaction(this.Connection.BeginTransaction());
    }
    
    #endregion

    #region IDisposable implementation
    
    /// <summary>
    /// <para>Overridden.  Performs the actual disposal tasks relating to this instance.</para>
    /// </summary>
    protected override void PerformDisposal()
    {
      this.Connection.Dispose();
      base.PerformDisposal();
    }
    
    #endregion
    
    #region constructors
    
    /// <summary>
    /// <para>Initialises this instance with a connection string and a provider namespace.</para>
    /// </summary>
    /// <param name="connectionString">
    /// A <see cref="System.String"/>
    /// </param>
    /// <param name="providerNamespace">
    /// A <see cref="System.String"/>
    /// </param>
    /// <param name="parent">
    /// A <see cref="IRepositoryFactory"/>
    /// </param>
    public DatabaseRepositoryConnection(string connectionString,
                                        string providerNamespace,
                                        IRepositoryFactory parent) : base(parent)
    {
      this.ConnectionString = connectionString;
      this.ProviderNamespace = providerNamespace;
      
      this.Connection = this.EstablishConnection();
    }
    
    /// <summary>
    /// <para>Performs static initialisation of this type.</para>
    /// </summary>
    static DatabaseRepositoryConnection()
    {
      _connectionProviders = new Dictionary<string, Type>();
    }
    
    /// <summary>
    /// <para>Implicit destructor for this type, only used if disposal is forgotten.</para>
    /// </summary>
    ~DatabaseRepositoryConnection()
    {
      this.Dispose();
    }
    
    #endregion
    
    #region static methods
    
    /// <summary>
    /// <para>
    /// Overloaded.  Searches the <see cref="AppDomain.CurrentDomain"/> for a type that is in the given
    /// <paramref name="providerNamespace"/> and implements <see cref="IDbConnection"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method performs a quite computationally-expensive, brute force/linear search through every
    /// <see cref="System.Type"/> that is available in every loaded <see cref="Assembly"/> in
    /// <see cref="AppDomain.CurrentDomain"/>.  Whilst it will terminate as soon as a matching type is found, the
    /// result should ideally be cached in a way which will persist for the duration of the application's lifecycle.
    /// Except in scenarios in which the application is frequently loading and unloading assemblies to/from the
    /// appdomain, the return value from this method will not change for the lifespan of the application.
    /// </para>
    /// </remarks>
    /// <param name="providerNamespace">
    /// A <see cref="System.String"/> indicating a fully-qualified namespace for the type that implements
    /// <see cref="IDbConnection"/>
    /// </param>
    /// <returns>
    /// A <see cref="Type"/>
    /// </returns>
    public static Type GetConnectionBackendType(string providerNamespace)
    {
      Type output = null;
      Assembly[] allLoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
      
      foreach(Assembly assembly in allLoadedAssemblies)
      {
        Type[] containedTypes = assembly.GetExportedTypes();
        
        foreach(Type type in containedTypes)
        {
          if(type.Namespace == providerNamespace
             && type.GetInterface(typeof(IDbConnection).FullName) != null)
          {
            output = type;
            break;
          }
        }
        
        if(output != null)
        {
          break;
        }
      }
      
      return output;
    }
    
    #endregion
  }
}

