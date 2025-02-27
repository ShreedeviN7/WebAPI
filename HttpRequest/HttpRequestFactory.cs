﻿using WebAPITemplate.Helpers;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using JsonContent = WebAPITemplate.Helpers.JsonContent;

namespace WebAPITemplate.HttpRequest
{
    public static class HttpRequestFactory
    {
        public static async Task<HttpResponseMessage> Get(string requestUri)
            => await Get(requestUri, "");

        public static async Task<HttpResponseMessage> Get(string requestUri, string bearerToken)
        {
            try
            {
                var builder = new HttpRequestBuilder()
                                    .AddMethod(HttpMethod.Get)
                                    .AddRequestUri(requestUri)
                                    .AddBearerToken(bearerToken);

                HttpResponseMessage response = await builder.SendAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return response;
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new SessionExpiredException("Session Expired");
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException("Not Found");
                else
                    throw new InternalServerException("Internal Server Error");
            }
            catch (Exception ex)
            {
                if (ex is SessionExpiredException || ex is NotFoundException)
                    throw ex;
                else
                    throw new InternalServerException("Internal Server Error");
            }
        }

        public static async Task<HttpResponseMessage> Post(string requestUri, object value)
            => await Post(requestUri, value, "");

        public static async Task<HttpResponseMessage> Post(
            string requestUri, object value, string bearerToken)
        {
            try
            {
                var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddContent(new JsonContent(value))
                                .AddBearerToken(bearerToken);

                //return await builder.SendAsync();
                HttpResponseMessage response = await builder.SendAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return response;
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new SessionExpiredException("Session Expired");
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException("Not Found");
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    return response;
                else
                    throw new InternalServerException("Internal Server Error");
            }
            catch (Exception ex)
            {
                if (ex is SessionExpiredException || ex is NotFoundException)
                    throw ex;
                else
                    throw new InternalServerException("Internal Server Error");
            }
        }

        public static async Task<HttpResponseMessage> Put(string requestUri, object value)
            => await Put(requestUri, value, "");

        public static async Task<HttpResponseMessage> Put(
            string requestUri, object value, string bearerToken)
        {
            try
            {
                var builder = new HttpRequestBuilder()
                                    .AddMethod(HttpMethod.Put)
                                    .AddRequestUri(requestUri)
                                    .AddContent(new JsonContent(value))
                                    .AddBearerToken(bearerToken);

                //return await builder.SendAsync();
                HttpResponseMessage response = await builder.SendAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return response;
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new SessionExpiredException("Session Expired");
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException("Not Found");
                else
                    throw new InternalServerException("Internal Server Error");
            }
            catch (Exception ex)
            {
                if (ex is SessionExpiredException || ex is NotFoundException)
                    throw ex;
                else
                    throw new InternalServerException("Internal Server Error");
            }
        }

        public static async Task<HttpResponseMessage> Patch(string requestUri, object value)
            => await Patch(requestUri, value, "");

        public static async Task<HttpResponseMessage> Patch(
            string requestUri, object value, string bearerToken)
        {
            try
            {
                var builder = new HttpRequestBuilder()
                                    .AddMethod(new HttpMethod("PATCH"))
                                    .AddRequestUri(requestUri)
                                    .AddContent(new PatchContent(value))
                                    .AddBearerToken(bearerToken);

                //return await builder.SendAsync();
                HttpResponseMessage response = await builder.SendAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return response;
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new SessionExpiredException("Session Expired");
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException("Not Found");
                else
                    throw new InternalServerException("Internal Server Error");
            }
            catch (Exception ex)
            {
                if (ex is SessionExpiredException || ex is NotFoundException)
                    throw ex;
                else
                    throw new InternalServerException("Internal Server Error");
            }
        }

        public static async Task<HttpResponseMessage> Delete(string requestUri)
            => await Delete(requestUri, "");

        public static async Task<HttpResponseMessage> Delete(
            string requestUri, string bearerToken)
        {
            try
            {
                var builder = new HttpRequestBuilder()
                                    .AddMethod(HttpMethod.Delete)
                                    .AddRequestUri(requestUri)
                                    .AddBearerToken(bearerToken);

                //return await builder.SendAsync();
                HttpResponseMessage response = await builder.SendAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return response;
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new SessionExpiredException("Session Expired");
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException("Not Found");
                else
                    throw new InternalServerException("Internal Server Error");
            }
            catch (Exception ex)
            {
                if (ex is SessionExpiredException || ex is NotFoundException)
                    throw ex;
                else
                    throw new InternalServerException("Internal Server Error");
            }
        }

        public static async Task<HttpResponseMessage> PostFile(string requestUri,
            string filePath, string apiParamName)
            => await PostFile(requestUri, filePath, apiParamName, "");

        public static async Task<HttpResponseMessage> PostFile(string requestUri,
            string filePath, string apiParamName, string bearerToken)
        {
            try
            {
                var builder = new HttpRequestBuilder()
                                    .AddMethod(HttpMethod.Post)
                                    .AddRequestUri(requestUri)
                                    .AddContent(new FileContent(filePath, apiParamName))
                                    .AddBearerToken(bearerToken);

                //return await builder.SendAsync();
                HttpResponseMessage response = await builder.SendAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return response;
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new SessionExpiredException("Session Expired");
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException("Not Found");
                else
                    throw new InternalServerException("Internal Server Error");
            }
            catch (Exception ex)
            {
                if (ex is SessionExpiredException || ex is NotFoundException)
                    throw ex;
                else
                    throw new InternalServerException("Internal Server Error");
            }
        }
    }
}
