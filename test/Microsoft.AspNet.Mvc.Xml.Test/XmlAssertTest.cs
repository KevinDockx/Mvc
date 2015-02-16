// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;
using Xunit.Sdk;

namespace Microsoft.AspNet.Mvc.Xml
{
    public class XmlAssertTest
    {
        [Theory]
        [InlineData("<A>hello</A>", "<A>hey</A>")]
        [InlineData("<A><B>hello world</B></A>", "<A><B>hello world!!</B></A>")]
        [InlineData("<A>hello<B>world</B>hello</A>", "<A>hello<B>world!</B>hello</A>")]
        public void ValidatesTextNodes(string input1, string input2)
        {
            Assert.Throws<EqualException>(() => XmlAssert.Equal(input1, input2));
        }

        [Theory]
        [InlineData("<A></A>", "<A></A>")]
        [InlineData("<A>hello<B>world</B>hello</A>", "<A>hello<B>world</B>hello</A>")]
        [InlineData("<A><!-- comment1 --><B></B></A>", "<A><!-- comment1 --><B></B></A>")]
        [InlineData("<A/>", "<A/>")]
        [InlineData("<A><![CDATA[<greeting></greeting>]]></A>", "<A><![CDATA[<greeting></greeting>]]></A>")]
        public void DoesNotFail_OnEmptyTextNodes(string input1, string input2)
        {
            XmlAssert.Equal(input1, input2);
        }

        [Theory]
        [InlineData("<?xml version=\"1.0\" encoding=\"UTF-8\"?><A></A>",
            "<A></A>")]
        [InlineData("<?xml version=\"1.0\" encoding=\"UTF-8\"?><A></A>",
            "<?xml version=\"1.0\" encoding=\"UTF-16\"?><A></A>")]
        public void Validates_XmlDeclaration(string input1, string input2)
        {
            Assert.Throws<EqualException>(() => XmlAssert.Equal(input1, input2));
        }

        [Fact]
        public void Validates_XmlDeclaration_IgnoresCase()
        {
            // Arrange
            var input1 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                        "<A><B color=\"red\" size=\"medium\">hello world</B></A>";
            var input2 = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                        "<A><B size=\"medium\" color=\"red\">hello world</B></A>";

            // Act and Assert
            XmlAssert.Equal(input1, input2);
        }

        [Fact]
        public void IgnoresAttributeOrder()
        {
            // Arrange
            var input1 = "<A><B color=\"red\" size=\"medium\">hello world</B></A>";
            var input2 = "<A><B size=\"medium\" color=\"red\">hello world</B></A>";

            // Act and Assert
            XmlAssert.Equal(input1, input2);
        }

        [Fact]
        public void ValidatesAttributes_IgnoringOrder()
        {
            // Arrange
            var input1 = "<A><B color=\"red\" size=\"medium\">hello world</B></A>";
            var input2 = "<A><B size=\"Medium\" color=\"red\">hello world</B></A>";

            // Act and Assert
            Assert.Throws<EqualException>(() => XmlAssert.Equal(input1, input2));
        }
    }
}