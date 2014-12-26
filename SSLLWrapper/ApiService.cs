﻿using System;
using Newtonsoft.Json;
using SSLLWrapper.Helpers;
using SSLLWrapper.Interfaces;
using SSLLWrapper.Models.Response;
using SSLLWrapper.Models.Response.BaseResponseSubModels;

namespace SSLLWrapper
{
    public class ApiService
    {
	    #region construction

	    private readonly IApi _api;
	    private readonly IHttpWebResponseHelper _webResponseHelper;
	    private readonly IRequestModelHelper _requestModelHelper;
	    private readonly IResponsePopulationHelper _responsePopulationHelper;
		private readonly IUrlHelper _urlHelper;
	    private string ApiUrl { get; set; }

	    public enum Publish
	    {
		    On,
		    Off
	    }

	    public enum ClearCache
	    {
		    On,
		    Off
	    }

	    public enum FromCache
	    {
		    On,
		    Off
	    }

	    public enum All
	    {
		    On,
		    Done
	    }

	    public ApiService(string apiUrl)
		{
			_api = new Api();
			_webResponseHelper = new HttpWebResponseHelper();
			_requestModelHelper = new RequestModelHelper();
		    _urlHelper = new UrlHelper();
			_responsePopulationHelper = new ResponsePopulationHelper();

		    ApiUrl = apiUrl;
		}

		#endregion

		public InfoModel Info()
		{
			var infoModel = new InfoModel();

			// Building new request model
		    var requestModel = _requestModelHelper.InfoProperties(ApiUrl, "info");

			try
			{
				// Making Api request and gathering response
				var webResponse = _api.MakeGetRequest(requestModel);

				// Binding result to model
				_responsePopulationHelper.InfoModel(webResponse, infoModel);

				if (infoModel.engineVersion != null)
				{
					infoModel.Online = true;
				}
			}
			catch (Exception ex)
			{
				infoModel.HasErrorOccurred = true;
				infoModel.Errors.Add(new Error { message = ex.ToString() });
			}

			// Checking if errors have occoured either from ethier api or wrapper
			if (infoModel.Errors.Count != 0 && !infoModel.HasErrorOccurred) { infoModel.HasErrorOccurred = true;}

			return infoModel;
		}

	    public AnalyzeModel Analyze(string host)
	    {
			// overloaded method to provide a default set of options
		    return Analyze(host, Publish.Off, ClearCache.On, FromCache.Off, All.On);
	    }

		public AnalyzeModel Analyze(string host, Publish publish, ClearCache clearCache, FromCache fromCache, All all)
		{
			var analyzeModel = new AnalyzeModel();

			// Checking host is valid before continuing
			if (!_urlHelper.IsValid(host))
			{
				analyzeModel.HasErrorOccurred = true;
				analyzeModel.Errors.Add(new Error {message = "Host does not pass preflight validation. No Api call has been made."});
				return analyzeModel;
			}

			// Building request model
			var requestModel = _requestModelHelper.AnalyzeProperties(ApiUrl, "analyze", host, publish.ToString().ToLower(), clearCache.ToString().ToLower(), 
				fromCache.ToString().ToLower(), all.ToString().ToLower());

			try
			{
				// Making Api request and gathering response
				var webResponse = _api.MakeGetRequest(requestModel);

				// Binding result to model
				_responsePopulationHelper.AnalyzeModel(webResponse, analyzeModel);
			}
			catch (Exception ex)
			{
				analyzeModel.HasErrorOccurred = true;
				analyzeModel.Errors.Add(new Error {message = ex.ToString()});
			}

			// Checking if errors have occoured either from ethier api or wrapper
			if (analyzeModel.Errors.Count != 0 && !analyzeModel.HasErrorOccurred) { analyzeModel.HasErrorOccurred = true; }

			return analyzeModel;
		}

		public EndpointDataModel GetEndpointData(string host, string s)
		{
			return GetEndpointData(host, s, FromCache.Off);
		}

		public EndpointDataModel GetEndpointData(string host, string s, FromCache fromCache)
	    {
		    var endpointDataModel = new EndpointDataModel();

			// Checking host is valid before continuing
			if (!_urlHelper.IsValid(host))
			{
				endpointDataModel.HasErrorOccurred = true;
				endpointDataModel.Errors.Add(new Error { message = "Host does not pass preflight validation. No Api call has been made." });
				return endpointDataModel;
			}

			// Building request model
			var requestModel = _requestModelHelper.GetEndpointDataProperties(ApiUrl, "getEndpointData", host, s,
				fromCache.ToString());

			try
			{
				// Making Api request and gathering response
				var webResponse = _api.MakeGetRequest(requestModel);

				// Binding result to model
				_responsePopulationHelper.EndpointDataModel(webResponse, endpointDataModel);
			}
			catch (Exception ex)
			{
				endpointDataModel.HasErrorOccurred = true;
				endpointDataModel.Errors.Add(new Error { message = ex.ToString() });
			}

			// Checking if errors have occoured either from ethier api or wrapper
			if (endpointDataModel.Errors.Count != 0 && !endpointDataModel.HasErrorOccurred) { endpointDataModel.HasErrorOccurred = true; }

		    return endpointDataModel;
	    }

	    public string GetStatusCodes()
	    {
		    throw new NotImplementedException();
	    }
    }
}
