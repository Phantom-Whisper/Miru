import {CircleCheck, ClipboardClock, Film, Home, Play, Tv} from 'lucide-react';
import Analytics from "../analytics/Analytics.tsx";

const menu = [
    { label: "Home", icon: Home },
    { label: "Movies", icon: Film},
    { label: "Series", icon: Tv},
    { label: "Watching", icon: Play },
    { label: "To Watch", icon: ClipboardClock },
    { label: "Watched", icon: CircleCheck },
];

function Sidebar () {
    return (
        <aside className="w-56 h-screen border-r border-zinc-800 px-4 py-6 flex flex-col gap-6">
            <h1 className="text-xl font-bold">Miru</h1>

            <nav className="flex flex-col gap-1">
                {menu.map((item) => (
                    <div
                        key={item.label}
                        className="flex items-center gap-3 px-3 py-2 rounded-lg hover:bg-zinc-800 cursor-pointer text-sm"
                    >
                        <item.icon size={16}/>
                        {item.label}
                    </div>
                ))}
            </nav>

            <div className="mt-auto">
                <Analytics
                    moviePercent={28}
                    seriesPercent={72}
                    totalHours={407}
                    totalMovies={128}
                    totalSeries={32}
                />
            </div>
        </aside>
    )
}

export default Sidebar