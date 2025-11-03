using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZIMAeTicket.Services
{
    public partial class SoteshopService
    {

        public SoteshopService()
        {
            _dbPath = Constants.DatabasePath;

            try
            {
                //TODO połączenie z bazą Soteshop
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed initializing database connection: {ex.Message}");
            }
        }
    }
}
