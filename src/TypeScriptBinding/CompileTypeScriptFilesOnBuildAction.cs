﻿// 
// CompileTypeScriptFilesOnBuildFileAction.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2013 Matthew Ward
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.TypeScriptBinding.Hosting;
using MonoDevelop.Core;
using MonoDevelop.Projects;

namespace ICSharpCode.TypeScriptBinding
{
	public class CompileTypeScriptFilesOnBuildAction : CompileTypeScriptAction
	{
		public void CompileFiles(IBuildTarget buildTarget)
		{
			using (IProgressMonitor progressMonitor = GetRunProcessMonitor()) {
				foreach (TypeScriptProject project in buildTarget.GetTypeScriptProjects()) {
					if (project.CompileOnBuild) {
						CompileFiles(project);
					}
				}
			}
		}
		
		void CompileFiles(TypeScriptProject project)
		{
			FilePath[] fileNames = project.GetTypeScriptFileNames().ToArray();
			if (fileNames.Length == 0)
				return;
			
			CompileFiles(project, fileNames);
		}
		
		void CompileFiles(TypeScriptProject project, FilePath[] fileNames)
		{
			ReportCompileStarting(project);
			
			var compiler = new TypeScriptCompiler(project);
			compiler.AddFiles(fileNames);
			
			Report(compiler.GetCommandLine());
			
			TypeScriptCompilerResult result = compiler.Compile();
			UpdateProject(project, result.GeneratedFiles);
			
			ReportCompileFinished(result.HasErrors);
		}
		
		void ReportCompileStarting(TypeScriptProject project)
		{
			Report("Compiling TypeScript files for project: {0}", project.Name);
		}
		
		void UpdateProject(TypeScriptProject project, IEnumerable<GeneratedTypeScriptFile> generatedFiles)
		{
			project.AddMissingFiles(generatedFiles);
		}
	}
}
