//  
//  CachingEventArgs.cs
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
using CraigFowler.Patterns.DDD.Entities;

namespace CraigFowler.Patterns.DDD.Caching
{
  /// <summary>
  /// <para>A <see cref="EventArgs"/> type for describing cache-related events.</para>
  /// </summary>
  public class CachingEventArgs : EventArgs
  {
    #region fields
    
    private CachingEventType _eventType;
    
    #endregion
    
    #region properties
    
    /// <summary>
    /// <para>Read-only.  Gets the the type of event that this instance describes.</para>
    /// </summary>
    public CachingEventType EventType
    {
      get {
        return _eventType;
      }
      private set {
        if(!Enum.IsDefined(typeof(CachingEventType), value))
        {
          throw new ArgumentOutOfRangeException("value");
        }
        
        _eventType = value;
      }
    }
    
    /// <summary>
    /// <para>Gets and sets the entity identity related to this event.</para>
    /// </summary>
    public IIdentity EntityIdentity
    {
      get;
      set;
    }
    
    #endregion
    
    #region constructors
    
    /// <summary>
    /// <para>Initialises this instance.</para>
    /// </summary>
    /// <param name="type">
    /// A <see cref="CachingEventType"/>
    /// </param>
    public CachingEventArgs (CachingEventType type)
    {
      this.EventType = type;
    }
    
    #endregion
  }
}

