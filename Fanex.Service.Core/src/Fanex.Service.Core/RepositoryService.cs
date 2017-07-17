using System;

namespace Fanex.Service.Core
{
    public interface RepositoryService
    {
        DateTime QueryLastDate();

        bool IsDataAvailable();
    }
}