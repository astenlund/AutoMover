namespace AutoMover.Test
{
    using NUnit.Framework;

    public class ExtensionTests
    {
        [Test]
        public void LeadingDotIsRemoved()
        {
            const string Input = @".ext=C:\Temp";
            const string Expected = @"ext=C:\Temp";
            
            var output = Input.RemoveLeading(".");

            Assert.That(output, Is.EqualTo(Expected));
        }

        [Test]
        public void EmptySubstringYieldsNoChange()
        {
            const string Input = @".ext=C:\Temp";
            const string Expected = @".ext=C:\Temp";
            
            var output = Input.RemoveLeading(string.Empty);

            Assert.That(output, Is.EqualTo(Expected));
        }

        [Test]
        public void SubstringInMiddleOfStringIsNotRemoved()
        {
            const string Input = @".ext=C:\Temp";
            const string Expected = @".ext=C:\Temp";

            var output = Input.RemoveLeading("ext");

            Assert.That(output, Is.EqualTo(Expected));
        }

        [Test]
        public void LongerSubstringIsRemoved()
        {
            const string Input = @".ext=C:\Temp";
            const string Expected = @"t=C:\Temp";

            var output = Input.RemoveLeading(".ex");

            Assert.That(output, Is.EqualTo(Expected));
        }
    }
}
