import { useMemo, useState } from "react";
import { Check, Search, SlidersHorizontal } from "lucide-react";

type Capability = {
  name: string;
  area: "Core" | "Play" | "Create" | "Progress";
  description: string;
  status: "Ready" | "In build";
};

const capabilities: Capability[] = [
  { name: "BeatClock + SongClock", area: "Core", description: "Beat-to-time conversion with audio-position awareness and pause compensation.", status: "Ready" },
  { name: "NoteScheduler", area: "Core", description: "Ordered, lane-aware chart events with deterministic scheduling boundaries.", status: "Ready" },
  { name: "Timing windows", area: "Play", description: "Perfect, great, good, miss, and hold-release judgement selection.", status: "Ready" },
  { name: "Replay portability", area: "Play", description: "Safe JSON export/import for repeatable input playback and analysis.", status: "Ready" },
  { name: "Chart editor", area: "Create", description: "Snapping, undo/redo, validation, and safe local chart import/export.", status: "Ready" },
  { name: "Campaign worlds", area: "Progress", description: "Objectives, stars, boss stages, and offline stage progress.", status: "Ready" },
  { name: "Calibration flow", area: "Play", description: "Eight-beat offset sampling for a more personal timing feel.", status: "Ready" },
  { name: "Community chart browser", area: "Create", description: "A future home for curated charts and creator discovery.", status: "In build" },
  { name: "Android release builds", area: "Progress", description: "Signed packages and device-matrix profiling for the stable release.", status: "In build" },
];

const filters = ["All", "Core", "Play", "Create", "Progress"] as const;

export default function FeatureMatrix() {
  const [query, setQuery] = useState("");
  const [filter, setFilter] = useState<(typeof filters)[number]>("All");
  const filtered = useMemo(() => capabilities.filter((item) => {
    const matchesFilter = filter === "All" || item.area === filter;
    const haystack = `${item.name} ${item.description} ${item.area}`.toLowerCase();
    return matchesFilter && haystack.includes(query.toLowerCase());
  }), [filter, query]);

  return (
    <section className="section matrix-section page-width">
      <div className="section-intro split-intro"><div><div className="section-kicker"><span className="kicker-line" />CAPABILITY INDEX</div><h2>Know what’s<br /><em>in the box.</em></h2></div><p>Explore the current surface area without digging through a changelog. Search by system, browse by discipline, and see what is ready today.</p></div>
      <div className="matrix-toolbar"><div className="matrix-search"><Search size={16} /><input aria-label="Search BeatForge capabilities" placeholder="Search capabilities" value={query} onChange={(event) => setQuery(event.target.value)} />{query && <button aria-label="Clear search" onClick={() => setQuery("")}>×</button>}</div><div className="matrix-filters"><SlidersHorizontal size={15} />{filters.map((item) => <button key={item} className={filter === item ? "active" : ""} onClick={() => setFilter(item)}>{item}</button>)}</div></div>
      <div className="matrix-table" role="table" aria-label="BeatForge capabilities">
        <div className="matrix-head" role="row"><span>CAPABILITY</span><span>AREA</span><span>WHAT IT DOES</span><span>STATE</span></div>
        {filtered.length > 0 ? filtered.map((item) => <div className="matrix-row" role="row" key={item.name}><strong>{item.name}</strong><span className={`matrix-area ${item.area.toLowerCase()}`}>{item.area}</span><p>{item.description}</p><span className={`matrix-status ${item.status === "Ready" ? "ready" : "building"}`}>{item.status === "Ready" && <Check size={12} />}{item.status}</span></div>) : <div className="matrix-empty">No capability matches “{query}”. Try a different system or clear the search.</div>}
      </div>
      <div className="matrix-foot"><span>{filtered.length} of {capabilities.length} capabilities shown</span><span>STATUS IS DOCUMENTED, NOT MARKETING.</span></div>
    </section>
  );
}
