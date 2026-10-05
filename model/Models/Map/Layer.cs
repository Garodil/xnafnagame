using System;

namespace model.Models.Map
{
    public abstract class Layer : IDisposable
    {
        protected bool _disposedValue;
        protected abstract void Dispose(bool disposing);

        ~Layer()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}