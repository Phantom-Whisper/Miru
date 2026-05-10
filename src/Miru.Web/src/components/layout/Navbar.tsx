import {Bell, CircleUserRound, Search} from "lucide-react";

function Navbar() {
    return (
        <header className="h-16 w-full flex items-center justify-end px-6 bg-black border-b border-zinc-800">
            <div className="flex items-center gap-6">
                <div className="relative">
                    <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-zinc-400"/>
                    <input
                        placeholder="Search..."
                        className="bg-zinc-800 pl-10 pr-4 py-2 rounded-xl outline-none text-sm w-72"
                    />
                </div>

                <Bell />
                <CircleUserRound />
            </div>
        </header>
    )
}

export default Navbar