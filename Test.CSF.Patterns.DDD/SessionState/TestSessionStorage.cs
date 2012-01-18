//  
//  TestMemorySessionStorage.cs
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
using NUnit.Framework;
using CSF.Patterns.DDD.SessionState;
using CSF.Patterns.DDD.Mocks.Entities;

namespace Test.CSF.Patterns.DDD.SessionState
{
  [TestFixture]
  [Category("Requires configuration")]
  [Description("The tests in this fixture depend upon a working session storage configuration.")]
  public class TestSessionStorage
  {
    #region setup and teardown
    
    [SetUp]
    public void Setup()
    {
      SessionStorage.Current.Abandon();
    }
    
    #endregion
    
    #region tests
    
    [Test]
    public void TestStoreValue()
    {
      SessionStorage.Current.StoreValue("test", new Person(5));
      Assert.AreEqual(new Person(5), SessionStorage.Current.GetValue("test"), "Test value is as expected");
    }
    
    [Test]
    public void TestClearValue()
    {
      SessionStorage.Current.StoreValue("test", new Person(5));
      Assert.AreEqual(new Person(5), SessionStorage.Current.GetValue("test"), "Test value is as expected");
      
      SessionStorage.Current.ClearValue("test");
      Assert.IsNull(SessionStorage.Current.GetValue("test"), "Test value is now null");
    }
    
    [Test]
    public void TestAbandon()
    {
      SessionStorage.Current.StoreValue("test", new Person(5));
      Assert.AreEqual(new Person(5), SessionStorage.Current.GetValue("test"), "Test value is as expected");
      
      SessionStorage.Current.Abandon();
      Assert.IsNull(SessionStorage.Current.GetValue("test"), "Test value is now null");
    }
    
    #endregion
  }
}

