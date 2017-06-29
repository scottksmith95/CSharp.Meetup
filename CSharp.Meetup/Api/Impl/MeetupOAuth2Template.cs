﻿#region License

/*
 * Copyright 2002-2012 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using System.Collections.Generic;
using CSharp.Meetup.Api.Interfaces;
using Spring.Json;
using Spring.Rest.Client;
using Spring.Social.OAuth2;
using Spring.Http.Converters;
using Spring.Http.Converters.Json;

namespace CSharp.Meetup.Api.Impl
{
    /// <summary>
    /// This is the central class for interacting with Meetup.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All Meetup operations require OAuth authentication. 
    /// To perform such operations, <see cref="MeetupOAuth2Template"/> must be constructed 
    /// with the minimal amount of information required to sign requests to Meetup's API 
    /// with an OAuth <code>Authorization</code> header.
    /// </para>
    /// </remarks>
    /// <author>Scott Smith</author>
    public sealed class MeetupOAuth2Template : AbstractOAuth2ApiBinding, IMeetup 
    {
		//private static readonly Uri ApiUriBase = new Uri("https://api.meetup.com");

        /// <summary>
        /// Create a new instance of <see cref="MeetupOAuth2Template"/>.
        /// </summary>
        /// <param name="accessToken">An access token acquired through OAuth authentication with Meetup.</param>
        public MeetupOAuth2Template(string accessToken) 
            : base(accessToken)
        {
            InitSubApis();
	    }

        #region IMeetup Members

        /// <summary>
        /// Gets the underlying <see cref="IRestOperations"/> object allowing for consumption of Meetup endpoints 
        /// that may not be otherwise covered by the API binding. 
        /// </summary>
        /// <remarks>
        /// The <see cref="IRestOperations"/> object returned is configured to include an OAuth "Authorization" header on all requests.
        /// </remarks>
        public IRestOperations RestOperations
        {
            get { return RestTemplate; }
        }

        #endregion

        /// <summary>
        /// Enables customization of the <see cref="RestTemplate"/> used to consume provider API resources.
        /// </summary>
        /// <remarks>
        /// An example use case might be to configure a custom error handler. 
        /// Note that this method is called after the RestTemplate has been configured with the message converters returned from GetMessageConverters().
        /// </remarks>
        /// <param name="restTemplate">The RestTemplate to configure.</param>
        protected override void ConfigureRestTemplate(RestTemplate restTemplate)
        {
            //restTemplate.BaseAddress = ApiUriBase;
            restTemplate.ErrorHandler = new MeetupErrorHandler();
        }

        /// <summary>
        /// Returns a list of <see cref="IHttpMessageConverter"/>s to be used by the internal <see cref="RestTemplate"/>.
        /// </summary>
        /// <remarks>
        /// This implementation adds <see cref="SpringJsonHttpMessageConverter"/> and <see cref="ByteArrayHttpMessageConverter"/> to the default list.
        /// </remarks>
        /// <returns>
        /// The list of <see cref="IHttpMessageConverter"/>s to be used by the internal <see cref="RestTemplate"/>.
        /// </returns>
        protected override IList<IHttpMessageConverter> GetMessageConverters()
        {
            IList<IHttpMessageConverter> converters = base.GetMessageConverters();
            converters.Add(new ByteArrayHttpMessageConverter());
            converters.Add(GetJsonMessageConverter());
            return converters;
        }

        /// <summary>
        /// Returns a <see cref="SpringJsonHttpMessageConverter"/> to be used by the internal <see cref="RestTemplate"/>.
        /// <para/>
        /// Override to customize the message converter (for example, to set a custom object mapper or supported media types).
        /// </summary>
        /// <returns>The configured <see cref="SpringJsonHttpMessageConverter"/>.</returns>
        private SpringJsonHttpMessageConverter GetJsonMessageConverter()
        {
            var jsonMapper = new JsonMapper();

            return new SpringJsonHttpMessageConverter(jsonMapper);
        }

        private void InitSubApis()
        {
        }
    }
}