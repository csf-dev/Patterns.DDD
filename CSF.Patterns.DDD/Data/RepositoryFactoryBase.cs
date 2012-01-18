//  
//  RepositoryFactoryBase.cs
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
using System.Collections.Generic;
using System.Reflection;
using CSF.Patterns.DDD.Data;
using CSF.Patterns.DDD.Entities;

namespace CSF.Patterns.DDD.Data
{
  /// <summary>
  /// <para>Base class for <see cref="IRepositoryFactory"/> implementations.</para>
  /// </summary>
  public abstract class RepositoryFactoryBase : IRepositoryFactory
  {
    #region constants
    
    /* HACK: C# forces us to stringly-type the name of the repository because it is generic.
     * Is there no better way to do this?
     */
    private const string GET_REPOSITORY_METHOD_NAME = "GetRepository";
    
    #endregion
    
    #region fields
    
    /// <summary>
    /// <para>Simple object to provide locking.</para>
    /// </summary>
    private object _syncRoot = new object();
    
    #endregion
    
    #region properties
    
    /// <summary>
    /// <para>
    /// Read-only.  Gets a cache of the mapping between <see cref="System.Type"/>s and the corresponding generic
    /// <see cref="System.Reflection.MethodInfo"/> that is used to create repositories.
    /// </para>
    /// </summary>
    protected Dictionary<Type, MethodInfo> GenericMethodMappingCache
    {
      get;
      private set;
    }
    
    #endregion
    
    #region IRepositoryFactory implementation
    
    /// <summary>
    /// <para>Creates a new repository connection.</para>
    /// </summary>
    /// <returns>
    /// A <see cref="IRepositoryConnection"/>
    /// </returns>
    public abstract IRepositoryConnection GetConnection();

    /// <summary>
    /// <para>Overloaded.  Gets the repository for a specific <paramref name="entityType"/>.</para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method provides a way of side-stepping into the generic <c>GetRepository</c> method, based on a
    /// <see cref="System.Type"/>.  It uses reflection and a lightweight caching mechanism to discover the appropriate
    /// generic version of the GetRepository method to use and then invokes that method.
    /// </para>
    /// </remarks>
    /// <param name="entityType">
    /// A <see cref="System.Type"/> that implements <see cref="IEntity"/>
    /// </param>
    /// <param name="connection">
    /// A <see cref="IRepositoryConnection"/>
    /// </param>
    /// <returns>
    /// An <see cref="IRepository"/> suitable for working with instances of the <paramref name="entityType"/>
    /// </returns>
    public virtual IRepository GetRepository (Type entityType, IRepositoryConnection connection)
    {
      if(entityType == null)
      {
        throw new ArgumentNullException("entityType");
      }
      
      if(!this.GenericMethodMappingCache.ContainsKey(entityType))
      {
        /* I'm performing this check inside the conditional to ensure that reflection is used as little as possible in
         * a 'cache hit' scenario - where the repository type has been seen before.
         */
        if(entityType.GetInterface(typeof(IEntity).FullName) == null)
        {
          throw new ArgumentException("The type must implement IEntity", "entityType");
        }
        
        lock(_syncRoot)
        {
          MethodInfo method = typeof(RepositoryFactoryBase).GetMethod(GET_REPOSITORY_METHOD_NAME,
                                                                      new Type[] { typeof(IRepositoryConnection) });
          
          /* We can drop ArgumentException on the floor because all it means is that the key was already added.  It
           * would indicate a highly-unlikely scenario where two threads get past the .ContainsKey check (above) at
           * the same time and then both try adding the new item.
           * 
           * This is not problematic in itself, since in this scenario both threads are adding exactly the same data.
           */
          try
          {
            this.GenericMethodMappingCache.Add(entityType, method.MakeGenericMethod(entityType));
          }
          catch(ArgumentException) {}
        }
      }
      
      return (IRepository) this.GenericMethodMappingCache[entityType].Invoke(this, new object[] { connection });
    }

    /// <summary>
    /// <para>Overloaded.  Gets the repository for a specific entity type.</para>
    /// </summary>
    /// <param name="connection">
    /// A <see cref="IRepositoryConnection"/>
    /// </param>
    /// <returns>
    /// An <see cref="IRepository"/>
    /// </returns>
    public abstract IRepository<TEntity> GetRepository<TEntity> (IRepositoryConnection connection) where TEntity : IEntity;
    
    #endregion
    
    #region constructor
    
    /// <summary>
    /// <para>Initialises this instance.</para>
    /// </summary>
    public RepositoryFactoryBase()
    {
      this.GenericMethodMappingCache = new Dictionary<Type, MethodInfo>();
    }
    
    #endregion
  }
}

