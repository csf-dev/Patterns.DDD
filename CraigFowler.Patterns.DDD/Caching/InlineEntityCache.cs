//  
//  InlineEntityCache.cs
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

namespace CraigFowler.Patterns.DDD.Caching
{
  /// <summary>
  /// <para>
  /// An <see cref="EntityCacheBase"/> that executes a <see cref="PerformCleanup"/> operation any time that the number
  /// of items contained within the cache reaches a <see cref="MaximumEntityCount"/> threshold.
  /// </para>
  /// </summary>
  public class InlineEntityCache : EntityCacheBase
  {
    #region constants
    
    private const int DEFAULT_MAX_ENTITIES = 1000;
    
    #endregion
    
    #region fields
    
    private int _maximumEntityCount;
    
    #endregion
    
    #region properties
    
    /// <summary>
    /// <para>
    /// Gets and sets the threshold for the maximum number of entities that this cache instance may contain.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// When this property is set, if the new limit brings this instance over the threshold, a cleanup operation is
    /// performed immediately.
    /// </para>
    /// </remarks>
    public int MaximumEntityCount
    {
      get {
        return _maximumEntityCount;
      }
      set {
        if(value < 1)
        {
          throw new ArgumentOutOfRangeException("value", "Maximum cached entities may not be less than one.");
        }
        
        try
        {
          this.SyncRoot.EnterWriteLock();
          _maximumEntityCount = value;
          if(this.EntityThresholdExceeded())
          {
            this.LockedCleanup();
          }
        }
        finally
        {
          if(this.SyncRoot.IsWriteLockHeld)
          {
            this.SyncRoot.ExitWriteLock();
          }
        }
      }
    }
    
    #endregion
    
    #region methods
    
    /// <summary>
    /// <para>Determines whether or not the threshold of entities stored in this cache has been exceeded or not.</para>
    /// </summary>
    /// <returns>
    /// A <see cref="System.Boolean"/>
    /// </returns>
    private bool EntityThresholdExceeded()
    {
      if(!this.SyncRoot.IsWriteLockHeld)
      {
        throw new InvalidOperationException("Cannot check the entity threshold unless an appropriate lock is held.");
      }
      
      return ((this.BackingStore != null)? this.BackingStore.Count : 0) >= this.MaximumEntityCount;
    }
    
    /// <summary>
    /// <para>Event handler for the addition of new items to this cache instance.</para>
    /// </summary>
    /// <param name="sender">
    /// A <see cref="System.Object"/>
    /// </param>
    /// <param name="ev">
    /// A <see cref="EventArgs"/>
    /// </param>
    private void HandleItemAdded(object sender, EventArgs ev)
    {
      CachingEventArgs args = ev as CachingEventArgs;
      
      if(Object.ReferenceEquals(sender, this)
         && args != null
         && args.EventType == CachingEventType.ItemAdded
         && this.EntityThresholdExceeded())
      {
        this.LockedCleanup();
      }
    }
    
    #endregion
    
    #region constructors
    
    /// <summary>
    /// <para>Initialises this instance.</para>
    /// </summary>
    /// <param name="policy">
    /// A <see cref="IReplacementPolicy"/>
    /// </param>
    /// <param name="backingStore">
    /// A <see cref="ICacheBackingStore"/>
    /// </param>
    public InlineEntityCache (IReplacementPolicy policy,
                              ICacheBackingStore backingStore) : this(policy, backingStore, null) {}
    
    /// <summary>
    /// <para>Initialises this instance, including a default item policy.</para>
    /// </summary>
    /// <param name="policy">
    /// A <see cref="IReplacementPolicy"/>
    /// </param>
    /// <param name="backingStore">
    /// A <see cref="ICacheBackingStore"/>
    /// </param>
    /// <param name="defaultItemPolicy">
    /// A <see cref="IItemPolicy"/>
    /// </param>
    public InlineEntityCache (IReplacementPolicy policy,
                              ICacheBackingStore backingStore,
                              IItemPolicy defaultItemPolicy) : base(policy, backingStore, defaultItemPolicy)
    {
      this.ItemAdded += HandleItemAdded;
    }
    
    #endregion
  }
}

