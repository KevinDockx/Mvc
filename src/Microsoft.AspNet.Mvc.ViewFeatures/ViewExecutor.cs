﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Actions;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Framework.Internal;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Utility type for rendering a <see cref="IView"/> to the response.
    /// </summary>
    public static class ViewExecutor
    {
        public static readonly MediaTypeHeaderValue DefaultContentType = new MediaTypeHeaderValue("text/html")
        {
            Encoding = Encoding.UTF8
        }.CopyAsReadOnly();

        /// <summary>
        /// Asynchronously renders the specified <paramref name="view"/> to the response body.
        /// </summary>
        /// <param name="view">The <see cref="IView"/> to render.</param>
        /// <param name="actionContext">The <see cref="ActionContext"/> for the current executing action.</param>
        /// <param name="viewData">The <see cref="ViewDataDictionary"/> for the view being rendered.</param>
        /// <param name="tempData">The <see cref="ITempDataDictionary"/> for the view being rendered.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous rendering.</returns>
        public static async Task ExecuteAsync([NotNull] IView view,
                                              [NotNull] ActionContext actionContext,
                                              [NotNull] ViewDataDictionary viewData,
                                              [NotNull] ITempDataDictionary tempData,
                                              [NotNull] HtmlHelperOptions htmlHelperOptions,
                                              MediaTypeHeaderValue contentType)
        {
            var response = actionContext.HttpContext.Response;

            if (contentType != null && contentType.Encoding == null)
            {
                // Do not modify the user supplied content type, so copy it instead
                contentType = contentType.Copy();
                contentType.Encoding = Encoding.UTF8;
            }

            response.ContentType = contentType?.ToString() ?? response.ContentType ?? DefaultContentType.ToString();

            using (var writer = new HttpResponseStreamWriter(response.Body, contentType?.Encoding ?? DefaultContentType.Encoding))
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    viewData,
                    tempData,
                    writer,
                    htmlHelperOptions);

                await view.RenderAsync(viewContext);
            }
        }
    }
}