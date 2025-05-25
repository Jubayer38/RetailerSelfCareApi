///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	Retailer Repository methods
///	Creation Date :	03-Jan-2024
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
using Domain.StaticClass;
using Domain.ViewModel;
using Infrastracture.DBManagers;
using MySqlConnector;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using static Domain.Enums.EnumCollections;

namespace Infrastracture.Repositories
{
    public class RetailerRepository : IDisposable
    {
        private readonly OracleDbManager _db;
        private readonly MySqlDbManager _mySql;

        public RetailerRepository()
        {
            _mySql = new();
        }

        public RetailerRepository(string ConnectionString)
        {
            _db = new(ConnectionString);
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
                _db.Dispose();
                _mySql.Dispose();
            }

            isDisposed = true;
        }
        #endregion==========|  Dispose Method  |==========


        public async Task<long> UpdateRetailer(RetailerDetailsRequest retailer)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailer.retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_NAME", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailer.retailerName });
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_ADDRESS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailer.retailerAddress });
            //_mySql.AddParameter(new MySqlParameter("P_RETAILER_EMAIL", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailer.email });
            //_mySql.AddParameter(new MySqlParameter("P_RETAILER_DOB", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailer.DOB });

            long result = await _mySql.InsertByStoredProcedureAsync("UPDATE_RETAILER_INFO");
            return result;
        }


        public async Task<DataTable> CustomerFeedback(RetailerRequest retailer)
        {
            try
            {
                _db.AddParameter(new OracleParameter("vRETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.retailerCode });
                _db.AddParameter(new OracleParameter("po_cursor", OracleDbType.RefCursor, ParameterDirection.Output));
                var result = _db.CallStoredProcedure_Select("RSLGETCTFEEDBACK");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "CustomerFeedback"));
            }
        }


        public async Task<string> RetailerRating(RetailerRequest retailer)
        {
            try
            {
                _db.AddParameter(new OracleParameter("VRETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.retailerCode });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));
                var result = _db.CallStoredProcedure_Select("RSLGETRTLRATING");

                string rating = result.Rows[0]["R_RATING"] as string ?? "0.0";
                return rating;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "RetailerRating"));
            }
        }


        public async Task<DataTable> GetSTK()
        {
            _db.AddParameter(new OracleParameter("po_cursor", OracleDbType.RefCursor, ParameterDirection.Output));
            var result = _db.CallStoredProcedure_Select("RSLGETSTKINFOS");
            return result;
        }


        public DataTable GetFAQ()
        {
            try
            {
                _db.AddParameter(new OracleParameter("po_cursor", OracleDbType.RefCursor, ParameterDirection.Output));
                var result = _db.CallStoredProcedure_Select("RSLGETFAQ");
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetFAQ"));
            }
        }


        public dynamic GetFAQModel(List<FAQModel> model)
        {
            try
            {
                dynamic faqList = model.GroupBy(x => x.categoryTitle).Select(g => new
                {
                    categoryTitle = g.First().categoryTitle,
                    details = g.Select(s => new { s.question, s.answer }).ToList()
                });
                return faqList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetFAQModel"));
            }
        }


        public async Task<DataTable> GetRetailerDetails(RetailerRequest retailer)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailer.retailerCode });

            var result = await _mySql.CallStoredProcedureSelectAsync("RSLGETRETAILERINFO");

            return result;
        }


        public async Task<Tuple<bool, string>> UpdateRetailerInfoMySQL(RetailerInfoRequest model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_ITOPUP_NUMBER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.iTopUpNumber });
                _mySql.AddParameter(new MySqlParameter("P_ISACTIVE", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.isActive });
                _mySql.AddParameter(new MySqlParameter("P_TYPE_NAME", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.typeName });
                _mySql.AddParameter(new MySqlParameter("P_CHANGE_BY", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.userName });

                MySqlParameter param = new(FrequentlyUsedDbParams.P_RETURN.ToString(), MySqlDbType.Int32) { Direction = ParameterDirection.Output };

                _mySql.AddParameter(param);

                object procReturn = await _mySql.CallStoredProcedureObjectAsync("RSLSYNC_RETAILER_INFO", FrequentlyUsedDbParams.P_RETURN.ToString());

                if (procReturn == null) return Tuple.Create(false, "Unable to update");
                else
                {
                    int res;
                    bool isIntVal = int.TryParse(procReturn.ToString(), out res);

                    if (isIntVal && res > 1)
                        return Tuple.Create(true, Message.Success);
                    else if (isIntVal && res == 0)
                        return Tuple.Create(false, "No Retailer Found.");
                    else
                        return Tuple.Create(false, procReturn.ToString());
                }

            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRetailerDetails"));
            }
        }


        public async Task<Tuple<bool, string>> UpdateDigitalServiceStatus(UpdateDigitalServiceStatus model)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_INTERNAL_USER", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.userName });
                _db.AddParameter(new OracleParameter("P_SERVICE_NAME", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.ServiceName });
                _db.AddParameter(new OracleParameter("P_ITOPUP_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.iTopUpNumber });
                _db.AddParameter(new OracleParameter("P_SUBSCRIBER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.SubscriberNumber });
                _db.AddParameter(new OracleParameter("P_REGISTERED_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = model.RegisteredDate });
                _db.AddParameter(new OracleParameter("P_LATITUDE", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.Latitude });
                _db.AddParameter(new OracleParameter("P_LONGITUDE", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.Longitude });
                OracleParameter param = new("PO_RETURN", OracleDbType.Varchar2, 210, null, ParameterDirection.Output);

                _db.AddParameter(param);
                _db.CallStoredProcedure("RSLSYNC_DS_STATUS");

                bool isNull = ((OracleString)param.Value).IsNull;
                if (isNull) return Tuple.Create(false, "Unable to update");
                else
                {
                    string result = ((OracleString)param.Value).Value;
                    int res;
                    bool isIntVal = int.TryParse(result, out res);

                    if (isIntVal && res > 0)
                        return Tuple.Create(true, Message.Success);
                    else if (isIntVal && res == 0)
                        return Tuple.Create(false, "No Digital Service Found.");
                    else
                        return Tuple.Create(false, result);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "UpdateDigitalServiceStatus"));
            }

        }


        public int RetailerBestPractice(ComplainRequest model, int userId)
        {
            try
            {
                int insertResult = 0;
                string imageNames = string.Empty;
                string FolderName = DateTime.Now.ToEnUSDateString("ddMMMMyyyy");
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });
                _db.AddParameter(new OracleParameter("P_TITLE", OracleDbType.NVarchar2, ParameterDirection.Input) { Value = model.title });
                _db.AddParameter(new OracleParameter("P_DESCRIPTION", OracleDbType.NVarchar2, ParameterDirection.Input) { Value = model.description });
                _db.AddParameter(new OracleParameter("P_USER_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = userId });

                long? result = _db.CallStoredProcedure_Insert("RSLSAVERBESTPRACTICEV2");

                insertResult = result.HasValue ? Convert.ToInt32(result) : 0;

                return insertResult;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "RetailerBestPractice"));
            }
        }


        /// Delete table rows by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tableName"></param>
        /// <param name="colName"></param>
        public async Task DeleteTableRows(long id, string tableName, string colName)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_TABLE_NAME", tableName));
                _db.AddParameter(new OracleParameter("P_COLUMN_NAME", colName));
                _db.AddParameter(new OracleParameter("P_ID", id));

                _db.CallStoredProcedure("RSLSPDELTETABLEROW");
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public int SaveBestPracticeImage(int bestPracticeId, string base64Header, string base64Str)
        {
            try
            {
                int result;
                _db.AddParameter(new OracleParameter("P_BEST_PRACTICE_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = bestPracticeId });
                _db.AddParameter(new OracleParameter("P_BASE64_HEADER", OracleDbType.Varchar2, ParameterDirection.Input) { Value = base64Header });
                _db.AddParameter(new OracleParameter("P_BASE64_DATA", OracleDbType.Clob, ParameterDirection.Input) { Value = base64Str });

                long? res = _db.CallStoredProcedure_Insert("RSLSP_INS_BESTPRACTICE_FILES");
                result = (int)res.GetValueOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveBestPracticeImage"));
            }
        }


        public DataTable BestPracticesImages(GetBPImagesRequest retailer)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_BP_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = retailer.id });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSL_GET_BP_IMAGES");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "BestPracticesImages"));
            }
        }


        public DataTable GetSIMSCStocksSummary(RetailerRequest retailer)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.retailerCode });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSLGET_SIM_SC_STOCK_SUMMARY");
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSIMSCStocksSummary"));
            }
        }


        public DataTable GetItopUpSummaryV2(RetailerRequest retailer)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.retailerCode });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSLGETITOPUPTOCKSUMMARY");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetItopUpSummaryV2"));
            }
        }


        public async Task<string> GetActiveQAListIDs(QuickAccessRequest model)
        {
            try
            {
                string result = string.Empty;
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });
                _db.AddParameter(new OracleParameter("P_DEVICE_ID", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.deviceId });
                _db.AddParameter(new OracleParameter("P_IS_PRIMARY", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.isPrimary });

                OracleParameter param = new("PO_RETURN", OracleDbType.Varchar2, 500)
                {
                    Direction = ParameterDirection.Output
                };

                _db.AddParameter(param);

                _db.CallStoredProcedure("RSL_GETQA_ACTIVE_ORDS_STR");

                if (!((OracleString)param.Value).IsNull)
                {
                    result = ((OracleString)param.Value).Value;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetActiveQAListIDs"));
            }
        }


        public async Task<string> GetInActiveQAListIDs(QuickAccessRequest model)
        {
            try
            {
                string result = string.Empty;
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });
                _db.AddParameter(new OracleParameter("P_DEVICE_ID", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.deviceId });
                _db.AddParameter(new OracleParameter("P_IS_PRIMARY", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.isPrimary });

                OracleParameter param = new("PO_RETURN", OracleDbType.Varchar2, 500)
                {
                    Direction = ParameterDirection.Output
                };

                _db.AddParameter(param);

                _db.CallStoredProcedure("RSL_GETQA_INACTIVE_ORDS_STR");

                if (!((OracleString)param.Value).IsNull)
                {
                    result = ((OracleString)param.Value).Value;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetInActiveQAListIDs"));
            }
        }


        public async Task<int> UpdateQuickAccessOrder(QuickAccessUpdateRequest model)
        {
            try
            {
                long? result = 0;

                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });
                _db.AddParameter(new OracleParameter("P_DEVICE_ID", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.deviceId });
                _db.AddParameter(new OracleParameter("P_ACTIVE_WEIDGETS", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.activeWeidgets });
                _db.AddParameter(new OracleParameter("P_INACTIVE_WIDGES", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.inactiveWeidgets });
                _db.AddParameter(new OracleParameter("P_USER_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.userId });

                result = _db.CallStoredProcedureInsertV2("RSL_UPDATE_QA_DEVICE_ORDER");
                return (int)result.GetValueOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "UpdateQuickAccessOrder"));
            }
        }


        public DataTable GetRSOAndLastTime(RetailerRequestV2 reqModel)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = reqModel.retailerCode });
                _db.AddParameter(new OracleParameter("P_ITOPUP_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input) { Value = reqModel.iTopUpNumber });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSLGETRSOANDLASTBLNCTIMEV2");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRSOAndLastTime"));
            }
        }


        public DataTable SearchOffersV2(SearchRequestV2 model)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });
                _db.AddParameter(new OracleParameter("P_SEARCH_TEXT", OracleDbType.NVarchar2, ParameterDirection.Input) { Value = model.searchText });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSLSEARCHOFFERS");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SearchOffersV2"));
            }
        }


        public DataTable TarVsAchvSummaryV4(RetailerRequestV2 retailerRequest)
        {
            try
            {
                _ = int.TryParse(retailerRequest.retailerCode.Substring(1), out int retailerId);
                _db.AddParameter(new OracleParameter("P_RETAILER_ID", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailerId });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSLTARVSACHVSUMMARY");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "TarVsAchvSummaryV4"));
            }
        }


        public DataTable TarVsAchvDeatilsV4(TarVsAchvRequestV2 tarVsAchvRequest)
        {
            try
            {
                _ = int.TryParse(tarVsAchvRequest.retailerCode.Substring(1), out int retailerId);
                _db.AddParameter(new OracleParameter("P_RETAILER_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = retailerId });
                _db.AddParameter(new OracleParameter("P_KPIID", OracleDbType.Decimal, ParameterDirection.Input) { Value = tarVsAchvRequest.kpiInt });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSLTARVSACHVDETAILS");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "TarVsAchvDeatilsV4"));
            }
        }


        public async Task<DataTable> GetArchivedData(ArchivedRequest model)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });
                _db.AddParameter(new OracleParameter("P_SEARCH_TEXT", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.searchText });
                _db.AddParameter(new OracleParameter("P_SORT_TYPE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.sortType });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSLGETARCHIVEDDATA");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetArchivedData"));
            }
        }


        public async Task<string> GetRSONumber(string retailerCode)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailerCode });

                string _outParam = nameof(FrequentlyUsedDbParams.P_RETURN);
                MySqlParameter param = new(_outParam, MySqlDbType.Int32) { Direction = ParameterDirection.Output };
                _mySql.AddParameter(param);
                object procReturn = await _mySql.CallStoredProcedureObjectAsync("GET_RSO_NUMBER", _outParam);
                return procReturn.DBNullToString();
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRSONumber"));
            }
        }


        public DataTable GetC2STransactions(TransactionsRequest model)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_MSISDN", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.iTopUpNumber });
            _db.AddParameter(new OracleParameter("P_SUBSCRIBER_NO", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.subscriberNo });
            _db.AddParameter(new OracleParameter("P_FROMDATE", OracleDbType.Date, ParameterDirection.Input) { Value = model.startDate });
            _db.AddParameter(new OracleParameter("P_TODATE", OracleDbType.Date, ParameterDirection.Input) { Value = model.endDate });
            _db.AddParameter(new OracleParameter("P_SORTBY_DATE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.sortByDate });
            _db.AddParameter(new OracleParameter("P_SORTBY_AMOUNT", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.sortByAmt });
            _db.AddParameter(new OracleParameter("P_SORTBY_INOUT", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.sortByInOut });

            _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLGETC2STRANSACTIONS");
            return result;
        }


        public DataTable GetC2CTransactions(TransactionsRequest model)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });
            _db.AddParameter(new OracleParameter("P_SUBSCRIBER_NO", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.subscriberNo });
            _db.AddParameter(new OracleParameter("P_FROMDATE", OracleDbType.Date, ParameterDirection.Input) { Value = model.startDate });
            _db.AddParameter(new OracleParameter("P_TODATE", OracleDbType.Date, ParameterDirection.Input) { Value = model.endDate });
            _db.AddParameter(new OracleParameter("P_SORTBY_DATE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.sortByDate });
            _db.AddParameter(new OracleParameter("P_SORTBY_AMOUNT", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.sortByAmt });
            _db.AddParameter(new OracleParameter("P_SORTBY_INOUT", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.sortByInOut });

            _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLGETC2CTRANSACTIONS");
            return result;
        }


        public async Task<DataTable> GetComplaintTypeList()
        {
            try
            {
                MySqlDbManager _mySql = new();
                DataTable result = await _mySql.CallStoredProcedureSelectAsync("GET_ALL_COMP_TYPE_LIST");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetComplaintTypeList"));
            }
        }


        public async Task<DataTable> GetComplaintTitleList(ComplaintTitleRequest model)
        {
            try
            {
                MySqlDbManager _mySql = new();
                _mySql.AddParameter(new MySqlParameter("P_COMPLAINT_TYPE_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = model.ComplaintTypeID });
                DataTable result = await _mySql.CallStoredProcedureSelectAsync("GET_ALL_COMP_TITLE_LIST");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetComplaintTitleList"));
            }
        }


        public async Task<long> SaveRaiseComplaint(RaiseComplSubmitRequest model, int userId)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_COMPLAINT_TYPE_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = model.complaintTypeID });
                _mySql.AddParameter(new MySqlParameter("P_PREFERRED_LEVEL_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = model.preferredLevelId });
                _mySql.AddParameter(new MySqlParameter("P_COMPLAINT_TITLE_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = model.complaintTitleId });
                _mySql.AddParameter(new MySqlParameter("P_DESCRIPTION", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.description });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = userId });
                _mySql.AddParameter(new MySqlParameter("P_FILE_LOCATION", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.FileLocation });

                long? result = await _mySql.InsertByStoredProcedureAsync("SAVE_RAISE_COMPLAINT");
                return result.GetValueOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveRaiseComplaint"));
            }
        }


        public async Task<DataTable> GetRaiseComplaintInfoV2(RaiseComplSubmitRequest model, long insertedCompId)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RAISE_COMPLAINT_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = insertedCompId });
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });

                DataTable result = await _mySql.CallStoredProcedureSelectAsync("RSL_GET_RAISE_COMPLAINT_INFO");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRaiseComplaintInfoV2"));
            }
        }


        public async Task<Tuple<bool, string>> UpdateRaiseComplaintStatusV2(UpdateRaiseComplaintRequest model)
        {
            try
            {
                bool isNull;

                _mySql.AddParameter(new MySqlParameter("P_INTERNAL_USER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.userName });
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_RAISE_COMPLAINT_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = model.raiseComplaintID });
                _mySql.AddParameter(new MySqlParameter("P_COMPLAINT_STATUS", MySqlDbType.Int16) { Direction = ParameterDirection.Input, Value = model.complaintStatus });

                MySqlParameter param = new(FrequentlyUsedDbParams.P_RETURN.ToString(), MySqlDbType.VarChar) { Direction = ParameterDirection.Output };
                _mySql.AddParameter(param);
                object procReturn = await _mySql.CallStoredProcedureObjectAsync("RSLSYNC_RC_STATUS", FrequentlyUsedDbParams.P_RETURN.ToString());
                string outPram = procReturn.ToString();

                if (outPram is null)
                {
                    isNull = true;
                }
                else
                {
                    isNull = false;
                }
                if (isNull) return Tuple.Create(false, "Unable to update");
                else
                {
                    string result = outPram;
                    bool isIntVal = int.TryParse(result, out int res);

                    if (isIntVal && res > 0)
                        return Tuple.Create(true, Message.Success);
                    else if (isIntVal && res == 0)
                        return Tuple.Create(false, "No Raise Complaint Found.");
                    else
                        return Tuple.Create(false, result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "UpdateRaiseComplaintStatusV2"));
            }
        }


        public async Task<Tuple<bool, string>> UpdateRaiseComplaintStatusFromSO(UpdateRaiseComplaintRequest model, long soTicketId)
        {
            try
            {
                bool isNull;

                _mySql.AddParameter(new MySqlParameter("P_INTERNAL_USER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.userName });
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_RAISE_COMPLAINT_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = model.raiseComplaintID });
                _mySql.AddParameter(new MySqlParameter("P_COMPLAINT_STATUS", MySqlDbType.Int16) { Direction = ParameterDirection.Input, Value = model.complaintStatus });
                _mySql.AddParameter(new MySqlParameter("P_SO_TICKET_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = soTicketId });

                MySqlParameter param = new(FrequentlyUsedDbParams.P_RETURN.ToString(), MySqlDbType.VarChar) { Direction = ParameterDirection.Output };
                _mySql.AddParameter(param);
                object procReturn = await _mySql.CallStoredProcedureObjectAsync("RSLSYNC_RC_STATUS_SO", FrequentlyUsedDbParams.P_RETURN.ToString());
                string outPram = procReturn.ToString();

                if (outPram is null)
                {
                    isNull = true;
                }
                else
                {
                    isNull = false;
                }
                if (isNull) return Tuple.Create(false, "Unable to update");
                else
                {
                    string result = outPram;
                    bool isIntVal = int.TryParse(result, out int res);

                    if (isIntVal && res > 0)
                        return Tuple.Create(true, Message.Success);
                    else if (isIntVal && res == 0)
                        return Tuple.Create(false, "No Raise Complaint Found.");
                    else
                        return Tuple.Create(false, result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "UpdateRaiseComplaintStatus"));
            }
        }


        public async Task<DataTable> GetSOUpdateStatus(HistoryPageRequestModelV2 model)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_START_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.startDate });
            _mySql.AddParameter(new MySqlParameter("P_END_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.endDate });

            DataTable result = await _mySql.CallStoredProcedureSelectAsync("RSL_GET_SO_UPDATE_STATUS");
            return result;
        }


        public async Task<long> UpdateSOTicketStatus(Ticket ticket)
        {
            _mySql.AddParameter(new MySqlParameter("P_STATUS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = ticket.currentStatus });
            _mySql.AddParameter(new MySqlParameter("P_SUPEROFFICE_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = ticket.ticketId });

            long result = await _mySql.InsertByStoredProcedureAsync("RSL_SO_TICKET_STATUS");
            return result;
        }


        public async Task<DataTable> GetRaiseComplaintHistory(HistoryPageRequestModelV2 model)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_START_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.startDate });
            _mySql.AddParameter(new MySqlParameter("P_END_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.endDate });

            DataTable result = await _mySql.CallStoredProcedureSelectAsync("GET_RAISE_COMPALINT_HISTORY");
            return result;
        }


        public async Task<DataTable> GetRSOInformation(RetailerRequestV2 retailer)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailer.retailerCode });
                DataTable result = await _mySql.CallStoredProcedureSelectAsync("GET_RSO_INFO");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRSOInformation"));
            }
        }


        public async Task<long> SaveRSORating(RSORatingReqModel model, int userId)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_RSO_NAME", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.name });
                _mySql.AddParameter(new MySqlParameter("P_RSO_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.code });
                _mySql.AddParameter(new MySqlParameter("P_RSO_NUMBER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.number });
                _mySql.AddParameter(new MySqlParameter("P_RSO_RATING", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = model.rating });
                _mySql.AddParameter(new MySqlParameter("P_COMMENT", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.comment });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = userId });
                _mySql.AddParameter(new MySqlParameter("P_STATUS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.status });

                long? result = await _mySql.InsertByStoredProcedureAsync("SAVE_RSO_RATING");
                return result.GetValueOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveRSORating"));
            }
        }


        public async Task<DataTable> GetRSORatingHistory(HistoryPageRequestModelV2 model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_START_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.startDate });
                _mySql.AddParameter(new MySqlParameter("P_END_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.endDate });

                DataTable result = await _mySql.CallStoredProcedureSelectAsync("GET_RSO_RATING_HISTORY");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRSORatingHistory"));
            }
        }


        public async Task<DataTable> GetNotifications(RetailerRequestV2 retailer)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailer.retailerCode });

                var result = await _mySql.CallStoredProcedureSelectAsync("RSLGETNOTIFICATIONSV2");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetNotifications"));
            }
        }


        public string[] GetRetailersTopThreeDeno(RetailerRequestV2 retailerRequest)
        {
            try
            {
                string iTopUpNumber = retailerRequest.iTopUpNumber.Substring(1);

                _db.AddParameter(new OracleParameter("P_ITOPUP_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input) { Value = iTopUpNumber });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                DataTable dt = _db.CallStoredProcedure_Select("SEARCH_RETAILERS_TOP_DENO");

                if (dt.Rows.Count > 0)
                {
                    var denoList = dt.AsEnumerable().Select(row => row["TXN_AMOUNT"].ToString()).ToArray();
                    return denoList;
                }
                else
                    return Array.Empty<string>();
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRetailersTopThreeDeno"));
            }
        }


        public DataTable RetailerDenoReport(SearchRequestV2 searchRequest)
        {
            try
            {
                int.TryParse(searchRequest.searchText, out int _amount);
                string iTopUpNumber = searchRequest.iTopUpNumber.Substring(1);

                _db.AddParameter(new OracleParameter("P_DENO_AMOUNT", OracleDbType.Decimal, ParameterDirection.Input) { Value = _amount });
                _db.AddParameter(new OracleParameter("P_ITOPUP_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input) { Value = iTopUpNumber });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                return _db.CallStoredProcedure_Select("GET_DENO_DETAILS_REPORT");
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "RetailerDenoReport"));
            }
        }


        public async Task<long> SaveContact(ContactSaveRequest contact)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = contact.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_CONTACT_NO", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = contact.contactNo });
                _mySql.AddParameter(new MySqlParameter("P_CONTACT_NAME", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = contact.contactName });

                long result = await _mySql.InsertByStoredProcedureAsync("SAVE_CONTACT");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveContact"));
            }
        }


        public async Task<int> VORByRetailer(VORModel model)
        {
            int insertResult = 0;

            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });
                _db.AddParameter(new OracleParameter("P_TITLE", OracleDbType.NVarchar2, ParameterDirection.Input) { Value = model.title });
                _db.AddParameter(new OracleParameter("P_DESCRIPTION", OracleDbType.NVarchar2, ParameterDirection.Input) { Value = model.description });
                _db.AddParameter(new OracleParameter("P_FEEDBACK_CAT_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.categoryId });
                _db.AddParameter(new OracleParameter("P_OPERATOR_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.operatorId });
                _db.AddParameter(new OracleParameter("P_USER_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.userId });

                long? result = _db.CallStoredProcedure_Insert("RSL_INSERT_VORV2");
                insertResult = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "VORByRetailer"));
            }

            return insertResult;
        }


        public async Task<int> SaveVorImage(int vorId, string base64Header, string base64Str)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_VOR_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = vorId });
                _db.AddParameter(new OracleParameter("P_BASE64_HEADER", OracleDbType.Varchar2, ParameterDirection.Input) { Value = base64Header });
                _db.AddParameter(new OracleParameter("P_BASE64_DATA", OracleDbType.Clob, ParameterDirection.Input) { Value = base64Str });

                long? res = _db.CallStoredProcedure_Insert("RSLSP_INSERT_VOR_FILES");
                return (int)res.GetValueOrDefault();

            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveVorImage"));
            }
        }

        public async Task<DataTable> GetRSOMemo(RsoMemoRequest retailer, int userId)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.retailerCode });
                _db.AddParameter(new OracleParameter("P_FROM_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = Convert.ToDateTime(retailer.startDate) });
                _db.AddParameter(new OracleParameter("P_TO_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = Convert.ToDateTime(retailer.endDate) });
                _db.AddParameter(new OracleParameter("P_USER_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = userId });

                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSL_GET_RSO_MEMO");

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRSOMemo"));
            }
        }


        public async Task<DataTable> GetRSOFeedback(RsoMemoRequest retailer, int userId)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.retailerCode });
                _db.AddParameter(new OracleParameter("P_FROM_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = Convert.ToDateTime(retailer.startDate) });
                _db.AddParameter(new OracleParameter("P_TO_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = Convert.ToDateTime(retailer.endDate) });
                _db.AddParameter(new OracleParameter("P_USER_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = userId });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSL_GET_RSO_FEEDBACK");

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRSOFeedback"));
            }
        }


        public async Task<DataTable> GetFeedbackCategoryList(int userId)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_USER_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = userId });

                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSL_GET_FB_CATEGORY_LIST");

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetFeedbackCategoryList"));
            }
        }


        public async Task<DataTable> GetOperatorList(int userId)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_USER_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = userId });

                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSL_GET_OPERATOR_LIST");

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetOperatorList"));
            }
        }


        public async Task<DataTable> GetProductRatingList(ProductRatingRequest model)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });
                _db.AddParameter(new OracleParameter("P_IS_ACQUISITION_OFFER", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.acquisition });
                _db.AddParameter(new OracleParameter("P_ISSIMREPLACEMENT", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.simReplacement });
                _db.AddParameter(new OracleParameter("P_USER_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.userId });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSL_GET_RR_PRODUCT_LIST");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetProductRatingList"));
            }
        }


        public async Task<DataTable> GetAllIRISProductRating(IrisOfferRequestNew model, int userId)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });
                _db.AddParameter(new OracleParameter("P_USER_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = userId });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSL_GET_IRIS_PRODUCT_RATING");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetAllIRISProductRating"));
            }
        }


        public async Task<DataTable> GetProductTypeList()
        {
            try
            {
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));
                var result = _db.CallStoredProcedure_Select("RSL_GET_RR_TYPE_LIST");

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetProductTypeList"));
            }
        }

        public async Task<DataTable> GetProductRatingHistory(HistoryPageRequestModel model)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });
                _db.AddParameter(new OracleParameter("P_START_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = model.startDate });
                _db.AddParameter(new OracleParameter("P_END_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = model.endDate });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSL_GET_RATING_HISTORY");
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetProductRatingHistory"));
            }
        }


        public async Task<DataTable> GetBTSLocation(string retailerCode)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailerCode });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSL_GET_BTSLOCATIONLIST");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetBTSLocation"));
            }
        }


        public async Task<DataTable> GetBTSLocationDetails(int lac, int cid)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_LAC", OracleDbType.Int32, ParameterDirection.Input) { Value = lac });
                _db.AddParameter(new OracleParameter("P_CELL_ID", OracleDbType.Int32, ParameterDirection.Input) { Value = cid });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSL_GET_BTSLOCATIONDETAILES");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetBTSLocationDetails"));
            }
        }


        public async Task<DataTable> GetContactList(string retailerCode)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailerCode });
                var result = await _mySql.CallStoredProcedureSelectAsync("GET_CONTACT_LIST");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetContactList"));
            }
        }


        public async Task<DataTable> GetCampaignKPIList(CampaignRequestV2 campaignRequest)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_CAMPAIGN_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = campaignRequest.campaignId });
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = campaignRequest.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = campaignRequest.userId });

                var result = await _mySql.CallStoredProcedureSelectAsync("GET_EXT_CAMP_KPI_LIST");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampaignKPIList"));
            }
        }


        public async Task<DataTable> GetCampaignRewardList(CampaignRequestV2 campaignRequest)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_CAMPAIGN_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = campaignRequest.campaignId });
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = campaignRequest.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = campaignRequest.userId });

                var result = await _mySql.CallStoredProcedureSelectAsync("GET_EXT_CAMP_REWARD_LIST");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampaignRewardList"));
            }
        }

        public async Task<DataTable> GetCampFurtherDetails(CampaignRequestV2 model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_CAMPAIGN_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.campaignId });
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.userId });

                var result = await _mySql.CallStoredProcedureSelectAsync("GET_EXT_CAMP_EN_DETAILS");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampFurtherDetails"));
            }
        }


        public async Task<DataTable> GetSelfCampaignKPIList(CampaignRequestV2 campaignRequest)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_CAMPAIGN_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = campaignRequest.campaignId });
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = campaignRequest.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = campaignRequest.userId });

                var result = await _mySql.CallStoredProcedureSelectAsync("GET_SELF_CAMPAIGN_KPI_LIST");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampaignKPIList"));
            }
        }

        public async Task<DataTable> GetSelfCampaignRewardList(CampaignRequestV2 campaignRequest)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_CAMPAIGN_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = campaignRequest.campaignId });
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = campaignRequest.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = campaignRequest.userId });

                var result = await _mySql.CallStoredProcedureSelectAsync("GET_SELF_CAMP_REWARD_LIST");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampaignRewardList"));
            }
        }


        public async Task<DataTable> GetSelfCampFurtherDetails(CampaignRequestV2 model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_CAMPAIGN_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.campaignId });
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.userId });

                var result = await _mySql.CallStoredProcedureSelectAsync("GET_SELF_CAMP_EN_DETAILS");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampFurtherDetails"));
            }
        }


        public async Task<DataTable> GetCampRetailerDates(SelfCampDatesRequest model, string ids)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_DURATION", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.duration });
                _mySql.AddParameter(new MySqlParameter("P_IDS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = ids });

                var result = await _mySql.CallStoredProcedureSelectAsync("GET_CAMP_RETAILER_DATES");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampRetailerDates"));
            }
        }


        public async Task<DataTable> GetSelfRewardList(SelfCampaignRewardRequest model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_TOTAL", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.total });
                var result = await _mySql.CallStoredProcedureSelectAsync("GET_SELF_REWARD_LIST");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfRewardList"));
            }
        }


        public async Task<DataTable> GetSelfCampDayList(string retailerCode, string ids)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_IDS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = ids });

                var result = await _mySql.CallStoredProcedureSelectAsync("GET_SELF_CAMP_DAYS_LIST");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampDayList"));
            }
        }


        public async Task<DataTable> GetCampKPIList(SelfKPIListRequest model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_IS_DARK", MySqlDbType.Double) { Direction = ParameterDirection.Input, Value = model.isDark ? 1 : 0 });

                var result = await _mySql.CallStoredProcedureSelectAsync("GET_SELF_CAMP_KPI_LIST");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampKPIList"));
            }
        }


        public async Task<int> EnrollExtCampaign(CampaignEnrollOrCancelRequest model)
        {
            long result = 0;

            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_CAMPAIGN_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.campaignId });
                _mySql.AddParameter(new MySqlParameter("P_REWARD_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.rewardId });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.userId });

                MySqlParameter param = new("P_RETURN", MySqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Output
                };

                _mySql.AddParameter(param);

                await _mySql.CallStoredProcedureSelectAsync("ENROLL_EXT_CAMPAIGN");

                if (param.Value != DBNull.Value)
                {
                    result = Convert.ToInt64(param.Value);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "EnrollExtCampaign"));
            }

            return Convert.ToInt32(result);
        }


        public async Task<int> EnrollSelfCampaign(CampaignEnrollOrCancelRequest model)
        {
            long result = 0;

            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_CAMPAIGN_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.campaignId });
                _mySql.AddParameter(new MySqlParameter("P_REWARD_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.rewardId });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.userId });


                MySqlParameter param = new("P_RETURN", MySqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Output
                };

                _mySql.AddParameter(param);

                await _mySql.CallStoredProcedureSelectAsync("ENROLL_SELF_CAMPAIGN");

                if (param.Value != DBNull.Value)
                    result = Convert.ToInt64(param.Value);

            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "EnrollSelfCampaign"));
            }

            return Convert.ToInt32(result);
        }


        public async Task<int> CancelExtCampaignEnroll(CampaignEnrollOrCancelRequest model)
        {
            long result = 0;

            try
            {

                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_CAMPAIGN_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.campaignId });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.userId });

                MySqlParameter param = new("P_RETURN", OracleDbType.Decimal)
                {
                    Direction = ParameterDirection.Output
                };

                _mySql.AddParameter(param);

                await _mySql.CallStoredProcedureSelectAsync("CANCEL_EXT_CAMP_ENROLL");

                if (param.Value != DBNull.Value)
                    result = Convert.ToInt64(param.Value);

            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "CancelExtCampaignEnroll"));
            }

            return Convert.ToInt32(result);
        }


        public async Task<int> CancelSelfCampaignEnroll(CampaignEnrollOrCancelRequest model)
        {
            long result = 0;

            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_CAMPAIGN_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.campaignId });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.userId });


                MySqlParameter param = new("P_RETURN", MySqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Output
                };

                _mySql.AddParameter(param);

                await _mySql.CallStoredProcedureSelectAsync("CANCEL_SELF_CAMP_ENROLL");

                if (param.Value != DBNull.Value)
                    result = Convert.ToInt64(param.Value);

            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "CancelSelfCampaignEnroll"));
            }

            return Convert.ToInt32(result);
        }


        public async Task<DataTable> GetCampHistoryKPIList(CampaignHistoryRequest model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });

                _mySql.AddParameter(new MySqlParameter("P_START_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.startDate });

                _mySql.AddParameter(new MySqlParameter("P_END_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.endDate });

                //db.AddParameter(new OracleParameter("P_USER_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.userId });

                /*  db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));*/
                var result = await _mySql.CallStoredProcedureSelectAsync("GET_EXT_CAMP_HISTORY_KPI");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampHistoryKPIList"));
            }
        }


        public async Task<DataTable> GetCampHistoryList(CampaignHistoryRequest model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_START_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.startDate });
                _mySql.AddParameter(new MySqlParameter("P_END_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.endDate });
                _mySql.AddParameter(new MySqlParameter("P_DATE_FIELD", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.dateField });

                var result = await _mySql.CallStoredProcedureSelectAsync("GET_EXT_CAMP_HISTORY");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampHistoryList"));
            }
        }


        public async Task<string> GetCampHistoryUpdateTill(CampaignHistoryRequest model)
        {

            string result = string.Empty;
            try
            {

                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });

                _mySql.AddParameter(new MySqlParameter("P_START_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.startDate });

                _mySql.AddParameter(new MySqlParameter("P_END_DATE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.endDate });

                MySqlParameter param = new("P_RETURN", MySqlDbType.VarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };

                _mySql.AddParameter(param);

                await _mySql.CallStoredProcedureSelectAsync("GET_EXT_CAMP_H_UPDT_TILL");

                result = param.Value != DBNull.Value ? param.Value.ToString() : string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampHistoryUpdateTill"));
            }

            return result;
        }


        public async Task<DataTable> GetSelfCampHistoryKPIList(CampaignHistoryRequest model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_START_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.startDate });
                _mySql.AddParameter(new MySqlParameter("P_END_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.endDate });

                var result = await _mySql.CallStoredProcedureSelectAsync("GET_SELF_CAMP_HISTORY_KPI");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampHistoryKPIList"));
            }
        }


        public async Task<DataTable> GetSelfCampHistoryList(CampaignHistoryRequest model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_START_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.startDate });
                _mySql.AddParameter(new MySqlParameter("P_END_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.endDate });
                _mySql.AddParameter(new MySqlParameter("P_DATE_FIELD", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.dateField });

                var result = await _mySql.CallStoredProcedureSelectAsync("GET_SELF_CAMP_HISTORY");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampHistoryList"));
            }
        }


        public async Task<string> GetSelfCampHistoryUpdateTill(CampaignHistoryRequest model)
        {
            string result = string.Empty;
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_START_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.startDate });
                _mySql.AddParameter(new MySqlParameter("P_END_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.endDate });

                MySqlParameter param = new("P_RETURN", MySqlDbType.VarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };

                _mySql.AddParameter(param);

                await _mySql.CallStoredProcedureSelectAsync("GET_SELF_CAMP_H_UPDT_TILL");

                if (param.Value != DBNull.Value)
                {
                    result = param.Value.ToString();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampHistoryUpdateTill"));
            }

            return result;
        }


        public async Task<DataTable> GetCampKPIDetails(CampaignKPIRequest model, string ids)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_DURATION", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.duration });
                _mySql.AddParameter(new MySqlParameter("P_IDS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = ids });
                _mySql.AddParameter(new MySqlParameter("P_START_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.startDate });
                _mySql.AddParameter(new MySqlParameter("P_END_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.endDate });

                var result = await _mySql.CallStoredProcedureSelectAsync("GET_SELF_CAMP_KPI_DETAILSV2");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampKPIDetails"));
            }
        }


        public async Task<string> CampaignByRetailer(CreateCampaignByRetailerRequest model)
        {
            string result = string.Empty;

            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_CAMP_TITLE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.campaignTitle });
                _mySql.AddParameter(new MySqlParameter("P_CAMP_DESCRIPTION", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.campaignDescription });
                _mySql.AddParameter(new MySqlParameter("P_CAMP_REWARD_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.rewardId });
                _mySql.AddParameter(new MySqlParameter("P_CAMP_REWARD_TYPE_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.rewardTypeId });
                _mySql.AddParameter(new MySqlParameter("P_CAMP_REWARD", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.reward });
                _mySql.AddParameter(new MySqlParameter("P_REWARD_IMAGE_LOCATION", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.rewardImageLocation });
                _mySql.AddParameter(new MySqlParameter("P_FROM_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.startDate });
                _mySql.AddParameter(new MySqlParameter("P_TO_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.endDate });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.userId });


                MySqlParameter param = new("P_RETURN", MySqlDbType.VarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };

                _mySql.AddParameter(param);

                await _mySql.CallStoredProcedureSelectAsync("CREATE_CAMP_BY_RETAILER");

                if (param.Value != DBNull.Value)
                {
                    result = param.Value.ToString();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "CampaignByRetailer"));
            }

            return result;
        }


        public async Task<long> InsertRetailerCampTarget(CampaignTargetRequestModel model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_NEW_CAMP_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.campaignId });
                _mySql.AddParameter(new MySqlParameter("P_NEW_CAMP_ENROLL_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.campEnrollId });
                _mySql.AddParameter(new MySqlParameter("P_KPI_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.kpiId });
                _mySql.AddParameter(new MySqlParameter("P_KPI_TARGET", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.kpiTarget });
                _mySql.AddParameter(new MySqlParameter("P_TARGET_UNIT", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.targetUnit });
                _mySql.AddParameter(new MySqlParameter("P_KPI_CONFIG_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.kpiConfigId });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.userId });

                MySqlParameter param = new("P_PKVALUE", MySqlDbType.Decimal, 500)
                {
                    Direction = ParameterDirection.Output
                };
                _mySql.AddParameter(param);

                await _mySql.CallStoredProcedureSelectAsync("CREATE_CAMP_TARGETS");
                return param.Value != DBNull.Value ? Convert.ToInt64(param.Value) : 0;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "InsertRetailerCampTarget"));
            }
        }


        public async Task DeleteInsertedCampaign(int campId, int campEnrollId, int rewardMapId)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_NEW_CAMP_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = campId });
                _mySql.AddParameter(new MySqlParameter("P_NEW_CAMP_ENROLL_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = campEnrollId });
                _mySql.AddParameter(new MySqlParameter("P_REWARD_MAP_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = rewardMapId });

                await _mySql.CallStoredProcedureSelectAsync("RSL_DELETE_INSERTED_CAMPAIGN");
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "DeleteInsertedCampaign"));
            }
        }


        public async Task<DataTable> GetDigitalServiceHistory(DigitalServiceHistoryRequest model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_FROMDATE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.startDate });
                _mySql.AddParameter(new MySqlParameter("P_TODATE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.endDate });

                DataTable result = await _mySql.CallStoredProcedureSelectAsync("RSL_GET_DIGITAL_SERVICE_HIST");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetDigitalServiceHistory"));
            }

        }


        public async Task<DataTable> GetDigitalProductList()
        {
            try
            {

                DataTable result = await _mySql.CallStoredProcedureSelectAsync("RSL_DIGITAL_PRODUCT_LIST");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetDigitalProductList"));
            }
        }


        public async Task DigitalServiceSmsSendToUser(int productId, string receiverNumber)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_PRODUCT_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = productId });
                _mySql.AddParameter(new MySqlParameter("P_MSISDN", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = receiverNumber });

                await _mySql.InsertByStoredProcedureAsync("RSL_SEND_DS_SMS_TO_USER");
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "DigitalServiceSmsSendToUser"));
            }
        }


        public async Task<Tuple<bool, string>> UpdateRaiseComplaintStatus(UpdateRaiseComplaintRequest model)
        {
            try
            {
                bool isNull;
                _mySql.AddParameter(new MySqlParameter("P_INTERNAL_USER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.userName });
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_RAISE_COMPLAINT_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = model.raiseComplaintID });
                _mySql.AddParameter(new MySqlParameter("P_COMPLAINT_STATUS", MySqlDbType.Int16) { Direction = ParameterDirection.Input, Value = model.complaintStatus });

                MySqlParameter param = new(FrequentlyUsedDbParams.P_RETURN.ToString(), MySqlDbType.VarChar) { Direction = ParameterDirection.Output };
                _mySql.AddParameter(param);
                object procReturn = await _mySql.CallStoredProcedureObjectAsync("RSLSYNC_RC_STATUS", FrequentlyUsedDbParams.P_RETURN.ToString());
                string outPram = procReturn.ToString();

                if (outPram is null)
                {
                    isNull = true;
                }
                else
                {
                    isNull = false;
                }
                if (isNull) return Tuple.Create(false, "Unable to update");
                else
                {
                    string result = outPram;
                    int res;
                    bool isIntVal = int.TryParse(result, out res);

                    if (isIntVal && res > 0)
                        return Tuple.Create(true, Message.Success);
                    else if (isIntVal && res == 0)
                        return Tuple.Create(false, "No Raise Complaint Found.");
                    else
                        return Tuple.Create(false, result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "UpdateRaiseComplaintStatus"));
            }
        }


        public async Task<DataTable> GetAppSettingsInfo()
        {
            try
            {
                var result = await _mySql.CallStoredProcedureSelectAsync("GET_APPSETTINGS_INFO");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetAppSettingsInfo"));
            }
        }


        public async Task<long> SaveDeviceTokens(DeviceTokenRequest model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_DEVICE_ID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.deviceId });
                _mySql.AddParameter(new MySqlParameter("P_DEVICE_TOKEN", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.deviceToken });

                var result = await _mySql.InsertByStoredProcedureAsync("RSLSAVEDEVICETOKEN");
                return Convert.ToInt64(result);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveDeviceTokens"));
            }
        }


        public async Task<string> GetRegionWisePopupCallingTime(string iTopUpNumber)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_ITOPUP_NUMBER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = iTopUpNumber });

                MySqlParameter param = new(FrequentlyUsedDbParams.P_RETURN.ToString(), MySqlDbType.VarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };

                _mySql.AddParameter(param);

                await _mySql.CallStoredProcedureSelectAsync("RSL_GET_REGION_WISE_POPUP_TIME");

                string result = param.Value != DBNull.Value ? param.Value.ToString() : string.Empty;

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRegionWisePopupCallingTime"));
            }
        }


        public async Task<long> LogoutDevice(DeviceStatusRequest model)
        {
            try
            {
                long result;
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_DEVICEID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.operationalDeviceId });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.userId });
                MySqlParameter param = new(FrequentlyUsedDbParams.P_RETURN.ToString(), MySqlDbType.Int32) { Direction = ParameterDirection.Output };

                _mySql.AddParameter(param);

                object procReturn = await _mySql.CallStoredProcedureObjectAsync("RSLLOGOUT", FrequentlyUsedDbParams.P_RETURN.ToString());

                result = Convert.ToInt32(procReturn.ToString());

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "LogoutDevice"));
            }
        }


        public async Task<DataTable> SecondaryDeviceList(RetailerRequest model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                DataTable result = await _mySql.CallStoredProcedureSelectAsync("SECONDARY_DEVICE_LIST");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SecondaryDeviceList"));
            }
        }


        public async Task<long> DeregisterDevice(DeviceStatusRequest model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_DEREGISTER_DEVICEID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.operationalDeviceId });

                long result = await _mySql.InsertByStoredProcedureAsync("RSLDEVICEDEREGISTRATION");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "DeregisterDevice"));
            }
        }


        public async Task<long> ChangeDeviceType(DeviceStatusRequest model)
        {
            try
            {
                long result = 0;
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_DEVICE_ID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.deviceId });
                _mySql.AddParameter(new MySqlParameter("P_OPERATIONAL_DEVICEID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.operationalDeviceId });

                MySqlParameter param = new(FrequentlyUsedDbParams.P_RETURN.ToString(), MySqlDbType.Int32) { Direction = ParameterDirection.Output };

                _mySql.AddParameter(param);

                object procReturn = await _mySql.CallStoredProcedureObjectAsync("RSLCHANGEDEVICETYPE", FrequentlyUsedDbParams.P_RETURN.ToString());

                result = Convert.ToInt64(procReturn.ToString());

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "ChangeDeviceType"));
            }
        }


        public async Task<long> EnableDisableDevice(DeviceStatusRequest model)
        {
            try
            {
                long result = 0;
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_OPERATIONAL_DEVICEID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.operationalDeviceId });
                _mySql.AddParameter(new MySqlParameter("P_DEVICE_STATUS", MySqlDbType.Int16) { Direction = ParameterDirection.Input, Value = model.deviceStatus });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = model.userId });

                MySqlParameter param = new(FrequentlyUsedDbParams.P_RETURN.ToString(), MySqlDbType.Int32) { Direction = ParameterDirection.Output };

                _mySql.AddParameter(param);

                object procReturn = await _mySql.CallStoredProcedureObjectAsync("RSLENABLEDISABLE", FrequentlyUsedDbParams.P_RETURN.ToString());

                result = Convert.ToInt64(procReturn.ToString());

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "EnableDisableDevice"));
            }
        }


        public async Task<int> GetNotificationCount(RetailerRequest retailer)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailer.retailerCode });

                MySqlParameter param = new(FrequentlyUsedDbParams.P_RETURN.ToString(), MySqlDbType.Decimal) { Direction = ParameterDirection.Output };
                _mySql.AddParameter(param);

                await _mySql.CallStoredProcedureSelectAsync("RSLGETNOTIFICATIONCOUNT");

                var count = param.Value != DBNull.Value ? Convert.ToInt32(param.Value) : 0;

                return count;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetNotificationCount"));
            }
        }


        public async Task<int> UpdateNotoficationStatus(RetailerNotificationRequest notificationRequest)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = notificationRequest.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_ID", MySqlDbType.Int32) { Direction = ParameterDirection.Input, Value = notificationRequest.notificationId });
                _mySql.AddParameter(new MySqlParameter("P_MODEL_TYPE", MySqlDbType.Int32) { Direction = ParameterDirection.Input, Value = notificationRequest.modelType });

                MySqlParameter param = new(FrequentlyUsedDbParams.P_RETURN.ToString(), MySqlDbType.Decimal) { Direction = ParameterDirection.Output };
                _mySql.AddParameter(param);

                await _mySql.CallStoredProcedureSelectAsync("RSLUPDATENOTIFICATIONSTATUS");
                int isSuccess = param.Value != DBNull.Value ? Convert.ToInt32(param.Value) : 0;
                return isSuccess;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "UpdateNotoficationStatus"));
            }
        }


        public async Task<DataTable> GetTickerMessages(RetailerRequest model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });

                var result = await _mySql.CallStoredProcedureSelectAsync("RSLGALLTICKERMESSAGE"); //RSLGETNOTIFICATIONSV2
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetTickerMessages"));
            }
        }

        public async Task<DataTable> GetC2SPostpaidTransactions(TransactionsRequest model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_MSISDN", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.iTopUpNumber });
                _mySql.AddParameter(new MySqlParameter("P_SUBSCRIBER_NO", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.subscriberNo });
                _mySql.AddParameter(new MySqlParameter("P_FROMDATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.startDate });
                _mySql.AddParameter(new MySqlParameter("P_TODATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.endDate });
                _mySql.AddParameter(new MySqlParameter("P_SORTBY_DATE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.sortByDate });
                _mySql.AddParameter(new MySqlParameter("P_SORTBY_AMOUNT", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.sortByAmt });
                _mySql.AddParameter(new MySqlParameter("P_SORTBY_INOUT", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.sortByInOut });

                DataTable dt = await _mySql.CallStoredProcedureSelectAsync("GET_POSTPAID_TRANSACTIONS");
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetC2SPostpaidTransactions"));
            }
        }


        public async Task<DataTable> GetCampaignList(CampaignRequestV3 campaignRequest)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = campaignRequest.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = campaignRequest.userId });

                var result = await _mySql.CallStoredProcedureSelectAsync("GET_EXT_CAMP_LIST_V2");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampaignList"));
            }
        }


        public async Task<DataTable> GetSelfCampaignList(CampaignRequestV3 campaignRequest)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = campaignRequest.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = campaignRequest.userId });

                var result = await _mySql.CallStoredProcedureSelectAsync("GET_SELF_CAMPAIGN_LIST");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampaignList"));
            }
        }


        public async Task<long> SaveDigitalService(DigitalServiceSubmitRequest model, int userId)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_PRODUCT_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = model.productId });
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_SUBSCRIBER_NUMBER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.subscriberNumber });
                _mySql.AddParameter(new MySqlParameter("P_LAT", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = model.lat });
                _mySql.AddParameter(new MySqlParameter("P_LNG", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = model.lng });
                _mySql.AddParameter(new MySqlParameter("P_USER_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = userId });

                long result = await _mySql.InsertByStoredProcedureAsync("RSL_SAVE_DIGITAL_SERVICE");

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveDigitalService"));
            }
        }

        public async Task<DataTable> GetCommunications(CommunicationRequestV4 retailer)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.retailerCode });
                _db.AddParameter(new OracleParameter("P_TYPE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.type.ToUpper() });
                _db.AddParameter(new OracleParameter("P_SEARCH_TEXT", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.searchText });
                _db.AddParameter(new OracleParameter("P_SORT_TYPE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.sortType });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("GET_COMMUNICATION_LIST");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCommunications"));
            }
        }


        public async Task<DataTable> BestPracticesHistory(RetailerRequestV2 retailer)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.retailerCode });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSL_GET_BP_HISTORY");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "BestPracticesHistory"));
            }
        }


        public async Task<long> SaveProductRating(SubmitProductRating model)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });
                _db.AddParameter(new OracleParameter("P_PRODUCT_TYPE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.offerType });
                _db.AddParameter(new OracleParameter("P_AMOUNT", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.amount });
                _db.AddParameter(new OracleParameter("P_RATING", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.rating });
                _db.AddParameter(new OracleParameter("P_USER_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.userId });

                var result = _db.CallStoredProcedureInsertV2("RSL_SAVE_PROD_RATING");
                return result.GetValueOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveProductRating"));
            }
        }


        public async Task<DataTable> GetDisclaimerNotices(string lan)
        {
            _db.AddParameter(new OracleParameter("P_LAN", OracleDbType.Varchar2, ParameterDirection.Input) { Value = lan, IsNullable = true });
            _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLGETALLDISCLAIMER");
            return result;
        }


        public async Task<int> DeleteContact(long contactId)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_CONTACT_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = contactId });

                string _outParamName = nameof(FrequentlyUsedDbParams.P_RETURN);
                MySqlParameter param = new(_outParamName, MySqlDbType.Int32) { Direction = ParameterDirection.Output };
                _mySql.AddParameter(param);

                object procReturn = await _mySql.CallStoredProcedureObjectAsync("DELETE_CONTACT", _outParamName);
                int result = Convert.ToInt32(procReturn.ToString());

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "DeleteContact"));
            }
        }


        public async Task<DataTable> LoadAllDeviceTokenByRetailer(string itopupNumber)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_ITOPUP_NUMBER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = itopupNumber });

                var result = await _mySql.CallStoredProcedureSelectAsync("GET_RETAILER_DEVICE_TOKENS");

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "LoadAllDeviceTokenByRetailer"));
            }
        }


        public async Task<long> SubmitGamificationResponse(GamificationResponseReq req)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = req.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_CAMPAIGN_NAME", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = req.campaignName });
                _mySql.AddParameter(new MySqlParameter("P_PLAY_TIME", MySqlDbType.DateTime) { Direction = ParameterDirection.Input, Value = req.playTime });

                long result = await _mySql.InsertByStoredProcedureAsync("SUBMIT_GAMIFICATION_RESPONSE");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SubmitGamificationResponse"));
            }
        }


        public async Task<DataTable> GetC2SOtfTransactions(TransactionsRequest model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_MSISDN", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.iTopUpNumber });
                _mySql.AddParameter(new MySqlParameter("P_SUBSCRIBER_NO", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.subscriberNo });
                _mySql.AddParameter(new MySqlParameter("P_FROM_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.startDate });
                _mySql.AddParameter(new MySqlParameter("P_TO_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.endDate });

                DataTable dt = await _mySql.CallStoredProcedureSelectAsync("GET_OTF_TRANSACTIONS");
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetC2SOtfTransactions"));
            }
        }

    }
}