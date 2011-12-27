//  
//  TestConfiguredDatabaseRepositoryFactory.cs
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
using CraigFowler.Patterns.DDD.Data;
using CraigFowler.Patterns.DDD.Data.Database;
using CraigFowler.Patterns.DDD.Mocks.Data;
using NUnit.Framework;

namespace Test.CraigFowler.Patterns.DDD.Data.Database
{
  [TestFixture]
  [Category("Requires configuration")]
  [Description("The tests in this fixture depend upon a working repository factory configuration containing " +
               "compatible settings.")]
  public class TestConfiguredDatabaseRepositoryFactory
  {
    #region set up
    
    [SetUp]
    public void Setup()
    {
#pragma warning disable 219
      /* Intentionally disable CS219 - we are touching this type in the test setup so that its assembly is definitely
       * loaded into the current AppDomain.  Otherwise the code that we are testing might not be able to find this
       * type.
       */
      Type typeLoader = typeof(global::CraigFowler.Patterns.DDD.Mocks.Data.DummyConfiguredFactory);
#pragma warning restore 219
    }
    
    #endregion
    
    #region tests
    
    [Test]
    public void TestConnectionStringName()
    {
      IRepositoryFactory factory;
      
      Assert.IsNotNull(RepositoryFactories.Factory("Test"), "Testing factory is not null");
      Assert.IsInstanceOfType(typeof(IRepositoryFactory),
                              RepositoryFactories.Factory("Test"),
                              "Implements interface");
      Assert.IsInstanceOfType(typeof(DummyConfiguredFactory),
                              RepositoryFactories.Factory("Test"),
                              "Correct type");
      
      factory = RepositoryFactories.Factory("Test");
      
      Assert.AreEqual("sample",
                      ((ConfiguredDatabaseRepositoryFactory) factory).ConnectionStringName,
                      "Correct connection string name");
      Assert.AreEqual("Server=127.0.0.1;Port=3306;Database=test;User ID=root;Allow User Variables=True",
                      ((ConfiguredDatabaseRepositoryFactory) factory).ConnectionString,
                      "Correct connection string");
    }
    
    #endregion
  }
}

