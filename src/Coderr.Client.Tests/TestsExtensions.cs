using System.Linq;
using codeRR.Client.Contracts;
using codeRR.Client.Reporters;
using Xunit.Sdk;

namespace Coderr.Client.Tests
{
    public static class TestExtensions
    {
        public static string GetCollectionProperty(this ErrorReportDTO dto, string collectionName, string propertyName)
        {
            var col = dto.ContextCollections.FirstOrDefault(x => x.Name == collectionName);
            if (col != null)
                return col.Property(propertyName);

            var collectionNames = string.Join(",", dto.ContextCollections.Select(x => x.Name));
            throw new AssertActualExpectedException(collectionName, null,
                $"Failed to find collection \'{collectionName}\', existing collections: {collectionNames}."
            );
        }

        public static string GetCollectionProperty(this ErrorReporterContext dto, string collectionName, string propertyName)
        {
            var col = dto.ContextCollections.FirstOrDefault(x => x.Name == collectionName);
            if (col != null)
                return col.Property(propertyName);

            var collectionNames = string.Join(",", dto.ContextCollections.Select(x => x.Name));
            throw new AssertActualExpectedException(collectionName, null,
                $"Failed to find collection \'{collectionName}\', existing collections: {collectionNames}."
            );
        }

        public static string Property(this ContextCollectionDTO collection, string propertyName)
        {
            if (collection.Properties.TryGetValue(propertyName, out string value))
                return value;

            var propertyNames = string.Join(",", collection.Properties.Keys);
            throw new AssertActualExpectedException(propertyName, null,
                $"Failed to find property  \'{propertyName}\', existing properties: {propertyNames}."
            );
        }
        public static string Property<T>(this ContextCollectionDTO collection, string propertyName)
        {
            if (collection.Properties.TryGetValue(propertyName, out string value))
                return value;

            var propertyNames = string.Join(",", collection.Properties.Keys);
            throw new AssertActualExpectedException(propertyName, null,
                $"Failed to find property  \'{propertyName}\', existing properties: {propertyNames}."
            );
        }
    }
}