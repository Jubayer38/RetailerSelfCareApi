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
using Infrastracture.Repositories;
using System.Data;

namespace Application.Services
{
    public class SalesService : IDisposable
    {
        private readonly SalesRepository _repo;

        public SalesService()
        {
            _repo = new();
        }

        public SalesService(string connectionString)
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


        public Task<DataTable> GetSalesWeeklyTrend(RetailerRequest retailerRequest)
        {
            try
            {
                Task<DataTable> result = _repo.GetSalesWeeklyTrend(retailerRequest);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSalesWeeklyTrend"));
            }
        }


        public Task<DataTable> GetSalesRoutePerformance(RetailerRequest retailerRequest)
        {
            try
            {
                Task<DataTable> result = _repo.GetSalesRoutePerformance(retailerRequest);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSalesRoutePerformance"));
            }
        }


        public Task<DataTable> GetThreeDaysSalesMemo(RetailerRequest retailerRequest)
        {
            try
            {
                Task<DataTable> result = _repo.GetThreeDaysSalesMemo(retailerRequest);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetThreeDaysSalesMemo"));
            }
        }


        public Task<DataTable> GetSalesSummaryV2(RetailerRequest retailer)
        {
            try
            {
                Task<DataTable> result = _repo.GetSalesSummaryV2(retailer);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSalesSummaryV2"));
            }
        }


        public Task<DataTable> GetSalesDetails(SalesDetailRequest salesDetails)
        {
            try
            {
                Task<DataTable> result = _repo.GetSalesDetails(salesDetails);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSalesDetails"));
            }
        }

    }
}