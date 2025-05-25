///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	Sales Controller
///	Creation Date :	09-Jan-2024
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
    public class SalesV2Service : IDisposable
    {
        private readonly SalesV2Repository _repo;

        public SalesV2Service()
        {
            _repo = new();
        }

        public SalesV2Service(string connectionString)
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


        public Task<DataTable> GetSalesUpdate(RetailerRequest retailerRequest)
        {
            try
            {
                Task<DataTable> result = _repo.GetSalesUpdate(retailerRequest);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSalesUpdate"));
            }
        }


        public Task<DataTable> GetTodaySalesMemo(RetailerRequest retailerRequest)
        {
            try
            {
                Task<DataTable> result = _repo.GetTodaySalesMemo(retailerRequest);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetTodaySalesMemo"));
            }
        }


        public async Task<DataTable> GetSalesWeeklyTrend(RetailerRequest retailerRequest)
        {
            try
            {
                DataTable result = await _repo.GetSalesWeeklyTrend(retailerRequest);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSalesWeeklyTrend"));
            }
        }


        public async Task<DataTable> GetSalesRoutePerformance(RetailerRequest retailerRequest)
        {
            try
            {
                DataTable result = await _repo.GetSalesRoutePerformance(retailerRequest);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSalesRoutePerformance"));
            }
        }


        public async Task<DataTable> GetThreeDaysSalesMemo(RetailerRequest retailerRequest)
        {
            try
            {
                DataTable result = await _repo.GetThreeDaysSalesMemo(retailerRequest);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetThreeDaysSalesMemo"));
            }
        }


        public DataTable GetSalesSummaryData(RetailerRequest retailer)
        {
            DataTable result = _repo.GetSalesSummaryData(retailer);
            return result;
        }


        public async Task<DataTable> GetSalesDetails(SalesDetailRequest salesDetails)
        {
            DataTable result = await _repo.GetSalesDetails(salesDetails);
            return result;
        }

    }
}