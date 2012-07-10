using Xunit;

namespace LogMonitor.Processors
{
    public class HelperTest
    {
        public class SplitLines
        {
            [Fact]
            public void SplitWindowsLines()
            {
                string content = "Hello\r\nWorld";

                string[] expected = new[] { "Hello", "World" };
                string[] actual = Helper.SplitLines(content);

                Assert.Equal(expected, actual);
            }

            [Fact]
            public void SplitUnixLines()
            {
                string content = "Hello\nWorld";

                string[] expected = new[] { "Hello", "World" };
                string[] actual = Helper.SplitLines(content);

                Assert.Equal(expected, actual);
            }

            [Fact]
            public void SplitPosixLines()
            {
                string content = "Hello\rWorld";

                string[] expected = new[] { "Hello", "World" };
                string[] actual = Helper.SplitLines(content);

                Assert.Equal(expected, actual);
            }

            [Fact]
            public void SplitMixedLines()
            {
                string content = "Hello\rWorld\nHello\r\nWorld";

                string[] expected = new[] { "Hello", "World", "Hello", "World" };
                string[] actual = Helper.SplitLines(content);

                Assert.Equal(expected, actual);
            }

            [Fact]
            public void SplitSingleLine()
            {
                string content = "Single line";

                string[] expected = new[] { content };
                string[] actual = Helper.SplitLines(content);

                Assert.Equal(expected, actual);
            }

            [Fact]
            public void EndOfLine()
            {
                string content = "Hello World\n";

                string[] expected = new[] { "Hello World", string.Empty };
                string[] actual = Helper.SplitLines(content);

                Assert.Equal(expected, actual);
            }

            [Fact]
            public void SplitNull()
            {
                string content = null;

                string[] expected = new string[0];
                string[] actual = Helper.SplitLines(content);

                Assert.Equal(expected, actual);
            }
        }
    }
}
