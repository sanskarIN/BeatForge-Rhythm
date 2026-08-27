# BeatForge Chart Format v1

Chart files are UTF-8 JSON documents with a top-level `schemaVersion` of `1`. They contain metadata, timing, lanes, and notes. The format is intentionally human-readable and uses stable string IDs.

```json
{
  "schemaVersion": 1,
  "metadata": {
    "id": "pulse-garden",
    "title": "Pulse Garden",
    "artist": "BeatForge Original",
    "author": "Sanskar",
    "difficulty": "Easy",
    "difficultyRating": 2,
    "tags": ["original", "starter"],
    "audioFile": ""
  },
  "initialBpm": 120,
  "beatOffsetSeconds": 0,
  "laneCount": 4,
  "bpmChanges": [],
  "notes": [
    { "id": "n-001", "beat": 1, "lane": 0, "type": "Tap", "durationBeats": 0, "direction": 0 }
  ]
}
```

## Fields

| Field | Type | Rule |
| --- | --- | --- |
| `schemaVersion` | integer | Must be `1` for this release |
| `metadata.id` | string | Stable ID, 1–128 characters |
| `metadata.title` | string | 1–200 characters |
| `metadata.difficulty` | enum | Beginner, Easy, Normal, Hard, Expert, Master |
| `metadata.difficultyRating` | integer | Display rating chosen by the creator |
| `initialBpm` | number | 20–400 BPM |
| `beatOffsetSeconds` | number | Audio alignment offset |
| `laneCount` | integer | 1–16 |
| `bpmChanges` | array | Strictly ordered, each with `beat` and `bpm` |
| `notes` | array | At most 100,000 validated notes |

Notes support `Tap`, `Hold`, `Swipe`, `DirectionalSwipe`, `Rapid`, `Simultaneous`, `Chain`, `Slide`, and `Special`. Holds and slides use `durationBeats`. Directional swipes may use `direction` from 0–3. `chainId` associates notes in a chain.

## Validation and migration

The importer rejects unknown schema versions, invalid ranges, duplicate note IDs, invalid lanes, invalid BPM changes, and oversized files. Version migration is applied before validation. Imported data is treated as data only; it is never evaluated as code and cannot choose a path outside the user-provided destination.

Future schema versions must provide a migration step and preserve stable note IDs where possible.
