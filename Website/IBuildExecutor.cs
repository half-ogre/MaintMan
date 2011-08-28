using System;

namespace MaintMan
{
    public interface IBuildExecutor
    {
        void Execute(
            Uri buildUrl,
            string message = null);
    }
}