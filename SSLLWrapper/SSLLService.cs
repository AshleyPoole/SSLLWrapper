﻿using System;
using SSLLWrapper.Domain;
using SSLLWrapper.External;
using SSLLWrapper.Interfaces;
using SSLLWrapper.Models.Response;
using SSLLWrapper.Models.Response.BaseSubModels;

namespace SSLLWrapper
{
    public class SSLLService
    {
	    #region construction

	    private readonly IApiProvider _apiProvider;
	    private readonly RequestModelFactory _requestModelFactory;
	    private readonly ResponsePopulation _responsePopulation;
		private readonly UrlValidation _urlValidation;
	    private string ApiUrl { get; set; }

	    public enum Publish
	    {
		    On,
		    Off
	    }

	    public enum ClearCache
	    {
		    On,
		    Off,
			Ignore
	    }

	    public enum FromCache
	    {
		    On,
		    Off,
			Ignore
	    }

	    public enum All
	    {
		    On,
		    Done
	    }

		public SSLLService(string apiUrl) : this(apiUrl, new SSLLabsApi())
	    {
	    }

		internal SSLLService(string apiUrl, IApiProvider apiProvider)
	    {
			_apiProvider = apiProvider;
			_requestModelFactory = new RequestModelFactory();
		    _urlValidation = new UrlValidation();
			_responsePopulation = new ResponsePopulation();
			ApiUrl = _urlValidation.Format(apiUrl);
	    }

		#endregion

		public Info Info()
		{
			var infoModel = new Info();
		    var requestModel = _requestModelFactory.NewInfoRequestModel(ApiUrl, "info");

			try
			{
				// Making Api request and binding result to model
				var webResponse = _apiProvider.MakeGetRequest(requestModel);
				infoModel = _responsePopulation.InfoModel(webResponse, infoModel);

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

			if (infoModel.Errors.Count != 0 && !infoModel.HasErrorOccurred) { infoModel.HasErrorOccurred = true;}

			return infoModel;
		}

	    public Analyze Analyze(string host)
	    {
			// overloaded method to provide a default set of options
		    return Analyze(host, Publish.Off, ClearCache.On, FromCache.Ignore, All.On);
	    }

		public Analyze Analyze(string host, Publish publish, ClearCache clearCache, FromCache fromCache, All all)
		{
			var analyzeModel = new Analyze();

			// Checking host is valid before continuing
			if (!_urlValidation.IsValid(host))
			{
				analyzeModel.HasErrorOccurred = true;
				analyzeModel.Errors.Add(new Error {message = "Host does not pass preflight validation. No Api call has been made."});
				return analyzeModel;
			}

			// Building request model
			var requestModel = _requestModelFactory.NewAnalyzeRequestModel(ApiUrl, "analyze", host, publish.ToString().ToLower(), clearCache.ToString().ToLower(), 
				fromCache.ToString().ToLower(), all.ToString().ToLower());

			try
			{
				var webResponse = _apiProvider.MakeGetRequest(requestModel);
				analyzeModel = _responsePopulation.AnalyzeModel(webResponse, analyzeModel);
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

		public Endpoint GetEndpointData(string host, string s)
		{
			return GetEndpointData(host, s, FromCache.Off);
		}

		public Endpoint GetEndpointData(string host, string s, FromCache fromCache)
	    {
		    var endpointModel = new Endpoint();

			// Checking host is valid before continuing
			if (!_urlValidation.IsValid(host))
			{
				endpointModel.HasErrorOccurred = true;
				endpointModel.Errors.Add(new Error { message = "Host does not pass preflight validation. No Api call has been made." });
				return endpointModel;
			}

			// Building request model
			var requestModel = _requestModelFactory.NewEndpointDataRequestModel(ApiUrl, "getEndpointData", host, s,
				fromCache.ToString());

			try
			{
				var webResponse = _apiProvider.MakeGetRequest(requestModel);
				endpointModel = _responsePopulation.EndpointModel(webResponse, endpointModel);
			}
			catch (Exception ex)
			{
				endpointModel.HasErrorOccurred = true;
				endpointModel.Errors.Add(new Error { message = ex.ToString() });
			}

			// Checking if errors have occoured either from ethier api or wrapper
			if (endpointModel.Errors.Count != 0 && !endpointModel.HasErrorOccurred) { endpointModel.HasErrorOccurred = true; }

		    return endpointModel;
	    }

	    public StatusCodes GetStatusCodes()
	    {
			var statusCodesModel = new StatusCodes();
		    var requestModel = _requestModelFactory.NewStatusCodesRequestModel(ApiUrl, "getStatusCodes");

		    try
		    {
			    var webResponse = _apiProvider.MakeGetRequest(requestModel);
			    statusCodesModel = _responsePopulation.StatusCodesModel(webResponse, statusCodesModel);
		    }
		    catch (Exception ex)
		    {
				statusCodesModel.HasErrorOccurred = true;
				statusCodesModel.Errors.Add(new Error { message = ex.ToString() });
		    }

		    return statusCodesModel;
	    }
    }
}
