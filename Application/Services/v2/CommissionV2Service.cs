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
using Infrastracture.Repositories.v2;
using System.Data;

namespace Application.Services.v2
{
    public class CommissionV2Service : IDisposable
    {
        private readonly CommissionV2Repository _repo;

        public CommissionV2Service()
        {
            _repo = new();
        }

        public CommissionV2Service(string connectionString)
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


        public async Task<DataTable> GetDailyCommSummary(CommissionRequest model)
        {
            try
            {
                DataTable result = await _repo.GetDailyCommSummary(model);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetDailyCommSummary"));
            }
        }


        public async Task<DataTable> GetDailyCommissionDetails(CommissionRequest model)
        {
            try
            {
                DataTable result = await _repo.GetDailyCommissionDetails(model);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetDailyCommDetails"));
            }
        }


        public async Task<DataTable> GetSalesVsCommission(SearchRequestV2 model)
        {
            try
            {
                DataTable result = await _repo.GetSalesVsCommission(model);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSalesVsCommission"));
            }
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