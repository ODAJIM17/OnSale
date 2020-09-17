using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OnSale.Common.Helpers
{
    public interface IFilesHelper
    {
        byte[] ReadFully(Stream input);
    }
}
