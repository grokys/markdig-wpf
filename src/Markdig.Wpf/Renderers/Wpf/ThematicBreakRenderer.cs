﻿// Copyright (c) 2016 Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license. 
// See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Documents;
using Markdig.Syntax;
using Markdig.Wpf;

namespace Markdig.Renderers.Wpf
{
    public class ThematicBreakRenderer : WpfObjectRenderer<ThematicBreakBlock>
    {
        protected override void Write(WpfRenderer renderer, ThematicBreakBlock obj)
        {
            var line = new System.Windows.Shapes.Line { X2 = 1 };
            line.SetResourceReference(Paragraph.StyleProperty, Styles.ThematicBreakStyleKey);

            var paragraph = new Paragraph
            {
                Inlines = { new InlineUIContainer(line) }
            };

            renderer.Push(paragraph);
            renderer.Pop();
        }
    }
}