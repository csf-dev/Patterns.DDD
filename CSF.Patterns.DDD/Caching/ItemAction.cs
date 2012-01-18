//  
//  ItemAction.cs
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


namespace CSF.Patterns.DDD.Caching
{
  /// <summary>
  /// <para>Enumerates the actions that a policy can take on a cached item.</para>
  /// </summary>
  public enum ItemAction
  {
    /// <summary>
    /// <para>
    /// The default action for an item.  This indicates that the cached item should be removed nor preserved as defined
    /// by a policy further down the line of processing.
    /// </para>
    /// </summary>
    Neutral         = 0,
    
    /// <summary>
    /// <para>Indicates that the item must be retained/preserved within the cache.</para>
    /// </summary>
    Preserve,
    
    /// <summary>
    /// <para>Indicates that the item must be removed from the cache.</para>
    /// </summary>
    Remove
  }
}

