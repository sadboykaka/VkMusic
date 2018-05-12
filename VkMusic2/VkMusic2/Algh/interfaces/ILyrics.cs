using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Algh.interfaces
{
    public interface ILyrics
    {
        Task<string> GetText(IAudio track);
    }
}
