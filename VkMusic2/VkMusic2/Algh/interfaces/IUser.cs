using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Algh.interfaces
{
    public interface IUser
    {
        Task<bool> Login(string name, string pass);

        Task<string> GetMobileVersion();

        bool Login();

        void Logout();
        IData Data { get; }

        bool IsConnected { get;}
    }
}
