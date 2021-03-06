﻿// Copyright (c) 2016-2017 Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license. 
// See the LICENSE.md file in the project root for more information.

using System;
using Markdig.Annotations;

// ReSharper disable once CheckNamespace
namespace Markdig.Xaml
{
    /// <summary>
    /// Provides extension methods for <see cref="MarkdownPipeline"/> to enable several Markdown extensions.
    /// </summary>
    public static class MarkdownExtensions
    {
        /// <summary>
        /// Uses all extensions supported by <c>Markdig.Xaml</c>.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseSupportedExtensions([NotNull] this MarkdownPipelineBuilder pipeline)
        {
            if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));
            return pipeline
                .UseAutoLinks();
        }
    }
}
