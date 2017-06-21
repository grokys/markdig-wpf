﻿// Copyright (c) 2016-2017 Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license. 
// See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Documents;
using Markdig.Annotations;
using Markdig.Syntax.Inlines;
using Markdig.Wpf;

namespace Markdig.Renderers.Wpf.Inlines
{
    /// <summary>
    /// A WPF renderer for a <see cref="LinkInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Wpf.WpfObjectRenderer{Markdig.Syntax.Inlines.LinkInline}" />
    public class LinkInlineRenderer : WpfObjectRenderer<LinkInline>
    {
        /// <inheritdoc/>
        protected override void Write(WpfRenderer renderer, [NotNull] LinkInline link)
        {            
            var url = link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url : link.Url;

            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                url = "#";
            }

            var hyperlink = new Hyperlink
            {
                //Command = Commands.Hyperlink,
                CommandParameter = url,
                NavigateUri = new Uri(url, UriKind.RelativeOrAbsolute),
                ToolTip = link.Title != string.Empty ? link.Title : null,
            };

            renderer.Push(hyperlink);
            renderer.WriteChildren(link);
            renderer.Pop();
        }
    }
}