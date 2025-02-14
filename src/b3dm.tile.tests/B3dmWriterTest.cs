﻿using System.IO;
using NUnit.Framework;

namespace B3dmCore;

public class B3dmWriterTest
{
    [Test]
    public void WriteB3dmWithCyrlllicCharacters()
    {
        // arrange
        var buildingGlb = File.ReadAllBytes(@"testfixtures/1.glb");
        var b3dm = new B3dm(buildingGlb);
        var batchTableJson = File.ReadAllText(@"testfixtures/BatchTableWithCyrillicCharacters.json");
        b3dm.BatchTableJson = batchTableJson;
        b3dm.FeatureTableJson = "{\"BATCH_LENGTH\":12} ";

        // act
        var bytes = b3dm.ToBytes();
        var b3dmActual = B3dmReader.ReadB3dm(new MemoryStream(bytes));

        // Assert
        Assert.That(b3dmActual.B3dmHeader.Validate().Count == 0);
    }

    [Test]
    public void WriteB3dmWithBatchTest()
    {
        // arrange
        var buildingGlb = File.ReadAllBytes(@"testfixtures/with_batch.glb");
        var batchTableJson = File.ReadAllText(@"testfixtures/BatchTableJsonExpected.json");

        var b3dmBytesExpected = File.OpenRead(@"testfixtures/with_batch.b3dm");
        var b3dmExpected = B3dmReader.ReadB3dm(b3dmBytesExpected);
        var errors = b3dmExpected.B3dmHeader.Validate();
        Assert.That(errors.Count > 0);

        var b3dm = new B3dm(buildingGlb);
        b3dm.FeatureTableJson = b3dmExpected.FeatureTableJson;
        b3dm.BatchTableJson = b3dmExpected.BatchTableJson;
        b3dm.FeatureTableBinary = b3dmExpected.FeatureTableBinary;
        b3dm.BatchTableBinary = b3dmExpected.BatchTableBinary;

        // act
        var result = "with_batch.b3dm";
        var bytes = b3dm.ToBytes();

        File.WriteAllBytes(result, bytes);

        var b3dmActual = B3dmReader.ReadB3dm(File.OpenRead(result));

        // Assert
        var errorsActual = b3dmActual.B3dmHeader.Validate();
        Assert.That(errorsActual.Count == 0);

        Assert.That(b3dmActual.B3dmHeader.Magic == b3dmExpected.B3dmHeader.Magic);
        Assert.That(b3dmActual.B3dmHeader.Version== b3dmExpected.B3dmHeader.Version);
        Assert.That(b3dmActual.B3dmHeader.FeatureTableJsonByteLength== b3dmExpected.B3dmHeader.FeatureTableJsonByteLength);
    }
}
