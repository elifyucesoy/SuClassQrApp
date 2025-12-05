using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuClassQrApp.Business
{
    public class QrService : IQrService
    {
        public string GenerateQrToken()
        {
            return Guid.NewGuid().ToString("N") + DateTime.UtcNow.Ticks.ToString("x");
        }

        public DateTime GetQrExpiry()
        {
            return DateTime.UtcNow.AddMinutes(15); // QR 15 dakika geçerli
        }

        public bool IsQrValid(string token, DateTime expiry)
        {
            return !string.IsNullOrEmpty(token) && expiry > DateTime.UtcNow;
        }
    }
}
