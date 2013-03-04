﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using Microsoft.VisualStudio.CodeCoverage;

namespace Toneri
{
	class Program
	{
		/// <summary>argument prefix: Input File Path</summary>
		private const string ARGS_PREFIX_INPUT_PATH = "/in:";

		/// <summary>argument prefix: Output File Path</summary>
		private const string ARGS_PREFIX_OUTPUT_PATH = "/out:";

		/// <summary>argument prefix: Symbols Directory</summary>
		private const string ARGS_PREFIX_SYMBOLS_DIR = "/symbols:";

		/// <summary>argument prefix: Exe Directory</summary>
		private const string ARGS_PREFIX_EXE_DIR = "/exedir:";

		/// <summary>argument prefix: Convert Xsl Path</summary>
		private const string ARGS_PREFIX_XSL_PATH = "/xsl:";

		/// <summary>
		/// Main process
		/// </summary>
		/// <param name="args">Command Line Arguments</param>
		static void Main(string[] args)
		{
			bool result = false;

			try
			{
				string inputPath  = ConvertArg(args, ARGS_PREFIX_INPUT_PATH);
				string outputPath = ConvertArg(args, ARGS_PREFIX_OUTPUT_PATH);
				string symbolsDir = ConvertArg(args, ARGS_PREFIX_SYMBOLS_DIR);
				string exeDir     = ConvertArg(args, ARGS_PREFIX_EXE_DIR);
				string xslPath    = ConvertArg(args, ARGS_PREFIX_XSL_PATH);

				if (!File.Exists(inputPath))
				{
					Console.WriteLine("input file not found. ({0})", inputPath);
					return;
				}

				Console.WriteLine("input file: {0}", inputPath);

				string inputDir = Path.GetDirectoryName(inputPath);
				CoverageInfoManager.SymPath = (string.IsNullOrEmpty(symbolsDir)) ? (inputDir) : (symbolsDir);
				CoverageInfoManager.ExePath = (string.IsNullOrEmpty(exeDir))     ? (inputDir) : (exeDir);

				CoverageInfo ci = CoverageInfoManager.CreateInfoFromFile(inputPath);
				CoverageDS data = ci.BuildDataSet(null);

				string outputWk = outputPath;
				if (string.IsNullOrEmpty(outputWk))
					outputWk = Path.ChangeExtension(inputPath, "xml");

				Console.WriteLine("output file: {0}", outputWk);

				if (xslPath == null)
					data.WriteXml(outputWk);
				else
					WriteTransformXml(data, outputWk, xslPath);

				result = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				Environment.Exit((result) ? (0) : (1));
			}
		}

		/// <summary>
		/// Convert Command Line Argument.
		/// </summary>
		/// <param name="args">Command Line Argument</param>
		/// <param name="prefix">target prefix</param>
		/// <returns></returns>
		private static string ConvertArg(string[] args, string prefix)
		{
			if (args != null)
			{
				foreach (string arg in args)
				{
					if ((arg != null) && arg.StartsWith(prefix))
					{
						return arg.Replace(prefix, string.Empty).Replace("\"", string.Empty);
					}
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// Write Transform Xml
		/// </summary>
		/// <param name="data">Coverage DataSet</param>
		/// <param name="outputPath">Output File Path</param>
		/// <param name="xslPath">Xsl File Path</param>
		private static void WriteTransformXml(CoverageDS data, string outputPath, string xslPath)
		{
			using (XmlReader reader = new XmlTextReader(new StringReader(data.GetXml())))
			{
				using (XmlWriter writer = new XmlTextWriter(outputPath, Encoding.UTF8))
				{
					XslCompiledTransform transform = new XslCompiledTransform();
					transform.Load(xslPath);
					transform.Transform(reader, writer);
				}
			}
			
		}
	}
}
