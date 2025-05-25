///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	14-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.RequestModel;
using Domain.ViewModel;
using Domain.ViewModel.LogModels;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.StaticClass
{
    public static class MappingConfig
    {
        public static void RegisterMapping(this IServiceCollection services)
        {
            TypeAdapterConfig<EVRechargeRequest, EvLogViewModel>
                .NewConfig()
                .Map(dest => dest.amount, src => src.amount)
                .Map(dest => dest.subMSISDN, src => src.subscriberNo);

            TypeAdapterConfig<ItopUpXmlRequest, EvLogViewModel>
                .NewConfig()
                .Map(dest => dest.amount, src => src.Amount)
                .Map(dest => dest.retMSISDN, src => src.Msisdn)
                .Map(dest => dest.subMSISDN, src => src.Msisdn2);

            TypeAdapterConfig<ItopUpXmlRequestV2, EvLogViewModel>
                .NewConfig()
                .Map(dest => dest.subMSISDN, src => src.Msisdn)
                .Map(dest => dest.retMSISDN, src => src.Msisdn2);

            TypeAdapterConfig<IrisRechargeRequest, IRISLogViewModel>
                .NewConfig()
                .Map(dest => dest.subMSISDN, src => src.subscriberNo);

            TypeAdapterConfig<IRISJSONRequestModel, IRISLogViewModel>
                .NewConfig()
                .Map(dest => dest.tranId, src => src.request.transactionID)
                .Map(dest => dest.retMSISDN, src => src.request.retailerMsisdn)
                .Map(dest => dest.subMSISDN, src => src.request.subscriberMsisdn);

            TypeAdapterConfig<IrisContinueRechargeRequest, IRISLogViewModel>
                .NewConfig()
                .Map(dest => dest.tranId, src => src.request.transactionID)
                .Map(dest => dest.amount, src => src.request.rechargeAmount)
                .Map(dest => dest.retMSISDN, src => src.request.retailerMsisdn)
                .Map(dest => dest.subMSISDN, src => src.request.subscriberMsisdn);

            TypeAdapterConfig<EvPinChangeXMLRequest, EvLogViewModel>
                .NewConfig()
                .Map(dest => dest.retMSISDN, src => src.msisdn);

            TypeAdapterConfig<IrisOfferRequest, IRISLogViewModel>
                .NewConfig()
                .Map(dest => dest.subMSISDN, src => src.subscriberNo);

            TypeAdapterConfig<RetailerRequestV2, IRISLogViewModel>
                .NewConfig()
                .Map(dest => dest.retailerCode, src => src.retailerCode)
                .Map(dest => dest.retMSISDN, src => src.iTopUpNumber);

        }
    }
}