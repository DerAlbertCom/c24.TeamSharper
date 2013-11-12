using NUnit.Framework;

namespace C24.TeamSharper.Tests
{
    [TestFixture]
    public class PathHelperTests
    {
        [Test]
        public void Test_if_relative_path_is_resolved_correctly_without_root_backslash()
        {
            const string basePath = "C:\\root";
            const string relativePath = "test";
            string result = PathHelper.MakeFilePathAbsoluteToDirectory(relativePath, basePath);

            Assert.AreEqual("C:\\root\\test", result);
        }

        [Test]
        public void Test_if_relative_path_is_resolved_correctly_with_root_backslash()
        {
            const string basePath = "C:\\root\\";
            const string relativePath = "test";
            string result = PathHelper.MakeFilePathAbsoluteToDirectory(relativePath, basePath);

            Assert.AreEqual("C:\\root\\test", result);
        }

        [Test]
        public void Test_if_relative_path_is_resolved_correctly_with_root_backslash_and_relative_backslash()
        {
            const string basePath = "C:\\root\\";
            const string relativePath = "test\\";
            string result = PathHelper.MakeFilePathAbsoluteToDirectory(relativePath, basePath);

            Assert.AreEqual("C:\\root\\test", result);
        }

        [Test]
        public void Test_if_relative_path_is_resolved_correctly_with_backslashes_on_every_end()
        {
            const string basePath = "C:\\root\\";
            const string relativePath = ".\\test\\";
            string result = PathHelper.MakeFilePathAbsoluteToDirectory(relativePath, basePath);

            Assert.AreEqual("C:\\root\\test", result);
        }

        [Test]
        public void Test_if_relative_path_is_resolved_correctly_with_navigation()
        {
            const string basePath = "C:\\root\\second";
            const string relativePath = "..\\test";
            string result = PathHelper.MakeFilePathAbsoluteToDirectory(relativePath, basePath);

            Assert.AreEqual("C:\\root\\test", result);
        }

        [Test]
        public void Test_if_relative_path_is_resolved_correctly_with_already_absolute_path()
        {
            const string basePath = "C:\\root\\second";
            const string relativePath = "C:\\root\\test\\";
            string result = PathHelper.MakeFilePathAbsoluteToDirectory(relativePath, basePath);

            Assert.AreEqual("C:\\root\\test", result);
        }

        [Test]
        public void Test_if_GetNavigationPath_returns_correct_value()
        {
            const string basePath = "C:\\root\\test\\";
            const string absolutePath = "C:\\second\\";
            string result = PathHelper.GetNavigationPath(basePath, absolutePath);

            Assert.AreEqual("..\\..\\second", result);
        }

        [Test]
        public void Test_if_MakeFilePathRelativeToDirectory_returns_correct_value()
        {
            const string basePath = "C:\\root\\test\\";
            const string absolutePath = "C:\\second\\file2.txt";
            string result = PathHelper.MakeFilePathRelativeToDirectory(absolutePath, basePath);

            Assert.AreEqual("..\\..\\second\\file2.txt", result);
        }
    }
}
