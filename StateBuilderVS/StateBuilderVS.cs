using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using System.ComponentModel;
using System.CodeDom.Compiler;
using Microsoft.VisualStudio.Shell;
using VSOLE = Microsoft.VisualStudio.OLE.Interop;
using System.CodeDom;
using System.IO;
using System.Xml;

namespace StateForge
{
    [Guid("60D7D5DE-7F3F-4F08-A243-BCE062ED8445")]
    class StateBuilderVS : IVsSingleFileGenerator, VSOLE::IObjectWithSite
    {
        private CodeDomProvider codeProvider;

        private string codeFileNameSpace;
        private string codeFilePath;

        private object site;

        private IVsGeneratorProgress codeGeneratorProgress;

        public CodeDomProvider CodeProvider
        {
            get
            {
                if (this.codeProvider == null)
                {
                    codeProvider = CodeDomProvider.CreateProvider("C#");
                }

                return this.codeProvider;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                this.codeProvider = value;
            }
        }

        #region IVsSingleFileGenerator Members

        public int DefaultExtension(out string ext)
        {
            string defExt;
            ext = string.Empty;

            defExt = this.CodeProvider.FileExtension;

            if (((defExt != null) && (defExt.Length > 0)) && (defExt[0] != '.'))
            {
                defExt = "." + defExt;
            }

            if (!string.IsNullOrEmpty(defExt))
            {
                ext = ".Designer" + defExt;
            }

            return 0;
        }

        public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] pbstrOutputFileContents, out uint pbstrOutputFileContentSize, IVsGeneratorProgress pGenerateProgress)
        {
            if (bstrInputFileContents == null)
            {
                throw new ArgumentNullException(bstrInputFileContents);
            }

            this.codeFilePath = wszInputFilePath;
            this.codeFileNameSpace = wszDefaultNamespace;
            this.codeGeneratorProgress = pGenerateProgress;

            byte[] generatedStuff = this.GenerateCode(wszInputFilePath, bstrInputFileContents);

            if (generatedStuff == null)
            {
                pbstrOutputFileContents[0] = IntPtr.Zero;
                pbstrOutputFileContentSize = 0;
            }
            else
            {
                pbstrOutputFileContents[0] = Marshal.AllocCoTaskMem(generatedStuff.Length);
                Marshal.Copy(generatedStuff, 0, pbstrOutputFileContents[0], generatedStuff.Length);
                pbstrOutputFileContentSize = (uint)generatedStuff.Length;
            }
            return 0;
        }
        #endregion

        #region IObjectWithSite Members

        public void GetSite(ref Guid riid, out IntPtr ppvSite)
        {
            if (this.site == null)
            {
                throw new Win32Exception(-2147467259);
            }

            IntPtr objectPointer = Marshal.GetIUnknownForObject(this.site);

            try
            {
                Marshal.QueryInterface(objectPointer, ref riid, out ppvSite);
                if (ppvSite == IntPtr.Zero)
                {
                    throw new Win32Exception(-2147467262);
                }
            }
            finally
            {
                if (objectPointer != IntPtr.Zero)
                {
                    Marshal.Release(objectPointer);
                    objectPointer = IntPtr.Zero;
                }
            }
        }

        public void SetSite(object pUnkSite)
        {
            this.site = pUnkSite;
            this.codeProvider = null;
        }

        #endregion

        #region Private Methods
        protected byte[] GenerateCode(string inputFileName, string inputFileContent)
        {
            CodeCompileUnit code;

            StreamWriter writer = new StreamWriter(new MemoryStream(), Encoding.UTF8);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(inputFileContent);

            //compileUnit = ClassGenerator.Create(doc, this.codeProvider);

            code = new CodeCompileUnit();
            code.UserData.Add("AllowLateBound", false);
            code.UserData.Add("RequireVariableDeclaration", true);

            CodeNamespace nameSpace = new CodeNamespace("foo");
            code.Namespaces.Add(nameSpace);

            CodeTypeDeclaration classObject = new CodeTypeDeclaration("Context");
            nameSpace.Types.Add(classObject);

            CodeConstructor ctor = new CodeConstructor();

            if (this.codeGeneratorProgress != null)
            {
                this.codeGeneratorProgress.Progress(0x4b, 100);
            }

            this.CodeProvider.GenerateCodeFromCompileUnit(code, writer, null);

            if (this.codeGeneratorProgress != null)
            {
                this.ThrowOnFailure(this.codeGeneratorProgress.Progress(100, 100));
            }
            writer.Flush();

            return this.StreamToBytes(writer.BaseStream);
        }

        protected byte[] StreamToBytes(Stream stream)
        {
            if (stream.Length == 0)
            {
                return new byte[0];
            }

            long pos = stream.Position;

            stream.Position = 0;

            byte[] buffer = new byte[(int)stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            stream.Position = pos;
            return buffer;
        }

        private void ThrowOnFailure(int hr)
        {
            if ((hr < 0))
            {
                Marshal.ThrowExceptionForHR(hr);
            }
        }
        #endregion
    }
}
