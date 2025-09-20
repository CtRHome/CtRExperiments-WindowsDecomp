using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutTheRope.iframework.core;

internal partial class Preferences
{

    private static partial Stream CreateFile(string name)
    {
        return File.Create(name);
    }

    private static partial Stream OpenFile(string name, FileMode mode)
    {
        return File.Open(name, mode);
    }

    private static partial void DeleteFile(string name)
    {
        File.Delete(name);
    }

    private static partial void MoveFile(string from, string to)
    {
        File.Move(from, to);
    }

    private static partial bool FileExists(string name)
    {
        return File.Exists(name);
    }

}