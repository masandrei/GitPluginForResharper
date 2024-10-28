using System;
using System.Linq;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.Util;
using JetBrains.Util.dataStructures.TypedIntrinsics;

namespace ReSharperPlugin.Git;

[HighlightingSource]
[StaticSeverityHighlighting(Severity.SUGGESTION, typeof(GitHighlighting))]
public class GitHighlighting : IHighlighting
{
    private readonly string _commitMessage;
    private readonly int _lineNumber;
    private readonly IDocument _document;
    public static int TaskLimit = 5;
    public string ToolTip => _commitMessage;
    public string ErrorStripeToolTip => ToolTip;
    public GitHighlighting(string commitMessage , int lineNumber, IDocument document)
    {
        _commitMessage = commitMessage;
        _lineNumber = lineNumber;
        _document = document;
    }

    public bool IsValid() => _lineNumber >= 0 && _lineNumber < (int)_document.GetLineCount();

    public DocumentRange CalculateRange()
    {
        var int32LineNumber = (Int32<DocLine>)_lineNumber;
        var line = _document.GetLineText(int32LineNumber);
        int startOffset = _document.GetLineStartOffset(int32LineNumber) +
                          line.TakeWhile(ch => char.IsWhiteSpace(ch)).Count();
        int endOffset = _document.GetLineEndOffsetNoLineBreak(int32LineNumber) -
                          line.Reverse().TakeWhile(ch => char.IsWhiteSpace(ch)).Count();
        // These lines are added for the sake of the task statement
        int symbolsUntilWhiteSpace = line.SkipWhile(char.IsWhiteSpace).TakeWhile(ch => !char.IsWhiteSpace(ch)).Count();
        int symbolsToHighlight = TaskLimit;
        if (symbolsUntilWhiteSpace + 1 == TaskLimit)
        {
            symbolsToHighlight = symbolsUntilWhiteSpace;
        }
        //
        var rangeToHighlight = new TextRange(startOffset, Math.Min(endOffset, startOffset + symbolsToHighlight));
        if (rangeToHighlight.IsValid)
        {
            TaskLimit -= symbolsToHighlight;
            return new DocumentRange(_document, rangeToHighlight);
        }

        return DocumentRange.InvalidRange;
    }

    
}