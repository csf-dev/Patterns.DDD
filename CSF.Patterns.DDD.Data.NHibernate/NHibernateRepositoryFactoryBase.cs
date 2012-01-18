//  
//  NHibernateRepositoryFactory.cs
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
using System.IO;
using System.Text;
using CSF.Patterns.DDD.Data.Database;
using CSF.Patterns.DDD.Entities;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace CSF.Patterns.DDD.Data.NHibernate
{
  /// <summary>
  /// <para>Provides a base class for NHibernate-based repository factories.</para>
  /// </summary>
  /// <remarks>
  /// <para>
  /// The main application-specific part of this type (that must be implemented by subclasses) is
  /// <see cref="GetConfiguration"/>.  This method gets configuration information that will be specific to the
  /// application.
  /// </para>
  /// </remarks>
  public abstract class NHibernateRepositoryFactoryBase : DatabaseRepositoryFactory
  {
    #region fields
    
    private ISessionFactory _cachedSessionFactory;
    
    #endregion
    
    #region methods
    
    /// <summary>
    /// <para>Overridden.  Creates an <see cref="IRepositoryConnection"/> from this factory.</para>
    /// </summary>
    /// <returns>
    /// A <see cref="IRepositoryConnection"/>
    /// </returns>
    public override IRepositoryConnection GetConnection ()
    {
      return new NHibernateRepositoryConnection(this.GetSessionFactory().OpenSession(), this);
    }
    
    /// <summary>
    /// <para>Gets the appropriate <see cref="IRepository"/> for a generic <see cref="IEntity"/> type.</para>
    /// </summary>
    /// <param name="connection">
    /// A <see cref="IRepositoryConnection"/>
    /// </param>
    /// <returns>
    /// An <see cref="IRepository"/>
    /// </returns>
    public override IRepository<TEntity> GetRepository<TEntity> (IRepositoryConnection connection)
    {
      return new NHibernateRepositoryBase<TEntity>((NHibernateRepositoryConnection) connection);
    }
    
    /// <summary>
    /// <para>
    /// Overloaded.  Makes use of <see cref="GetConfiguration"/> to export/generate a database schema for the object
    /// model that the application is configured with.  This overload writes that schema to a string.
    /// </para>
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/>
    /// </returns>
    public string ExportSchema()
    {
      StringBuilder builder = new StringBuilder();
      
      using(TextWriter writer = new StringWriter(builder))
      {
        this.ExportSchema(writer);
      }
      
      return builder.ToString();
    }
    
    /// <summary>
    /// <para>
    /// Overloaded.  Makes use of <see cref="GetConfiguration"/> to export/generate a database schema for the object
    /// model that the application is configured with.  This overload writes that schema to a file, found at
    /// <paramref name="filePath"/>.
    /// </para>
    /// </summary>
    /// <param name="filePath">
    /// A <see cref="System.String"/>
    /// </param>
    public void ExportSchema(string filePath)
    {
      using(TextWriter writer = new StreamWriter(filePath))
      {
        this.ExportSchema(writer);
      }
    }
    
    /// <summary>
    /// <para>
    /// Overloaded.  Makes use of <see cref="GetConfiguration"/> to export/generate a database schema for the object
    /// model that the application is configured with.  This overload writes that schema to a <see cref="TextWriter"/>.
    /// </para>
    /// </summary>
    /// <param name="writer">
    /// A <see cref="TextWriter"/>
    /// </param>
    public virtual void ExportSchema(TextWriter writer)
    {
      SchemaExport exporter = new SchemaExport(this.GetConfiguration()).SetDelimiter(";");
      exporter.Execute(false, false, false, null, writer);
    }
    
    /// <summary>
    /// <para>
    /// Gets an <see cref="NHibernate.ISessionFactory"/>, creating it (using <see cref="GetConfiguration"/>)
    /// if required.
    /// </para>
    /// </summary>
    /// <returns>
    /// A <see cref="ISessionFactory"/>
    /// </returns>
    protected ISessionFactory GetSessionFactory()
    {
      if(_cachedSessionFactory == null)
      {
        _cachedSessionFactory = this.GetConfiguration().BuildSessionFactory();
      }
      
      return _cachedSessionFactory;
    }
    
    /// <summary>
    /// <para>
    /// Gets a fully-populated (ready for use) <see cref="NHibernate.Cfg.Configuration"/> specific to this repository
    /// actory.
    /// </para>
    /// </summary>
    /// <returns>
    /// A <see cref="Configuration"/>
    /// </returns>
    protected abstract Configuration GetConfiguration();
    
    #endregion
  }
}

