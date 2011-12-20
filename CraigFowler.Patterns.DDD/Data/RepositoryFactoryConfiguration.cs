//  
//  RepositoryFactoryConfiguration.cs
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
using System.Configuration;
using System.Reflection;

namespace CraigFowler.Patterns.DDD.Data
{
  /// <summary>
  /// <para>Represents the configuration for all of the repository factories available to the application.</para>
  /// </summary>
  public class RepositoryFactoryConfiguration : ConfigurationSection
  {
    #region properties

    /// <summary>
    /// <para>
    /// Gets and sets a collection of <see cref="FactoryConfiguration"/> that contain the
    /// configurations for the various factories.
    /// </para>
    /// </summary>
    [ConfigurationProperty("factories", IsDefaultCollection = false, IsRequired = true)]
    public FactoryConfigurationCollection FactoryConfigurations {
      get {
        return (FactoryConfigurationCollection) this["factories"];
      }
      set {
        this["factories"] = value;
      }
    }

    #endregion

    #region methods

    /// <summary>
    /// <para>Gets a collection of <see cref="IRepositoryFactory"/> from the current instance.</para>
    /// </summary>
    /// <returns>
    /// A collection of <see cref="IRepositoryFactory"/>
    /// </returns>
    public Dictionary<string, IRepositoryFactory> GetAllFactories ()
    {
      Dictionary<string, IRepositoryFactory> output = new Dictionary<string, IRepositoryFactory>();
      
      foreach(FactoryConfiguration configuration in this.FactoryConfigurations)
      {
        output.Add(configuration.Name, configuration.GetFactory());
      }
      
      return output;
    }

    #endregion

    #region contained types

    /// <summary>
    /// <para>Represents the collection of <see cref="FactoryConfiguration"/></para>
    /// </summary>
    public class FactoryConfigurationCollection : ConfigurationElementCollection
    {
      /// <summary>
      /// <para>Initialises this instance</para>
      /// </summary>
      public FactoryConfigurationCollection () {}

      /// <summary>
      /// <para>Overridden. Creates a new <see cref="FactoryConfiguration"/>.</para>
      /// </summary>
      /// <returns>
      /// A <see cref="ConfigurationElement"/>
      /// </returns>
      protected override ConfigurationElement CreateNewElement ()
      {
        return new FactoryConfiguration();
      }

      /// <summary>
      /// <para>Overridden. Creates a new <see cref="FactoryConfiguration"/>.</para>
      /// </summary>
      /// <param name="elementName">
      /// A <see cref="System.String"/>
      /// </param>
      /// <returns>
      /// A <see cref="ConfigurationElement"/>
      /// </returns>
      protected override ConfigurationElement CreateNewElement (string elementName)
      {
        return new FactoryConfiguration();
      }

      /// <summary>
      /// <para>
      /// Gets the <see cref="FactoryConfiguration.Name"/> for the given
      /// <see cref="FactoryConfiguration"/>.
      /// </para>
      /// </summary>
      /// <param name="element">
      /// A <see cref="ConfigurationElement"/>
      /// </param>
      /// <returns>
      /// A <see cref="Object"/>
      /// </returns>
      protected override Object GetElementKey (ConfigurationElement element)
      {
        FactoryConfiguration typedElement = element as FactoryConfiguration;
        
        if(typedElement == null)
        {
          throw new ArgumentException("The configuration element is an incorrect type.", "element");
        }
        
        return typedElement.Name;
      }
      
      /// <summary>
      /// <para>Read-only.  Gets the type of configuration collection this the current instance represents.</para>
      /// </summary>
      public override ConfigurationElementCollectionType CollectionType
      {
        get {
          return ConfigurationElementCollectionType.AddRemoveClearMap;
        }
      }
      
      /// <summary>
      /// <para>
      /// Indexer gets and sets a <see cref="FactoryConfiguration"/> at the given <paramref name="index"/>.
      /// </para>
      /// </summary>
      /// <param name="index">
      /// A <see cref="System.Int32"/>
      /// </param>
      public FactoryConfiguration this[int index]
      {
        get {
          return (FactoryConfiguration) BaseGet (index);
        }
        set {
          if(BaseGet(index) != null)
          {
            BaseRemoveAt(index);
          }
          BaseAdd(index, value);
        }
      }

      /// <summary>
      /// <para>Read-only.  Indexer gets a <see cref="FactoryConfiguration"/> of the given name.</para>
      /// </summary>
      /// <param name="Name">
      /// A <see cref="System.String"/>
      /// </param>
      public new FactoryConfiguration this[string Name]
      {
        get {
          return (FactoryConfiguration) BaseGet(Name);
        }
      }
    }
    
    /// <summary>
    /// <para>
    /// Configuration element that describes a configuration that is capable of creating a single
    /// <see cref="IRepositoryFactory"/>.
    /// </para>
    /// </summary>
    public class FactoryConfiguration : ConfigurationElement
    {
      /// <summary>
      /// <para>Gets and sets the name of the repository factory.</para>
      /// </summary>
      [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
      public string Name {
        get {
          return (string) this["name"];
        }
        set {
          this["name"] = value;
        }
      }

      /// <summary>
      /// <para>Gets and sets the fully qualified name of the <see cref="Type"/> for the repository factory.</para>
      /// </summary>
      /// <remarks>
      /// <para>This type must implement <see cref="IRepositoryFactory"/>.</para>
      /// </remarks>
      [ConfigurationProperty("type", IsRequired = true)]
      public string TypeName {
        get {
          return (string) this["type"];
        }
        set {
          this["type"] = value;
        }
      }

      /// <summary>
      /// <para>Gets and sets a collection of <see cref="FactoryPropertyConfiguration"/>.</para>
      /// </summary>
      [ConfigurationProperty("settings", IsRequired = true, IsDefaultCollection = false)]
      public FactoryPropertyConfigurationCollection Settings {
        get {
          return (FactoryPropertyConfigurationCollection) this["settings"];
        }
        set {
          this["settings"] = value;
        }
      }

      /// <summary>
      /// <para>Creates a single repository factory</para>
      /// </summary>
      /// <returns>
      /// A <see cref="IRepositoryFactory"/>
      /// </returns>
      public IRepositoryFactory GetFactory ()
      {
        IRepositoryFactory output;
        Type factoryType = null;
        ConstructorInfo constructor;
        
        foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
          factoryType = assembly.GetType(this.TypeName);
          if(factoryType != null)
          {
            break;
          }
        }
        
        if(factoryType == null)
        {
          throw new TypeLoadException(String.Format("Could not load the type '{0}'.", this.TypeName));
        }
        
        if(factoryType.GetInterface(typeof(IRepositoryFactory).FullName) == null)
        {
          throw new InvalidOperationException(String.Format("The type '{0}' does not implement IRepositoryFactory.",
                                                            factoryType.FullName));
        }
        
        constructor = factoryType.GetConstructor(new Type[0]);
        if(constructor == null)
        {
          throw new InvalidOperationException(String.Format("The type '{0}' does not have a parameterless constructor",
                                                            factoryType.FullName));
        }
        
        output = (IRepositoryFactory) constructor.Invoke(null);
        if(output == null)
        {
          throw new InvalidOperationException("Output is null.");
        }
        this.PopulateProperties(output);
        return output;
      }
      
      /// <summary>
      /// <para>
      /// Populates all of the properties of the repository factory with values from <see cref="Settings"/>.
      /// </para>
      /// </summary>
      /// <param name="factory">
      /// A <see cref="IRepositoryFactory"/>
      /// </param>
      private void PopulateProperties(IRepositoryFactory factory)
      {
        foreach(FactoryPropertyConfiguration setting in this.Settings)
        {
          MemberInfo[] members = factory.GetType().GetMember(setting.PropertyName);
          if(members.Length == 1)
          {
            MemberInfo member = members[0];
            
            if(member is PropertyInfo)
            {
              ((PropertyInfo) member).GetSetMethod().Invoke(factory, new object[1] { setting.Value });
            }
            else if(member is FieldInfo)
            {
              ((FieldInfo) member).SetValue(factory, new object[1] { setting.Value });
            }
          }
        }
      }
    }

    /// <summary>
    /// <para>Represents the collection of <see cref="FactoryPropertyConfiguration"/></para>
    /// </summary>
    public class FactoryPropertyConfigurationCollection : ConfigurationElementCollection
    {
      /// <summary>
      /// <para>Initialises this instance</para>
      /// </summary>
      public FactoryPropertyConfigurationCollection () {}

      /// <summary>
      /// <para>Overridden. Creates a new <see cref="FactoryPropertyConfiguration"/>.</para>
      /// </summary>
      /// <returns>
      /// A <see cref="ConfigurationElement"/>
      /// </returns>
      protected override ConfigurationElement CreateNewElement ()
      {
        return new FactoryPropertyConfiguration();
      }

      /// <summary>
      /// <para>Overridden. Creates a new <see cref="FactoryPropertyConfiguration"/>.</para>
      /// </summary>
      /// <param name="elementName">
      /// A <see cref="System.String"/>
      /// </param>
      /// <returns>
      /// A <see cref="ConfigurationElement"/>
      /// </returns>
      protected override ConfigurationElement CreateNewElement (string elementName)
      {
        return new FactoryPropertyConfiguration(elementName);
      }

      /// <summary>
      /// <para>
      /// Gets the <see cref="FactoryPropertyConfiguration.PropertyName"/> for the given
      /// <see cref="FactoryPropertyConfiguration"/>.
      /// </para>
      /// </summary>
      /// <param name="element">
      /// A <see cref="ConfigurationElement"/>
      /// </param>
      /// <returns>
      /// A <see cref="Object"/>
      /// </returns>
      protected override Object GetElementKey (ConfigurationElement element)
      {
        FactoryPropertyConfiguration typedElement = element as FactoryPropertyConfiguration;
        
        if(typedElement == null)
        {
          throw new ArgumentException("The configuration element is an incorrect type.", "element");
        }
        
        return typedElement.PropertyName;
      }
      
      /// <summary>
      /// <para>Read-only.  Gets the type of configuration collection this the current instance represents.</para>
      /// </summary>
      public override ConfigurationElementCollectionType CollectionType
      {
        get {
          return ConfigurationElementCollectionType.AddRemoveClearMap;
        }
      }
      
      /// <summary>
      /// <para>
      /// Indexer gets and sets a <see cref="FactoryPropertyConfiguration"/> at the given <paramref name="index"/>.
      /// </para>
      /// </summary>
      /// <param name="index">
      /// A <see cref="System.Int32"/>
      /// </param>
      public FactoryPropertyConfiguration this[int index]
      {
        get {
          return (FactoryPropertyConfiguration) BaseGet (index);
        }
        set {
          if(BaseGet(index) != null)
          {
            BaseRemoveAt(index);
          }
          BaseAdd(index, value);
        }
      }

      /// <summary>
      /// <para>Read-only.  Indexer gets a <see cref="FactoryPropertyConfiguration"/> of the given name.</para>
      /// </summary>
      /// <param name="Name">
      /// A <see cref="System.String"/>
      /// </param>
      public new FactoryPropertyConfiguration this[string Name]
      {
        get {
          return (FactoryPropertyConfiguration) BaseGet(Name);
        }
      }
    }

    /// <summary>
    /// <para>
    /// Represents a single <see cref="System.String"/> property configuration for a
    /// <see cref="FactoryConfiguration"/>.
    /// </para>
    /// </summary>
    public class FactoryPropertyConfiguration : ConfigurationElement
    {
      /// <summary>
      /// <para>Gets and sets the name of a property that the <see cref="Value"/> will be stored in.</para>
      /// </summary>
      [ConfigurationProperty("property", IsRequired = true, IsKey = true)]
      public string PropertyName {
        get {
          return (string) this["property"];
        }
        set {
          this["property"] = value;
        }
      }
      
      /// <summary>
      /// <para>
      /// Gets and sets a value that will be stored into the <see cref="PropertyName"/> once the
      /// <see cref="IRepositoryFactory"/> is created.
      /// </para>
      /// </summary>
      [ConfigurationProperty("value", IsRequired = true)]
      public string Value {
        get {
          return (string) this["value"];
        }
        set {
          this["value"] = value;
        }
      }
      
      /// <summary>
      /// <para>Initialises this instance.</para>
      /// </summary>
      public FactoryPropertyConfiguration () {}
      
      /// <summary>
      /// <para>Initialises this instance, dropping an element name parameter on the floor.</para>
      /// </summary>
      /// <param name="elementName">
      /// A <see cref="System.String"/>
      /// </param>
      public FactoryPropertyConfiguration (string elementName) : this() {}
    }
    
    #endregion
  }
}

