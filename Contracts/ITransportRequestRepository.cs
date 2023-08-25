using Entities.Models;
using Entities.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ITransportRequestRepository : IRepositoryBase<TransportRequest>
    {
        PagedList<TransportRequest> GetAllTransportRequests([Optional] TransportRequestParameters transportRequestParameters);
        TransportRequest GetTransportRequestById(Guid idTransportRequest);

        void CreateTransportRequest(TransportRequest transportRequest);
        void UpdateTransportRequest(TransportRequest transportRequest);
        void DeleteTransportRequest(TransportRequest transportRequest);
    }
}
