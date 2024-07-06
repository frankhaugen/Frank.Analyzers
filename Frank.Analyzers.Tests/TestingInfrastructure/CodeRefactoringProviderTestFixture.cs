﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Text;

namespace Frank.Analyzers.Tests.TestingInfrastructure
{
    public abstract class CodeRefactoringProviderTestFixture : CodeActionProviderTestFixture
    {
        private IEnumerable<CodeAction> GetRefactoring(Document document, TextSpan span)
        {
            CodeRefactoringProvider provider = CreateCodeRefactoringProvider;
            List<CodeAction> actions = new List<CodeAction>();
            CodeRefactoringContext context = new CodeRefactoringContext(document, span, (a) => actions.Add(a), CancellationToken.None);
            provider.ComputeRefactoringsAsync(context).Wait();
            return actions;
        }

        protected void TestNoActions(string markup)
        {
            if (!markup.Contains('\r'))
            {
                markup = markup.Replace("\n", "\r\n");
            }

            MarkupTestFile.GetSpan(markup, out string code, out TextSpan span);

            Document document = CreateDocument(code);
            IEnumerable<CodeAction> actions = GetRefactoring(document, span);

            Assert.True(actions == null || actions.Count() == 0);
        }

        protected void Test(
            string markup,
            string expected,
            int actionIndex = 0,
            bool compareTokens = false)
        {
            if (!markup.Contains('\r'))
            {
                markup = markup.Replace("\n", "\r\n");
            }

            if (!expected.Contains('\r'))
            {
                expected = expected.Replace("\n", "\r\n");
            }

            MarkupTestFile.GetSpan(markup, out string code, out TextSpan span);

            Document document = CreateDocument(code);
            IEnumerable<CodeAction> actions = GetRefactoring(document, span);

            Assert.NotNull(actions);

            CodeAction action = actions.ElementAt(actionIndex);
            Assert.NotNull(action);

            var operations = action.GetOperationsAsync(CancellationToken.None).GetAwaiter().GetResult();
            
            Assert.NotEmpty(operations);
            
            var applyChangesOperation = operations.OfType<ApplyChangesOperation>().ToList();
            Assert.NotNull(applyChangesOperation);
            Assert.NotEmpty(applyChangesOperation);
            
            ApplyChangesOperation edit = applyChangesOperation.First();
            VerifyDocument(expected, compareTokens, edit.ChangedSolution.GetDocument(document.Id));
        }

        protected abstract CodeRefactoringProvider CreateCodeRefactoringProvider { get; }
    }
}
