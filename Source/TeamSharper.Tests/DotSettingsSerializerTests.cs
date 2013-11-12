using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace C24.TeamSharper.Tests
{
    [TestFixture]
    public class DotSettingsSerializerTests
    {
        private const string testXmlWithTwoLayers =
            @"<wpf:ResourceDictionary xml:space=""preserve"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:s=""clr-namespace:System;assembly=mscorlib"" xmlns:ss=""urn:shemas-jetbrains-com:settings-storage-xaml"" xmlns:wpf=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
    <s:String x:Key=""/Default/Environment/InjectedLayers/FileInjectedLayer/=2798567F2085034798DD1D442322FE8E/AbsolutePath/@EntryValue"">C:\Github\ilkerResharperRepo\resharper.csharp.teamsettings\settings\Aviva-CSharp-Coding-Guidelines.DotSettings</s:String>
    <s:String x:Key=""/Default/Environment/InjectedLayers/FileInjectedLayer/=2798567F2085034798DD1D442322FE8E/RelativePath/@EntryValue"">..\..\..\..\Github\ilkerResharperRepo\resharper.csharp.teamsettings\settings\Aviva-CSharp-Coding-Guidelines.DotSettings</s:String>
    <s:Boolean x:Key=""/Default/Environment/InjectedLayers/FileInjectedLayer/=2798567F2085034798DD1D442322FE8E/@KeyIndexDefined"">True</s:Boolean>
    
    <s:Boolean x:Key=""/Default/Environment/InjectedLayers/InjectedLayerCustomization/=File2798567F2085034798DD1D442322FE8E/@KeyIndexDefined"">True</s:Boolean>
    <s:Double x:Key=""/Default/Environment/InjectedLayers/InjectedLayerCustomization/=File2798567F2085034798DD1D442322FE8E/RelativePriority/@EntryValue"">1</s:Double>
    
    <s:Boolean x:Key=""/Default/Environment/InjectedLayers/FileInjectedLayer/=64FB3392D3C8034BBD15C2C46BF672D5/@KeyIndexDefined"">True</s:Boolean>
    <s:String x:Key=""/Default/Environment/InjectedLayers/FileInjectedLayer/=64FB3392D3C8034BBD15C2C46BF672D5/AbsolutePath/@EntryValue"">C:\Github\ilkerResharperRepo\resharper.csharp.teamsettings\settings\Second.DotSettings</s:String>
    <s:String x:Key=""/Default/Environment/InjectedLayers/FileInjectedLayer/=64FB3392D3C8034BBD15C2C46BF672D5/RelativePath/@EntryValue"">..\..\..\..\Github\ilkerResharperRepo\resharper.csharp.teamsettings\settings\Second.DotSettings</s:String>
        
    <s:Boolean x:Key=""/Default/Environment/InjectedLayers/InjectedLayerCustomization/=File64FB3392D3C8034BBD15C2C46BF672D5/@KeyIndexDefined"">True</s:Boolean>
    <s:Double x:Key=""/Default/Environment/InjectedLayers/InjectedLayerCustomization/=File64FB3392D3C8034BBD15C2C46BF672D5/RelativePriority/@EntryValue"">2</s:Double>
</wpf:ResourceDictionary>";

        private const string testXmlWithoutLayers =
            @"<wpf:ResourceDictionary xml:space=""preserve"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:s=""clr-namespace:System;assembly=mscorlib"" xmlns:ss=""urn:shemas-jetbrains-com:settings-storage-xaml"" xmlns:wpf=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""></wpf:ResourceDictionary>";

        [Test]
        public void Get_all_InjectedLayersElements_returns_correct_result_when_there_are_no_layers()
        {
            // Arrange:
            var parser = new DotSettingsSerializer();

            // Act:
            IEnumerable<XElement> elements = parser.GetAllInjectedLayerElements(XElement.Parse(testXmlWithoutLayers));

            // Assert:
            Assert.IsFalse(elements.Any());
        }

        [Test]
        public void Get_all_InjectedLayersElements_returns_correct_result_when_there_are_layers()
        {
            // Arrange:
            var parser = new DotSettingsSerializer();

            // Act:
            IEnumerable<XElement> elements = parser.GetAllInjectedLayerElements(XElement.Parse(testXmlWithTwoLayers));

            // Assert:
            Assert.AreEqual(10, elements.Count());
        }

        [Test]
        public void ExtractID_should_return_the_layer_id_for_pattern_1()
        {
            // Arrange
            var parser = new DotSettingsSerializer();
            
            // Act

            var result = parser.ExtractId(@"/Default/Environment/InjectedLayers/FileInjectedLayer/=2798567F2085034798DD1D442322FE8E/AbsolutePath/@EntryValue");

            // Assert

            Assert.AreEqual("2798567F2085034798DD1D442322FE8E", result);
        }

        [Test]
        public void ExtractID_should_return_the_layer_id_for_pattern_2()
        {
            // Arrange
            var parser = new DotSettingsSerializer();
            
            // Act

            var result = parser.ExtractId(@"/Default/Environment/InjectedLayers/InjectedLayerCustomization/=File2798567F2085034798DD1D442322FE8E/@KeyIndexDefined");

            // Assert

            Assert.AreEqual("2798567F2085034798DD1D442322FE8E", result);
        }

        [Test]
        public void GetAllInjectedLayerElementsGroupedByLayer_returns_the_Xmlelements_grouped_by_ID()
        {
            // Arrange:
            var parser = new DotSettingsSerializer();

            // Act:
            IEnumerable<IGrouping<string, XElement>> groups = parser.GetAllInjectedLayerElementsGroupedByLayer(XElement.Parse(testXmlWithTwoLayers)).ToList();

            // Assert:
            Assert.AreEqual(2, groups.Count());

            var firstGroup = groups.SingleOrDefault(x => x.Key.Equals("2798567F2085034798DD1D442322FE8E"));
            Assert.NotNull(firstGroup);

            var secondGroup = groups.SingleOrDefault(x => x.Key.Equals("64FB3392D3C8034BBD15C2C46BF672D5"));
            Assert.NotNull(secondGroup);
        }

        [Test]
        public void GetAllLayers_returns_the_layers_of_the_DotSettings_file()
        {
            // Arrange
            var parser = new DotSettingsSerializer();

            // Act:
            var result = parser.ParseLayers(XElement.Parse(testXmlWithTwoLayers)).ToList();


            // Assert

            Assert.AreEqual(2, result.Count);
            var firstLayer = result.SingleOrDefault(x => x.Id.Equals(new Guid("2798567F2085034798DD1D442322FE8E")));

            Assert.NotNull(firstLayer);
            Assert.AreEqual(@"C:\Github\ilkerResharperRepo\resharper.csharp.teamsettings\settings\Aviva-CSharp-Coding-Guidelines.DotSettings", firstLayer.AbsolutePath);
            Assert.AreEqual(@"..\..\..\..\Github\ilkerResharperRepo\resharper.csharp.teamsettings\settings\Aviva-CSharp-Coding-Guidelines.DotSettings", firstLayer.RelativePath);
            Assert.AreEqual((double)1, firstLayer.RelativePriority);

            var secondLayer = result.SingleOrDefault(x => x.Id.Equals(new Guid("64FB3392D3C8034BBD15C2C46BF672D5")));

            Assert.NotNull(secondLayer);
            Assert.AreEqual(@"C:\Github\ilkerResharperRepo\resharper.csharp.teamsettings\settings\Second.DotSettings", secondLayer.AbsolutePath);
            Assert.AreEqual(@"..\..\..\..\Github\ilkerResharperRepo\resharper.csharp.teamsettings\settings\Second.DotSettings", secondLayer.RelativePath);
            Assert.AreEqual((double)2, secondLayer.RelativePriority);

        }
    }
}
