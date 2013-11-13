using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace C24.TeamSharper
{
    public sealed class DotSettingsSerializer : IDotSettingsSerializer
    {
        private const string absolutePathKey = "/Default/Environment/InjectedLayers/FileInjectedLayer/={0}/AbsolutePath/@EntryValue";
        private const string relativePathKey = "/Default/Environment/InjectedLayers/FileInjectedLayer/={0}/RelativePath/@EntryValue";
        private const string keyIndexDefinedKey = "/Default/Environment/InjectedLayers/FileInjectedLayer/={0}/@KeyIndexDefined";
        private const string keyIndexDefinedKey2 = "/Default/Environment/InjectedLayers/InjectedLayerCustomization/=File{0}/@KeyIndexDefined";
        private const string entryValueKey = "/Default/Environment/InjectedLayers/InjectedLayerCustomization/=File{0}/RelativePriority/@EntryValue";

        private static readonly Regex extractIdRegex = new Regex(@"\/=(File)?(?<Id>([0-9A-F])+)\/", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        // ---------------- SAVING ------------------------------------------------

        public void Save(DotSettings dotSettings)
        {
            this.CreateXml(dotSettings).Save(dotSettings.FilePath);
        }

        private XElement CreateXml(DotSettings dotSettings)
        {
            return new XElement(
                Xmlns.Winfx + "ResourceDictionary",
                new XAttribute(XNamespace.Xml + "space", "preserve"),
                new XAttribute(XNamespace.Xmlns + "x", Xmlns.Xaml.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "s", Xmlns.Corlib.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "ss", Xmlns.Storage.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "wpf", Xmlns.Winfx.NamespaceName),
                dotSettings.Layers.OrderBy(x => x.RelativePriority).SelectMany(CreateLayerElements));
        }

        private IEnumerable<XElement> CreateLayerElements(DotSettingsLayer dotSettingsLayer)
        {
            string id = dotSettingsLayer.Id.ToString("N", CultureInfo.InvariantCulture).ToUpperInvariant();
            string relativePriority = dotSettingsLayer.RelativePriority.ToString("N", CultureInfo.InvariantCulture).TrimEnd('0', '.');
            yield return new XElement(Xmlns.Corlib + "String", new XAttribute(Xmlns.Xaml + "Key", string.Format(absolutePathKey, id)), dotSettingsLayer.AbsolutePath);
            yield return new XElement(Xmlns.Corlib + "String", new XAttribute(Xmlns.Xaml + "Key", string.Format(relativePathKey, id)), dotSettingsLayer.RelativePath);
            yield return new XElement(Xmlns.Corlib + "Boolean", new XAttribute(Xmlns.Xaml + "Key", string.Format(keyIndexDefinedKey, id)), "True");
            yield return new XElement(Xmlns.Corlib + "Boolean", new XAttribute(Xmlns.Xaml + "Key", string.Format(keyIndexDefinedKey2, id)), "True");
            yield return new XElement(Xmlns.Corlib + "Double", new XAttribute(Xmlns.Xaml + "Key", string.Format(entryValueKey, id)), relativePriority);
        }

        // ---------------- LOADING ------------------------------------------------

        public IEnumerable<DotSettings> LoadAll(string rootDirectory)
        {
            return Directory
                .EnumerateFiles(rootDirectory, "*.sln", SearchOption.AllDirectories)
                .Select(slnFile => this.DeserializeFromFile(string.Concat(slnFile, ".DotSettings")));
        }

        private DotSettings DeserializeFromFile(string filePath)
        {
            bool fileExists = File.Exists(filePath);
            IEnumerable<DotSettingsLayer> layers = fileExists ? this.ParseLayers(XElement.Load(filePath)) : Enumerable.Empty<DotSettingsLayer>();
            return new DotSettings(filePath, layers, fileExists);
        }

        internal IEnumerable<DotSettingsLayer> ParseLayers(XElement xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            IEnumerable<IGrouping<string, XElement>> groups = GetAllInjectedLayerElementsGroupedByLayer(xml);
            return groups.Select(group => CreateLayerFromGroup(group, group.Key));
        }

        internal IEnumerable<XElement> GetAllInjectedLayerElements(XElement xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            return xml.Elements().Where(x => (ExtractKey(x) ?? string.Empty).StartsWith("/Default/Environment/InjectedLayers/"));
        }

        internal IEnumerable<IGrouping<string, XElement>> GetAllInjectedLayerElementsGroupedByLayer(XElement xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            return GetAllInjectedLayerElements(xml).GroupBy(x => ExtractId(ExtractKey(x)));
        }

        private string ExtractKey(XElement x)
        {
            return (string) x.Attribute(Xmlns.Xaml + "Key");
        }

        public string ExtractId(string key)
        {
            Group group = extractIdRegex.Match(key).Groups["Id"];
            return group.Captures[0].Value;
        }

        private DotSettingsLayer CreateLayerFromGroup(IEnumerable<XElement> group, string id)
        {
            IList<XElement> elements = group as IList<XElement> ?? group.ToList();

            return new DotSettingsLayer
            {
                Id = new Guid(id),
                AbsolutePath = (string) elements.Single(x => ExtractKey(x).EndsWith(@"/AbsolutePath/@EntryValue")),
                RelativePath = (string) elements.Single(x => ExtractKey(x).EndsWith(@"/RelativePath/@EntryValue")),
                RelativePriority = (double) elements.Single(x => ExtractKey(x).EndsWith(@"/RelativePriority/@EntryValue"))
            };
        }
    }
}
