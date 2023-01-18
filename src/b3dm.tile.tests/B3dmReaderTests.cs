using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.IO;

namespace B3dmCore.Tests;

public class B3dmReaderTests
{
    Stream b3dmfile;
    string expectedMagicHeader = "b3dm";
    int expectedVersionHeader = 1;

    [SetUp]
    public void Setup()
    {
        b3dmfile = File.OpenRead(@"testfixtures/1_expected.b3dm");
        Assert.IsTrue(b3dmfile != null);
    }

    [Test]
    public void ReadB3dmTest()
    {
        // arrange

        // act
        var b3dm = B3dmReader.ReadB3dm(b3dmfile);
        var stream = new MemoryStream(b3dm.GlbData);
        var glb = SharpGLTF.Schema2.ModelRoot.ReadGLB(stream);
        Assert.IsTrue(glb.Asset.Version.Major == 2.0);
        Assert.IsTrue(glb.Asset.Generator == "SharpGLTF 1.0.0-alpha0009");

        // assert
        Assert.IsTrue(expectedMagicHeader == b3dm.B3dmHeader.Magic);
        Assert.IsTrue(expectedVersionHeader == b3dm.B3dmHeader.Version);
        Assert.IsTrue(b3dm.BatchTableJson.Length >= 0);
        Assert.IsTrue(b3dm.GlbData.Length > 0);
    }

    [Test]
    public void ReadB3dmWithGlbTest()
    {
        // arrange
        var buildingGlb = File.ReadAllBytes(@"testfixtures/building.glb");

        // act

        var b3dm = new B3dm(buildingGlb);

        // assert
        Assert.IsTrue(b3dm.GlbData.Length == 2924);
    }

    [Test]
    public void ReadB3dmWithBatchTest()
    {
        // arrange
        var batchB3dm = File.OpenRead(@"testfixtures/with_batch.b3dm");
        var expectedBatchTableJsonText = File.ReadAllText(@"testfixtures/BatchTableJsonExpected.json");
        var expectedBatchTableJson = JObject.Parse(expectedBatchTableJsonText);

        // act
        var b3dm = B3dmReader.ReadB3dm(batchB3dm);
        var actualBatchTableJson = JObject.Parse(b3dm.BatchTableJson);

        // assert
        Assert.IsTrue(b3dm.FeatureTableJson == "{\"BATCH_LENGTH\":12} ");
        Assert.AreEqual(expectedBatchTableJson,actualBatchTableJson);
    }

    [Test]
    public void ReadNederland3DB3dmTest()
    {
        // arrange
        var b3dmfile1 = File.OpenRead(@"testfixtures/nederland3d_6825.b3dm");

        // act
        var b3dm = B3dmReader.ReadB3dm(b3dmfile1);

        // assert
        Assert.IsTrue(expectedMagicHeader == b3dm.B3dmHeader.Magic);
        Assert.IsTrue(expectedVersionHeader == b3dm.B3dmHeader.Version);
    }
}