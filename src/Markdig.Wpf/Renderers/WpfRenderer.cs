﻿// Copyright (c) 2016-2017 Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license. 
// See the LICENSE.md file in the project root for more information.

using System.Collections.Generic;
using System.Windows.Documents;
using Markdig.Helpers;
using Markdig.Renderers.Wpf;
using Markdig.Renderers.Wpf.Inlines;
using System.Runtime.CompilerServices;
using System;
using Markdig.Annotations;
using System.Linq;
using Markdig.Wpf;
using System.Windows.Markup;
using Markdig.Renderers.Wpf.Extensions;

namespace Markdig.Renderers
{
    /// <summary>
    /// WPF renderer for a Markdown <see cref="Syntax.MarkdownDocument"/> object.
    /// </summary>
    /// <seealso cref="Renderers.RendererBase" />
    public class WpfRenderer : RendererBase
    {
        private readonly Stack<IAddChild> stack = new Stack<IAddChild>();
        private char[] buffer;

        public WpfRenderer(FlowDocument document)
        {
            buffer = new char[1024];
            Document = document;
            document.SetResourceReference(Paragraph.StyleProperty, Styles.DocumentStyleKey);
            stack.Push(document);

            // Default block renderers
            ObjectRenderers.Add(new CodeBlockRenderer());
            ObjectRenderers.Add(new ListRenderer());
            ObjectRenderers.Add(new HeadingRenderer());
            ObjectRenderers.Add(new ParagraphRenderer());
            ObjectRenderers.Add(new QuoteBlockRenderer());
            ObjectRenderers.Add(new ThematicBreakRenderer());

            // Default inline renderers
            ObjectRenderers.Add(new AutolinkInlineRenderer());
            ObjectRenderers.Add(new CodeInlineRenderer());
            ObjectRenderers.Add(new DelimiterInlineRenderer());
            ObjectRenderers.Add(new EmphasisInlineRenderer());
            ObjectRenderers.Add(new LineBreakInlineRenderer());
            ObjectRenderers.Add(new LinkInlineRenderer());
            ObjectRenderers.Add(new LiteralInlineRenderer());

            // Extension renderers
            ObjectRenderers.Add(new TaskListRenderer());
        }

        public FlowDocument Document { get; }

        /// <inheritdoc/>
        public override object Render(Syntax.MarkdownObject markdownObject)
        {
            Write(markdownObject);
            return Document;
        }

        /// <summary>
        /// Writes the inlines of a leaf inline.
        /// </summary>
        /// <param name="leafBlock">The leaf block.</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteLeafInline([NotNull] Syntax.LeafBlock leafBlock)
        {
            if (leafBlock == null) throw new ArgumentNullException(nameof(leafBlock));
            var inline = (Syntax.Inlines.Inline)leafBlock.Inline;
            if (inline != null)
            {
                while (inline != null)
                {
                    Write(inline);
                    inline = inline.NextSibling;
                }
            }
        }

        /// <summary>
        /// Writes the lines of a <see cref="LeafBlock"/>
        /// </summary>
        /// <param name="leafBlock">The leaf block.</param>
        /// <param name="writeEndOfLines">if set to <c>true</c> write end of lines.</param>
        /// <param name="escape">if set to <c>true</c> escape the content for HTML</param>
        /// <param name="softEscape">Only escape &lt; and &amp;</param>
        /// <returns>This instance</returns>
        public void WriteLeafRawLines([NotNull] Syntax.LeafBlock leafBlock)
        {
            if (leafBlock == null) throw new ArgumentNullException(nameof(leafBlock));
            if (leafBlock.Lines.Lines != null)
            {
                var lines = leafBlock.Lines;
                var slices = lines.Lines;
                for (int i = 0; i < lines.Count; i++)
                {
                    WriteText(ref slices[i].Slice);
                    WriteInline(new LineBreak());
                }
            }
        }

        internal void Push(IAddChild o)
        {
            stack.Push(o);
        }

        internal void Pop()
        {
            var popped = stack.Pop();
            stack.Peek().AddChild(popped);
        }

        internal void WriteBlock(Block block)
        {
            stack.Peek().AddChild(block);
        }

        internal void WriteInline(Inline inline)
        {
            AddInline(stack.Peek(), inline);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteText(ref StringSlice slice)
        {
            if (slice.Start > slice.End)
                return;

            WriteText(slice.Text, slice.Start, slice.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteText(string text)
        {
            WriteInline(new Run(text));
        }

        internal void WriteText([CanBeNull] string text, int offset, int length)
        {
            if (text == null)
                return;

            if (offset == 0 && text.Length == length)
            {
                WriteText(text);
            }
            else
            {
                if (length > buffer.Length)
                {
                    buffer = text.ToCharArray();
                    WriteText(new string(buffer, offset, length));
                }
                else
                {
                    text.CopyTo(offset, buffer, 0, length);
                    WriteText(new string(buffer, 0, length));
                }
            }
        }

        private void AddInline(IAddChild parent, Inline inline)
        {
            if (!EndsWithSpace(parent) && !StartsWithSpace(inline))
            {
                parent.AddText(" ");
            }

            parent.AddChild(inline);
        }

        private bool StartsWithSpace(Inline inline)
        {
            var run = inline as Run;
            var span = inline as Span;

            if (run != null)
            {
                return run.Text.Length == 0 || run.Text.First().IsWhitespace();
            }
            else if (span != null)
            {
                return StartsWithSpace(span.Inlines.FirstInline);
            }

            return true;
        }

        private bool EndsWithSpace(IAddChild element)
        {
            var inlines = (element as Span)?.Inlines ?? (element as Paragraph)?.Inlines;
            var run = inlines?.LastInline as Run;
            var span = inlines?.LastInline as Span;

            if (run != null)
            {
                return run.Text.Length == 0 || run.Text.Last().IsWhitespace();
            }
            else if (span != null)
            {
                return EndsWithSpace(span);
            }

            return true;
        }
    }
}
