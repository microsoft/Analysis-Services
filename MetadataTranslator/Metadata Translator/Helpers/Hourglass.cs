using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Metadata_Translator
{
    public class Hourglass : IDisposable
    {
        private Cursor previousCursor;

        public Hourglass()
        {
            previousCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;
        }

        public void Dispose()
        {
            Mouse.OverrideCursor = previousCursor;
        }
    }
}