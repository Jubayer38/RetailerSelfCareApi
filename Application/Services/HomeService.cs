///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	11-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Infrastracture.Repositories;


namespace Application.Services
{
    public class HomeService : IDisposable
    {
        private readonly LMSRepository _repo;

        public HomeService()
        {
            _repo = new();
        }

        public HomeService(string connectionString)
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



    }
}
