///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	LoginSmartPos Controller
///	Creation Date :	11-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.Helpers;
using Domain.RequestModel;
using Domain.ResponseModel;
using Domain.ViewModel;
using Infrastracture.Repositories;
using Newtonsoft.Json;
using System.Data;

namespace Application.Services
{
    public class LiftingService : IDisposable
    {
        private readonly LiftingRepository _repo;

        public LiftingService()
        {
            _repo = new();
        }

        public LiftingService(string connectionString)
        {
            _repo = new(connectionString);
        }

        #region==========|  Dispose Method  |==========
        private bool isDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                _repo.Dispose();
            }

            isDisposed = true;
        }
        #endregion==========|  Dispose Method  |==========

        public async Task<DataTable> ProductCategory(ProductCategoryRequest categoryRequest)
        {
            try
            {
                DataTable result = await _repo.ProductCategory(categoryRequest);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSalesDetails"));
            }
        }


        public async Task<SimStatusModel> CheckSimStatus(SimStatusRequestModel simStatus, string url)
        {
            try
            {
                if (!string.IsNullOrEmpty(simStatus.msisdn))
                    return await CheckSimStatusByMsisdn(simStatus, url);

                return await _repo.CheckSimStatusByserialNo(simStatus);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSalesDetails"));
            }

        }


        public async Task<long> SaveUsingOracleBulkCopy(DataTable dt)
        {
            return await _repo.SaveUsingOracleBulkCopy(dt);
        }


        public async Task<long> UpdateStockRequisitionDeliveredOrder()
        {
            return await _repo.UpdateStockRequisitionDeliveredOrder();
        }


        public async Task<long> GetExistRequest(LiftingRequest model)
        {
            return await _repo.GetExistRequest(model);
        }


        public async Task<long> SaveLiftingV3(LiftingRequest liftingRequest)
        {
            return await _repo.SaveLiftingV3(liftingRequest);
        }


        public async Task<long> UpdateStockRequestStatus(UpdateLifting model)
        {
            return await _repo.UpdateStockRequestStatus(model);
        }


        public async Task<DataTable> LiftingHistoryV3(HistoryPageRequestModel liftingRequest)
        {
            return await _repo.LiftingHistoryV3(liftingRequest);
        }


        #region==========|| private method ||==========

        private static async Task<SimStatusModel> CheckSimStatusByMsisdn(SimStatusRequestModel simStatus, string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    string status = "not available";
                    HttpResponseMessage response = await client.GetAsync(simStatus.msisdn);
                    var responseString = await response.Content.ReadAsStringAsync();
                    dynamic json = JsonConvert.DeserializeObject<dynamic>(responseString)!;

                    status = (response.IsSuccessStatusCode) ? json.data.attributes.status : json.errors.title;
                    SimStatusModel simStatusModel = new(new DataTable().NewRow()) { isAvailable = false, productName = status };
                    simStatusModel.isAvailable = status.Contains("available") || simStatusModel.isAvailable;
                    return simStatusModel;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "CheckSimStatusByMsisdn"));
            }
        }

        #endregion==========|| private method ||==========

    }
}