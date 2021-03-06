﻿
using System;
using System.Linq;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.TypeScriptBinding;
using ICSharpCode.TypeScriptBinding.Hosting;
using Rhino.Mocks;

namespace TypeScriptBinding.Tests.Parsing
{
	public abstract class ParseTests
	{
		public ICompilationUnit CompilationUnit { get; private set; }
		public IProjectContent ProjectContent { get; private set; }
		
		public TypeScriptCompilationUnit TypeScriptCompilationUnit {
			get { return CompilationUnit as TypeScriptCompilationUnit; }
		}
		
		public void Parse(string text, string fileName = @"d:\projects\MyProject\test.ts")
		{
			ProjectContent = MockRepository.GenerateStub<IProjectContent>();
			var textBuffer = new StringTextBuffer(text);
			
			var scriptLoader = new ParseTestScriptLoader();
			var logger = new LanguageServiceLogger();
			ITypeScriptContextFactory contextFactory = MockRepository.GenerateStub<ITypeScriptContextFactory>();
			contextFactory
				.Stub(f => f.CreateContext())
				.Return(new TypeScriptContext(scriptLoader, logger));
			
			var parser = new TypeScriptParser(contextFactory);
			CompilationUnit = parser.Parse(ProjectContent, fileName, textBuffer);
		}
		
		public IClass GetFirstClass()
		{
			return GetClassAtIndex(0);
		}
		
		public IClass GetSecondClass()
		{
			return GetClassAtIndex(1);
		}
		
		public IClass GetClassAtIndex(int index)
		{
			return CompilationUnit.Classes.Skip(index).First();
		}
	}
}
