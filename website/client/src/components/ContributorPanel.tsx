import { ArrowUpRight, BookOpen, GitBranch, Terminal } from "lucide-react";

const steps = [
  { number: "01", title: "Clone the rhythm", copy: "Pull the repository and open it in Godot 4 .NET.", icon: GitBranch },
  { number: "02", title: "Run the core", copy: "Build the standalone C# timing layer before opening the client.", icon: Terminal },
  { number: "03", title: "Make a chart", copy: "Start with Pulse Garden, then export your own JSON chart.", icon: BookOpen },
];

export default function ContributorPanel() {
  return (
    <section className="section contributor-section page-width">
      <div className="contributor-card">
        <div className="contributor-heading"><div className="section-kicker"><span className="kicker-line" />FIRST SESSION</div><h2>Make your<br /><em>first commit.</em></h2><p>There is a place for a timing nerd, a chart maker, a systems thinker, and the person who just wants the menu to feel better.</p><a className="text-link" href="https://github.com/sanskarIN/BeatForge-Rhythm/blob/main/CONTRIBUTING.md" target="_blank" rel="noreferrer">Read contributing notes <ArrowUpRight size={15} /></a></div>
        <div className="contributor-steps">{steps.map(({ number, title, copy, icon: Icon }) => <div className="contributor-step" key={number}><div className="contributor-step-top"><span className="contributor-number">{number}</span><Icon size={18} /></div><h3>{title}</h3><p>{copy}</p></div>)}</div>
      </div>
    </section>
  );
}
