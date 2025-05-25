
///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	All Commission related service
///	Creation Date :	14-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.Helpers;
using Domain.RequestModel;
using Infrastracture.Repositories;
using System.Data;

namespace Application.Services
{
    public class CommissionService : IDisposable
    {
        private readonly CommissionRepository _repo;

        public CommissionService()
        {
            _repo = new();
        }

        public CommissionService(string connectionString)
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


        public Task<DataTable> GetDailyCommSummary(CommissionRequest model)
        {
            try
            {
                Task<DataTable> result = _repo.GetDailyCommSummary(model);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetDailyCommSummary"));
            }
        }


        public Task<DataTable> GetDailyCommDetails(CommissionRequest model)
        {
            try
            {
                Task<DataTable> result = _repo.GetDailyCommDetails(model);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetDailyCommDetails"));
            }
        }


        public async Task<DataTable> GetSalesVsCommission(SearchRequestV2 model)
        {
            DataTable result = await _repo.GetSalesVsCommission(model);
            return result;
        }


        public async Task<DataTable> StatementSummary(SearchRequest model, DateTime fd, DateTime td)
        {
            try
            {
                DataTable result = await _repo.StatementSummary(model, fd, td);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "StatementSummary"));
            }
        }


        public async Task<DataTable> StatementDetails(SearchRequest model, DateTime fd, DateTime td)
        {
            try
            {
                DataTable result = await _repo.StatementDetails(model, fd, td);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "StatementDetails"));
            }
        }


        public async Task<DataTable> TarVsAchvSummary(RetailerRequestV2 retailerRequest)
        {
            return await _repo.TarVsAchvSummary(retailerRequest);
        }


        public async Task<DataTable> TarVsAchvDeatils(TarVsAchvRequestV2 tarVsAchvRequest)
        {
            return await _repo.TarVsAchvDeatils(tarVsAchvRequest);
        }

    }
}