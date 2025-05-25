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
using Infrastracture.Repositories.v2;
using System.Data;

namespace Application.Services.v2
{
    public class LiftingV2Service : IDisposable
    {
        private readonly LiftingV2Repository _repo;

        public LiftingV2Service()
        {
            _repo = new();
        }

        public LiftingV2Service(string connectionString)
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
                {
                    HttpService httpService = new();
                    return await httpService.CallCheckSimStatusApi(simStatus, url);
                }

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

    }
}