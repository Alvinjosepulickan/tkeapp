using System.IO;

namespace TKE.CPQ.DocEngine.Generators

{
    public interface IDocumentGenerator
    {
        Stream CreateDocument();
        Stream CreateDocument(string templatePath, string data);
        Stream CreateDocument(string templatePath, string data, string headerTemplate);
        void Save(Stream stream, bool incrementalUpdate = false);
        void Save(string fileName, bool incrementalUpdate = false);

    }
}
