
using System;

namespace CraigFowler.Patterns.DDD
{
  public interface IConfigurationRoot
  {
    IRepositoryConfiguration Repository { get; }
  }
}
