using System;
using System.Collections.Generic;
using System.Text;

namespace EbookApp
{
    public interface IXamHelper
    {
        string PDTtoText(string filename);
        string WordToText(string filename);

    }
}
