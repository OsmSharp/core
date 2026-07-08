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

using System.Collections.Generic;
using System.IO;

namespace OsmSharp.IO.PBF;

/// <summary>
/// Sorted list of <see cref="PBFBlockEntry"/> keyed by <see cref="PBFBlockEntry.FileOffset"/>.
/// Populated as blobs are decoded (by <c>PBFOsmStreamSource</c>) or written (by
/// <c>PBFOsmStreamTarget</c>); consulted by the source's warm-path skip to avoid
/// decompressing blobs whose contents we know we don't want.
///
/// Deliberately internal — exposing raw offsets to public callers would let a wrong
/// value silently misread the file, and the win from cross-process caching isn't
/// worth that risk. Cross-process caching, when needed, goes through a verified
/// sidecar file via <see cref="BlockIndexSidecar"/>.
/// </summary>
internal sealed class PBFBlockIndex
{
    private readonly List<PBFBlockEntry> _entries = new List<PBFBlockEntry>();

    /// <summary>
    /// Direct read-only view of the sorted entry list. Safe to iterate while the owning
    /// source/target is idle; do not iterate while a decode or write pass is in flight.
    /// </summary>
    public IReadOnlyList<PBFBlockEntry> Snapshot() => _entries;

    public bool TryGetAt(long offset, out PBFBlockEntry entry)
    {
        var i = this.BinarySearchByOffset(offset);
        if (i < 0) { entry = default; return false; }
        entry = _entries[i];
        return true;
    }

    public void Upsert(PBFBlockEntry entry)
    {
        var i = this.BinarySearchByOffset(entry.FileOffset);
        if (i >= 0)
        {
            // Merge: preserve any Tier-2 info that a previous decode/write learned but this one didn't.
            var existing = _entries[i];
            if (!entry.NodeIdsKnown && existing.NodeIdsKnown)
            {
                entry.NodeIdsKnown = true;
                entry.NodeMinId = existing.NodeMinId;
                entry.NodeMaxId = existing.NodeMaxId;
            }
            if (!entry.WayIdsKnown && existing.WayIdsKnown)
            {
                entry.WayIdsKnown = true;
                entry.WayMinId = existing.WayMinId;
                entry.WayMaxId = existing.WayMaxId;
            }
            if (!entry.RelationIdsKnown && existing.RelationIdsKnown)
            {
                entry.RelationIdsKnown = true;
                entry.RelationMinId = existing.RelationMinId;
                entry.RelationMaxId = existing.RelationMaxId;
            }
            _entries[i] = entry;
        }
        else
        {
            _entries.Insert(~i, entry);
        }
    }

    private int BinarySearchByOffset(long offset)
    {
        var lo = 0;
        var hi = _entries.Count - 1;
        while (lo <= hi)
        {
            var mid = lo + ((hi - lo) >> 1);
            var midOffset = _entries[mid].FileOffset;
            if (midOffset == offset) return mid;
            if (midOffset < offset) lo = mid + 1;
            else hi = mid - 1;
        }
        return ~lo;
    }
}

internal struct PBFBlockEntry
{
    public long FileOffset;
    public long EndOffset;

    public bool HasNodes;
    public bool HasWays;
    public bool HasRelations;

    public bool NodeIdsKnown;
    public long NodeMinId;
    public long NodeMaxId;

    public bool WayIdsKnown;
    public long WayMinId;
    public long WayMaxId;

    public bool RelationIdsKnown;
    public long RelationMinId;
    public long RelationMaxId;
}

/// <summary>
/// On-disk sidecar for a <see cref="PBFBlockIndex"/>. Format:
/// <code>
///   [8]  magic       "OSMBIDX\0"
///   [4]  version     uint32
///   [8]  pbfLength   int64
///   [8]  pbfMtimeUtc int64 ticks
///   [4]  entryCount  uint32
///   [N × entry]      per-entry: FileOffset(8) EndOffset(8) Flags(1) [min,max × known-tiers]
/// </code>
/// Validation on load: magic + version + PBF length + PBF mtime must all match exactly.
/// Any mismatch or truncation discards silently. Writes go through a temp file + atomic
/// rename so a crash mid-write never leaves a torn sidecar visible.
/// </summary>
internal static class BlockIndexSidecar
{
    private static readonly byte[] Magic =
        { (byte)'O', (byte)'S', (byte)'M', (byte)'B', (byte)'I', (byte)'D', (byte)'X', 0 };
    private const uint Version = 1;

    /// <summary>
    /// Attempts to populate <paramref name="index"/> from the sidecar at
    /// <paramref name="sidecarPath"/>. Returns <c>true</c> when at least one entry was
    /// loaded successfully. Silently discards on missing file, format mismatch, or a
    /// PBF length/mtime that no longer matches.
    /// </summary>
    public static bool TryLoad(string sidecarPath, string pbfPath, PBFBlockIndex index)
    {
        try
        {
            var pbfInfo = new FileInfo(pbfPath);
            if (!pbfInfo.Exists) return false;
            var pbfLength = pbfInfo.Length;
            var pbfMtimeTicks = pbfInfo.LastWriteTimeUtc.Ticks;

            using var fs = File.OpenRead(sidecarPath);
            using var br = new BinaryReader(fs);

            // Magic + version — either off means a foreign or older file, discard.
            var magic = br.ReadBytes(Magic.Length);
            if (magic.Length != Magic.Length) return false;
            for (var i = 0; i < Magic.Length; i++)
            {
                if (magic[i] != Magic[i]) return false;
            }
            var version = br.ReadUInt32();
            if (version != Version) return false;

            // Fingerprint — length + mtime must match the current PBF exactly. If either
            // is off, offsets in the sidecar may point at the wrong bytes; discard.
            var storedLength = br.ReadInt64();
            var storedMtimeTicks = br.ReadInt64();
            if (storedLength != pbfLength) return false;
            if (storedMtimeTicks != pbfMtimeTicks) return false;

            var entryCount = br.ReadUInt32();
            var loaded = 0;
            for (var i = 0; i < entryCount; i++)
            {
                var entry = ReadBlockEntry(br);
                if (entry.FileOffset < 0 || entry.EndOffset <= entry.FileOffset
                    || entry.EndOffset > storedLength)
                {
                    return loaded > 0;
                }
                index.Upsert(entry);
                loaded++;
            }
            return loaded > 0;
        }
        catch (EndOfStreamException)
        {
            // Truncated / malformed sidecar. Keep whatever entries made it in cleanly.
            return true;
        }
        catch (IOException)
        {
            // Transient I/O — sidecar unavailable, run cold.
            return false;
        }
    }

    /// <summary>
    /// Writes <paramref name="entries"/> to the sidecar at <paramref name="sidecarPath"/>
    /// via temp-file + atomic rename. Returns <c>true</c> on success, <c>false</c> on
    /// I/O failure (which is swallowed — callers cannot recover meaningfully and should
    /// simply continue).
    /// </summary>
    public static bool Save(string sidecarPath, string pbfPath, IReadOnlyList<PBFBlockEntry> entries)
    {
        try
        {
            var pbfInfo = new FileInfo(pbfPath);
            if (!pbfInfo.Exists) return false;
            var pbfLength = pbfInfo.Length;
            var pbfMtimeTicks = pbfInfo.LastWriteTimeUtc.Ticks;

            var tmpPath = sidecarPath + ".tmp";
            using (var fs = File.Create(tmpPath))
            using (var bw = new BinaryWriter(fs))
            {
                bw.Write(Magic);
                bw.Write(Version);
                bw.Write(pbfLength);
                bw.Write(pbfMtimeTicks);
                bw.Write((uint)entries.Count);
                foreach (var entry in entries) WriteBlockEntry(bw, entry);
            }

            // Atomic swap: on POSIX and NTFS this is rename() / ReplaceFile, so either the
            // old valid sidecar or the new one is at the target — never a torn write.
            File.Move(tmpPath, sidecarPath, overwrite: true);
            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }

    private static PBFBlockEntry ReadBlockEntry(BinaryReader br)
    {
        var entry = new PBFBlockEntry
        {
            FileOffset = br.ReadInt64(),
            EndOffset = br.ReadInt64(),
        };
        var flags = br.ReadByte();
        entry.HasNodes = (flags & 0x01) != 0;
        entry.HasWays = (flags & 0x02) != 0;
        entry.HasRelations = (flags & 0x04) != 0;
        entry.NodeIdsKnown = (flags & 0x08) != 0;
        entry.WayIdsKnown = (flags & 0x10) != 0;
        entry.RelationIdsKnown = (flags & 0x20) != 0;

        if (entry.NodeIdsKnown)
        {
            entry.NodeMinId = br.ReadInt64();
            entry.NodeMaxId = br.ReadInt64();
        }
        if (entry.WayIdsKnown)
        {
            entry.WayMinId = br.ReadInt64();
            entry.WayMaxId = br.ReadInt64();
        }
        if (entry.RelationIdsKnown)
        {
            entry.RelationMinId = br.ReadInt64();
            entry.RelationMaxId = br.ReadInt64();
        }
        return entry;
    }

    private static void WriteBlockEntry(BinaryWriter bw, PBFBlockEntry entry)
    {
        bw.Write(entry.FileOffset);
        bw.Write(entry.EndOffset);

        byte flags = 0;
        if (entry.HasNodes) flags |= 0x01;
        if (entry.HasWays) flags |= 0x02;
        if (entry.HasRelations) flags |= 0x04;
        if (entry.NodeIdsKnown) flags |= 0x08;
        if (entry.WayIdsKnown) flags |= 0x10;
        if (entry.RelationIdsKnown) flags |= 0x20;
        bw.Write(flags);

        if (entry.NodeIdsKnown)
        {
            bw.Write(entry.NodeMinId);
            bw.Write(entry.NodeMaxId);
        }
        if (entry.WayIdsKnown)
        {
            bw.Write(entry.WayMinId);
            bw.Write(entry.WayMaxId);
        }
        if (entry.RelationIdsKnown)
        {
            bw.Write(entry.RelationMinId);
            bw.Write(entry.RelationMaxId);
        }
    }
}
