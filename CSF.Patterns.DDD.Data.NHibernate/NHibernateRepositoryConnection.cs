//  
//  NHibernateRepositoryConnection.cs
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
using NHibernate;

namespace CSF.Patterns.DDD.Data.NHibernate
{
  /// <summary>
  /// <para>Repository connection that wraps an <see cref="NHibernate.ISession"/>.</para>
  /// </summary>
  public class NHibernateRepositoryConnection : RepositoryConnectionBase
  {
    #region properties
    
    /// <summary>
    /// <para>Read-only.  Gets the underlying NHibernate session object.</para>
    /// </summary>
    public ISession Session
    {
      get;
      private set;
    }
    
    #endregion
    
    #region methods
    
    /// <summary>
    /// <para>Performs disposal-related tasks on this connection.</para>
    /// </summary>
    /// <remarks>
    /// <para>This method flushes and disposes of the underlying <see cref="Session"/>.</para>
    /// </remarks>
    protected override void PerformDisposal ()
    {
      this.Session.Flush();
      this.Session.Dispose();
      base.PerformDisposal();
    }
    
    /// <summary>
    /// <para>Read-only.  Gets whether or not the current instance supports transactions.</para>
    /// </summary>
    public override bool HasTransactionSupport
    {
      get {
        return true;
      }
    }
    
    /// <summary>
    /// <para>
    /// Creates and returns a <see cref="NHibernateRepositoryTransaction"/>, wrapping a native NHibernate transaction.
    /// </para>
    /// </summary>
    /// <returns>
    /// A <see cref="IRepositoryTransaction"/>
    /// </returns>
    public override IRepositoryTransaction CreateTransaction ()
    {
      return new NHibernateRepositoryTransaction(this.Session.BeginTransaction());
    }
    
    #endregion
    
    #region constructor
    
    /// <summary>
    /// <para>Initialises the current instance.</para>
    /// </summary>
    /// <param name="session">
    /// A <see cref="ISession"/>
    /// </param>
    /// <param name="factory">
    /// A <see cref="IRepositoryFactory"/>
    /// </param>
    public NHibernateRepositoryConnection(ISession session, IRepositoryFactory factory) : base(factory)
    {
      this.Session = session;
    }
    
    #endregion
  }
}

