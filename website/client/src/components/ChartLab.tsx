import { useEffect, useMemo, useState } from "react";
import { Activity, Check, Pause, Play, RotateCcw, Sparkles } from "lucide-react";

type NoteKind = "tap" | "hold" | "swipe";

type LabNote = {
  lane: number;
  beat: number;
  kind: NoteKind;
  label: string;
};

const tracks = [
  { name: "Pulse Garden", artist: "BeatForge Lab", bpm: "128.00", difficulty: "EASY" },
  { name: "Glass Circuit", artist: "Community preview", bpm: "146.00", difficulty: "HARD" },
  { name: "Night Signal", artist: "Synth Assembly", bpm: "172.00", difficulty: "EXPERT" },
];

const notes: LabNote[] = [
  { lane: 0, beat: 1.1, kind: "tap", label: "TAP" },
  { lane: 2, beat: 2.0, kind: "hold", label: "HOLD" },
  { lane: 1, beat: 3.1, kind: "tap", label: "TAP" },
  { lane: 3, beat: 4.2, kind: "swipe", label: "SWIPE" },
  { lane: 0, beat: 5.3, kind: "tap", label: "TAP" },
  { lane: 2, beat: 6.2, kind: "tap", label: "TAP" },
  { lane: 1, beat: 7.1, kind: "hold", label: "HOLD" },
  { lane: 3, beat: 8.3, kind: "tap", label: "TAP" },
];

const laneNames = ["NORTH", "EAST", "SOUTH", "WEST"];

export default function ChartLab() {
  const [trackIndex, setTrackIndex] = useState(0);
  const [running, setRunning] = useState(false);
  const [beat, setBeat] = useState(1.1);
  const track = tracks[trackIndex];

  useEffect(() => {
    if (!running) return;
    const timer = window.setInterval(() => {
      setBeat((current) => (current >= 8.8 ? 1.1 : current + 0.1));
    }, 85);
    return () => window.clearInterval(timer);
  }, [running]);

  const currentNote = useMemo(
    () => notes.reduce((closest, note) => Math.abs(note.beat - beat) < Math.abs(closest.beat - beat) ? note : closest, notes[0]),
    [beat],
  );

  const reset = () => {
    setRunning(false);
    setBeat(1.1);
  };

  return (
    <section id="chart-lab" className="section lab-section">
      <div className="page-width">
        <div className="section-intro split-intro">
          <div><div className="section-kicker"><span className="kicker-line" />TRY THE SYSTEM</div><h2>Chart it.<br /><em>Feel it.</em></h2></div>
          <p>A small interactive preview of the chart loop. Switch songs, press play, and watch the playhead chase the next judgement window.</p>
        </div>
        <div className="lab-shell">
          <div className="lab-toolbar">
            <div className="lab-toolbar-label"><Activity size={15} /> CHART LAB / LIVE PREVIEW</div>
            <div className="lab-actions">
              {tracks.map((item, index) => <button key={item.name} className={`lab-track ${trackIndex === index ? "active" : ""}`} onClick={() => { setTrackIndex(index); reset(); }}>{item.name}</button>)}
            </div>
          </div>
          <div className="lab-body">
            <div className="lab-playfield">
              <div className="lab-axis"><span>BEAT 01</span><span>BEAT 04</span><span>BEAT 08</span></div>
              <div className="lab-playhead" style={{ left: `${Math.min(92, 7 + ((beat - 1.1) / 7.7) * 85)}%` }} />
              {laneNames.map((lane, laneIndex) => <div className="lab-lane" key={lane}><span className="lab-lane-name">{lane}</span><span className="lab-lane-line" />{notes.filter((note) => note.lane === laneIndex).map((note) => <span key={`${note.lane}-${note.beat}`} className={`lab-note ${note.kind} ${Math.abs(note.beat - beat) < .18 ? "near" : ""}`} style={{ left: `${7 + ((note.beat - 1.1) / 7.7) * 85}%` }}><i />{note.kind === "hold" && <b />}</span>)}</div>)}
              <div className="lab-judgement"><span className="lab-judgement-label">NEXT WINDOW</span><strong>{currentNote.label}</strong><span className="lab-judgement-ms">± 42 MS</span></div>
            </div>
            <aside className="lab-inspector">
              <div className="inspector-top"><span className="inspector-dot" /> {running ? "CLOCK RUNNING" : "CLOCK PAUSED"}</div>
              <div className="inspector-title"><small>NOW PLAYING</small><h3>{track.name}</h3><p>{track.artist}</p></div>
              <div className="inspector-metrics"><div><span>BPM</span><strong>{track.bpm}</strong></div><div><span>GRADE</span><strong>{track.difficulty}</strong></div></div>
              <div className="inspector-buttons"><button className="button button-primary" onClick={() => setRunning((value) => !value)}>{running ? <Pause size={15} /> : <Play size={15} />} {running ? "Pause" : "Play chart"}</button><button className="icon-button" aria-label="Reset chart" onClick={reset}><RotateCcw size={16} /></button></div>
              <div className="inspector-note"><Sparkles size={15} /><span>Every note is positioned from chart time, not from the animation frame.</span></div>
              <div className="inspector-foot"><span><Check size={13} /> DETERMINISTIC</span><span>OFFSET +0.0 MS</span></div>
            </aside>
          </div>
        </div>
      </div>
    </section>
  );
}
