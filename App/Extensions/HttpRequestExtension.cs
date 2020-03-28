using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Extensions.HttpRequestExtensions
{
    public static partial class HttpRequestExtension
    {
		public static bool IsAjax(this HttpRequest req)
		{
			bool result = false;

			var xreq = req.Headers.ContainsKey("x-requested-with");
			if (xreq)
			{
				result = req.Headers["x-requested-with"] == "XMLHttpRequest";
			}

			return result;
		}

	}
}
