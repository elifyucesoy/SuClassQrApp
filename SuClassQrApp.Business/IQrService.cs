using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuClassQrApp.Business
{
    public interface IQrService
    {
        string GenerateQrToken();
        DateTime GetQrExpiry();
        bool IsQrValid(string token, DateTime expiry);
    }
}
