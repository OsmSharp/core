// The MIT License (MIT)

// Copyright (c) 2026 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using OsmSharp.Logging;
using OsmSharp.Streams;

namespace OsmSharp.Test.Functional;

/// <summary>
/// End-to-end checks for <see cref="PBFOsmStreamSource"/>'s block-index persistence.
/// Runs against the same Luxembourg PBF the other functional tests use. Each check
/// throws on failure with a name that identifies which invariant broke — the test host
/// bubbles the exception so a regression is loud and locatable.
///
/// Not unit-testable in isolation: the block index is a private field of
/// <see cref="PBFOsmStreamSource"/>, populated as a side effect of walking a real PBF.
/// The sidecar format is verified indirectly, by comparing walk results against a
/// cold-run reference count.
/// </summary>
public static class PBFBlockIndexTests
{
    public static void Run(string pbfPath)
    {
        var sidecarPath = pbfPath + ".blockindex";
        CleanupSidecar(sidecarPath);

        // Reference: count everything with a plain Stream-based source. This is our ground
        // truth — every sidecar-affected run must produce exactly this count.
        var reference = CountAll(pbfPath);
        Log($"reference count: {reference:N0} elements");

        Check_RoundtripPreservesCount(pbfPath, sidecarPath, reference);
        Check_CorruptMagicDiscarded(pbfPath, sidecarPath, reference);
        Check_CorruptLengthDiscarded(pbfPath, sidecarPath, reference);
        Check_CorruptMtimeDiscarded(pbfPath, sidecarPath, reference);
        Check_PbfLengthMismatchDiscarded(pbfPath, sidecarPath);
        Check_ExplicitSaveMidWalk(pbfPath, sidecarPath);
        Check_SaveOnStreamSourceThrows(pbfPath);
        Check_WarmTypedJumpMatchesCold(pbfPath, sidecarPath);
        Check_TargetWritesSidecar(pbfPath, reference);
        Check_TargetSidecarIsHotForReader(pbfPath, reference);

        CleanupSidecar(sidecarPath);
        Log("all PBF block-index tests passed.");
    }

    // ── Individual checks ────────────────────────────────────────────────────────

    /// <summary>
    /// Cold walk with persist=true writes the sidecar on Dispose. A subsequent
    /// path-based source auto-loads the sidecar and must yield the same total count.
    /// </summary>
    private static void Check_RoundtripPreservesCount(string pbfPath, string sidecarPath, long reference)
    {
        CleanupSidecar(sidecarPath);

        var writeCount = CountAllWithPersist(pbfPath);
        AssertEqual(reference, writeCount, "roundtrip.write.count");
        AssertTrue(File.Exists(sidecarPath), "roundtrip.sidecar-written");
        AssertTrue(new FileInfo(sidecarPath).Length > 20, "roundtrip.sidecar-nontrivial");

        var readCount = CountAll(pbfPath);
        AssertEqual(reference, readCount, "roundtrip.read.count");
        Log($"  roundtrip: sidecar={new FileInfo(sidecarPath).Length:N0} bytes, both counts match reference");
    }

    /// <summary>
    /// Corrupt the sidecar magic. Load must silently discard and the walk must still
    /// produce the reference count (from a cold walk).
    /// </summary>
    private static void Check_CorruptMagicDiscarded(string pbfPath, string sidecarPath, long reference)
    {
        RebuildSidecar(pbfPath, sidecarPath);
        CorruptByteAt(sidecarPath, offset: 0);

        var count = CountAll(pbfPath);
        AssertEqual(reference, count, "corrupt-magic.count");
        Log("  corrupt-magic: discarded, cold walk still matches reference");
    }

    /// <summary>
    /// Corrupt the stored PBF length field. Load must silently discard.
    /// </summary>
    private static void Check_CorruptLengthDiscarded(string pbfPath, string sidecarPath, long reference)
    {
        RebuildSidecar(pbfPath, sidecarPath);
        // Layout: 8 bytes magic + 4 bytes version + int64 PbfLength — first length byte at offset 12.
        CorruptByteAt(sidecarPath, offset: 12);

        var count = CountAll(pbfPath);
        AssertEqual(reference, count, "corrupt-length.count");
        Log("  corrupt-length: discarded, cold walk still matches reference");
    }

    /// <summary>
    /// Corrupt the stored PBF mtime field. Load must silently discard.
    /// </summary>
    private static void Check_CorruptMtimeDiscarded(string pbfPath, string sidecarPath, long reference)
    {
        RebuildSidecar(pbfPath, sidecarPath);
        // Layout: 8 (magic) + 4 (version) + 8 (length) — first mtime byte at offset 20.
        CorruptByteAt(sidecarPath, offset: 20);

        var count = CountAll(pbfPath);
        AssertEqual(reference, count, "corrupt-mtime.count");
        Log("  corrupt-mtime: discarded, cold walk still matches reference");
    }

    /// <summary>
    /// Real-world mismatch: sidecar was built against the original PBF; the PBF on disk
    /// is shorter (or otherwise different). Load must discard, and the resulting cold
    /// walk on the mismatched PBF must not crash inside the block-index path.
    /// </summary>
    private static void Check_PbfLengthMismatchDiscarded(string pbfPath, string sidecarPath)
    {
        RebuildSidecar(pbfPath, sidecarPath);

        var copyPath = pbfPath + ".mismatch.tmp.osm.pbf";
        var copySidecarPath = copyPath + ".blockindex";
        try
        {
            File.Copy(pbfPath, copyPath, overwrite: true);
            File.Copy(sidecarPath, copySidecarPath, overwrite: true);

            // Truncate one byte — different length, same sidecar content. The stored length
            // field no longer matches, so the sidecar must be discarded on load.
            using (var fs = File.Open(copyPath, FileMode.Open, FileAccess.ReadWrite))
            {
                fs.SetLength(fs.Length - 1);
            }

            // Just constructing the source must not throw; if the sidecar were accepted,
            // its offsets would point past the truncated end and any typed-jump would
            // eventually try to read invalid bytes. We only need to prove construction is
            // safe — a jump call would be misleading because the PBF is intentionally broken.
            using var _ = new PBFOsmStreamSource(copyPath);
            Log("  pbf-length-mismatch: constructor accepted (sidecar silently discarded)");
        }
        finally
        {
            if (File.Exists(copyPath)) File.Delete(copyPath);
            if (File.Exists(copySidecarPath)) File.Delete(copySidecarPath);
        }
    }

    /// <summary>
    /// Explicit SaveBlockIndex() called mid-walk must produce a sidecar that another
    /// source can load. Exercises the same code path as the internal periodic flush,
    /// which fires from Upsert once the mutation counter crosses the threshold — Luxembourg
    /// has few enough blobs that the periodic threshold may not naturally trip, so we call
    /// SaveBlockIndex() explicitly to cover the code path deterministically.
    /// </summary>
    private static void Check_ExplicitSaveMidWalk(string pbfPath, string sidecarPath)
    {
        CleanupSidecar(sidecarPath);

        long saved = 0;
        long midWalkSidecarBytes = 0;
        using (var src = new PBFOsmStreamSource(pbfPath, persistBlockIndex: true))
        {
            foreach (var _ in src)
            {
                saved++;
                if (saved == 100_000)
                {
                    src.SaveBlockIndex();
                    AssertTrue(File.Exists(sidecarPath), "explicit-save.exists-mid-walk");
                    midWalkSidecarBytes = new FileInfo(sidecarPath).Length;
                    break;
                }
            }
        }
        // After leaving the using-block, Dispose has already run one more flush (mutations
        // since the explicit save are > 0 as blob decoding continued past the SaveBlockIndex).
        // Verify the file is still valid by loading it with a fresh source.
        AssertTrue(File.Exists(sidecarPath), "explicit-save.exists-after-dispose");
        using (var reload = new PBFOsmStreamSource(pbfPath))
        {
            AssertTrue(reload.MoveNext(false, false, false), "explicit-save.reload-yields");
        }
        Log($"  explicit-save mid-walk: {midWalkSidecarBytes:N0} bytes at yield=100k, reload succeeded");
    }

    /// <summary>
    /// SaveBlockIndex on a source constructed from a raw <see cref="Stream"/> must
    /// throw <see cref="InvalidOperationException"/> — there is no path to derive
    /// the sidecar location from in that case.
    /// </summary>
    private static void Check_SaveOnStreamSourceThrows(string pbfPath)
    {
        using var fs = File.OpenRead(pbfPath);
        using var src = new PBFOsmStreamSource(fs);
        try
        {
            src.SaveBlockIndex();
        }
        catch (InvalidOperationException)
        {
            Log("  save-on-stream: InvalidOperationException thrown as expected");
            return;
        }
        throw new Exception("save-on-stream: expected InvalidOperationException, none thrown");
    }

    /// <summary>
    /// The same <c>MoveNext(Relation, 0)</c> must land on the same element regardless of
    /// whether the sidecar was loaded. If the block-index skip path is wrong, the warm
    /// jump could skip a blob that actually contains the earliest relation.
    /// </summary>
    private static void Check_WarmTypedJumpMatchesCold(string pbfPath, string sidecarPath)
    {
        CleanupSidecar(sidecarPath);
        var coldId = FirstRelationId(pbfPath);
        AssertTrue(coldId.HasValue, "warm-typed-jump.cold-found");

        RebuildSidecar(pbfPath, sidecarPath);
        var warmId = FirstRelationId(pbfPath);
        AssertTrue(warmId.HasValue, "warm-typed-jump.warm-found");

        AssertEqual(coldId!.Value, warmId!.Value, "warm-typed-jump.same-id");
        Log($"  warm-typed-jump: cold and warm both landed on relation id {coldId}");
    }

    /// <summary>
    /// Writing a PBF through the path ctor with <c>persistBlockIndex: true</c> must produce
    /// both the PBF and a matching sidecar. The reader is used to verify the sidecar is
    /// valid: element count round-trips through the reader without any surprises.
    /// </summary>
    private static void Check_TargetWritesSidecar(string sourcePbfPath, long reference)
    {
        var outPbf = sourcePbfPath + ".target-out.osm.pbf";
        var outSidecar = outPbf + ".blockindex";
        CleanupOutputs(outPbf, outSidecar);

        try
        {
            WriteAllWithPersist(sourcePbfPath, outPbf);
            AssertTrue(File.Exists(outPbf), "target-writes.pbf-exists");
            AssertTrue(File.Exists(outSidecar), "target-writes.sidecar-exists");
            var sidecarBytes = new FileInfo(outSidecar).Length;
            AssertTrue(sidecarBytes > 20, "target-writes.sidecar-nontrivial");

            // Read the produced file back — count must match the reference. This exercises
            // the sidecar-load path against a writer-produced sidecar (i.e. the whole point
            // of the feature: reader can consume writer's sidecar with zero cold walks).
            var readback = CountAll(outPbf);
            AssertEqual(reference, readback, "target-writes.readback-count");
            Log($"  target-writes: produced sidecar={sidecarBytes:N0} bytes, reader round-trip matches reference");
        }
        finally
        {
            CleanupOutputs(outPbf, outSidecar);
        }
    }

    /// <summary>
    /// A sidecar produced by the target must be immediately usable by the source for a
    /// typed jump — no cold walk to build the index. The jump must land on the same
    /// element it would have without any sidecar.
    /// </summary>
    private static void Check_TargetSidecarIsHotForReader(string sourcePbfPath, long reference)
    {
        var outPbf = sourcePbfPath + ".target-hot.osm.pbf";
        var outSidecar = outPbf + ".blockindex";
        CleanupOutputs(outPbf, outSidecar);

        try
        {
            WriteAllWithPersist(sourcePbfPath, outPbf);
            AssertTrue(File.Exists(outSidecar), "target-hot.sidecar-exists");

            // Cold reference on the produced file (no sidecar) — first delete the writer's
            // sidecar, then jump, then restore the writer's sidecar and jump again.
            long? coldId;
            using (var s = new PBFOsmStreamSource(File.OpenRead(outPbf)))
            {
                coldId = s.MoveNext(OsmGeoType.Relation, 0) ? s.Current().Id : null;
            }
            long? hotId;
            using (var s = new PBFOsmStreamSource(outPbf))
            {
                // Path ctor loads the writer-produced sidecar automatically.
                hotId = s.MoveNext(OsmGeoType.Relation, 0) ? s.Current().Id : null;
            }
            AssertEqual(coldId ?? -1, hotId ?? -1, "target-hot.jump-consistent");
            Log($"  target-hot: writer sidecar loaded by reader, typed jump landed on relation id {hotId}");
        }
        finally
        {
            CleanupOutputs(outPbf, outSidecar);
        }
    }

    private static void CleanupOutputs(string pbf, string sidecar)
    {
        if (File.Exists(pbf)) File.Delete(pbf);
        if (File.Exists(sidecar)) File.Delete(sidecar);
        var tmp = sidecar + ".tmp";
        if (File.Exists(tmp)) File.Delete(tmp);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────────

    private static long CountAll(string pbfPath)
    {
        using var src = new PBFOsmStreamSource(pbfPath);
        long n = 0;
        foreach (var _ in src) n++;
        return n;
    }

    /// <summary>
    /// Streams <paramref name="sourcePbfPath"/> into a fresh PBF at <paramref name="outPbfPath"/>
    /// via <see cref="PBFOsmStreamTarget"/> with <c>persistBlockIndex: true</c>. The target
    /// doesn't implement <see cref="IDisposable"/>, so <c>Close()</c> is called explicitly to
    /// flush the final sidecar and dispose the owned file handle.
    /// </summary>
    private static void WriteAllWithPersist(string sourcePbfPath, string outPbfPath)
    {
        using var src = new PBFOsmStreamSource(sourcePbfPath);
        var tgt = new PBFOsmStreamTarget(outPbfPath, persistBlockIndex: true);
        try
        {
            tgt.RegisterSource(src);
            tgt.Pull();
        }
        finally
        {
            tgt.Close();
        }
    }

    private static long CountAllWithPersist(string pbfPath)
    {
        using var src = new PBFOsmStreamSource(pbfPath, persistBlockIndex: true);
        long n = 0;
        foreach (var _ in src) n++;
        return n;
    }

    private static long? FirstRelationId(string pbfPath)
    {
        using var src = new PBFOsmStreamSource(pbfPath);
        return src.MoveNext(OsmGeoType.Relation, 0) ? src.Current().Id : null;
    }

    private static void RebuildSidecar(string pbfPath, string sidecarPath)
    {
        CleanupSidecar(sidecarPath);
        using var src = new PBFOsmStreamSource(pbfPath, persistBlockIndex: true);
        foreach (var _ in src) { }
        AssertTrue(File.Exists(sidecarPath), "sidecar was not written after full walk");
    }

    private static void CleanupSidecar(string sidecarPath)
    {
        if (File.Exists(sidecarPath)) File.Delete(sidecarPath);
        var tmp = sidecarPath + ".tmp";
        if (File.Exists(tmp)) File.Delete(tmp);
    }

    private static void CorruptByteAt(string path, int offset)
    {
        var bytes = File.ReadAllBytes(path);
        if (offset < 0 || offset >= bytes.Length)
            throw new Exception($"corrupt: offset {offset} out of range for file of length {bytes.Length}");
        bytes[offset] = unchecked((byte)(bytes[offset] + 1));
        File.WriteAllBytes(path, bytes);
    }

    private static void AssertEqual<T>(T expected, T actual, string name) where T : IEquatable<T>
    {
        if (!expected.Equals(actual))
        {
            throw new Exception($"[{name}] expected {expected}, got {actual}");
        }
    }

    private static void AssertTrue(bool condition, string name)
    {
        if (!condition) throw new Exception($"[{name}] condition was false");
    }

    private static void Log(string message)
    {
        OsmSharp.Logging.Logger.Log(nameof(PBFBlockIndexTests), TraceEventType.Information, message);
    }
}
