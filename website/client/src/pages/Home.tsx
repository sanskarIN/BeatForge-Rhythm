import { useState } from "react";
import {
  ArrowDownRight,
  ArrowUpRight,
  AudioLines,
  BookOpen,
  Check,
  ChevronRight,
  CircleDot,
  Code2,
  ExternalLink,
  Github,
  Gauge,
  Gamepad2,
  GitBranch,
  Layers3,
  Menu,
  Mic2,
  Play,
  Radio,
  Sparkles,
  Target,
  Timer,
  Trophy,
  X,
  Zap,
} from "lucide-react";
import ChartLab from "@/components/ChartLab";
import ContributorPanel from "@/components/ContributorPanel";
import FeatureMatrix from "@/components/FeatureMatrix";

const features = [
  {
    icon: Timer,
    index: "01",
    title: "Timing, isolated",
    copy: "A deterministic BeatClock and SongClock keep BPM changes, offsets, pause compensation, and scoring out of the UI layer.",
    accent: "mint",
  },
  {
    icon: Gamepad2,
    index: "02",
    title: "Feel every input",
    copy: "Tap, hold, slide, swipe, and directional swipe judgement share one strongly typed timing boundary.",
    accent: "violet",
  },
  {
    icon: Layers3,
    index: "03",
    title: "Create without friction",
    copy: "A chart editor with snapping, undo/redo, validation, safe import, and versioned JSON makes authoring approachable.",
    accent: "coral",
  },
  {
    icon: Radio,
    index: "04",
    title: "Offline by design",
    copy: "Settings, calibration, favorites, best scores, achievements, and campaign progress stay local and portable.",
    accent: "blue",
  },
];

const architecture = [
  { label: "TimingEngine", detail: "BeatClock · SongClock · NoteScheduler", color: "mint" },
  { label: "ScoringEngine", detail: "Windows · combo · grades · replay", color: "violet" },
  { label: "Gameplay", detail: "Tap · hold · slide · swipe", color: "coral" },
  { label: "ChartEditor", detail: "Snap · validate · undo · export", color: "blue" },
];

const roadmap = [
  { version: "v0.1", title: "Foundation", detail: "Godot C# project, docs, sample chart, and CI.", done: true },
  { version: "v0.2", title: "Timing engine", detail: "Audio position, calibrated offsets, BPM changes, and pause compensation.", done: true },
  { version: "v0.3", title: "Playable rhythm", detail: "Tap/hold surface, combo, score, practice controls, and richer notes.", done: true },
  { version: "v0.6", title: "Campaign", detail: "Worlds, stages, objectives, stars, and challenge conditions.", done: true },
  { version: "v1.0", title: "Stable release", detail: "Android packaging, device matrix, profiling, and signed builds.", done: false },
];

function SectionKicker({ children }: { children: React.ReactNode }) {
  return <div className="section-kicker"><span className="kicker-line" />{children}</div>;
}

function BrandMark() {
  return (
    <div className="brand-mark" aria-label="BeatForge logo">
      <span className="brand-orb"><span /></span>
      <span className="brand-name">BEAT<span>FORGE</span></span>
    </div>
  );
}

export default function Home() {
  const [menuOpen, setMenuOpen] = useState(false);
  const [activeFeature, setActiveFeature] = useState(0);

  const closeMenu = () => setMenuOpen(false);

  return (
    <main className="site-shell">
      <div className="noise" aria-hidden="true" />
      <header className={`site-nav ${menuOpen ? "nav-open" : ""}`}>
        <a className="brand-link" href="#top" onClick={closeMenu}><BrandMark /></a>
        <button className="menu-toggle" aria-label={menuOpen ? "Close menu" : "Open menu"} onClick={() => setMenuOpen((value) => !value)}>
          {menuOpen ? <X size={20} /> : <Menu size={20} />}
        </button>
        <nav className="nav-links" aria-label="Primary navigation">
          <a href="#systems" onClick={closeMenu}>Systems</a>
          <a href="#play" onClick={closeMenu}>The loop</a>
          <a href="#chart-lab" onClick={closeMenu}>Chart lab</a>
          <a href="#roadmap" onClick={closeMenu}>Roadmap</a>
          <a href="#open-source" onClick={closeMenu}>Open source</a>
          <a className="nav-cta" href="https://github.com/sanskarIN/BeatForge-Rhythm" target="_blank" rel="noreferrer" onClick={closeMenu}>View on GitHub <ArrowUpRight size={14} /></a>
        </nav>
      </header>

      <section id="top" className="hero-section">
        <div className="hero-backdrop" aria-hidden="true"><div className="hero-grid" /><div className="hero-wave wave-one" /><div className="hero-wave wave-two" /></div>
        <div className="hero-content page-width">
          <div className="hero-copy">
            <SectionKicker>RHYTHM / LAB / 001</SectionKicker>
            <h1>Make the beat<br /><em>yours.</em></h1>
            <p className="hero-lede">BeatForge is an open rhythm lab for players and creators — built around timing you can trust, charts you can shape, and progress that stays yours.</p>
            <div className="hero-actions">
              <a className="button button-primary" href="https://github.com/sanskarIN/BeatForge-Rhythm" target="_blank" rel="noreferrer"><Github size={17} /> Explore the source <ArrowUpRight size={15} /></a>
              <a className="text-link" href="#systems">See what’s inside <ArrowDownRight size={16} /></a>
            </div>
            <div className="hero-meta"><span><CircleDot size={12} /> Godot 4 + C#</span><span><CircleDot size={12} /> Offline-first</span><span><CircleDot size={12} /> Apache-2.0</span></div>
          </div>
          <div className="hero-console" aria-label="BeatForge timing console visualization">
            <div className="console-top"><span>LIVE TIMING / PULSE GARDEN</span><span className="console-status"><i /> SYNCED</span></div>
            <div className="console-main">
              <div className="console-readout"><span className="readout-label">CURRENT BEAT</span><strong>04<span>.250</span></strong><span className="readout-sub">BPM 128.00 <b>+0.0 ms</b></span></div>
              <div className="beat-visual"><div className="beat-axis"><span>01</span><span>02</span><span>03</span><span>04</span><span>05</span></div><div className="beat-lanes"><i /><i /><i /><i /></div><div className="beat-note note-a" /><div className="beat-note note-b" /><div className="beat-note note-c" /><div className="beat-playhead" /></div>
            </div>
            <div className="console-bottom"><span><AudioLines size={13} /> AUDIO CLOCK</span><span>PERFECT <b>98.4%</b></span><span>COMBO <b>042</b></span></div>
          </div>
        </div>
        <div className="hero-scroll"><span>SCROLL TO EXPLORE</span><ChevronRight size={15} /></div>
      </section>

      <section className="signal-strip"><div className="page-width signal-inner"><span>BUILT FOR THE MOMENT BETWEEN THE BEATS</span><div className="signal-pulse"><i /><i /><i /><i /><i /><i /><i /></div><span>OPEN / LOCAL / PRECISE</span></div></section>

      <section id="systems" className="section section-systems page-width">
        <div className="section-intro split-intro"><div><SectionKicker>THE INSTRUMENT</SectionKicker><h2>A game engine<br />with <em>ears.</em></h2></div><p>Every layer has a job. BeatForge keeps its timing engine independent so the playfield can stay expressive without making accuracy fragile.</p></div>
        <div className="feature-layout">
          <div className="feature-list" role="tablist" aria-label="BeatForge systems">
            {features.map((feature, index) => { const Icon = feature.icon; return <button key={feature.index} className={`feature-tab ${activeFeature === index ? "active" : ""}`} onClick={() => setActiveFeature(index)} role="tab" aria-selected={activeFeature === index}><span className={`feature-icon ${feature.accent}`}><Icon size={19} /></span><span className="feature-tab-copy"><small>{feature.index}</small><strong>{feature.title}</strong></span><ChevronRight className="feature-chevron" size={18} /></button>; })}
          </div>
          <div className={`feature-stage ${features[activeFeature].accent}`} role="tabpanel">
            <div className="stage-orbit orbit-a" /><div className="stage-orbit orbit-b" /><div className="stage-crosshair"><span /><span /><span /><span /></div>
            <div className="stage-copy"><span className="stage-index">SYSTEM / {features[activeFeature].index}</span><h3>{features[activeFeature].title}</h3><p>{features[activeFeature].copy}</p><div className="stage-chip"><Zap size={13} /> C# / DETERMINISTIC / READY</div></div>
          </div>
        </div>
      </section>

      <section id="play" className="section play-section">
        <div className="page-width play-layout">
          <div className="play-visual"><div className="play-visual-label">THE PLAYFIELD / 04 LANES</div><div className="play-lane lane-1"><span className="play-note note-long" /></div><div className="play-lane lane-2"><span className="play-note note-dot" /></div><div className="play-lane lane-3"><span className="play-note note-swipe" /></div><div className="play-lane lane-4"><span className="play-note note-dot violet-dot" /></div><div className="play-line" /><div className="play-score"><small>PERSONAL BEST</small><strong>000<em>984</em></strong></div></div>
          <div className="play-copy"><SectionKicker>THE LOOP</SectionKicker><h2>Listen.<br /><em>Respond.</em><br />Repeat.</h2><p>Drop into a chart, calibrate your setup, and chase the cleanest possible line. Practice speed, accessibility controls, and replay-ready scoring keep the feedback loop yours.</p><div className="stat-row"><div><strong>16</strong><span>core checks passing</span></div><div><strong>52</strong><span>achievement seeds</span></div><div><strong>0</strong><span>bundled tracks</span></div></div><a className="text-link" href="https://github.com/sanskarIN/BeatForge-Rhythm#readme" target="_blank" rel="noreferrer">Read the build notes <ArrowUpRight size={15} /></a></div>
        </div>
      </section>

      <section className="section architecture-section page-width">
        <div className="section-intro split-intro"><div><SectionKicker>UNDER THE HOOD</SectionKicker><h2>Small parts.<br /><em>Strong pulse.</em></h2></div><p>Chart data, audio position, scoring, replay, storage, and UI are separate on purpose. It makes the project easier to test, extend, and learn from.</p></div>
        <div className="architecture-map"><div className="map-spine" />{architecture.map((item, index) => <div className={`arch-node ${item.color}`} key={item.label}><div className="arch-dot"><span>{String(index + 1).padStart(2, "0")}</span></div><div><h3>{item.label}</h3><p>{item.detail}</p></div></div>)}</div>
        <div className="code-note"><Code2 size={17} /><span><strong>THE RULE:</strong> gameplay never depends directly on SQLite, Godot UI, or audio assets.</span><ArrowUpRight size={16} /></div>
      </section>

      <ChartLab />

      <FeatureMatrix />

      <section id="roadmap" className="section roadmap-section">
        <div className="page-width"><div className="section-intro split-intro"><div><SectionKicker>THE NEXT DOWNBEAT</SectionKicker><h2>Built in<br /><em>public.</em></h2></div><p>BeatForge grows in small, meaningful slices. No empty controls. No fabricated milestones. Every phase is documented, tested, and pushed.</p></div><div className="roadmap-list">{roadmap.map((item) => <div className={`roadmap-row ${item.done ? "done" : ""}`} key={item.version}><div className="roadmap-version">{item.version}</div><div className="roadmap-marker">{item.done ? <Check size={14} /> : <span />}</div><div className="roadmap-content"><h3>{item.title}</h3><p>{item.detail}</p></div><div className="roadmap-state">{item.done ? "SHIPPED" : "IN ORBIT"}</div></div>)}</div></div>
      </section>

      <ContributorPanel />

      <section id="open-source" className="section open-section page-width">
        <div className="open-card"><div className="open-glow" /><div className="open-copy"><SectionKicker>THE INVITATION</SectionKicker><h2>Bring your<br /><em>own rhythm.</em></h2><p>BeatForge is Apache-2.0, offline-first, and made to be opened up. Try the sample chart, inspect the core, build a chart, or help shape the next release.</p><div className="open-actions"><a className="button button-primary" href="https://github.com/sanskarIN/BeatForge-Rhythm" target="_blank" rel="noreferrer"><Github size={17} /> Open repository <ArrowUpRight size={15} /></a><a className="button button-ghost" href="https://github.com/sanskarIN/BeatForge-Rhythm/issues" target="_blank" rel="noreferrer">Find an issue <ExternalLink size={15} /></a></div></div><div className="open-orbit"><div className="orbit-core"><Sparkles size={22} /></div><div className="orbit-ring ring-one" /><div className="orbit-ring ring-two" /><span className="orbit-tag tag-top">APACHE / 2.0</span><span className="orbit-tag tag-bottom">C# / GODOT</span></div></div>
      </section>

      <footer className="site-footer page-width"><div className="footer-top"><BrandMark /><div className="footer-links"><a href="https://github.com/sanskarIN/BeatForge-Rhythm" target="_blank" rel="noreferrer">GitHub <ArrowUpRight size={13} /></a><a href="https://github.com/sanskarIN/BeatForge-Rhythm/blob/main/ARCHITECTURE.md" target="_blank" rel="noreferrer">Architecture <ArrowUpRight size={13} /></a><a href="https://github.com/sanskarIN/BeatForge-Rhythm/blob/main/CONTRIBUTING.md" target="_blank" rel="noreferrer">Contribute <ArrowUpRight size={13} /></a></div></div><div className="footer-bottom"><span>BEATFORGE: RHYTHM LAB / MADE BY SANSKAR</span><span>OPEN SOURCE, IN PROGRESS, IN TIME.</span></div></footer>
    </main>
  );
}
