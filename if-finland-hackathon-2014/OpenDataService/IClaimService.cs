using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using OpenDataService.DataManagement;

namespace OpenDataService {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IClaimsService" in both code and config file together.
    [ServiceContract]
    public interface IClaimsService {

        [OperationContract]
        Claim GetClaimById(int id);

        [OperationContract]
        Dictionary<string, int> GetAccidentTypesByCoordinates(Location coords, int range);

        [OperationContract]
        IList<int> GetHourlyStatistics(string claimType);

        [OperationContract]
        IList<int> GetDailyStatistics(string claimType);

        [OperationContract]
        IList<Claim> GetClaimsByCoordinates(Location coords, int range);

        [OperationContract]
        IList<string> GetAddresses();

        [OperationContract]
        void WriteLocations(string address, Location uniLocation);

        [OperationContract]
        IList<Location> GetLocationPacksByData(int skipmultiplier);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    
}
