﻿namespace SSLLWrapper.Models
{
	public class InfoModel
	{
		public string engineVersion { get; set; }
		public string criteriaVersion { get; set; }
		public int clientMaxAssessments { get; set; }
		public string notice { get; set; }
		public bool Online { get; set; }

		public InfoModel()
		{
			// Assigning default online status
			this.Online = false;
		}
	}
}
